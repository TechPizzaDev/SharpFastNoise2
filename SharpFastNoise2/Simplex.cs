using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct Simplex<mask32v, float32v, int32v, TFunctions> :
        INoiseGenerator2D<float32v, int32v>,
        INoiseGenerator3D<float32v, int32v>,
        INoiseGenerator4D<float32v, int32v>
        where mask32v : unmanaged
        where float32v : unmanaged
        where int32v : unmanaged
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions F = default;
        private static Utils<mask32v, float32v, int32v, TFunctions> Utils = default;
        private static FastSimd<mask32v, float32v, int32v, TFunctions> FSS = default;

        public int Count => F.Count;

        //public float32v Gen(int32v seed, float32v x)
        //{
        //    throw new System.NotImplementedException();
        //}

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y)
        {
            const float SQRT3 = 1.7320508075688772935274463415059f;
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

            float32v x2 = F.Add(x0, F.Broad_f32(G2 * 2 - 1));
            float32v y2 = F.Add(y0, F.Broad_f32(G2 * 2 - 1));

            float32v t0 = F.FNMulAdd_f32(x0, x0, F.FNMulAdd_f32(y0, y0, F.Broad_f32(0.5f)));
            float32v t1 = F.FNMulAdd_f32(x1, x1, F.FNMulAdd_f32(y1, y1, F.Broad_f32(0.5f)));
            float32v t2 = F.FNMulAdd_f32(x2, x2, F.FNMulAdd_f32(y2, y2, F.Broad_f32(0.5f)));

            t0 = F.Max_f32(t0, F.Broad_f32(0));
            t1 = F.Max_f32(t1, F.Broad_f32(0));
            t2 = F.Max_f32(t2, F.Broad_f32(0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);

            float32v n0 = Utils.GetGradientDot(
                Utils.HashPrimes(seed, i, j),
                x0, y0);

            float32v n1 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    FSS.NMaskedAdd_i32(j, F.Broad_i32(Primes.Y), i1)),
                x1, y1);

            float32v n2 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y))),
                x2, y2);

            float32v last = F.FMulAdd_f32(n0, t0, F.FMulAdd_f32(n1, t1, F.Mul(n2, t2)));
            return F.Mul(F.Broad_f32(38.283687591552734375f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        {
            const float F3 = 1.0f / 3.0f;
            const float G3 = 1.0f / 2.0f;

            float32v s = F.Mul(F.Broad_f32(F3), F.Add(F.Add(x, y), z));
            x = F.Add(x, s);
            y = F.Add(y, s);
            z = F.Add(z, s);

            float32v x0 = F.Floor_f32(x);
            float32v y0 = F.Floor_f32(y);
            float32v z0 = F.Floor_f32(z);
            float32v xi = F.Sub(x, x0);
            float32v yi = F.Sub(y, y0);
            float32v zi = F.Sub(z, z0);

            int32v i = F.Mul(F.Convertf32_i32(x0), F.Broad_i32(Primes.X));
            int32v j = F.Mul(F.Convertf32_i32(y0), F.Broad_i32(Primes.Y));
            int32v k = F.Mul(F.Convertf32_i32(z0), F.Broad_i32(Primes.Z));

            mask32v x_ge_y = F.GreaterThanOrEqual(xi, yi);
            mask32v y_ge_z = F.GreaterThanOrEqual(yi, zi);
            mask32v x_ge_z = F.GreaterThanOrEqual(xi, zi);

            float32v g = F.Mul(F.Broad_f32(G3), F.Add(F.Add(xi, yi), zi));
            x0 = F.Sub(xi, g);
            y0 = F.Sub(yi, g);
            z0 = F.Sub(zi, g);

            mask32v i1 = F.And(x_ge_y, x_ge_z);
            mask32v j1 = F.BitwiseAndNot_m32(y_ge_z, x_ge_y);
            mask32v k1 = F.BitwiseAndNot_m32(F.Complement(x_ge_z), y_ge_z);

            mask32v i2 = F.Or(x_ge_y, x_ge_z);
            mask32v j2 = F.Or(F.Complement(x_ge_y), y_ge_z);
            mask32v k2 = F.And(x_ge_z, y_ge_z); //NMasked

            float32v x1 = F.Add(FSS.MaskedSub_f32(x0, F.Broad_f32(1), i1), F.Broad_f32(G3));
            float32v y1 = F.Add(FSS.MaskedSub_f32(y0, F.Broad_f32(1), j1), F.Broad_f32(G3));
            float32v z1 = F.Add(FSS.MaskedSub_f32(z0, F.Broad_f32(1), k1), F.Broad_f32(G3));
            float32v x2 = F.Add(FSS.MaskedSub_f32(x0, F.Broad_f32(1), i2), F.Broad_f32(G3 * 2));
            float32v y2 = F.Add(FSS.MaskedSub_f32(y0, F.Broad_f32(1), j2), F.Broad_f32(G3 * 2));
            float32v z2 = F.Add(FSS.NMaskedSub_f32(z0, F.Broad_f32(1), k2), F.Broad_f32(G3 * 2));
            float32v x3 = F.Add(x0, F.Broad_f32(G3 * 3 - 1));
            float32v y3 = F.Add(y0, F.Broad_f32(G3 * 3 - 1));
            float32v z3 = F.Add(z0, F.Broad_f32(G3 * 3 - 1));

            float32v t0 = F.FNMulAdd_f32(x0, x0, F.FNMulAdd_f32(y0, y0, F.FNMulAdd_f32(z0, z0, F.Broad_f32(0.6f))));
            float32v t1 = F.FNMulAdd_f32(x1, x1, F.FNMulAdd_f32(y1, y1, F.FNMulAdd_f32(z1, z1, F.Broad_f32(0.6f))));
            float32v t2 = F.FNMulAdd_f32(x2, x2, F.FNMulAdd_f32(y2, y2, F.FNMulAdd_f32(z2, z2, F.Broad_f32(0.6f))));
            float32v t3 = F.FNMulAdd_f32(x3, x3, F.FNMulAdd_f32(y3, y3, F.FNMulAdd_f32(z3, z3, F.Broad_f32(0.6f))));

            t0 = F.Max_f32(t0, F.Broad_f32(0));
            t1 = F.Max_f32(t1, F.Broad_f32(0));
            t2 = F.Max_f32(t2, F.Broad_f32(0));
            t3 = F.Max_f32(t3, F.Broad_f32(0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);
            t3 = F.Mul(t3, t3);
            t3 = F.Mul(t3, t3);

            float32v n0 = Utils.GetGradientDot(
                Utils.HashPrimes(seed, i, j, k), x0, y0, z0);

            float32v n1 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    FSS.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j1),
                    FSS.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k1)),
                x1, y1, z1);

            float32v n2 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i2),
                    FSS.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j2),
                    FSS.NMaskedAdd_i32(k, F.Broad_i32(Primes.Z), k2)),
                x2, y2, z2);

            float32v n3 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y)),
                    F.Add(k, F.Broad_i32(Primes.Z))),
                x3, y3, z3);

            float32v last = F.FMulAdd_f32(n0, t0, F.FMulAdd_f32(n1, t1, F.FMulAdd_f32(n2, t2, F.Mul(n3, t3))));
            return F.Mul(F.Broad_f32(32.69428253173828125f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z, float32v w)
        {
            const float SQRT5 = 2.236067977499f;
            const float F4 = (SQRT5 - 1.0f) / 4.0f;
            const float G4 = (5.0f - SQRT5) / 20.0f;

            float32v s = F.Mul(F.Broad_f32(F4), F.Add(F.Add(F.Add(x, y), z), w));
            x = F.Add(x, s);
            y = F.Add(y, s);
            z = F.Add(z, s);
            w = F.Add(w, s);

            float32v x0 = F.Floor_f32(x);
            float32v y0 = F.Floor_f32(y);
            float32v z0 = F.Floor_f32(z);
            float32v w0 = F.Floor_f32(w);
            float32v xi = F.Sub(x, x0);
            float32v yi = F.Sub(y, y0);
            float32v zi = F.Sub(z, z0);
            float32v wi = F.Sub(w, w0);

            int32v i = F.Mul(F.Convertf32_i32(x0), F.Broad_i32(Primes.X));
            int32v j = F.Mul(F.Convertf32_i32(y0), F.Broad_i32(Primes.Y));
            int32v k = F.Mul(F.Convertf32_i32(z0), F.Broad_i32(Primes.Z));
            int32v l = F.Mul(F.Convertf32_i32(w0), F.Broad_i32(Primes.W));

            float32v g = F.Mul(F.Broad_f32(G4), F.Add(F.Add(F.Add(xi, yi), zi), wi));
            x0 = F.Sub(xi, g);
            y0 = F.Sub(yi, g);
            z0 = F.Sub(zi, g);
            w0 = F.Sub(wi, g);

            int32v rankx = F.Broad_i32(0);
            int32v ranky = F.Broad_i32(0);
            int32v rankz = F.Broad_i32(0);
            int32v rankw = F.Broad_i32(0);

            mask32v x_ge_y = F.GreaterThanOrEqual(x0, y0);
            rankx = FSS.MaskedIncrement_i32(rankx, x_ge_y);
            ranky = FSS.MaskedIncrement_i32(ranky, F.Complement(x_ge_y));

            mask32v x_ge_z = F.GreaterThanOrEqual(x0, z0);
            rankx = FSS.MaskedIncrement_i32(rankx, x_ge_z);
            rankz = FSS.MaskedIncrement_i32(rankz, F.Complement(x_ge_z));

            mask32v x_ge_w = F.GreaterThanOrEqual(x0, w0);
            rankx = FSS.MaskedIncrement_i32(rankx, x_ge_w);
            rankw = FSS.MaskedIncrement_i32(rankw, F.Complement(x_ge_w));

            mask32v y_ge_z = F.GreaterThanOrEqual(y0, z0);
            ranky = FSS.MaskedIncrement_i32(ranky, y_ge_z);
            rankz = FSS.MaskedIncrement_i32(rankz, F.Complement(y_ge_z));

            mask32v y_ge_w = F.GreaterThanOrEqual(y0, w0);
            ranky = FSS.MaskedIncrement_i32(ranky, y_ge_w);
            rankw = FSS.MaskedIncrement_i32(rankw, F.Complement(y_ge_w));

            mask32v z_ge_w = F.GreaterThanOrEqual(z0, w0);
            rankz = FSS.MaskedIncrement_i32(rankz, z_ge_w);
            rankw = FSS.MaskedIncrement_i32(rankw, F.Complement(z_ge_w));

            mask32v i1 = F.GreaterThan(rankx, F.Broad_i32(2));
            mask32v j1 = F.GreaterThan(ranky, F.Broad_i32(2));
            mask32v k1 = F.GreaterThan(rankz, F.Broad_i32(2));
            mask32v l1 = F.GreaterThan(rankw, F.Broad_i32(2));

            mask32v i2 = F.GreaterThan(rankx, F.Broad_i32(1));
            mask32v j2 = F.GreaterThan(ranky, F.Broad_i32(1));
            mask32v k2 = F.GreaterThan(rankz, F.Broad_i32(1));
            mask32v l2 = F.GreaterThan(rankw, F.Broad_i32(1));

            mask32v i3 = F.GreaterThan(rankx, F.Broad_i32(0));
            mask32v j3 = F.GreaterThan(ranky, F.Broad_i32(0));
            mask32v k3 = F.GreaterThan(rankz, F.Broad_i32(0));
            mask32v l3 = F.GreaterThan(rankw, F.Broad_i32(0));

            float32v x1 = F.Add(FSS.MaskedSub_f32(x0, F.Broad_f32(1), i1), F.Broad_f32(G4));
            float32v y1 = F.Add(FSS.MaskedSub_f32(y0, F.Broad_f32(1), j1), F.Broad_f32(G4));
            float32v z1 = F.Add(FSS.MaskedSub_f32(z0, F.Broad_f32(1), k1), F.Broad_f32(G4));
            float32v w1 = F.Add(FSS.MaskedSub_f32(w0, F.Broad_f32(1), l1), F.Broad_f32(G4));
            float32v x2 = F.Add(FSS.MaskedSub_f32(x0, F.Broad_f32(1), i2), F.Broad_f32(G4 * 2));
            float32v y2 = F.Add(FSS.MaskedSub_f32(y0, F.Broad_f32(1), j2), F.Broad_f32(G4 * 2));
            float32v z2 = F.Add(FSS.MaskedSub_f32(z0, F.Broad_f32(1), k2), F.Broad_f32(G4 * 2));
            float32v w2 = F.Add(FSS.MaskedSub_f32(w0, F.Broad_f32(1), l2), F.Broad_f32(G4 * 2));
            float32v x3 = F.Add(FSS.MaskedSub_f32(x0, F.Broad_f32(1), i3), F.Broad_f32(G4 * 3));
            float32v y3 = F.Add(FSS.MaskedSub_f32(y0, F.Broad_f32(1), j3), F.Broad_f32(G4 * 3));
            float32v z3 = F.Add(FSS.MaskedSub_f32(z0, F.Broad_f32(1), k3), F.Broad_f32(G4 * 3));
            float32v w3 = F.Add(FSS.MaskedSub_f32(w0, F.Broad_f32(1), l3), F.Broad_f32(G4 * 3));
            float32v x4 = F.Add(x0, F.Broad_f32(G4 * 4 - 1));
            float32v y4 = F.Add(y0, F.Broad_f32(G4 * 4 - 1));
            float32v z4 = F.Add(z0, F.Broad_f32(G4 * 4 - 1));
            float32v w4 = F.Add(w0, F.Broad_f32(G4 * 4 - 1));

            float32v t0 = F.FNMulAdd_f32(
                x0, x0, F.FNMulAdd_f32(y0, y0, F.FNMulAdd_f32(z0, z0, F.FNMulAdd_f32(w0, w0, F.Broad_f32(0.6f)))));

            float32v t1 = F.FNMulAdd_f32(
                x1, x1, F.FNMulAdd_f32(y1, y1, F.FNMulAdd_f32(z1, z1, F.FNMulAdd_f32(w1, w1, F.Broad_f32(0.6f)))));

            float32v t2 = F.FNMulAdd_f32(
                x2, x2, F.FNMulAdd_f32(y2, y2, F.FNMulAdd_f32(z2, z2, F.FNMulAdd_f32(w2, w2, F.Broad_f32(0.6f)))));

            float32v t3 = F.FNMulAdd_f32(
                x3, x3, F.FNMulAdd_f32(y3, y3, F.FNMulAdd_f32(z3, z3, F.FNMulAdd_f32(w3, w3, F.Broad_f32(0.6f)))));

            float32v t4 = F.FNMulAdd_f32(
                x4, x4, F.FNMulAdd_f32(y4, y4, F.FNMulAdd_f32(z4, z4, F.FNMulAdd_f32(w4, w4, F.Broad_f32(0.6f)))));

            t0 = F.Max_f32(t0, F.Broad_f32(0));
            t1 = F.Max_f32(t1, F.Broad_f32(0));
            t2 = F.Max_f32(t2, F.Broad_f32(0));
            t3 = F.Max_f32(t3, F.Broad_f32(0));
            t4 = F.Max_f32(t4, F.Broad_f32(0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);
            t3 = F.Mul(t3, t3);
            t3 = F.Mul(t3, t3);
            t4 = F.Mul(t4, t4);
            t4 = F.Mul(t4, t4);

            float32v n0 = Utils.GetGradientDot(Utils.HashPrimes(seed, i, j, k, l), x0, y0, z0, w0);

            float32v n1 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    FSS.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j1),
                    FSS.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k1),
                    FSS.MaskedAdd_i32(l, F.Broad_i32(Primes.W), l1)),
                x1, y1, z1, w1);

            float32v n2 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i2),
                    FSS.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j2),
                    FSS.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k2),
                    FSS.MaskedAdd_i32(l, F.Broad_i32(Primes.W), l2)),
                x2, y2, z2, w2);

            float32v n3 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i3),
                    FSS.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j3),
                    FSS.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k3),
                    FSS.MaskedAdd_i32(l, F.Broad_i32(Primes.W), l3)),
                x3, y3, z3, w3);

            float32v n4 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y)),
                    F.Add(k, F.Broad_i32(Primes.Z)),
                    F.Add(l, F.Broad_i32(Primes.W))),
                x4, y4, z4, w4);

            float32v last = F.FMulAdd_f32(
                n0,
                t0,
                F.FMulAdd_f32(n1, t1, F.FMulAdd_f32(n2, t2, F.FMulAdd_f32(n3, t3, F.Mul(n4, t4)))));

            return F.Mul(F.Broad_f32(27f), last);
        }
    }
}
