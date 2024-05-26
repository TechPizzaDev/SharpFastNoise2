using System.Runtime.CompilerServices;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2
{
    public partial struct Utils<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32, F>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimes(i32 seed, i32 x, i32 y)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, y));

            hash = F.Mul(hash, F.Broad(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimes(i32 seed, i32 x, i32 y, i32 z)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, z)));

            hash = F.Mul(hash, F.Broad(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimes(i32 seed, i32 x, i32 y, i32 z, i32 w)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            hash = F.Mul(hash, F.Broad(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, y));

            hash = F.Mul(hash, F.Broad(0x27d4eb2d));
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y, i32 z)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, z)));

            hash = F.Mul(hash, F.Broad(0x27d4eb2d));
            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y, i32 z, i32 w)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            hash = F.Mul(hash, F.Broad(0x27d4eb2d));
            return hash;
        }

        //public static float32v GetValueCoord(int32v seed, P...primedPos )
        //{
        //    int32v hash = seed;
        //    hash ^= (primedPos ^ ...);
        //
        //    hash = hash.Mul(hash.Mul(F.Broad_i32(0x27d4eb2d)));
        //    return F.Converti32_f32(hash).Mul(F.Broad_f32(1.0f / int.MaxValue));
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Lerp(f32 a, f32 b, f32 t)
        {
            return F.FMulAdd(t, F.Sub(b, a), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InterpHermite(f32 t)
        {
            return F.Mul(F.Mul(t, t), F.FNMulAdd_f32(t, F.Broad((float)2), F.Broad((float)3)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InterpQuintic(f32 t)
        {
            return F.Mul(
                F.Mul(F.Mul(t, t), t),
                F.FMulAdd(
                    t,
                    F.FMulAdd(t, F.Broad((float)6), F.Broad((float)-15)),
                    F.Broad((float)10)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(DistanceFunction distFunc, f32 dX, f32 dY)
        {
            switch (distFunc)
            {
                default:
                case DistanceFunction.Euclidean:
                    return DistanceEuclidean<m32, f32, i32, F>.CalcDistance(dX, dY);

                case DistanceFunction.EuclideanSquared:
                    return DistanceEuclideanSquared<m32, f32, i32, F>.CalcDistance(dX, dY);

                case DistanceFunction.Manhattan:
                    return DistanceManhattan<m32, f32, i32, F>.CalcDistance(dX, dY);

                case DistanceFunction.Hybrid:
                    return DistanceHybrid<m32, f32, i32, F>.CalcDistance(dX, dY);

                case DistanceFunction.MaxAxis:
                    return DistanceMaxAxis<m32, f32, i32, F>.CalcDistance(dX, dY);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(DistanceFunction distFunc, f32 dX, f32 dY, f32 dZ)
        {
            switch (distFunc)
            {
                default:
                case DistanceFunction.Euclidean:
                    return DistanceEuclidean<m32, f32, i32, F>.CalcDistance(dX, dY, dZ);

                case DistanceFunction.EuclideanSquared:
                    return DistanceEuclideanSquared<m32, f32, i32, F>.CalcDistance(dX, dY, dZ);

                case DistanceFunction.Manhattan:
                    return DistanceManhattan<m32, f32, i32, F>.CalcDistance(dX, dY, dZ);

                case DistanceFunction.Hybrid:
                    return DistanceHybrid<m32, f32, i32, F>.CalcDistance(dX, dY, dZ);

                case DistanceFunction.MaxAxis:
                    return DistanceMaxAxis<m32, f32, i32, F>.CalcDistance(dX, dY, dZ);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(DistanceFunction distFunc, f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            switch (distFunc)
            {
                default:
                case DistanceFunction.Euclidean:
                    return DistanceEuclidean<m32, f32, i32, F>.CalcDistance(dX, dY, dZ, dW);

                case DistanceFunction.EuclideanSquared:
                    return DistanceEuclideanSquared<m32, f32, i32, F>.CalcDistance(dX, dY, dZ, dW);

                case DistanceFunction.Manhattan:
                    return DistanceManhattan<m32, f32, i32, F>.CalcDistance(dX, dY, dZ, dW);

                case DistanceFunction.Hybrid:
                    return DistanceHybrid<m32, f32, i32, F>.CalcDistance(dX, dY, dZ, dW);

                case DistanceFunction.MaxAxis:
                    return DistanceMaxAxis<m32, f32, i32, F>.CalcDistance(dX, dY, dZ, dW);
            }
        }
    }
}
