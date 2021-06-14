using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct OpenSimplex2<mask32v, float32v, int32v, TFunctions> :
        //INoiseGenerator1D<mask32v, float32v, int32v>,
        INoiseGenerator2D<mask32v, float32v, int32v>,
        INoiseGenerator3D<mask32v, float32v, int32v>
        where mask32v : unmanaged, IFMask<mask32v>
        where float32v : unmanaged, IFVector<float32v, mask32v>
        where int32v : unmanaged, IFVector<int32v, mask32v>
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions FS = default;
        private static Utils<mask32v, float32v, int32v, TFunctions> Utils = default;
        private static FastSimd<mask32v, float32v, int32v, TFunctions> FSS = default;

        public int Count => default(float32v).Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y)
        {
            const float SQRT3 = 1.7320508075f;
            const float F2 = 0.5f * (SQRT3 - 1.0f);
            const float G2 = (3.0f - SQRT3) / 6.0f;

            float32v f = FS.Broad_f32(F2).Mul(x.Add(y));
            float32v x0 = FS.Floor_f32(x.Add(f));
            float32v y0 = FS.Floor_f32(y.Add(f));

            int32v i = FS.Convertf32_i32(x0).Mul(FS.Broad_i32(Primes.X));
            int32v j = FS.Convertf32_i32(y0).Mul(FS.Broad_i32(Primes.Y));

            float32v g = FS.Broad_f32(G2).Mul(x0.Add(y0));
            x0 = x.Sub(x0.Sub(g));
            y0 = y.Sub(y0.Sub(g));

            mask32v i1 = x0.GreaterThan(y0);
            //mask32v j1 = ~i1; //NMasked funcs

            float32v x1 = FSS.MaskedSub_f32(x0, FS.Broad_f32(1f), i1).Add(FS.Broad_f32(G2));
            float32v y1 = FSS.NMaskedSub_f32(y0, FS.Broad_f32(1f), i1).Add(FS.Broad_f32(G2));
            float32v x2 = x0.Add(FS.Broad_f32((G2 * 2) - 1));
            float32v y2 = y0.Add(FS.Broad_f32((G2 * 2) - 1));

            float32v t0 = FS.Broad_f32(0.5f).Sub(x0.Mul(x0)).Sub(y0.Mul(y0));
            float32v t1 = FS.Broad_f32(0.5f).Sub(x1.Mul(x1)).Sub(y1.Mul(y1));
            float32v t2 = FS.Broad_f32(0.5f).Sub(x2.Mul(x2)).Sub(y2.Mul(y2));

            t0 = FS.Max_f32(t0, FS.Broad_f32(0));
            t1 = FS.Max_f32(t1, FS.Broad_f32(0));
            t2 = FS.Max_f32(t2, FS.Broad_f32(0));

            t0 = t0.Mul(t0);
            t0 = t0.Mul(t0);
            t1 = t1.Mul(t1);
            t1 = t1.Mul(t1);
            t2 = t2.Mul(t2);
            t2 = t2.Mul(t2);

            float32v n0 = Utils.GetGradientDotFancy(
                Utils.HashPrimes(seed, i, j), x0, y0);

            float32v n1 = Utils.GetGradientDotFancy(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i1),
                    FSS.NMaskedAdd_i32(j, FS.Broad_i32(Primes.Y), i1)),
                x1, y1);

            float32v n2 = Utils.GetGradientDotFancy(
                Utils.HashPrimes(
                    seed,
                    i.Add(FS.Broad_i32(Primes.X)),
                    j.Add(FS.Broad_i32(Primes.Y))),
                x2, y2);

            float32v last = FS.FMulAdd_f32(n0, t0, FS.FMulAdd_f32(n1, t1, n2.Mul(t2)));
            return FS.Broad_f32(49.918426513671875f).Mul(last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        {
            float32v f = FS.Broad_f32(2.0f / 3.0f).Mul(x.Add(y).Add(z));
            float32v xr = f.Sub(x);
            float32v yr = f.Sub(y);
            float32v zr = f.Sub(z);

            float32v val = FS.Broad_f32(0);
            for (int i = 0; ; i++)
            {
                float32v v0xr = FS.Round_f32(xr);
                float32v v0yr = FS.Round_f32(yr);
                float32v v0zr = FS.Round_f32(zr);
                float32v d0xr = xr.Sub(v0xr);
                float32v d0yr = yr.Sub(v0yr);
                float32v d0zr = zr.Sub(v0zr);

                float32v score0xr = FS.Abs_f32(d0xr);
                float32v score0yr = FS.Abs_f32(d0yr);
                float32v score0zr = FS.Abs_f32(d0zr);
                mask32v dir0xr = FS.Max_f32(score0yr, score0zr).LessThanOrEqual(score0xr);
                mask32v dir0yr = FSS.BitwiseAndNot_m32(FS.Max_f32(score0zr, score0xr).LessThanOrEqual(score0yr), dir0xr);
                mask32v dir0zr = (dir0xr.Or(dir0yr)).Complement();
                float32v v1xr = FSS.MaskedAdd_f32(v0xr, FS.Broad_f32(1.0f).Or(FS.Broad_f32(-1.0f).And(d0xr)), dir0xr);
                float32v v1yr = FSS.MaskedAdd_f32(v0yr, FS.Broad_f32(1.0f).Or(FS.Broad_f32(-1.0f).And(d0yr)), dir0yr);
                float32v v1zr = FSS.MaskedAdd_f32(v0zr, FS.Broad_f32(1.0f).Or(FS.Broad_f32(-1.0f).And(d0zr)), dir0zr);
                float32v d1xr = xr.Sub(v1xr);
                float32v d1yr = yr.Sub(v1yr);
                float32v d1zr = zr.Sub(v1zr);

                int32v hv0xr = FS.Convertf32_i32(v0xr).Mul(FS.Broad_i32(Primes.X));
                int32v hv0yr = FS.Convertf32_i32(v0yr).Mul(FS.Broad_i32(Primes.Y));
                int32v hv0zr = FS.Convertf32_i32(v0zr).Mul(FS.Broad_i32(Primes.Z));

                int32v hv1xr = FS.Convertf32_i32(v1xr).Mul(FS.Broad_i32(Primes.X));
                int32v hv1yr = FS.Convertf32_i32(v1yr).Mul(FS.Broad_i32(Primes.Y));
                int32v hv1zr = FS.Convertf32_i32(v1zr).Mul(FS.Broad_i32(Primes.Z));

                float32v t0 = FS.FNMulAdd_f32(d0zr, d0zr, FS.FNMulAdd_f32(d0yr, d0yr, FS.FNMulAdd_f32(d0xr, d0xr, FS.Broad_f32(0.6f))));
                float32v t1 = FS.FNMulAdd_f32(d1zr, d1zr, FS.FNMulAdd_f32(d1yr, d1yr, FS.FNMulAdd_f32(d1xr, d1xr, FS.Broad_f32(0.6f))));
                t0 = FS.Max_f32(t0, FS.Broad_f32(0));
                t1 = FS.Max_f32(t1, FS.Broad_f32(0));
                t0 = t0.Mul(t0);
                t0 = t0.Mul(t0);
                t1 = t1.Mul(t1);
                t1 = t1.Mul(t1);

                float32v v0 = Utils.GetGradientDot(Utils.HashPrimes(seed, hv0xr, hv0yr, hv0zr), d0xr, d0yr, d0zr);
                float32v v1 = Utils.GetGradientDot(Utils.HashPrimes(seed, hv1xr, hv1yr, hv1zr), d1xr, d1yr, d1zr);

                val = FS.FMulAdd_f32(v0, t0, FS.FMulAdd_f32(v1, t1, val));

                if (i == 1)
                {
                    break;
                }

                xr = xr.Add(FS.Broad_f32(0.5f));
                yr = yr.Add(FS.Broad_f32(0.5f));
                zr = zr.Add(FS.Broad_f32(0.5f));
                seed = seed.Complement();
            }

            return FS.Broad_f32(32.69428253173828125f).Mul(val);
        }
    }
}
