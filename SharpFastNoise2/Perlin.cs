using System.Runtime.CompilerServices;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public struct Perlin<m32, f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : unmanaged, IFunctionList<m32, f32, i32>
    {
        public static int Count => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, i32 seed)
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

            xs = Utils<m32, f32, i32, F>.InterpQuintic(xs);
            ys = Utils<m32, f32, i32, F>.InterpQuintic(ys);

            return F.Mul(F.Broad_f32(0.579106986522674560546875f), Utils<m32, f32, i32, F>.Lerp(
                Utils<m32, f32, i32, F>.Lerp(
                    Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0), xf0, yf0),
                    Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0), xf1, yf0),
                    xs),
                Utils<m32, f32, i32, F>.Lerp(
                    Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1), xf0, yf1),
                    Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1), xf1, yf1),
                    xs),
                ys));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
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

            xs = Utils<m32, f32, i32, F>.InterpQuintic(xs);
            ys = Utils<m32, f32, i32, F>.InterpQuintic(ys);
            zs = Utils<m32, f32, i32, F>.InterpQuintic(zs);

            return F.Mul(F.Broad_f32(0.964921414852142333984375f), Utils<m32, f32, i32, F>.Lerp(
                Utils<m32, f32, i32, F>.Lerp(
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0, z0), xf0, yf0, zf0),
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0, z0), xf1, yf0, zf0),
                        xs),
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1, z0), xf0, yf1, zf0),
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1, z0), xf1, yf1, zf0),
                        xs),
                    ys),
                Utils<m32, f32, i32, F>.Lerp(
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0, z1), xf0, yf0, zf1),
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0, z1), xf1, yf0, zf1),
                        xs),
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1, z1), xf0, yf1, zf1),
                        Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1, z1), xf1, yf1, zf1),
                        xs),
                    ys),
                zs));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
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

            xs = Utils<m32, f32, i32, F>.InterpQuintic(xs);
            ys = Utils<m32, f32, i32, F>.InterpQuintic(ys);
            zs = Utils<m32, f32, i32, F>.InterpQuintic(zs);
            ws = Utils<m32, f32, i32, F>.InterpQuintic(ws);

            return F.Mul(F.Broad_f32(0.964921414852142333984375f), Utils<m32, f32, i32, F>.Lerp(
                Utils<m32, f32, i32, F>.Lerp(
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0, z0, w0), xf0, yf0, zf0, wf0),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0, z0, w0), xf1, yf0, zf0, wf0),
                            xs),
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1, z0, w0), xf0, yf1, zf0, wf0),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1, z0, w0), xf1, yf1, zf0, wf0),
                            xs),
                        ys),
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0, z1, w0), xf0, yf0, zf1, wf0),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0, z1, w0), xf1, yf0, zf1, wf0),
                            xs),
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1, z1, w0), xf0, yf1, zf1, wf0),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1, z1, w0), xf1, yf1, zf1, wf0),
                            xs),
                        ys),
                    zs),
                Utils<m32, f32, i32, F>.Lerp(
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0, z0, w1), xf0, yf0, zf0, wf1),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0, z0, w1), xf1, yf0, zf0, wf1),
                            xs),
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1, z0, w1), xf0, yf1, zf0, wf1),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1, z0, w1), xf1, yf1, zf0, wf1),
                            xs),
                        ys),
                    Utils<m32, f32, i32, F>.Lerp(
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y0, z1, w1), xf0, yf0, zf1, wf1),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y0, z1, w1), xf1, yf0, zf1, wf1),
                            xs),
                        Utils<m32, f32, i32, F>.Lerp(
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x0, y1, z1, w1), xf0, yf1, zf1, wf1),
                            Utils<m32, f32, i32, F>.GetGradientDot(Utils<m32, f32, i32, F>.HashPrimes(seed, x1, y1, z1, w1), xf1, yf1, zf1, wf1),
                            xs),
                        ys),
                    zs),
                ws));
        }
    }
}
