using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public interface IFloat32<f32, i32>
        where f32 : IFloat32<f32, i32>
        where i32 : IInt32<f32, i32>
    {
        static abstract f32 operator +(f32 a, f32 b);
    }

    public interface IInt32<f32, i32>
        where f32 : IFloat32<f32, i32>
        where i32 : IInt32<f32, i32>
    {
        static abstract i32 operator +(i32 a, i32 b);
    }

    public static class Generator
    {
        public static f32 Magic<f32, i32>(f32 a, f32 b, i32 c)
            where f32 : IFloat32<f32, i32>
            where i32 : IInt32<f32, i32>
        {
            return a + b;
        }

        public static float Call()
        {
            F32Wrap r = Magic(new F32Wrap(1), new F32Wrap(2), new I32Wrap(3));
            return r.Value;
        }
    }

    public struct I32Wrap : IInt32<F32Wrap, I32Wrap>
    {
        public int Value;

        public I32Wrap(int value)
        {
            Value = value;
        }

        public static I32Wrap operator +(I32Wrap a, I32Wrap b)
        {
            return new I32Wrap(a.Value + b.Value);
        }
    }

    public struct F32Wrap : IFloat32<F32Wrap, I32Wrap>
    {
        public float Value;

        public F32Wrap(float value)
        {
            Value = value;
        }

        public static F32Wrap operator +(F32Wrap a, F32Wrap b)
        {
            return new F32Wrap(a.Value + b.Value);
        }
    }

    public struct Perlin<m32, f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : unmanaged, IFunctionList<m32, f32, i32>
    {
        private static Utils<m32, f32, i32, F> Utils = default;

        public static int Count => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(i32 seed, f32 x, f32 y)
        {
            f32 xs = F.Floor_f32(x);
            f32 ys = F.Floor_f32(y);

            i32 x0 = F.Mul(F.Convertf32_i32(xs), F.Broad_i32(Primes.X));
            i32 y0 = F.Mul(F.Convertf32_i32(ys), F.Broad_i32(Primes.Y));
            i32 x1 = F.Add(x0, F.Broad_i32(Primes.X));
            i32 y1 = F.Add(y0, F.Broad_i32(Primes.Y));

            f32 xf0 = xs = F.Sub(x, xs);
            f32 yf0 = ys = F.Sub(y, ys);
            f32 xf1 = F.Sub(xf0, F.Broad_f32(1));
            f32 yf1 = F.Sub(yf0, F.Broad_f32(1));

            xs = Utils.InterpQuintic(xs);
            ys = Utils.InterpQuintic(ys);

            return F.Mul(F.Broad_f32(0.579106986522674560546875f), Utils.Lerp(
                Utils.Lerp(
                    Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0), xf0, yf0),
                    Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0), xf1, yf0),
                    xs),
                Utils.Lerp(
                    Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1), xf0, yf1),
                    Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1), xf1, yf1),
                    xs),
                ys));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(i32 seed, f32 x, f32 y, f32 z)
        {
            f32 xs = F.Floor_f32(x);
            f32 ys = F.Floor_f32(y);
            f32 zs = F.Floor_f32(z);

            i32 x0 = F.Mul(F.Convertf32_i32(xs), F.Broad_i32(Primes.X));
            i32 y0 = F.Mul(F.Convertf32_i32(ys), F.Broad_i32(Primes.Y));
            i32 z0 = F.Mul(F.Convertf32_i32(zs), F.Broad_i32(Primes.Z));
            i32 x1 = F.Add(x0, F.Broad_i32(Primes.X));
            i32 y1 = F.Add(y0, F.Broad_i32(Primes.Y));
            i32 z1 = F.Add(z0, F.Broad_i32(Primes.Z));

            f32 xf0 = xs = F.Sub(x, xs);
            f32 yf0 = ys = F.Sub(y, ys);
            f32 zf0 = zs = F.Sub(z, zs);
            f32 xf1 = F.Sub(xf0, F.Broad_f32(1));
            f32 yf1 = F.Sub(yf0, F.Broad_f32(1));
            f32 zf1 = F.Sub(zf0, F.Broad_f32(1));

            xs = Utils.InterpQuintic(xs);
            ys = Utils.InterpQuintic(ys);
            zs = Utils.InterpQuintic(zs);

            return F.Mul(F.Broad_f32(0.964921414852142333984375f), Utils.Lerp(
                Utils.Lerp(
                    Utils.Lerp(
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0, z0), xf0, yf0, zf0),
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0, z0), xf1, yf0, zf0),
                        xs),
                    Utils.Lerp(
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1, z0), xf0, yf1, zf0),
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1, z0), xf1, yf1, zf0),
                        xs),
                    ys),
                Utils.Lerp(
                    Utils.Lerp(
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0, z1), xf0, yf0, zf1),
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0, z1), xf1, yf0, zf1),
                        xs),
                    Utils.Lerp(
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1, z1), xf0, yf1, zf1),
                        Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1, z1), xf1, yf1, zf1),
                        xs),
                    ys),
                zs));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(i32 seed, f32 x, f32 y, f32 z, f32 w)
        {
            f32 xs = F.Floor_f32(x);
            f32 ys = F.Floor_f32(y);
            f32 zs = F.Floor_f32(z);
            f32 ws = F.Floor_f32(w);

            i32 x0 = F.Mul(F.Convertf32_i32(xs), F.Broad_i32(Primes.X));
            i32 y0 = F.Mul(F.Convertf32_i32(ys), F.Broad_i32(Primes.Y));
            i32 z0 = F.Mul(F.Convertf32_i32(zs), F.Broad_i32(Primes.Z));
            i32 w0 = F.Mul(F.Convertf32_i32(ws), F.Broad_i32(Primes.W));
            i32 x1 = F.Add(x0, F.Broad_i32(Primes.X));
            i32 y1 = F.Add(y0, F.Broad_i32(Primes.Y));
            i32 z1 = F.Add(z0, F.Broad_i32(Primes.Z));
            i32 w1 = F.Add(w0, F.Broad_i32(Primes.W));

            f32 xf0 = xs = F.Sub(x, xs);
            f32 yf0 = ys = F.Sub(y, ys);
            f32 zf0 = zs = F.Sub(z, zs);
            f32 wf0 = ws = F.Sub(w, ws);
            f32 xf1 = F.Sub(xf0, F.Broad_f32(1));
            f32 yf1 = F.Sub(yf0, F.Broad_f32(1));
            f32 zf1 = F.Sub(zf0, F.Broad_f32(1));
            f32 wf1 = F.Sub(wf0, F.Broad_f32(1));

            xs = Utils.InterpQuintic(xs);
            ys = Utils.InterpQuintic(ys);
            zs = Utils.InterpQuintic(zs);
            ws = Utils.InterpQuintic(ws);

            return F.Mul(F.Broad_f32(0.964921414852142333984375f), Utils.Lerp(
                Utils.Lerp(
                    Utils.Lerp(
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0, z0, w0), xf0, yf0, zf0, wf0),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0, z0, w0), xf1, yf0, zf0, wf0),
                            xs),
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1, z0, w0), xf0, yf1, zf0, wf0),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1, z0, w0), xf1, yf1, zf0, wf0),
                            xs),
                        ys),
                    Utils.Lerp(
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0, z1, w0), xf0, yf0, zf1, wf0),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0, z1, w0), xf1, yf0, zf1, wf0),
                            xs),
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1, z1, w0), xf0, yf1, zf1, wf0),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1, z1, w0), xf1, yf1, zf1, wf0),
                            xs),
                        ys),
                    zs),
                Utils.Lerp(
                    Utils.Lerp(
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0, z0, w1), xf0, yf0, zf0, wf1),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0, z0, w1), xf1, yf0, zf0, wf1),
                            xs),
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1, z0, w1), xf0, yf1, zf0, wf1),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1, z0, w1), xf1, yf1, zf0, wf1),
                            xs),
                        ys),
                    Utils.Lerp(
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y0, z1, w1), xf0, yf0, zf1, wf1),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y0, z1, w1), xf1, yf0, zf1, wf1),
                            xs),
                        Utils.Lerp(
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x0, y1, z1, w1), xf0, yf1, zf1, wf1),
                            Utils.GetGradientDot(Utils.HashPrimes(seed, x1, y1, z1, w1), xf1, yf1, zf1, wf1),
                            xs),
                        ys),
                    zs),
                ws));
        }
    }
}
