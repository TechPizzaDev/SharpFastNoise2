using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public struct Simplex<m32, f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32>
    {
        public static int UnitSize => F.Count;

        readonly int INoiseGenerator.UnitSize => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            const float SQRT3 = 1.7320508075688772935274463415059f;
            const float F2 = 0.5f * (SQRT3 - 1.0f);
            const float G2 = (3.0f - SQRT3) / 6.0f;

            f32 f = F.Mul(F.Broad_f32(F2), F.Add(x, y));
            f32 x0 = F.Floor_f32(F.Add(x, f));
            f32 y0 = F.Floor_f32(F.Add(y, f));

            i32 i = F.Mul(F.Convertf32_i32(x0), F.Broad_i32(Primes.X));
            i32 j = F.Mul(F.Convertf32_i32(y0), F.Broad_i32(Primes.Y));

            f32 g = F.Mul(F.Broad_f32(G2), F.Add(x0, y0));
            x0 = F.Sub(x, F.Sub(x0, g));
            y0 = F.Sub(y, F.Sub(y0, g));

            m32 i1 = F.GreaterThan(x0, y0);
            //m32 j1 = ~i1; //NMasked funcs

            f32 x1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(x0, F.Broad_f32(1f), i1), F.Broad_f32(G2));
            f32 y1 = F.Add(Utils<m32, f32, i32, F>.NMaskedSub_f32(y0, F.Broad_f32(1f), i1), F.Broad_f32(G2));

            f32 x2 = F.Add(x0, F.Broad_f32(G2 * 2 - 1));
            f32 y2 = F.Add(y0, F.Broad_f32(G2 * 2 - 1));

            f32 t0 = F.FNMulAdd_f32(x0, x0, F.FNMulAdd_f32(y0, y0, F.Broad_f32(0.5f)));
            f32 t1 = F.FNMulAdd_f32(x1, x1, F.FNMulAdd_f32(y1, y1, F.Broad_f32(0.5f)));
            f32 t2 = F.FNMulAdd_f32(x2, x2, F.FNMulAdd_f32(y2, y2, F.Broad_f32(0.5f)));

            t0 = F.Max_f32(t0, F.Broad_f32(0));
            t1 = F.Max_f32(t1, F.Broad_f32(0));
            t2 = F.Max_f32(t2, F.Broad_f32(0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);

            f32 n0 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(seed, i, j),
                x0, y0);

            f32 n1 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    Utils<m32, f32, i32, F>.NMaskedAdd_i32(j, F.Broad_i32(Primes.Y), i1)),
                x1, y1);

            f32 n2 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y))),
                x2, y2);

            f32 last = F.FMulAdd_f32(n0, t0, F.FMulAdd_f32(n1, t1, F.Mul(n2, t2)));
            return F.Mul(F.Broad_f32(38.283687591552734375f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            const float F3 = 1.0f / 3.0f;
            const float G3 = 1.0f / 2.0f;

            f32 s = F.Mul(F.Broad_f32(F3), F.Add(F.Add(x, y), z));
            x = F.Add(x, s);
            y = F.Add(y, s);
            z = F.Add(z, s);

            f32 x0 = F.Floor_f32(x);
            f32 y0 = F.Floor_f32(y);
            f32 z0 = F.Floor_f32(z);
            f32 xi = F.Sub(x, x0);
            f32 yi = F.Sub(y, y0);
            f32 zi = F.Sub(z, z0);

            i32 i = F.Mul(F.Convertf32_i32(x0), F.Broad_i32(Primes.X));
            i32 j = F.Mul(F.Convertf32_i32(y0), F.Broad_i32(Primes.Y));
            i32 k = F.Mul(F.Convertf32_i32(z0), F.Broad_i32(Primes.Z));

            m32 x_ge_y = F.GreaterThanOrEqual(xi, yi);
            m32 y_ge_z = F.GreaterThanOrEqual(yi, zi);
            m32 x_ge_z = F.GreaterThanOrEqual(xi, zi);

            f32 g = F.Mul(F.Broad_f32(G3), F.Add(F.Add(xi, yi), zi));
            x0 = F.Sub(xi, g);
            y0 = F.Sub(yi, g);
            z0 = F.Sub(zi, g);

            m32 i1 = F.And(x_ge_y, x_ge_z);
            m32 j1 = F.BitwiseAndNot_m32(y_ge_z, x_ge_y);
            m32 k1 = F.BitwiseAndNot_m32(F.Complement(x_ge_z), y_ge_z);

            m32 i2 = F.Or(x_ge_y, x_ge_z);
            m32 j2 = F.Or(F.Complement(x_ge_y), y_ge_z);
            m32 k2 = F.And(x_ge_z, y_ge_z); //NMasked

            f32 x1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(x0, F.Broad_f32(1), i1), F.Broad_f32(G3));
            f32 y1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(y0, F.Broad_f32(1), j1), F.Broad_f32(G3));
            f32 z1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(z0, F.Broad_f32(1), k1), F.Broad_f32(G3));
            f32 x2 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(x0, F.Broad_f32(1), i2), F.Broad_f32(G3 * 2));
            f32 y2 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(y0, F.Broad_f32(1), j2), F.Broad_f32(G3 * 2));
            f32 z2 = F.Add(Utils<m32, f32, i32, F>.NMaskedSub_f32(z0, F.Broad_f32(1), k2), F.Broad_f32(G3 * 2));
            f32 x3 = F.Add(x0, F.Broad_f32(G3 * 3 - 1));
            f32 y3 = F.Add(y0, F.Broad_f32(G3 * 3 - 1));
            f32 z3 = F.Add(z0, F.Broad_f32(G3 * 3 - 1));

            f32 t0 = F.FNMulAdd_f32(x0, x0, F.FNMulAdd_f32(y0, y0, F.FNMulAdd_f32(z0, z0, F.Broad_f32(0.6f))));
            f32 t1 = F.FNMulAdd_f32(x1, x1, F.FNMulAdd_f32(y1, y1, F.FNMulAdd_f32(z1, z1, F.Broad_f32(0.6f))));
            f32 t2 = F.FNMulAdd_f32(x2, x2, F.FNMulAdd_f32(y2, y2, F.FNMulAdd_f32(z2, z2, F.Broad_f32(0.6f))));
            f32 t3 = F.FNMulAdd_f32(x3, x3, F.FNMulAdd_f32(y3, y3, F.FNMulAdd_f32(z3, z3, F.Broad_f32(0.6f))));

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

            f32 n0 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(seed, i, j, k), x0, y0, z0);

            f32 n1 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j1),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k1)),
                x1, y1, z1);

            f32 n2 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i2),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j2),
                    Utils<m32, f32, i32, F>.NMaskedAdd_i32(k, F.Broad_i32(Primes.Z), k2)),
                x2, y2, z2);

            f32 n3 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y)),
                    F.Add(k, F.Broad_i32(Primes.Z))),
                x3, y3, z3);

            f32 last = F.FMulAdd_f32(n0, t0, F.FMulAdd_f32(n1, t1, F.FMulAdd_f32(n2, t2, F.Mul(n3, t3))));
            return F.Mul(F.Broad_f32(32.69428253173828125f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
        {
            const float SQRT5 = 2.236067977499f;
            const float F4 = (SQRT5 - 1.0f) / 4.0f;
            const float G4 = (5.0f - SQRT5) / 20.0f;

            f32 s = F.Mul(F.Broad_f32(F4), F.Add(F.Add(F.Add(x, y), z), w));
            x = F.Add(x, s);
            y = F.Add(y, s);
            z = F.Add(z, s);
            w = F.Add(w, s);

            f32 x0 = F.Floor_f32(x);
            f32 y0 = F.Floor_f32(y);
            f32 z0 = F.Floor_f32(z);
            f32 w0 = F.Floor_f32(w);
            f32 xi = F.Sub(x, x0);
            f32 yi = F.Sub(y, y0);
            f32 zi = F.Sub(z, z0);
            f32 wi = F.Sub(w, w0);

            i32 i = F.Mul(F.Convertf32_i32(x0), F.Broad_i32(Primes.X));
            i32 j = F.Mul(F.Convertf32_i32(y0), F.Broad_i32(Primes.Y));
            i32 k = F.Mul(F.Convertf32_i32(z0), F.Broad_i32(Primes.Z));
            i32 l = F.Mul(F.Convertf32_i32(w0), F.Broad_i32(Primes.W));

            f32 g = F.Mul(F.Broad_f32(G4), F.Add(F.Add(F.Add(xi, yi), zi), wi));
            x0 = F.Sub(xi, g);
            y0 = F.Sub(yi, g);
            z0 = F.Sub(zi, g);
            w0 = F.Sub(wi, g);

            i32 rankx = F.Broad_i32(0);
            i32 ranky = F.Broad_i32(0);
            i32 rankz = F.Broad_i32(0);
            i32 rankw = F.Broad_i32(0);

            m32 x_ge_y = F.GreaterThanOrEqual(x0, y0);
            rankx = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankx, x_ge_y);
            ranky = Utils<m32, f32, i32, F>.MaskedIncrement_i32(ranky, F.Complement(x_ge_y));

            m32 x_ge_z = F.GreaterThanOrEqual(x0, z0);
            rankx = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankx, x_ge_z);
            rankz = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankz, F.Complement(x_ge_z));

            m32 x_ge_w = F.GreaterThanOrEqual(x0, w0);
            rankx = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankx, x_ge_w);
            rankw = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankw, F.Complement(x_ge_w));

            m32 y_ge_z = F.GreaterThanOrEqual(y0, z0);
            ranky = Utils<m32, f32, i32, F>.MaskedIncrement_i32(ranky, y_ge_z);
            rankz = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankz, F.Complement(y_ge_z));

            m32 y_ge_w = F.GreaterThanOrEqual(y0, w0);
            ranky = Utils<m32, f32, i32, F>.MaskedIncrement_i32(ranky, y_ge_w);
            rankw = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankw, F.Complement(y_ge_w));

            m32 z_ge_w = F.GreaterThanOrEqual(z0, w0);
            rankz = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankz, z_ge_w);
            rankw = Utils<m32, f32, i32, F>.MaskedIncrement_i32(rankw, F.Complement(z_ge_w));

            m32 i1 = F.GreaterThan(rankx, F.Broad_i32(2));
            m32 j1 = F.GreaterThan(ranky, F.Broad_i32(2));
            m32 k1 = F.GreaterThan(rankz, F.Broad_i32(2));
            m32 l1 = F.GreaterThan(rankw, F.Broad_i32(2));

            m32 i2 = F.GreaterThan(rankx, F.Broad_i32(1));
            m32 j2 = F.GreaterThan(ranky, F.Broad_i32(1));
            m32 k2 = F.GreaterThan(rankz, F.Broad_i32(1));
            m32 l2 = F.GreaterThan(rankw, F.Broad_i32(1));

            m32 i3 = F.GreaterThan(rankx, F.Broad_i32(0));
            m32 j3 = F.GreaterThan(ranky, F.Broad_i32(0));
            m32 k3 = F.GreaterThan(rankz, F.Broad_i32(0));
            m32 l3 = F.GreaterThan(rankw, F.Broad_i32(0));

            f32 x1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(x0, F.Broad_f32(1), i1), F.Broad_f32(G4));
            f32 y1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(y0, F.Broad_f32(1), j1), F.Broad_f32(G4));
            f32 z1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(z0, F.Broad_f32(1), k1), F.Broad_f32(G4));
            f32 w1 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(w0, F.Broad_f32(1), l1), F.Broad_f32(G4));
            f32 x2 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(x0, F.Broad_f32(1), i2), F.Broad_f32(G4 * 2));
            f32 y2 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(y0, F.Broad_f32(1), j2), F.Broad_f32(G4 * 2));
            f32 z2 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(z0, F.Broad_f32(1), k2), F.Broad_f32(G4 * 2));
            f32 w2 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(w0, F.Broad_f32(1), l2), F.Broad_f32(G4 * 2));
            f32 x3 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(x0, F.Broad_f32(1), i3), F.Broad_f32(G4 * 3));
            f32 y3 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(y0, F.Broad_f32(1), j3), F.Broad_f32(G4 * 3));
            f32 z3 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(z0, F.Broad_f32(1), k3), F.Broad_f32(G4 * 3));
            f32 w3 = F.Add(Utils<m32, f32, i32, F>.MaskedSub_f32(w0, F.Broad_f32(1), l3), F.Broad_f32(G4 * 3));
            f32 x4 = F.Add(x0, F.Broad_f32(G4 * 4 - 1));
            f32 y4 = F.Add(y0, F.Broad_f32(G4 * 4 - 1));
            f32 z4 = F.Add(z0, F.Broad_f32(G4 * 4 - 1));
            f32 w4 = F.Add(w0, F.Broad_f32(G4 * 4 - 1));

            f32 t0 = F.FNMulAdd_f32(
                x0, x0, F.FNMulAdd_f32(y0, y0, F.FNMulAdd_f32(z0, z0, F.FNMulAdd_f32(w0, w0, F.Broad_f32(0.6f)))));

            f32 t1 = F.FNMulAdd_f32(
                x1, x1, F.FNMulAdd_f32(y1, y1, F.FNMulAdd_f32(z1, z1, F.FNMulAdd_f32(w1, w1, F.Broad_f32(0.6f)))));

            f32 t2 = F.FNMulAdd_f32(
                x2, x2, F.FNMulAdd_f32(y2, y2, F.FNMulAdd_f32(z2, z2, F.FNMulAdd_f32(w2, w2, F.Broad_f32(0.6f)))));

            f32 t3 = F.FNMulAdd_f32(
                x3, x3, F.FNMulAdd_f32(y3, y3, F.FNMulAdd_f32(z3, z3, F.FNMulAdd_f32(w3, w3, F.Broad_f32(0.6f)))));

            f32 t4 = F.FNMulAdd_f32(
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

            f32 n0 = Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, i, j, k, l), x0, y0, z0, w0);

            f32 n1 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j1),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k1),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(l, F.Broad_i32(Primes.W), l1)),
                x1, y1, z1, w1);

            f32 n2 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i2),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j2),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k2),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(l, F.Broad_i32(Primes.W), l2)),
                x2, y2, z2, w2);

            f32 n3 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i3),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(j, F.Broad_i32(Primes.Y), j3),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(k, F.Broad_i32(Primes.Z), k3),
                    Utils<m32, f32, i32, F>.MaskedAdd_i32(l, F.Broad_i32(Primes.W), l3)),
                x3, y3, z3, w3);

            f32 n4 = Utils<m32, f32, i32, F>.GetGradientDot(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y)),
                    F.Add(k, F.Broad_i32(Primes.Z)),
                    F.Add(l, F.Broad_i32(Primes.W))),
                x4, y4, z4, w4);

            f32 last = F.FMulAdd_f32(
                n0,
                t0,
                F.FMulAdd_f32(n1, t1, F.FMulAdd_f32(n2, t2, F.FMulAdd_f32(n3, t3, F.Mul(n4, t4)))));

            return F.Mul(F.Broad_f32(27f), last);
        }

        public OutputMinMax GenUniformGrid2D(
            Span<float> destination,
            int xStart,
            int yStart,
            int xSize,
            int ySize,
            float frequency,
            int seed)
        {
            return NoiseGenerator2DHelper.GenUniformGrid<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xStart,
                yStart,
                xSize,
                ySize,
                frequency,
                seed);
        }

        public OutputMinMax GenUniformGrid3D(
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int xSize,
            int ySize,
            int zSize,
            float frequency,
            int seed)
        {
            return NoiseGenerator3DHelper.GenUniformGrid<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xStart,
                yStart,
                zStart,
                xSize,
                ySize,
                zSize,
                frequency,
                seed);
        }

        public OutputMinMax GenUniformGrid4D(
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int wStart,
            int xSize,
            int ySize,
            int zSize,
            int wSize,
            float frequency,
            int seed)
        {
            return NoiseGenerator4DHelper.GenUniformGrid<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xStart,
                yStart,
                zStart,
                wStart,
                xSize,
                ySize,
                zSize,
                wSize,
                frequency,
                seed);
        }

        public OutputMinMax GenPositionArray2D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            float xOffset,
            float yOffset,
            int seed)
        {
            return NoiseGenerator2DHelper.GenPositionArray<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xPosArray,
                yPosArray,
                xOffset,
                yOffset,
                seed);
        }

        public OutputMinMax GenPositionArray3D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            int seed)
        {
            return NoiseGenerator3DHelper.GenPositionArray<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xPosArray,
                yPosArray,
                zPosArray,
                xOffset,
                yOffset,
                zOffset,
                seed);
        }

        public OutputMinMax GenPositionArray4D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            ReadOnlySpan<float> wPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            float wOffset,
            int seed)
        {
            return NoiseGenerator4DHelper.GenPositionArray<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xPosArray,
                yPosArray,
                zPosArray,
                wPosArray,
                xOffset,
                yOffset,
                zOffset,
                wOffset,
                seed);
        }

        public float GenSingle2D(float x, float y, int seed)
        {
            return NoiseGenerator2DHelper.GenSingle<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                x,
                y,
                seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return NoiseGenerator3DHelper.GenSingle<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                x,
                y,
                z,
                seed);
        }

        public float GenSingle4D(float x, float y, float z, float w, int seed)
        {
            return NoiseGenerator4DHelper.GenSingle<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                x,
                y,
                z,
                w,
                seed);
        }

        public OutputMinMax GenTileable2D(
            Span<float> destination,
            int xSize,
            int ySize,
            float frequency,
            int seed)
        {
            return NoiseGenerator4DHelper.GenTileable<m32, f32, i32, F, Simplex<m32, f32, i32, F>>(
                ref this,
                destination,
                xSize,
                ySize,
                frequency,
                seed);
        }
    }
}
