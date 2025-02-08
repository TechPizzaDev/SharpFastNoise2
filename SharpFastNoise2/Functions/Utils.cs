using System.Runtime.CompilerServices;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2
{
    public partial struct Utils<f32, i32, F>
        where F : IFunctionList<f32, i32, F>
    {
        private const int Prime = 0x27d4eb2d;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimes(i32 seed, i32 x, i32 y)
        {
            i32 hash = F.Xor(seed, F.Xor(x, y));

            hash = F.Mul(hash, F.Broad(Prime));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimes(i32 seed, i32 x, i32 y, i32 z)
        {
            i32 hash = F.Xor(seed, F.Xor(x, F.Xor(y, z)));

            hash = F.Mul(hash, F.Broad(Prime));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimes(i32 seed, i32 x, i32 y, i32 z, i32 w)
        {
            i32 hash = F.Xor(seed, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            hash = F.Mul(hash, F.Broad(Prime));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y)
        {
            i32 hash = F.Xor(seed, F.Xor(x, y));

            return F.Mul(hash, F.Broad(Prime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y, i32 z)
        {
            i32 hash = F.Xor(seed, F.Xor(x, F.Xor(y, z)));

            return F.Mul(hash, F.Broad(Prime));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y, i32 z, i32 w)
        {
            i32 hash = F.Xor(seed, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            return F.Mul(hash, F.Broad(Prime));
        }

        //public static float32v GetValueCoord(int32v seed, P...primedPos )
        //{
        //    int32v hash = seed;
        //    hash ^= (primedPos ^ ...);
        //
        //    hash = hash.Mul(hash.Mul(F.Broad(Prime)));
        //    return F.Converti32_f32(hash).Mul(F.Broad(1.0f / int.MaxValue));
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Lerp(f32 a, f32 b, f32 t)
        {
            return F.FMulAdd(t, F.Sub(b, a), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InterpHermite(f32 t)
        {
            return F.Mul(F.Mul(t, t), F.FMulAdd(t, F.Broad(-2f), F.Broad(3f)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InterpQuintic(f32 t)
        {
            return F.Mul(
                F.Mul(F.Mul(t, t), t),
                F.FMulAdd(
                    t,
                    F.FMulAdd(t, F.Broad(6f), F.Broad(-15f)),
                    F.Broad(10f)));
        }

        public static f32 CalcDistance(DistanceFunction distFunc, f32 dX, f32 dY)
        {
            return distFunc switch
            {
                DistanceFunction.Euclidean => DistanceEuclidean<f32, i32, F>.CalcDistance(dX, dY),
                DistanceFunction.EuclideanSquared => DistanceEuclideanSquared<f32, i32, F>.CalcDistance(dX, dY),
                DistanceFunction.Manhattan => DistanceManhattan<f32, i32, F>.CalcDistance(dX, dY),
                DistanceFunction.Hybrid => DistanceHybrid<f32, i32, F>.CalcDistance(dX, dY),
                DistanceFunction.MaxAxis => DistanceMaxAxis<f32, i32, F>.CalcDistance(dX, dY),
                _ => DistanceEuclideanEstimate<f32, i32, F>.CalcDistance(dX, dY),
            };
        }

        public static f32 CalcDistance(DistanceFunction distFunc, f32 dX, f32 dY, f32 dZ)
        {
            return distFunc switch
            {
                DistanceFunction.Euclidean => DistanceEuclidean<f32, i32, F>.CalcDistance(dX, dY, dZ),
                DistanceFunction.EuclideanSquared => DistanceEuclideanSquared<f32, i32, F>.CalcDistance(dX, dY, dZ),
                DistanceFunction.Manhattan => DistanceManhattan<f32, i32, F>.CalcDistance(dX, dY, dZ),
                DistanceFunction.Hybrid => DistanceHybrid<f32, i32, F>.CalcDistance(dX, dY, dZ),
                DistanceFunction.MaxAxis => DistanceMaxAxis<f32, i32, F>.CalcDistance(dX, dY, dZ),
                _ => DistanceEuclideanEstimate<f32, i32, F>.CalcDistance(dX, dY, dZ),
            };
        }

        public static f32 CalcDistance(DistanceFunction distFunc, f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            return distFunc switch
            {
                DistanceFunction.Euclidean => DistanceEuclidean<f32, i32, F>.CalcDistance(dX, dY, dZ, dW),
                DistanceFunction.EuclideanSquared => DistanceEuclideanSquared<f32, i32, F>.CalcDistance(dX, dY, dZ, dW),
                DistanceFunction.Manhattan => DistanceManhattan<f32, i32, F>.CalcDistance(dX, dY, dZ, dW),
                DistanceFunction.Hybrid => DistanceHybrid<f32, i32, F>.CalcDistance(dX, dY, dZ, dW),
                DistanceFunction.MaxAxis => DistanceMaxAxis<f32, i32, F>.CalcDistance(dX, dY, dZ, dW),
                _ => DistanceEuclideanEstimate<f32, i32, F>.CalcDistance(dX, dY, dZ, dW),
            };
        }
    }
}
