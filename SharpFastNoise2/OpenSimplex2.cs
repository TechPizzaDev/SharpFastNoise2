using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct OpenSimplex2<mask32v, float32v, int32v, TFunctions> :
        //INoiseGenerator1D<float32v, int32v>,
        INoiseGenerator2D<float32v, int32v>,
        INoiseGenerator3D<float32v, int32v>
        where mask32v : unmanaged
        where float32v : unmanaged
        where int32v : unmanaged
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions F = default;
        private static Utils<mask32v, float32v, int32v, TFunctions> Utils = default;
        private static FastSimd<mask32v, float32v, int32v, TFunctions> FSS = default;

        public int Count => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y)
        {
            const float SQRT3 = 1.7320508075f;
            const float F2 = 0.5f * (SQRT3 - 1.0f);
            const float G2 = (3.0f - SQRT3) / 6.0f;

            float32v f = F.Mul(F.Broad_f32(F2), F.Add(x, y));
            float32v x0 = F.Floor_f32(F.Add(x, f));
            float32v y0 = F.Floor_f32(F.Add(y, f));

            int32v i = F.Mul(F.Convertf32_i32(x0), F.Broad_i32(Primes.X));
            int32v j = F.Mul(F.Convertf32_i32(y0), F.Broad_i32(Primes.Y));

            float32v g = F.Mul(F.Broad_f32(G2), F.Add(x0, y0));
            x0 = F.Sub(x, F.Sub(x0, g));
            y0 = F.Sub(y, F.Sub(y0, g));

            mask32v i1 = F.GreaterThan(x0, y0);
            //mask32v j1 = ~i1; //NMasked funcs

            float32v x1 = F.Add(FSS.MaskedSub_f32(x0, F.Broad_f32(1f), i1), F.Broad_f32(G2));
            float32v y1 = F.Add(FSS.NMaskedSub_f32(y0, F.Broad_f32(1f), i1), F.Broad_f32(G2));
            float32v x2 = F.Add(x0, F.Broad_f32((G2 * 2) - 1));
            float32v y2 = F.Add(y0, F.Broad_f32((G2 * 2) - 1));

            float32v t0 = F.Sub(F.Sub(F.Broad_f32(0.5f), F.Mul(x0, x0)), F.Mul(y0, y0));
            float32v t1 = F.Sub(F.Sub(F.Broad_f32(0.5f), F.Mul(x1, x1)), F.Mul(y1, y1));
            float32v t2 = F.Sub(F.Sub(F.Broad_f32(0.5f), F.Mul(x2, x2)), F.Mul(y2, y2));

            t0 = F.Max_f32(t0, F.Broad_f32(0));
            t1 = F.Max_f32(t1, F.Broad_f32(0));
            t2 = F.Max_f32(t2, F.Broad_f32(0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);

            float32v n0 = Utils.GetGradientDotFancy(
                Utils.HashPrimes(seed, i, j), x0, y0);

            float32v n1 = Utils.GetGradientDotFancy(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    FSS.NMaskedAdd_i32(j, F.Broad_i32(Primes.Y), i1)),
                x1, y1);

            float32v n2 = Utils.GetGradientDotFancy(
                Utils.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y))),
                x2, y2);

            float32v last = F.FMulAdd_f32(n0, t0, F.FMulAdd_f32(n1, t1, F.Mul(n2, t2)));
            return F.Mul(F.Broad_f32(49.918426513671875f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        {
            float32v f = F.Mul(F.Broad_f32(2.0f / 3.0f), F.Add(F.Add(x, y), z));
            float32v xr = F.Sub(f, x);
            float32v yr = F.Sub(f, y);
            float32v zr = F.Sub(f, z);

            float32v val = F.Broad_f32(0);
            for (int i = 0; ; i++)
            {
                float32v v0xr = F.Round_f32(xr);
                float32v v0yr = F.Round_f32(yr);
                float32v v0zr = F.Round_f32(zr);
                float32v d0xr = F.Sub(xr, v0xr);
                float32v d0yr = F.Sub(yr, v0yr);
                float32v d0zr = F.Sub(zr, v0zr);

                float32v score0xr = F.Abs_f32(d0xr);
                float32v score0yr = F.Abs_f32(d0yr);
                float32v score0zr = F.Abs_f32(d0zr);
                mask32v dir0xr = F.LessThanOrEqual(F.Max_f32(score0yr, score0zr), score0xr);
                mask32v dir0yr = FSS.BitwiseAndNot_m32(F.LessThanOrEqual(F.Max_f32(score0zr, score0xr), score0yr), dir0xr);
                mask32v dir0zr = F.Complement(F.Or(dir0xr, dir0yr));
                float32v v1xr = FSS.MaskedAdd_f32(v0xr, F.Or(F.Broad_f32(1.0f), F.And(F.Broad_f32(-1.0f), d0xr)), dir0xr);
                float32v v1yr = FSS.MaskedAdd_f32(v0yr, F.Or(F.Broad_f32(1.0f), F.And(F.Broad_f32(-1.0f), d0yr)), dir0yr);
                float32v v1zr = FSS.MaskedAdd_f32(v0zr, F.Or(F.Broad_f32(1.0f), F.And(F.Broad_f32(-1.0f), d0zr)), dir0zr);
                float32v d1xr = F.Sub(xr, v1xr);
                float32v d1yr = F.Sub(yr, v1yr);
                float32v d1zr = F.Sub(zr, v1zr);

                int32v hv0xr = F.Mul(F.Convertf32_i32(v0xr), F.Broad_i32(Primes.X));
                int32v hv0yr = F.Mul(F.Convertf32_i32(v0yr), F.Broad_i32(Primes.Y));
                int32v hv0zr = F.Mul(F.Convertf32_i32(v0zr), F.Broad_i32(Primes.Z));

                int32v hv1xr = F.Mul(F.Convertf32_i32(v1xr), F.Broad_i32(Primes.X));
                int32v hv1yr = F.Mul(F.Convertf32_i32(v1yr), F.Broad_i32(Primes.Y));
                int32v hv1zr = F.Mul(F.Convertf32_i32(v1zr), F.Broad_i32(Primes.Z));

                float32v t0 = F.FNMulAdd_f32(d0zr, d0zr, F.FNMulAdd_f32(d0yr, d0yr, F.FNMulAdd_f32(d0xr, d0xr, F.Broad_f32(0.6f))));
                float32v t1 = F.FNMulAdd_f32(d1zr, d1zr, F.FNMulAdd_f32(d1yr, d1yr, F.FNMulAdd_f32(d1xr, d1xr, F.Broad_f32(0.6f))));
                t0 = F.Max_f32(t0, F.Broad_f32(0));
                t1 = F.Max_f32(t1, F.Broad_f32(0));
                t0 = F.Mul(t0, t0);
                t0 = F.Mul(t0, t0);
                t1 = F.Mul(t1, t1);
                t1 = F.Mul(t1, t1);

                float32v v0 = Utils.GetGradientDot(Utils.HashPrimes(seed, hv0xr, hv0yr, hv0zr), d0xr, d0yr, d0zr);
                float32v v1 = Utils.GetGradientDot(Utils.HashPrimes(seed, hv1xr, hv1yr, hv1zr), d1xr, d1yr, d1zr);

                val = F.FMulAdd_f32(v0, t0, F.FMulAdd_f32(v1, t1, val));

                if (i == 1)
                {
                    break;
                }

                xr = F.Add(xr, F.Broad_f32(0.5f));
                yr = F.Add(yr, F.Broad_f32(0.5f));
                zr = F.Add(zr, F.Broad_f32(0.5f));
                seed = F.Complement(seed);
            }

            return F.Mul(F.Broad_f32(32.69428253173828125f), val);
        }
    }
}
