using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public struct Perlin<f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where F : IFunctionList<f32, i32, F>
    {
        public static int UnitSize => F.Count;

        readonly int INoiseGenerator.UnitSize => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            f32 xs = F.Floor(x);
            f32 ys = F.Floor(y);

            i32 x0 = F.Mul(F.Convert_i32(xs), F.Broad(Primes.X));
            i32 y0 = F.Mul(F.Convert_i32(ys), F.Broad(Primes.Y));

            i32 x1 = F.Add(x0, F.Broad(Primes.X));
            i32 y1 = F.Add(y0, F.Broad(Primes.Y));

            f32 xf0 = xs = F.Sub(x, xs);
            f32 yf0 = ys = F.Sub(y, ys);

            f32 fc1 = F.Broad(1f);
            f32 xf1 = F.Sub(xf0, fc1);
            f32 yf1 = F.Sub(yf0, fc1);

            xs = Utils<f32, i32, F>.InterpQuintic(xs);
            ys = Utils<f32, i32, F>.InterpQuintic(ys);

            return F.Mul(F.Broad(0.579106986522674560546875f), Utils<f32, i32, F>.Lerp(
                Utils<f32, i32, F>.Lerp(
                    F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0), xf0, yf0),
                    F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0), xf1, yf0),
                    xs),
                Utils<f32, i32, F>.Lerp(
                    F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1), xf0, yf1),
                    F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1), xf1, yf1),
                    xs),
                ys));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            f32 xs = F.Floor(x);
            f32 ys = F.Floor(y);
            f32 zs = F.Floor(z);

            i32 x0 = F.Mul(F.Convert_i32(xs), F.Broad(Primes.X));
            i32 y0 = F.Mul(F.Convert_i32(ys), F.Broad(Primes.Y));
            i32 z0 = F.Mul(F.Convert_i32(zs), F.Broad(Primes.Z));

            i32 x1 = F.Add(x0, F.Broad(Primes.X));
            i32 y1 = F.Add(y0, F.Broad(Primes.Y));
            i32 z1 = F.Add(z0, F.Broad(Primes.Z));

            f32 xf0 = xs = F.Sub(x, xs);
            f32 yf0 = ys = F.Sub(y, ys);
            f32 zf0 = zs = F.Sub(z, zs);

            f32 fc1 = F.Broad(1f);
            f32 xf1 = F.Sub(xf0, fc1);
            f32 yf1 = F.Sub(yf0, fc1);
            f32 zf1 = F.Sub(zf0, fc1);

            xs = Utils<f32, i32, F>.InterpQuintic(xs);
            ys = Utils<f32, i32, F>.InterpQuintic(ys);
            zs = Utils<f32, i32, F>.InterpQuintic(zs);

            return F.Mul(F.Broad(0.964921414852142333984375f), Utils<f32, i32, F>.Lerp(
                Utils<f32, i32, F>.Lerp(
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0, z0), xf0, yf0, zf0),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0, z0), xf1, yf0, zf0),
                        xs),
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1, z0), xf0, yf1, zf0),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1, z0), xf1, yf1, zf0),
                        xs),
                    ys),
                Utils<f32, i32, F>.Lerp(
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0, z1), xf0, yf0, zf1),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0, z1), xf1, yf0, zf1),
                        xs),
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1, z1), xf0, yf1, zf1),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1, z1), xf1, yf1, zf1),
                        xs),
                    ys),
                zs));
        }

        public readonly f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
        {
            f32 xs = F.Floor(x);
            f32 ys = F.Floor(y);
            f32 zs = F.Floor(z);
            f32 ws = F.Floor(w);

            i32 x0 = F.Mul(F.Convert_i32(xs), F.Broad(Primes.X));
            i32 y0 = F.Mul(F.Convert_i32(ys), F.Broad(Primes.Y));
            i32 z0 = F.Mul(F.Convert_i32(zs), F.Broad(Primes.Z));
            i32 w0 = F.Mul(F.Convert_i32(ws), F.Broad(Primes.W));

            f32 xf0 = F.Sub(x, xs);
            f32 yf0 = F.Sub(y, ys);
            f32 zf0 = F.Sub(z, zs);
            f32 wf0 = F.Sub(w, ws);

            xs = Utils<f32, i32, F>.InterpQuintic(xf0);
            ys = Utils<f32, i32, F>.InterpQuintic(yf0);
            zs = Utils<f32, i32, F>.InterpQuintic(zf0);
            ws = Utils<f32, i32, F>.InterpQuintic(wf0);

            // Split inputs A and B for W-lerp into two functions due to JIT limits.
            f32 a = GenA(
                seed, xs, ys, zs,
                x0, y0, z0, w0,
                xf0, yf0, zf0, wf0);

            f32 b = GenB(
                seed, xs, ys, zs,
                x0, y0, z0, w0,
                xf0, yf0, zf0, wf0);

            return F.Mul(F.Broad(0.964921414852142333984375f), Utils<f32, i32, F>.Lerp(a, b, ws));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static f32 GenA(
            i32 seed, f32 xs, f32 ys, f32 zs,
            i32 x0, i32 y0, i32 z0, i32 w0,
            f32 xf0, f32 yf0, f32 zf0, f32 wf0)
        {
            f32 fc1 = F.Broad(1f);
            f32 xf1 = F.Sub(xf0, fc1);
            f32 yf1 = F.Sub(yf0, fc1);
            f32 zf1 = F.Sub(zf0, fc1);
            f32 wf1 = F.Sub(wf0, fc1);
            
            i32 x1 = F.Add(x0, F.Broad(Primes.X));
            i32 y1 = F.Add(y0, F.Broad(Primes.Y));
            i32 z1 = F.Add(z0, F.Broad(Primes.Z));
            i32 w1 = F.Add(w0, F.Broad(Primes.W));

            return Utils<f32, i32, F>.Lerp(
                Utils<f32, i32, F>.Lerp(
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0, z0, w0), xf0, yf0, zf0, wf0),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0, z0, w0), xf1, yf0, zf0, wf0),
                        xs),
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1, z0, w0), xf0, yf1, zf0, wf0),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1, z0, w0), xf1, yf1, zf0, wf0),
                        xs),
                    ys),
                Utils<f32, i32, F>.Lerp(
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0, z1, w0), xf0, yf0, zf1, wf0),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0, z1, w0), xf1, yf0, zf1, wf0),
                        xs),
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1, z1, w0), xf0, yf1, zf1, wf0),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1, z1, w0), xf1, yf1, zf1, wf0),
                        xs),
                    ys),
                zs);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static f32 GenB(
            i32 seed, f32 xs, f32 ys, f32 zs,
            i32 x0, i32 y0, i32 z0, i32 w0,
            f32 xf0, f32 yf0, f32 zf0, f32 wf0)
        {
            f32 fc1 = F.Broad(1f);
            f32 xf1 = F.Sub(xf0, fc1);
            f32 yf1 = F.Sub(yf0, fc1);
            f32 zf1 = F.Sub(zf0, fc1);
            f32 wf1 = F.Sub(wf0, fc1);
            
            i32 x1 = F.Add(x0, F.Broad(Primes.X));
            i32 y1 = F.Add(y0, F.Broad(Primes.Y));
            i32 z1 = F.Add(z0, F.Broad(Primes.Z));
            i32 w1 = F.Add(w0, F.Broad(Primes.W));

            return Utils<f32, i32, F>.Lerp(
                Utils<f32, i32, F>.Lerp(
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0, z0, w1), xf0, yf0, zf0, wf1),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0, z0, w1), xf1, yf0, zf0, wf1),
                        xs),
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1, z0, w1), xf0, yf1, zf0, wf1),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1, z0, w1), xf1, yf1, zf0, wf1),
                        xs),
                    ys),
                Utils<f32, i32, F>.Lerp(
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y0, z1, w1), xf0, yf0, zf1, wf1),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y0, z1, w1), xf1, yf0, zf1, wf1),
                        xs),
                    Utils<f32, i32, F>.Lerp(
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x0, y1, z1, w1), xf0, yf1, zf1, wf1),
                        F.GetGradientDot(Utils<f32, i32, F>.HashPrimes(seed, x1, y1, z1, w1), xf1, yf1, zf1, wf1),
                        xs),
                    ys),
                zs);
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
            return NoiseGenerator2DHelper.GenUniformGrid<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator3DHelper.GenUniformGrid<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator4DHelper.GenUniformGrid<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator2DHelper.GenPositionArray<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator3DHelper.GenPositionArray<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator4DHelper.GenPositionArray<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator2DHelper.GenSingle<f32, i32, F, Perlin<f32, i32, F>>(
                ref this,
                x,
                y,
                seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return NoiseGenerator3DHelper.GenSingle<f32, i32, F, Perlin<f32, i32, F>>(
                ref this,
                x,
                y,
                z,
                seed);
        }

        public float GenSingle4D(float x, float y, float z, float w, int seed)
        {
            return NoiseGenerator4DHelper.GenSingle<f32, i32, F, Perlin<f32, i32, F>>(
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
            return NoiseGenerator4DHelper.GenTileable<f32, i32, F, Perlin<f32, i32, F>>(
                ref this,
                destination,
                xSize,
                ySize,
                frequency,
                seed);
        }
    }
}
