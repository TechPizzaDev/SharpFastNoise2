﻿using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public struct OpenSimplex2<m32, f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32, F>
    {
        public static int UnitSize => F.Count;

        readonly int INoiseGenerator.UnitSize => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            const float SQRT3 = 1.7320508075f;
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

            f32 x1 = F.Add(F.MaskedSub_f32(x0, F.Broad_f32(1f), i1), F.Broad_f32(G2));
            f32 y1 = F.Add(F.NMaskedSub_f32(y0, F.Broad_f32(1f), i1), F.Broad_f32(G2));
            f32 x2 = F.Add(x0, F.Broad_f32((G2 * 2) - 1));
            f32 y2 = F.Add(y0, F.Broad_f32((G2 * 2) - 1));

            f32 t0 = F.Sub(F.Sub(F.Broad_f32(0.5f), F.Mul(x0, x0)), F.Mul(y0, y0));
            f32 t1 = F.Sub(F.Sub(F.Broad_f32(0.5f), F.Mul(x1, x1)), F.Mul(y1, y1));
            f32 t2 = F.Sub(F.Sub(F.Broad_f32(0.5f), F.Mul(x2, x2)), F.Mul(y2, y2));

            t0 = F.Max_f32(t0, F.Broad_f32(0));
            t1 = F.Max_f32(t1, F.Broad_f32(0));
            t2 = F.Max_f32(t2, F.Broad_f32(0));

            t0 = F.Mul(t0, t0);
            t0 = F.Mul(t0, t0);
            t1 = F.Mul(t1, t1);
            t1 = F.Mul(t1, t1);
            t2 = F.Mul(t2, t2);
            t2 = F.Mul(t2, t2);

            f32 n0 = F.GetGradientDotFancy(
                Utils<m32, f32, i32, F>.HashPrimes(seed, i, j), x0, y0);

            f32 n1 = F.GetGradientDotFancy(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    F.MaskedAdd_i32(i, F.Broad_i32(Primes.X), i1),
                    F.NMaskedAdd_i32(j, F.Broad_i32(Primes.Y), i1)),
                x1, y1);

            f32 n2 = F.GetGradientDotFancy(
                Utils<m32, f32, i32, F>.HashPrimes(
                    seed,
                    F.Add(i, F.Broad_i32(Primes.X)),
                    F.Add(j, F.Broad_i32(Primes.Y))),
                x2, y2);

            f32 last = F.FMulAdd_f32(n0, t0, F.FMulAdd_f32(n1, t1, F.Mul(n2, t2)));
            return F.Mul(F.Broad_f32(49.918426513671875f), last);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            f32 f = F.Mul(F.Broad_f32(2.0f / 3.0f), F.Add(F.Add(x, y), z));
            f32 xr = F.Sub(f, x);
            f32 yr = F.Sub(f, y);
            f32 zr = F.Sub(f, z);

            f32 val = F.Broad_f32(0);
            for (int i = 0; ; i++)
            {
                f32 v0xr = F.Round_f32(xr);
                f32 v0yr = F.Round_f32(yr);
                f32 v0zr = F.Round_f32(zr);
                f32 d0xr = F.Sub(xr, v0xr);
                f32 d0yr = F.Sub(yr, v0yr);
                f32 d0zr = F.Sub(zr, v0zr);

                f32 score0xr = F.Abs_f32(d0xr);
                f32 score0yr = F.Abs_f32(d0yr);
                f32 score0zr = F.Abs_f32(d0zr);
                m32 dir0xr = F.LessThanOrEqual(F.Max_f32(score0yr, score0zr), score0xr);
                m32 dir0yr = F.BitwiseAndNot_m32(F.LessThanOrEqual(F.Max_f32(score0zr, score0xr), score0yr), dir0xr);
                m32 dir0zr = F.Complement(F.Or(dir0xr, dir0yr));
                f32 v1xr = F.MaskedAdd_f32(v0xr, F.Or(F.Broad_f32(1.0f), F.And(F.Broad_f32(-1.0f), d0xr)), dir0xr);
                f32 v1yr = F.MaskedAdd_f32(v0yr, F.Or(F.Broad_f32(1.0f), F.And(F.Broad_f32(-1.0f), d0yr)), dir0yr);
                f32 v1zr = F.MaskedAdd_f32(v0zr, F.Or(F.Broad_f32(1.0f), F.And(F.Broad_f32(-1.0f), d0zr)), dir0zr);
                f32 d1xr = F.Sub(xr, v1xr);
                f32 d1yr = F.Sub(yr, v1yr);
                f32 d1zr = F.Sub(zr, v1zr);

                i32 hv0xr = F.Mul(F.Convertf32_i32(v0xr), F.Broad_i32(Primes.X));
                i32 hv0yr = F.Mul(F.Convertf32_i32(v0yr), F.Broad_i32(Primes.Y));
                i32 hv0zr = F.Mul(F.Convertf32_i32(v0zr), F.Broad_i32(Primes.Z));

                i32 hv1xr = F.Mul(F.Convertf32_i32(v1xr), F.Broad_i32(Primes.X));
                i32 hv1yr = F.Mul(F.Convertf32_i32(v1yr), F.Broad_i32(Primes.Y));
                i32 hv1zr = F.Mul(F.Convertf32_i32(v1zr), F.Broad_i32(Primes.Z));

                f32 t0 = F.FNMulAdd_f32(d0zr, d0zr, F.FNMulAdd_f32(d0yr, d0yr, F.FNMulAdd_f32(d0xr, d0xr, F.Broad_f32(0.6f))));
                f32 t1 = F.FNMulAdd_f32(d1zr, d1zr, F.FNMulAdd_f32(d1yr, d1yr, F.FNMulAdd_f32(d1xr, d1xr, F.Broad_f32(0.6f))));
                t0 = F.Max_f32(t0, F.Broad_f32(0));
                t1 = F.Max_f32(t1, F.Broad_f32(0));
                t0 = F.Mul(t0, t0);
                t0 = F.Mul(t0, t0);
                t1 = F.Mul(t1, t1);
                t1 = F.Mul(t1, t1);

                f32 v0 = F.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, hv0xr, hv0yr, hv0zr), d0xr, d0yr, d0zr);
                f32 v1 = F.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, hv1xr, hv1yr, hv1zr), d1xr, d1yr, d1zr);

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


        public OutputMinMax GenUniformGrid2D(
            Span<float> destination,
            int xStart,
            int yStart,
            int xSize,
            int ySize,
            float frequency,
            int seed)
        {
            return NoiseGenerator2DHelper.GenUniformGrid<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
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
            return NoiseGenerator3DHelper.GenUniformGrid<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
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

        //public OutputMinMax GenUniformGrid4D(
        //    Span<float> destination,
        //    int xStart,
        //    int yStart,
        //    int zStart,
        //    int wStart,
        //    int xSize,
        //    int ySize,
        //    int zSize,
        //    int wSize,
        //    float frequency,
        //    int seed)
        //{
        //    return NoiseGenerator4DHelper.GenUniformGrid<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
        //        ref this,
        //        destination,
        //        xStart,
        //        yStart,
        //        zStart,
        //        wStart,
        //        xSize,
        //        ySize,
        //        zSize,
        //        wSize,
        //        frequency,
        //        seed);
        //}

        public OutputMinMax GenPositionArray2D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            float xOffset,
            float yOffset,
            int seed)
        {
            return NoiseGenerator2DHelper.GenPositionArray<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
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
            return NoiseGenerator3DHelper.GenPositionArray<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
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

        //public OutputMinMax GenPositionArray4D(
        //    Span<float> destination,
        //    ReadOnlySpan<float> xPosArray,
        //    ReadOnlySpan<float> yPosArray,
        //    ReadOnlySpan<float> zPosArray,
        //    ReadOnlySpan<float> wPosArray,
        //    float xOffset,
        //    float yOffset,
        //    float zOffset,
        //    float wOffset,
        //    int seed)
        //{
        //    return NoiseGenerator4DHelper.GenPositionArray<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
        //        ref this,
        //        destination,
        //        xPosArray,
        //        yPosArray,
        //        zPosArray,
        //        wPosArray,
        //        xOffset,
        //        yOffset,
        //        zOffset,
        //        wOffset,
        //        seed);
        //}

        public float GenSingle2D(float x, float y, int seed)
        {
            return NoiseGenerator2DHelper.GenSingle<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
                ref this,
                x,
                y,
                seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return NoiseGenerator3DHelper.GenSingle<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
                ref this,
                x,
                y,
                z,
                seed);
        }

        //public float GenSingle4D(float x, float y, float z, float w, int seed)
        //{
        //    return NoiseGenerator4DHelper.GenSingle<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
        //        ref this,
        //        x,
        //        y,
        //        z,
        //        w,
        //        seed);
        //}
        //
        //public OutputMinMax GenTileable2D(
        //    Span<float> destination,
        //    int xSize,
        //    int ySize,
        //    float frequency,
        //    int seed)
        //{
        //    return NoiseGenerator4DHelper.GenTileable<m32, f32, i32, F, OpenSimplex2<m32, f32, i32, F>>(
        //        ref this,
        //        destination,
        //        xSize,
        //        ySize,
        //        frequency,
        //        seed);
        //}
    }
}
