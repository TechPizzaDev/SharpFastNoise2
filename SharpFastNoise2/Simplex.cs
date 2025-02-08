using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public struct Simplex<f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<f32, i32, F>
    {
        public static int UnitSize => F.Count;

        readonly int INoiseGenerator.UnitSize => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            const float SQRT3 = 1.7320508075688772935274463415059f;
            const float F2 = 0.5f * (SQRT3 - 1.0f);
            const float G2 = (3.0f - SQRT3) / 6.0f;

            f32 f = F.Mul(F.Broad(F2), F.Add(x, y));
            f32 x0 = F.Floor(F.Add(x, f));
            f32 y0 = F.Floor(F.Add(y, f));

            i32 i = F.Mul(F.Convert_i32(x0), F.Broad(Primes.X));
            i32 j = F.Mul(F.Convert_i32(y0), F.Broad(Primes.Y));

            f32 g = F.Mul(F.Broad(G2), F.Add(x0, y0));
            x0 = F.Sub(x, F.Sub(x0, g));
            y0 = F.Sub(y, F.Sub(y0, g));

            f32 i1 = F.GreaterThan(x0, y0);
            //m32 j1 = ~i1; //NMasked funcs

            f32 x1 = F.Add(F.MaskSub(x0, F.Broad(1f), i1), F.Broad(G2));
            f32 y1 = F.Add(F.NMaskSub(y0, F.Broad(1f), i1), F.Broad(G2));

            f32 x2 = F.Add(x0, F.Broad(G2 * 2 - 1));
            f32 y2 = F.Add(y0, F.Broad(G2 * 2 - 1));

            f32 t0 = F.FNMulAdd(x0, x0, F.FNMulAdd(y0, y0, F.Broad(0.5f)));
            f32 t1 = F.FNMulAdd(x1, x1, F.FNMulAdd(y1, y1, F.Broad(0.5f)));
            f32 t2 = F.FNMulAdd(x2, x2, F.FNMulAdd(y2, y2, F.Broad(0.5f)));

            t0 = F.Max(t0, F.Broad((float)0));
            t1 = F.Max(t1, F.Broad((float)0));
            t2 = F.Max(t2, F.Broad((float)0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);

            f32 n0 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(seed, i, j),
                x0, y0);

            f32 n1 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskAdd(i, F.Broad(Primes.X), F.Cast_i32(i1)),
                    F.NMaskAdd(j, F.Broad(Primes.Y), F.Cast_i32(i1))),
                x1, y1);

            f32 n2 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad(Primes.X)),
                    F.Add(j, F.Broad(Primes.Y))),
                x2, y2);

            f32 last = F.FMulAdd(n0, t0, F.FMulAdd(n1, t1, F.Mul(n2, t2)));
            return F.Mul(F.Broad(38.283687591552734375f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            const float F3 = 1.0f / 3.0f;
            const float G3 = 1.0f / 2.0f;

            f32 s = F.Mul(F.Broad(F3), F.Add(F.Add(x, y), z));
            x = F.Add(x, s);
            y = F.Add(y, s);
            z = F.Add(z, s);

            f32 x0 = F.Floor(x);
            f32 y0 = F.Floor(y);
            f32 z0 = F.Floor(z);
            f32 xi = F.Sub(x, x0);
            f32 yi = F.Sub(y, y0);
            f32 zi = F.Sub(z, z0);

            i32 i = F.Mul(F.Convert_i32(x0), F.Broad(Primes.X));
            i32 j = F.Mul(F.Convert_i32(y0), F.Broad(Primes.Y));
            i32 k = F.Mul(F.Convert_i32(z0), F.Broad(Primes.Z));

            f32 x_ge_y = F.GreaterThanOrEqual(xi, yi);
            f32 y_ge_z = F.GreaterThanOrEqual(yi, zi);
            f32 x_ge_z = F.GreaterThanOrEqual(xi, zi);

            f32 g = F.Mul(F.Broad(G3), F.Add(F.Add(xi, yi), zi));
            x0 = F.Sub(xi, g);
            y0 = F.Sub(yi, g);
            z0 = F.Sub(zi, g);

            f32 i1 = F.And(x_ge_y, x_ge_z);
            f32 j1 = F.AndNot(y_ge_z, x_ge_y);
            f32 k1 = F.AndNot(F.Not(x_ge_z), y_ge_z);

            f32 i2 = F.Or(x_ge_y, x_ge_z);
            f32 j2 = F.Or(F.Not(x_ge_y), y_ge_z);
            f32 k2 = F.And(x_ge_z, y_ge_z); //NMasked

            f32 x1 = F.Add(F.MaskSub(x0, F.Broad(1f), i1), F.Broad(G3));
            f32 y1 = F.Add(F.MaskSub(y0, F.Broad(1f), j1), F.Broad(G3));
            f32 z1 = F.Add(F.MaskSub(z0, F.Broad(1f), k1), F.Broad(G3));
            f32 x2 = F.Add(F.MaskSub(x0, F.Broad(1f), i2), F.Broad(G3 * 2));
            f32 y2 = F.Add(F.MaskSub(y0, F.Broad(1f), j2), F.Broad(G3 * 2));
            f32 z2 = F.Add(F.NMaskSub(z0, F.Broad(1f), k2), F.Broad(G3 * 2));
            f32 x3 = F.Add(x0, F.Broad(G3 * 3 - 1));
            f32 y3 = F.Add(y0, F.Broad(G3 * 3 - 1));
            f32 z3 = F.Add(z0, F.Broad(G3 * 3 - 1));

            f32 t0 = F.FNMulAdd(x0, x0, F.FNMulAdd(y0, y0, F.FNMulAdd(z0, z0, F.Broad(0.6f))));
            f32 t1 = F.FNMulAdd(x1, x1, F.FNMulAdd(y1, y1, F.FNMulAdd(z1, z1, F.Broad(0.6f))));
            f32 t2 = F.FNMulAdd(x2, x2, F.FNMulAdd(y2, y2, F.FNMulAdd(z2, z2, F.Broad(0.6f))));
            f32 t3 = F.FNMulAdd(x3, x3, F.FNMulAdd(y3, y3, F.FNMulAdd(z3, z3, F.Broad(0.6f))));

            t0 = F.Max(t0, F.Broad((float)0));
            t1 = F.Max(t1, F.Broad((float)0));
            t2 = F.Max(t2, F.Broad((float)0));
            t3 = F.Max(t3, F.Broad((float)0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);
            t3 = F.Mul(t3, t3);
            t3 = F.Mul(t3, t3);

            f32 n0 = F.GetGradientDot(
                Utils< f32, i32, F>.HashPrimes(seed, i, j, k), x0, y0, z0);

            f32 n1 = F.GetGradientDot(
                Utils< f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskAdd(i, F.Broad(Primes.X), F.Cast_i32(i1)),
                    F.MaskAdd(j, F.Broad(Primes.Y), F.Cast_i32(j1)),
                    F.MaskAdd(k, F.Broad(Primes.Z), F.Cast_i32(k1))),
                x1, y1, z1);

            f32 n2 = F.GetGradientDot(
                Utils< f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskAdd(i, F.Broad(Primes.X), F.Cast_i32(i2)),
                    F.MaskAdd(j, F.Broad(Primes.Y), F.Cast_i32(j2)),
                    F.NMaskAdd(k, F.Broad(Primes.Z), F.Cast_i32(k2))),
                x2, y2, z2);

            f32 n3 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad(Primes.X)),
                    F.Add(j, F.Broad(Primes.Y)),
                    F.Add(k, F.Broad(Primes.Z))),
                x3, y3, z3);

            f32 last = F.FMulAdd(n0, t0, F.FMulAdd(n1, t1, F.FMulAdd(n2, t2, F.Mul(n3, t3))));
            return F.Mul(F.Broad(32.69428253173828125f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
        {
            const float SQRT5 = 2.236067977499f;
            const float F4 = (SQRT5 - 1.0f) / 4.0f;
            const float G4 = (5.0f - SQRT5) / 20.0f;

            f32 s = F.Mul(F.Broad(F4), F.Add(F.Add(F.Add(x, y), z), w));
            x = F.Add(x, s);
            y = F.Add(y, s);
            z = F.Add(z, s);
            w = F.Add(w, s);

            f32 x0 = F.Floor(x);
            f32 y0 = F.Floor(y);
            f32 z0 = F.Floor(z);
            f32 w0 = F.Floor(w);
            f32 xi = F.Sub(x, x0);
            f32 yi = F.Sub(y, y0);
            f32 zi = F.Sub(z, z0);
            f32 wi = F.Sub(w, w0);

            i32 i = F.Mul(F.Convert_i32(x0), F.Broad(Primes.X));
            i32 j = F.Mul(F.Convert_i32(y0), F.Broad(Primes.Y));
            i32 k = F.Mul(F.Convert_i32(z0), F.Broad(Primes.Z));
            i32 l = F.Mul(F.Convert_i32(w0), F.Broad(Primes.W));

            f32 g = F.Mul(F.Broad(G4), F.Add(F.Add(F.Add(xi, yi), zi), wi));
            x0 = F.Sub(xi, g);
            y0 = F.Sub(yi, g);
            z0 = F.Sub(zi, g);
            w0 = F.Sub(wi, g);

            i32 rankx = F.Broad(0);
            i32 ranky = rankx;
            i32 rankz = rankx;
            i32 rankw = rankx;

            i32 x_ge_y = F.Cast_i32(F.GreaterThanOrEqual(x0, y0));
            rankx = F.MaskIncrement(rankx, x_ge_y);
            ranky = F.MaskIncrement(ranky, F.Not(x_ge_y));

            i32 x_ge_z = F.Cast_i32(F.GreaterThanOrEqual(x0, z0));
            rankx = F.MaskIncrement(rankx, x_ge_z);
            rankz = F.MaskIncrement(rankz, F.Not(x_ge_z));

            i32 x_ge_w = F.Cast_i32(F.GreaterThanOrEqual(x0, w0));
            rankx = F.MaskIncrement(rankx, x_ge_w);
            rankw = F.MaskIncrement(rankw, F.Not(x_ge_w));

            i32 y_ge_z = F.Cast_i32(F.GreaterThanOrEqual(y0, z0));
            ranky = F.MaskIncrement(ranky, y_ge_z);
            rankz = F.MaskIncrement(rankz, F.Not(y_ge_z));

            i32 y_ge_w = F.Cast_i32(F.GreaterThanOrEqual(y0, w0));
            ranky = F.MaskIncrement(ranky, y_ge_w);
            rankw = F.MaskIncrement(rankw, F.Not(y_ge_w));

            i32 z_ge_w = F.Cast_i32(F.GreaterThanOrEqual(z0, w0));
            rankz = F.MaskIncrement(rankz, z_ge_w);
            rankw = F.MaskIncrement(rankw, F.Not(z_ge_w));

            i32 ic2 = F.Broad(2);
            i32 i1 = F.GreaterThan(rankx, ic2);
            i32 j1 = F.GreaterThan(ranky, ic2);
            i32 k1 = F.GreaterThan(rankz, ic2);
            i32 l1 = F.GreaterThan(rankw, ic2);

            i32 ic1 = F.Broad(1);
            i32 i2 = F.GreaterThan(rankx, ic1);
            i32 j2 = F.GreaterThan(ranky, ic1);
            i32 k2 = F.GreaterThan(rankz, ic1);
            i32 l2 = F.GreaterThan(rankw, ic1);

            i32 ic0 = F.Broad(0);
            i32 i3 = F.GreaterThan(rankx, ic0);
            i32 j3 = F.GreaterThan(ranky, ic0);
            i32 k3 = F.GreaterThan(rankz, ic0);
            i32 l3 = F.GreaterThan(rankw, ic0);

            f32 fc1 = F.Broad(1f);
            f32 fcG4 = F.Broad(G4);
            f32 x1 = F.Add(F.MaskSub(x0, fc1, F.Cast_f32(i1)), fcG4);
            f32 y1 = F.Add(F.MaskSub(y0, fc1, F.Cast_f32(j1)), fcG4);
            f32 z1 = F.Add(F.MaskSub(z0, fc1, F.Cast_f32(k1)), fcG4);
            f32 w1 = F.Add(F.MaskSub(w0, fc1, F.Cast_f32(l1)), fcG4);
            
            f32 fc2G4 = F.Broad(G4 * 2);
            f32 x2 = F.Add(F.MaskSub(x0, fc1, F.Cast_f32(i2)), fc2G4);
            f32 y2 = F.Add(F.MaskSub(y0, fc1, F.Cast_f32(j2)), fc2G4);
            f32 z2 = F.Add(F.MaskSub(z0, fc1, F.Cast_f32(k2)), fc2G4);
            f32 w2 = F.Add(F.MaskSub(w0, fc1, F.Cast_f32(l2)), fc2G4);
            
            f32 fc3G4 = F.Broad(G4 * 3);
            f32 x3 = F.Add(F.MaskSub(x0, fc1, F.Cast_f32(i3)), fc3G4);
            f32 y3 = F.Add(F.MaskSub(y0, fc1, F.Cast_f32(j3)), fc3G4);
            f32 z3 = F.Add(F.MaskSub(z0, fc1, F.Cast_f32(k3)), fc3G4);
            f32 w3 = F.Add(F.MaskSub(w0, fc1, F.Cast_f32(l3)), fc3G4);
            
            f32 x4 = F.Add(x0, F.Broad(G4 * 4 - 1));
            f32 y4 = F.Add(y0, F.Broad(G4 * 4 - 1));
            f32 z4 = F.Add(z0, F.Broad(G4 * 4 - 1));
            f32 w4 = F.Add(w0, F.Broad(G4 * 4 - 1));

            f32 fc0d6 = F.Broad(0.6f);
            f32 t0 = F.FNMulAdd(x0, x0, F.FNMulAdd(y0, y0, F.FNMulAdd(z0, z0, F.FNMulAdd(w0, w0, fc0d6))));
            f32 t1 = F.FNMulAdd(x1, x1, F.FNMulAdd(y1, y1, F.FNMulAdd(z1, z1, F.FNMulAdd(w1, w1, fc0d6))));
            f32 t2 = F.FNMulAdd(x2, x2, F.FNMulAdd(y2, y2, F.FNMulAdd(z2, z2, F.FNMulAdd(w2, w2, fc0d6))));
            f32 t3 = F.FNMulAdd(x3, x3, F.FNMulAdd(y3, y3, F.FNMulAdd(z3, z3, F.FNMulAdd(w3, w3, fc0d6))));
            f32 t4 = F.FNMulAdd(x4, x4, F.FNMulAdd(y4, y4, F.FNMulAdd(z4, z4, F.FNMulAdd(w4, w4, fc0d6))));

            f32 fc0 = F.Broad(0f);
            t0 = F.Max(t0, fc0);
            t1 = F.Max(t1, fc0);
            t2 = F.Max(t2, fc0);
            t3 = F.Max(t3, fc0);
            t4 = F.Max(t4, fc0);

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

            f32 n0 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(seed, i, j, k, l), x0, y0, z0, w0);

            f32 n1 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskAdd(i, F.Broad(Primes.X), i1),
                    F.MaskAdd(j, F.Broad(Primes.Y), j1),
                    F.MaskAdd(k, F.Broad(Primes.Z), k1),
                    F.MaskAdd(l, F.Broad(Primes.W), l1)),
                x1, y1, z1, w1);

            f32 n2 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskAdd(i, F.Broad(Primes.X), i2),
                    F.MaskAdd(j, F.Broad(Primes.Y), j2),
                    F.MaskAdd(k, F.Broad(Primes.Z), k2),
                    F.MaskAdd(l, F.Broad(Primes.W), l2)),
                x2, y2, z2, w2);

            f32 n3 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskAdd(i, F.Broad(Primes.X), i3),
                    F.MaskAdd(j, F.Broad(Primes.Y), j3),
                    F.MaskAdd(k, F.Broad(Primes.Z), k3),
                    F.MaskAdd(l, F.Broad(Primes.W), l3)),
                x3, y3, z3, w3);

            f32 n4 = F.GetGradientDot(
                Utils<f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad(Primes.X)),
                    F.Add(j, F.Broad(Primes.Y)),
                    F.Add(k, F.Broad(Primes.Z)),
                    F.Add(l, F.Broad(Primes.W))),
                x4, y4, z4, w4);

            f32 last = F.FMulAdd(
                n0,
                t0,
                F.FMulAdd(n1, t1, F.FMulAdd(n2, t2, F.FMulAdd(n3, t3, F.Mul(n4, t4)))));

            return F.Mul(F.Broad(27f), last);
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
            return NoiseGenerator2DHelper.GenUniformGrid<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator3DHelper.GenUniformGrid<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator4DHelper.GenUniformGrid<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator2DHelper.GenPositionArray<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator3DHelper.GenPositionArray<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator4DHelper.GenPositionArray<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator2DHelper.GenSingle<f32, i32, F, Simplex<f32, i32, F>>(
                ref this,
                x,
                y,
                seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return NoiseGenerator3DHelper.GenSingle<f32, i32, F, Simplex<f32, i32, F>>(
                ref this,
                x,
                y,
                z,
                seed);
        }

        public float GenSingle4D(float x, float y, float z, float w, int seed)
        {
            return NoiseGenerator4DHelper.GenSingle<f32, i32, F, Simplex<f32, i32, F>>(
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
            return NoiseGenerator4DHelper.GenTileable<f32, i32, F, Simplex<f32, i32, F>>(
                ref this,
                destination,
                xSize,
                ySize,
                frequency,
                seed);
        }
    }
}
