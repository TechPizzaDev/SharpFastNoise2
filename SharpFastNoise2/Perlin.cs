using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct Perlin<mask32v, float32v, int32v, TFunctions> :
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

        public int Count => F.Count;

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public float32v Gen(int32v seed, float32v x, float32v y)
        {
            float32v xs = F.Floor_f32(x);
            float32v ys = F.Floor_f32(y);

            int32v x0 = F.Mul(F.Convertf32_i32(xs), F.Broad_i32(Primes.X));
            int32v y0 = F.Mul(F.Convertf32_i32(ys), F.Broad_i32(Primes.Y));
            int32v x1 = F.Add(x0, F.Broad_i32(Primes.X));
            int32v y1 = F.Add(y0, F.Broad_i32(Primes.Y));

            float32v xf0 = xs = F.Sub(x, xs);
            float32v yf0 = ys = F.Sub(y, ys);
            float32v xf1 = F.Sub(xf0, F.Broad_f32(1));
            float32v yf1 = F.Sub(yf0, F.Broad_f32(1));

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
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        {
            float32v xs = F.Floor_f32(x);
            float32v ys = F.Floor_f32(y);
            float32v zs = F.Floor_f32(z);

            int32v x0 = F.Mul(F.Convertf32_i32(xs), F.Broad_i32(Primes.X));
            int32v y0 = F.Mul(F.Convertf32_i32(ys), F.Broad_i32(Primes.Y));
            int32v z0 = F.Mul(F.Convertf32_i32(zs), F.Broad_i32(Primes.Z));
            int32v x1 = F.Add(x0, F.Broad_i32(Primes.X));
            int32v y1 = F.Add(y0, F.Broad_i32(Primes.Y));
            int32v z1 = F.Add(z0, F.Broad_i32(Primes.Z));

            float32v xf0 = xs = F.Sub(x, xs);
            float32v yf0 = ys = F.Sub(y, ys);
            float32v zf0 = zs = F.Sub(z, zs);
            float32v xf1 = F.Sub(xf0, F.Broad_f32(1));
            float32v yf1 = F.Sub(yf0, F.Broad_f32(1));
            float32v zf1 = F.Sub(zf0, F.Broad_f32(1));

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
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z, float32v w)
        {
            float32v xs = F.Floor_f32(x);
            float32v ys = F.Floor_f32(y);
            float32v zs = F.Floor_f32(z);
            float32v ws = F.Floor_f32(w);

            int32v x0 = F.Mul(F.Convertf32_i32(xs), F.Broad_i32(Primes.X));
            int32v y0 = F.Mul(F.Convertf32_i32(ys), F.Broad_i32(Primes.Y));
            int32v z0 = F.Mul(F.Convertf32_i32(zs), F.Broad_i32(Primes.Z));
            int32v w0 = F.Mul(F.Convertf32_i32(ws), F.Broad_i32(Primes.W));
            int32v x1 = F.Add(x0, F.Broad_i32(Primes.X));
            int32v y1 = F.Add(y0, F.Broad_i32(Primes.Y));
            int32v z1 = F.Add(z0, F.Broad_i32(Primes.Z));
            int32v w1 = F.Add(w0, F.Broad_i32(Primes.W));

            float32v xf0 = xs = F.Sub(x, xs);
            float32v yf0 = ys = F.Sub(y, ys);
            float32v zf0 = zs = F.Sub(z, zs);
            float32v wf0 = ws = F.Sub(w, ws);
            float32v xf1 = F.Sub(xf0, F.Broad_f32(1));
            float32v yf1 = F.Sub(yf0, F.Broad_f32(1));
            float32v zf1 = F.Sub(zf0, F.Broad_f32(1));
            float32v wf1 = F.Sub(wf0, F.Broad_f32(1));

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
