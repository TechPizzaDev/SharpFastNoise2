using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct Simplex<mask32v, float32v, int32v, TFunctions> :
        //INoiseGenerator1D<mask32v, float32v, int32v>,
        INoiseGenerator2D<mask32v, float32v, int32v>,
        INoiseGenerator3D<mask32v, float32v, int32v>,
        INoiseGenerator4D<mask32v, float32v, int32v>
        where mask32v : unmanaged, IFMask<mask32v>
        where float32v : unmanaged, IFVector<float32v, mask32v>
        where int32v : unmanaged, IFVector<int32v, mask32v>
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions FS = default;
        private static Utils<mask32v, float32v, int32v, TFunctions> Utils = default;
        private static FastSimd<mask32v, float32v, int32v, TFunctions> FSS = default;

        public int Count => default(float32v).Count;

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

            float32v x2 = x0.Add(FS.Broad_f32(G2 * 2 - 1));
            float32v y2 = y0.Add(FS.Broad_f32(G2 * 2 - 1));

            float32v t0 = FS.FNMulAdd_f32(x0, x0, FS.FNMulAdd_f32(y0, y0, FS.Broad_f32(0.5f)));
            float32v t1 = FS.FNMulAdd_f32(x1, x1, FS.FNMulAdd_f32(y1, y1, FS.Broad_f32(0.5f)));
            float32v t2 = FS.FNMulAdd_f32(x2, x2, FS.FNMulAdd_f32(y2, y2, FS.Broad_f32(0.5f)));

            t0 = FS.Max_f32(t0, FS.Broad_f32(0));
            t1 = FS.Max_f32(t1, FS.Broad_f32(0));
            t2 = FS.Max_f32(t2, FS.Broad_f32(0));

            t0 = t0.Mul(t0);
            t0 = t0.Mul(t0);
            t1 = t1.Mul(t1);
            t1 = t1.Mul(t1);
            t2 = t2.Mul(t2);
            t2 = t2.Mul(t2);

            float32v n0 = Utils.GetGradientDot(
                Utils.HashPrimes(seed, i, j),
                x0, y0);

            float32v n1 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i1),
                    FSS.NMaskedAdd_i32(j, FS.Broad_i32(Primes.Y), i1)),
                x1, y1);

            float32v n2 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    i.Add(FS.Broad_i32(Primes.X)),
                    j.Add(FS.Broad_i32(Primes.Y))),
                x2, y2);

            float32v last = FS.FMulAdd_f32(n0, t0, FS.FMulAdd_f32(n1, t1, n2.Mul(t2)));
            return FS.Broad_f32(38.283687591552734375f).Mul(last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        {
            const float F3 = 1.0f / 3.0f;
            const float G3 = 1.0f / 2.0f;

            float32v s = FS.Broad_f32(F3).Mul(x.Add(y).Add(z));
            x = x.Add(s);
            y = y.Add(s);
            z = z.Add(s);

            float32v x0 = FS.Floor_f32(x);
            float32v y0 = FS.Floor_f32(y);
            float32v z0 = FS.Floor_f32(z);
            float32v xi = x.Sub(x0);
            float32v yi = y.Sub(y0);
            float32v zi = z.Sub(z0);

            int32v i = FS.Convertf32_i32(x0).Mul(FS.Broad_i32(Primes.X));
            int32v j = FS.Convertf32_i32(y0).Mul(FS.Broad_i32(Primes.Y));
            int32v k = FS.Convertf32_i32(z0).Mul(FS.Broad_i32(Primes.Z));

            mask32v x_ge_y = xi.GreaterThanOrEqual(yi);
            mask32v y_ge_z = yi.GreaterThanOrEqual(zi);
            mask32v x_ge_z = xi.GreaterThanOrEqual(zi);

            float32v g = FS.Broad_f32(G3).Mul(xi.Add(yi).Add(zi));
            x0 = xi.Sub(g);
            y0 = yi.Sub(g);
            z0 = zi.Sub(g);

            mask32v i1 = x_ge_y.And(x_ge_z);
            mask32v j1 = FSS.BitwiseAndNot_m32(y_ge_z, x_ge_y);
            mask32v k1 = FSS.BitwiseAndNot_m32(x_ge_z.Complement(), y_ge_z);

            mask32v i2 = x_ge_y.Or(x_ge_z);
            mask32v j2 = x_ge_y.Complement().Or(y_ge_z);
            mask32v k2 = x_ge_z.And(y_ge_z); //NMasked

            float32v x1 = FSS.MaskedSub_f32(x0, FS.Broad_f32(1), i1).Add(FS.Broad_f32(G3));
            float32v y1 = FSS.MaskedSub_f32(y0, FS.Broad_f32(1), j1).Add(FS.Broad_f32(G3));
            float32v z1 = FSS.MaskedSub_f32(z0, FS.Broad_f32(1), k1).Add(FS.Broad_f32(G3));
            float32v x2 = FSS.MaskedSub_f32(x0, FS.Broad_f32(1), i2).Add(FS.Broad_f32(G3 * 2));
            float32v y2 = FSS.MaskedSub_f32(y0, FS.Broad_f32(1), j2).Add(FS.Broad_f32(G3 * 2));
            float32v z2 = FSS.NMaskedSub_f32(z0, FS.Broad_f32(1), k2).Add(FS.Broad_f32(G3 * 2));
            float32v x3 = x0.Add(FS.Broad_f32(G3 * 3 - 1));
            float32v y3 = y0.Add(FS.Broad_f32(G3 * 3 - 1));
            float32v z3 = z0.Add(FS.Broad_f32(G3 * 3 - 1));

            float32v t0 = FS.FNMulAdd_f32(x0, x0, FS.FNMulAdd_f32(y0, y0, FS.FNMulAdd_f32(z0, z0, FS.Broad_f32(0.6f))));
            float32v t1 = FS.FNMulAdd_f32(x1, x1, FS.FNMulAdd_f32(y1, y1, FS.FNMulAdd_f32(z1, z1, FS.Broad_f32(0.6f))));
            float32v t2 = FS.FNMulAdd_f32(x2, x2, FS.FNMulAdd_f32(y2, y2, FS.FNMulAdd_f32(z2, z2, FS.Broad_f32(0.6f))));
            float32v t3 = FS.FNMulAdd_f32(x3, x3, FS.FNMulAdd_f32(y3, y3, FS.FNMulAdd_f32(z3, z3, FS.Broad_f32(0.6f))));

            t0 = FS.Max_f32(t0, FS.Broad_f32(0));
            t1 = FS.Max_f32(t1, FS.Broad_f32(0));
            t2 = FS.Max_f32(t2, FS.Broad_f32(0));
            t3 = FS.Max_f32(t3, FS.Broad_f32(0));

            t0 = t0.Mul(t0);
            t0 = t0.Mul(t0);
            t1 = t1.Mul(t1);
            t1 = t1.Mul(t1);
            t2 = t2.Mul(t2);
            t2 = t2.Mul(t2);
            t3 = t3.Mul(t3);
            t3 = t3.Mul(t3);

            float32v n0 = Utils.GetGradientDot(
                Utils.HashPrimes(seed, i, j, k), x0, y0, z0);

            float32v n1 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i1),
                    FSS.MaskedAdd_i32(j, FS.Broad_i32(Primes.Y), j1),
                    FSS.MaskedAdd_i32(k, FS.Broad_i32(Primes.Z), k1)),
                x1, y1, z1);

            float32v n2 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i2),
                    FSS.MaskedAdd_i32(j, FS.Broad_i32(Primes.Y), j2),
                    FSS.NMaskedAdd_i32(k, FS.Broad_i32(Primes.Z), k2)),
                x2, y2, z2);

            float32v n3 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    i.Add(FS.Broad_i32(Primes.X)),
                    j.Add(FS.Broad_i32(Primes.Y)),
                    k.Add(FS.Broad_i32(Primes.Z))),
                x3, y3, z3);

            float32v last = FS.FMulAdd_f32(n0, t0, FS.FMulAdd_f32(n1, t1, FS.FMulAdd_f32(n2, t2, n3.Mul(t3))));
            return FS.Broad_f32(32.69428253173828125f).Mul(last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z, float32v w)
        {
            const float SQRT5 = 2.236067977499f;
            const float F4 = (SQRT5 - 1.0f) / 4.0f;
            const float G4 = (5.0f - SQRT5) / 20.0f;

            float32v s = FS.Broad_f32(F4).Mul(x.Add(y).Add(z).Add(w));
            x = x.Add(s);
            y = y.Add(s);
            z = z.Add(s);
            w = w.Add(s);

            float32v x0 = FS.Floor_f32(x);
            float32v y0 = FS.Floor_f32(y);
            float32v z0 = FS.Floor_f32(z);
            float32v w0 = FS.Floor_f32(w);
            float32v xi = x.Sub(x0);
            float32v yi = y.Sub(y0);
            float32v zi = z.Sub(z0);
            float32v wi = w.Sub(w0);

            int32v i = FS.Convertf32_i32(x0).Mul(FS.Broad_i32(Primes.X));
            int32v j = FS.Convertf32_i32(y0).Mul(FS.Broad_i32(Primes.Y));
            int32v k = FS.Convertf32_i32(z0).Mul(FS.Broad_i32(Primes.Z));
            int32v l = FS.Convertf32_i32(w0).Mul(FS.Broad_i32(Primes.W));

            float32v g = FS.Broad_f32(G4).Mul(xi.Add(yi).Add(zi).Add(wi));
            x0 = xi.Sub(g);
            y0 = yi.Sub(g);
            z0 = zi.Sub(g);
            w0 = wi.Sub(g);

            int32v rankx = FS.Broad_i32(0);
            int32v ranky = FS.Broad_i32(0);
            int32v rankz = FS.Broad_i32(0);
            int32v rankw = FS.Broad_i32(0);

            mask32v x_ge_y = x0.GreaterThanOrEqual(y0);
            rankx = FSS.MaskedIncrement_i32(rankx, x_ge_y);
            ranky = FSS.MaskedIncrement_i32(ranky, x_ge_y.Complement());

            mask32v x_ge_z = x0.GreaterThanOrEqual(z0);
            rankx = FSS.MaskedIncrement_i32(rankx, x_ge_z);
            rankz = FSS.MaskedIncrement_i32(rankz, x_ge_z.Complement());

            mask32v x_ge_w = x0.GreaterThanOrEqual(w0);
            rankx = FSS.MaskedIncrement_i32(rankx, x_ge_w);
            rankw = FSS.MaskedIncrement_i32(rankw, x_ge_w.Complement());

            mask32v y_ge_z = y0.GreaterThanOrEqual(z0);
            ranky = FSS.MaskedIncrement_i32(ranky, y_ge_z);
            rankz = FSS.MaskedIncrement_i32(rankz, y_ge_z.Complement());

            mask32v y_ge_w = y0.GreaterThanOrEqual(w0);
            ranky = FSS.MaskedIncrement_i32(ranky, y_ge_w);
            rankw = FSS.MaskedIncrement_i32(rankw, y_ge_w.Complement());

            mask32v z_ge_w = z0.GreaterThanOrEqual(w0);
            rankz = FSS.MaskedIncrement_i32(rankz, z_ge_w);
            rankw = FSS.MaskedIncrement_i32(rankw, z_ge_w.Complement());

            mask32v i1 = rankx.GreaterThan(FS.Broad_i32(2));
            mask32v j1 = ranky.GreaterThan(FS.Broad_i32(2));
            mask32v k1 = rankz.GreaterThan(FS.Broad_i32(2));
            mask32v l1 = rankw.GreaterThan(FS.Broad_i32(2));

            mask32v i2 = rankx.GreaterThan(FS.Broad_i32(1));
            mask32v j2 = ranky.GreaterThan(FS.Broad_i32(1));
            mask32v k2 = rankz.GreaterThan(FS.Broad_i32(1));
            mask32v l2 = rankw.GreaterThan(FS.Broad_i32(1));

            mask32v i3 = rankx.GreaterThan(FS.Broad_i32(0));
            mask32v j3 = ranky.GreaterThan(FS.Broad_i32(0));
            mask32v k3 = rankz.GreaterThan(FS.Broad_i32(0));
            mask32v l3 = rankw.GreaterThan(FS.Broad_i32(0));

            float32v x1 = FSS.MaskedSub_f32(x0, FS.Broad_f32(1), i1).Add(FS.Broad_f32(G4));
            float32v y1 = FSS.MaskedSub_f32(y0, FS.Broad_f32(1), j1).Add(FS.Broad_f32(G4));
            float32v z1 = FSS.MaskedSub_f32(z0, FS.Broad_f32(1), k1).Add(FS.Broad_f32(G4));
            float32v w1 = FSS.MaskedSub_f32(w0, FS.Broad_f32(1), l1).Add(FS.Broad_f32(G4));
            float32v x2 = FSS.MaskedSub_f32(x0, FS.Broad_f32(1), i2).Add(FS.Broad_f32(G4 * 2));
            float32v y2 = FSS.MaskedSub_f32(y0, FS.Broad_f32(1), j2).Add(FS.Broad_f32(G4 * 2));
            float32v z2 = FSS.MaskedSub_f32(z0, FS.Broad_f32(1), k2).Add(FS.Broad_f32(G4 * 2));
            float32v w2 = FSS.MaskedSub_f32(w0, FS.Broad_f32(1), l2).Add(FS.Broad_f32(G4 * 2));
            float32v x3 = FSS.MaskedSub_f32(x0, FS.Broad_f32(1), i3).Add(FS.Broad_f32(G4 * 3));
            float32v y3 = FSS.MaskedSub_f32(y0, FS.Broad_f32(1), j3).Add(FS.Broad_f32(G4 * 3));
            float32v z3 = FSS.MaskedSub_f32(z0, FS.Broad_f32(1), k3).Add(FS.Broad_f32(G4 * 3));
            float32v w3 = FSS.MaskedSub_f32(w0, FS.Broad_f32(1), l3).Add(FS.Broad_f32(G4 * 3));
            float32v x4 = x0.Add(FS.Broad_f32(G4 * 4 - 1));
            float32v y4 = y0.Add(FS.Broad_f32(G4 * 4 - 1));
            float32v z4 = z0.Add(FS.Broad_f32(G4 * 4 - 1));
            float32v w4 = w0.Add(FS.Broad_f32(G4 * 4 - 1));

            float32v t0 = FS.FNMulAdd_f32(
                x0, x0, FS.FNMulAdd_f32(y0, y0, FS.FNMulAdd_f32(z0, z0, FS.FNMulAdd_f32(w0, w0, FS.Broad_f32(0.6f)))));

            float32v t1 = FS.FNMulAdd_f32(
                x1, x1, FS.FNMulAdd_f32(y1, y1, FS.FNMulAdd_f32(z1, z1, FS.FNMulAdd_f32(w1, w1, FS.Broad_f32(0.6f)))));

            float32v t2 = FS.FNMulAdd_f32(
                x2, x2, FS.FNMulAdd_f32(y2, y2, FS.FNMulAdd_f32(z2, z2, FS.FNMulAdd_f32(w2, w2, FS.Broad_f32(0.6f)))));

            float32v t3 = FS.FNMulAdd_f32(
                x3, x3, FS.FNMulAdd_f32(y3, y3, FS.FNMulAdd_f32(z3, z3, FS.FNMulAdd_f32(w3, w3, FS.Broad_f32(0.6f)))));

            float32v t4 = FS.FNMulAdd_f32(
                x4, x4, FS.FNMulAdd_f32(y4, y4, FS.FNMulAdd_f32(z4, z4, FS.FNMulAdd_f32(w4, w4, FS.Broad_f32(0.6f)))));

            t0 = FS.Max_f32(t0, FS.Broad_f32(0));
            t1 = FS.Max_f32(t1, FS.Broad_f32(0));
            t2 = FS.Max_f32(t2, FS.Broad_f32(0));
            t3 = FS.Max_f32(t3, FS.Broad_f32(0));
            t4 = FS.Max_f32(t4, FS.Broad_f32(0));

            t0 = t0.Mul(t0);
            t0 = t0.Mul(t0);
            t1 = t1.Mul(t1);
            t1 = t1.Mul(t1);
            t2 = t2.Mul(t2);
            t2 = t2.Mul(t2);
            t3 = t3.Mul(t3);
            t3 = t3.Mul(t3);
            t4 = t4.Mul(t4);
            t4 = t4.Mul(t4);

            float32v n0 = Utils.GetGradientDot(Utils.HashPrimes(seed, i, j, k, l), x0, y0, z0, w0);

            float32v n1 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i1),
                    FSS.MaskedAdd_i32(j, FS.Broad_i32(Primes.Y), j1),
                    FSS.MaskedAdd_i32(k, FS.Broad_i32(Primes.Z), k1),
                    FSS.MaskedAdd_i32(l, FS.Broad_i32(Primes.W), l1)),
                x1, y1, z1, w1);

            float32v n2 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i2),
                    FSS.MaskedAdd_i32(j, FS.Broad_i32(Primes.Y), j2),
                    FSS.MaskedAdd_i32(k, FS.Broad_i32(Primes.Z), k2),
                    FSS.MaskedAdd_i32(l, FS.Broad_i32(Primes.W), l2)),
                x2, y2, z2, w2);

            float32v n3 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    FSS.MaskedAdd_i32(i, FS.Broad_i32(Primes.X), i3),
                    FSS.MaskedAdd_i32(j, FS.Broad_i32(Primes.Y), j3),
                    FSS.MaskedAdd_i32(k, FS.Broad_i32(Primes.Z), k3),
                    FSS.MaskedAdd_i32(l, FS.Broad_i32(Primes.W), l3)),
                x3, y3, z3, w3);

            float32v n4 = Utils.GetGradientDot(
                Utils.HashPrimes(
                    seed,
                    i.Add(FS.Broad_i32(Primes.X)),
                    j.Add(FS.Broad_i32(Primes.Y)),
                    k.Add(FS.Broad_i32(Primes.Z)),
                    l.Add(FS.Broad_i32(Primes.W))),
                x4, y4, z4, w4);

            float32v last = FS.FMulAdd_f32(
                n0,
                t0,
                FS.FMulAdd_f32(n1, t1, FS.FMulAdd_f32(n2, t2, FS.FMulAdd_f32(n3, t3, n4.Mul(t4)))));

            return FS.Broad_f32(27f).Mul(last);
        }
    }
}
