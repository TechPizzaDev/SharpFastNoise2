using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2
{
    public partial struct Utils<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32>
    {
        public const float ROOT2 = 1.4142135623730950488f;
        public const float ROOT3 = 1.7320508075688772935f;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static f32 GetGradientDotFancy(i32 hash, f32 fX, f32 fY)
        {
            if (typeof(i32) == typeof(Vector256<int>))
            {
                i32 index = F.Convertf32_i32(F.Mul(
                    F.Converti32_f32(F.And(hash, F.Broad_i32(0x3FFFFF))),
                    F.Broad_f32(1.3333333333333333f)));

                Vector256<float> gX = Avx2.PermuteVar8x32(
                    Vector256.Create(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0),
                    Unsafe.BitCast<i32, Vector256<int>>(index));

                Vector256<float> gY = Avx2.PermuteVar8x32(
                    Vector256.Create(1, -1, 0, 0, ROOT3, ROOT3, 2, 2),
                    Unsafe.BitCast<i32, Vector256<int>>(index));

                // Bit-8 = Flip sign of a + b
                return F.Xor(
                    F.FMulAdd_f32(
                        Unsafe.BitCast<Vector256<float>, f32>(gX),
                        fX,
                        F.Mul(fY, Unsafe.BitCast<Vector256<float>, f32>(gY))),
                    F.Casti32_f32(F.LeftShift(F.RightShift(index, 3), 31)));
            }
            else
            {
                i32 index = F.Convertf32_i32(F.Mul(F.Converti32_f32(F.And(hash, F.Broad_i32(0x3FFFFF))), F.Broad_f32(1.3333333333333333f)));

                // Bit-4 = Choose X Y ordering
                m32 xy;

                if (F.Count == 1)
                {
                    xy = F.NotEqual(F.And(index, F.Broad_i32(1 << 2)), F.Broad_i32(0));
                }
                else
                {
                    i32 xyV = F.LeftShift(index, 29);
                    if (!Sse41.IsSupported)
                    {
                        xyV = F.RightShift(xyV, 31);
                    }
                    xy = Unsafe.BitCast<i32, m32>(xyV);
                }

                f32 a = F.Select_f32(xy, fY, fX);
                f32 b = F.Select_f32(xy, fX, fY);

                // Bit-1 = b flip sign
                b = F.Xor(b, F.Casti32_f32(F.LeftShift(index, 31)));

                // Bit-2 = Mul a by 2 or Root3
                m32 aMul2;

                if (F.Count == 1)
                {
                    aMul2 = F.NotEqual(F.And(index, F.Broad_i32(1 << 1)), F.Broad_i32(0));
                }
                else
                {
                    i32 aMul2V = F.RightShift(F.LeftShift(index, 30), 31);
                    aMul2 = Unsafe.BitCast<i32, m32>(aMul2V);
                }

                a = F.Mul(a, F.Select_f32(aMul2, F.Broad_f32(2), F.Broad_f32(ROOT3)));
                // b zero value if a mul 2
                b = F.NMask_f32(b, aMul2);

                // Bit-8 = Flip sign of a + b
                return F.Xor(F.Add(a, b), F.Casti32_f32(F.LeftShift(F.RightShift(index, 3), 31)));
            }
        }

        //template<typename SIMD = FS, std::enable_if_t<(SIMD::SIMD_Level == FastSIMD::Level_AVX512)>* = nullptr>
        //F.INLINE static float32v GetGradientDotFancy(int32v hash, float32v fX, float32v fY)
        //{
        //    int32v index = F.Convertf32_i32(F.Converti32_f32(hash & F.Broadcast(0x3FFFFF)) * float32v(1.3333333333333333f));
        //
        //    float32v gX = _mm512_permutexvar_ps(index, float32v(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0, -ROOT3, -ROOT3, -2, -2, -1, 1, 0, 0));
        //    float32v gY = _mm512_permutexvar_ps(index, float32v(1, -1, 0, 0, ROOT3, ROOT3, 2, 2, -1, 1, 0, 0, -ROOT3, -ROOT3, -2, -2));
        //
        //    return F.FMulAdd_f32(gX, fX, fY * gY);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY)
        {
            if (typeof(i32) == typeof(Vector256<int>))
            {
                Vector256<float> gX = Avx2.PermuteVar8x32(
                    Vector256.Create(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1),
                    Unsafe.BitCast<i32, Vector256<int>>(hash));

                Vector256<float> gY = Avx2.PermuteVar8x32(
                    Vector256.Create(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2),
                    Unsafe.BitCast<i32, Vector256<int>>(hash));

                return F.FMulAdd_f32(
                    Unsafe.BitCast<Vector256<float>, f32>(gX),
                    fX,
                    F.Mul(fY, Unsafe.BitCast<Vector256<float>, f32>(gY)));
            }
            else
            {
                // ( 1+R2, 1 ) ( -1-R2, 1 ) ( 1+R2, -1 ) ( -1-R2, -1 )
                // ( 1, 1+R2 ) ( 1, -1-R2 ) ( -1, 1+R2 ) ( -1, -1-R2 )

                i32 bit1 = F.LeftShift(hash, 31);
                i32 bit2 = F.LeftShift(F.RightShift(hash, 1), 31);
                m32 mbit4;

                if (F.Count == 1)
                {
                    mbit4 = F.NotEqual(F.And(hash, F.Broad_i32(1 << 2)), F.Broad_i32(0));
                }
                else
                {
                    i32 bit4 = F.LeftShift(hash, 29);
                    if (!Sse41.IsSupported)
                    {
                        bit4 = F.RightShift(bit4, 31);
                    }
                    mbit4 = Unsafe.BitCast<i32, m32>(bit4);
                }

                fX = F.Xor(fX, F.Casti32_f32(bit1));
                fY = F.Xor(fY, F.Casti32_f32(bit2));

                f32 a = F.Select_f32(mbit4, fY, fX);
                f32 b = F.Select_f32(mbit4, fX, fY);

                return F.FMulAdd_f32(F.Broad_f32(1.0f + ROOT2), a, b);
            }
        }

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX512>* = nullptr>
        // F.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY)
        //{
        //    float32v gX = _mm512_permutexvar_ps(hash, float32v(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1, 1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1));
        //    float32v gY = _mm512_permutexvar_ps(hash, float32v(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2, 1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2));
        //
        //    return F.FMulAdd_f32(gX, fX, fY * gY);
        //}

        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY, f32 fZ)
        {
            i32 hasha13 = F.And(hash, F.Broad_i32(13));

            //if h < 8 then x, else y
            f32 u = F.Select_f32(F.LessThan(hasha13, F.Broad_i32(8)), fX, fY);

            //if h < 4 then y else if h is 12 or 14 then x else z
            f32 v = F.Select_f32(F.Equal(hasha13, F.Broad_i32(12)), fX, fZ);
            v = F.Select_f32(F.LessThan(hasha13, F.Broad_i32(2)), fY, v);

            //if h1 then -u else u
            //if h2 then -v else v
            f32 h1 = F.Casti32_f32(F.LeftShift(hash, 31));
            f32 h2 = F.Casti32_f32(F.LeftShift(F.And(hash, F.Broad_i32(2)), 30));
            //then add them
            return F.Add(F.Xor(u, h1), F.Xor(v, h2));
        }

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX512>* = nullptr>
        //F.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY, float32v fZ)
        //{
        //    float32v gX = _mm512_permutexvar_ps(hash, float32v(1, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0, 1, 0, -1, 0));
        //    float32v gY = _mm512_permutexvar_ps(hash, float32v(1, 1, -1, -1, 0, 0, 0, 0, 1, -1, 1, -1, 1, -1, 1, -1));
        //    float32v gZ = _mm512_permutexvar_ps(hash, float32v(0, 0, 0, 0, 1, 1, -1, -1, 1, 1, -1, -1, 0, 1, 0, -1));
        //
        //    return F.FMulAdd_f32(gX, fX, F.FMulAdd_f32(fY, gY, fZ * gZ));
        //}

        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY, f32 fZ, f32 fW)
        {
            i32 p = F.And(hash, F.Broad_i32(3 << 3));

            f32 a = F.Select_f32(F.GreaterThan(p, F.Broad_i32(0)), fX, fY);
            f32 b;

            if (Sse41.IsSupported)
            {
                b = F.Select_f32(F.GreaterThan(p, F.Broad_i32(1 << 3)), fY, fZ);
            }
            else
            {
                i32 mask = F.LeftShift(hash, 27);
                b = F.Select_f32(Unsafe.BitCast<i32, m32>(mask), fY, fZ);
            }

            f32 c = F.Select_f32(F.GreaterThan(p, F.Broad_i32(2 << 3)), fZ, fW);

            unchecked
            {
                f32 aSign = F.Casti32_f32(F.LeftShift(hash, 31));
                f32 bSign = F.Casti32_f32(F.And(F.LeftShift(hash, 30), F.Broad_i32((int) 0x80000000)));
                f32 cSign = F.Casti32_f32(F.And(F.LeftShift(hash, 29), F.Broad_i32((int) 0x80000000)));
                return F.Add(F.Add(F.Xor(a, aSign), F.Xor(b, bSign)), F.Xor(c, cSign));
            }
        }

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX512>* = nullptr>
        //F.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY, float32v fZ, float32v fW)
        //{
        //    float32v gX = _mm512_permutex2var_ps(float32v(0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 1, -1, 1, -1, 1, -1), hash, float32v(1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1));
        //    float32v gY = _mm512_permutex2var_ps(float32v(1, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0), hash, float32v(1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1));
        //    float32v gZ = _mm512_permutex2var_ps(float32v(1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1), hash, float32v(0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, -1, -1, -1, -1));
        //    float32v gW = _mm512_permutex2var_ps(float32v(1, 1, 1, 1, -1, -1, -1, -1, 1, 1, 1, 1, -1, -1, -1, -1), hash, float32v(1, 1, 1, 1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0));
        //
        //    return F.FMulAdd_f32(gX, fX, F.FMulAdd_f32(fY, gY, F.FMulAdd_f32(fZ, gZ, fW * gW)));
        //}

        public static i32 HashPrimes(i32 seed, i32 x, i32 y)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, y));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        public static i32 HashPrimes(i32 seed, i32 x, i32 y, i32 z)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, z)));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        public static i32 HashPrimes(i32 seed, i32 x, i32 y, i32 z, i32 w)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, y));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return hash;
        }

        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y, i32 z)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, z)));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return hash;
        }

        public static i32 HashPrimesHB(i32 seed, i32 x, i32 y, i32 z, i32 w)
        {
            i32 hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
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
            return F.FMulAdd_f32(t, F.Sub(b, a), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InterpHermite(f32 t)
        {
            return F.Mul(F.Mul(t, t), F.FNMulAdd_f32(t, F.Broad_f32(2), F.Broad_f32(3)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InterpQuintic(f32 t)
        {
            return F.Mul(
                F.Mul(F.Mul(t, t), t),
                F.FMulAdd_f32(
                    t,
                    F.FMulAdd_f32(t, F.Broad_f32(6), F.Broad_f32(-15)),
                    F.Broad_f32(10)));
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
