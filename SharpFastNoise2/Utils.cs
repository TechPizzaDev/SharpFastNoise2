using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public struct Utils<mask32v, float32v, int32v, TFunctions>
        where mask32v : unmanaged
        where float32v : unmanaged
        where int32v : unmanaged
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions F = default;

        public const float ROOT2 = 1.4142135623730950488f;
        public const float ROOT3 = 1.7320508075688772935f;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float32v GetGradientDotFancy(int32v hash, float32v fX, float32v fY)
        {
            if (typeof(int32v) == typeof(Vector256<int>))
            {
                int32v index = F.Convertf32_i32(F.Mul(
                    F.Converti32_f32(F.And(hash, F.Broad_i32(0x3FFFFF))),
                    F.Broad_f32(1.3333333333333333f)));
            
                Vector256<float> gX = Avx2.PermuteVar8x32(
                    Vector256.Create(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0),
                    Unsafe.As<int32v, Vector256<int>>(ref index));
            
                Vector256<float> gY = Avx2.PermuteVar8x32(
                    Vector256.Create(1, -1, 0, 0, ROOT3, ROOT3, 2, 2),
                    Unsafe.As<int32v, Vector256<int>>(ref index));
            
                // Bit-8 = Flip sign of a + b
                return F.Xor(
                    F.FMulAdd_f32(
                        Unsafe.As<Vector256<float>, float32v>(ref gX),
                        fX,
                        F.Mul(fY, Unsafe.As<Vector256<float>, float32v>(ref gY))),
                    F.Casti32_f32(F.LeftShift(F.RightShift(index, 3), 31)));
            }
            else
            {
                int32v index = F.Convertf32_i32(F.Mul(F.Converti32_f32(F.And(hash, F.Broad_i32(0x3FFFFF))), F.Broad_f32(1.3333333333333333f)));

                // Bit-4 = Choose X Y ordering
                mask32v xy;

                if (F.Count == 1)
                {
                    xy = F.NotEqual(F.And(index, F.Broad_i32(1 << 2)), F.Broad_i32(0));
                }
                else
                {
                    int32v xyV = F.LeftShift(index, 29);
                    if (!Sse41.IsSupported)
                    {
                        xyV = F.RightShift(xyV, 31);
                    }
                    xy = Unsafe.As<int32v, mask32v>(ref xyV);
                }

                float32v a = F.Select_f32(xy, fY, fX);
                float32v b = F.Select_f32(xy, fX, fY);

                // Bit-1 = b flip sign
                b = F.Xor(b, F.Casti32_f32(F.LeftShift(index, 31)));

                // Bit-2 = Mul a by 2 or Root3
                mask32v aMul2;

                if (F.Count == 1)
                {
                    aMul2 = F.NotEqual(F.And(index, F.Broad_i32(1 << 1)), F.Broad_i32(0));
                }
                else
                {
                    int32v aMul2V = F.RightShift(F.LeftShift(index, 30), 31);
                    aMul2 = Unsafe.As<int32v, mask32v>(ref aMul2V);
                }

                a = F.Mul(a, F.Select_f32(aMul2, F.Broad_f32(2), F.Broad_f32(ROOT3)));
                // b zero value if a mul 2
                b = F.NMask_f32(b, aMul2);

                // Bit-8 = Flip sign of a + b
                return F.Xor(F.Add(a, b), F.Casti32_f32(F.LeftShift(F.RightShift(index, 3), 31)));
            }
        }

        //template<typename SIMD = FS, std::enable_if_t<(SIMD::SIMD_Level == FastSIMD::Level_AVX512)>* = nullptr>
        //FS.INLINE static float32v GetGradientDotFancy(int32v hash, float32v fX, float32v fY)
        //{
        //    int32v index = FS.Convertf32_i32(FS.Converti32_f32(hash & FS.Broadcast(0x3FFFFF)) * float32v(1.3333333333333333f));
        //
        //    float32v gX = _mm512_permutexvar_ps(index, float32v(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0, -ROOT3, -ROOT3, -2, -2, -1, 1, 0, 0));
        //    float32v gY = _mm512_permutexvar_ps(index, float32v(1, -1, 0, 0, ROOT3, ROOT3, 2, 2, -1, 1, 0, 0, -ROOT3, -ROOT3, -2, -2));
        //
        //    return FS.FMulAdd_f32(gX, fX, fY * gY);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public float32v GetGradientDot(int32v hash, float32v fX, float32v fY)
        {
            if (typeof(int32v) == typeof(Vector256<int>))
            {
                Vector256<float> gX = Avx2.PermuteVar8x32(
                    Vector256.Create(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1),
                    Unsafe.As<int32v, Vector256<int>>(ref hash));

                Vector256<float> gY = Avx2.PermuteVar8x32(
                    Vector256.Create(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2),
                    Unsafe.As<int32v, Vector256<int>>(ref hash));

                return F.FMulAdd_f32(
                    Unsafe.As<Vector256<float>, float32v>(ref gX),
                    fX,
                    F.Mul(fY, Unsafe.As<Vector256<float>, float32v>(ref gY)));
            }
            else
            {
                // ( 1+R2, 1 ) ( -1-R2, 1 ) ( 1+R2, -1 ) ( -1-R2, -1 )
                // ( 1, 1+R2 ) ( 1, -1-R2 ) ( -1, 1+R2 ) ( -1, -1-R2 )

                int32v bit1 = F.LeftShift(hash, 31);
                int32v bit2 = F.LeftShift(F.RightShift(hash, 1), 31);
                mask32v mbit4;

                if (F.Count == 1)
                {
                    mbit4 = F.NotEqual(F.And(hash, F.Broad_i32(1 << 2)), F.Broad_i32(0));
                }
                else
                {
                    int32v bit4 = F.LeftShift(hash, 29);
                    if (!Sse41.IsSupported)
                    {
                        bit4 = F.RightShift(bit4, 31);
                    }
                    mbit4 = Unsafe.As<int32v, mask32v>(ref bit4);
                }

                fX = F.Xor(fX, F.Casti32_f32(bit1));
                fY = F.Xor(fY, F.Casti32_f32(bit2));

                float32v a = F.Select_f32(mbit4, fY, fX);
                float32v b = F.Select_f32(mbit4, fX, fY);

                return F.FMulAdd_f32(F.Broad_f32(1.0f + ROOT2), a, b);
            }
        }

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX2>* = nullptr>
        //FS.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY)
        //{
        //    float32v gX = _mm256_permutevar8x32_ps(float32v(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1), hash);
        //    float32v gY = _mm256_permutevar8x32_ps(float32v(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2), hash);
        //
        //    return FS.FMulAdd_f32(gX, fX, fY * gY);
        //}

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX512>* = nullptr>
        // FS.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY)
        //{
        //    float32v gX = _mm512_permutexvar_ps(hash, float32v(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1, 1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1));
        //    float32v gY = _mm512_permutexvar_ps(hash, float32v(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2, 1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2));
        //
        //    return FS.FMulAdd_f32(gX, fX, fY * gY);
        //}

        public float32v GetGradientDot(int32v hash, float32v fX, float32v fY, float32v fZ)
        {
            int32v hasha13 = F.And(hash, F.Broad_i32(13));

            //if h < 8 then x, else y
            float32v u = F.Select_f32(F.LessThan(hasha13, F.Broad_i32(8)), fX, fY);

            //if h < 4 then y else if h is 12 or 14 then x else z
            float32v v = F.Select_f32(F.Equal(hasha13, F.Broad_i32(12)), fX, fZ);
            v = F.Select_f32(F.LessThan(hasha13, F.Broad_i32(2)), fY, v);

            //if h1 then -u else u
            //if h2 then -v else v
            float32v h1 = F.Casti32_f32(F.LeftShift(hash, 31));
            float32v h2 = F.Casti32_f32(F.LeftShift(F.And(hash, F.Broad_i32(2)), 30));
            //then add them
            return F.Add(F.Xor(u, h1), F.Xor(v, h2));
        }

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX512>* = nullptr>
        //FS.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY, float32v fZ)
        //{
        //    float32v gX = _mm512_permutexvar_ps(hash, float32v(1, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0, 1, 0, -1, 0));
        //    float32v gY = _mm512_permutexvar_ps(hash, float32v(1, 1, -1, -1, 0, 0, 0, 0, 1, -1, 1, -1, 1, -1, 1, -1));
        //    float32v gZ = _mm512_permutexvar_ps(hash, float32v(0, 0, 0, 0, 1, 1, -1, -1, 1, 1, -1, -1, 0, 1, 0, -1));
        //
        //    return FS.FMulAdd_f32(gX, fX, FS.FMulAdd_f32(fY, gY, fZ * gZ));
        //}

        public float32v GetGradientDot(int32v hash, float32v fX, float32v fY, float32v fZ, float32v fW)
        {
            int32v p = F.And(hash, F.Broad_i32(3 << 3));

            float32v a = F.Select_f32(F.GreaterThan(p, F.Broad_i32(0)), fX, fY);
            float32v b;

            if (Sse41.IsSupported)
            {
                b = F.Select_f32(F.GreaterThan(p, F.Broad_i32(1 << 3)), fY, fZ);
            }
            else
            {
                int32v mask = F.LeftShift(hash, 27);
                b = F.Select_f32(Unsafe.As<int32v, mask32v>(ref mask), fY, fZ);
            }

            float32v c = F.Select_f32(F.GreaterThan(p, F.Broad_i32(2 << 3)), fZ, fW);

            unchecked
            {
                float32v aSign = F.Casti32_f32(F.LeftShift(hash, 31));
                float32v bSign = F.Casti32_f32(F.And(F.LeftShift(hash, 30), F.Broad_i32((int)0x80000000)));
                float32v cSign = F.Casti32_f32(F.And(F.LeftShift(hash, 29), F.Broad_i32((int)0x80000000)));
                return F.Add(F.Add(F.Xor(a, aSign), F.Xor(b, bSign)), F.Xor(c, cSign));
            }
        }

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX512>* = nullptr>
        //FS.INLINE static float32v GetGradientDot(int32v hash, float32v fX, float32v fY, float32v fZ, float32v fW)
        //{
        //    float32v gX = _mm512_permutex2var_ps(float32v(0, 0, 0, 0, 0, 0, 0, 0, 1, -1, 1, -1, 1, -1, 1, -1), hash, float32v(1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1));
        //    float32v gY = _mm512_permutex2var_ps(float32v(1, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0), hash, float32v(1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1));
        //    float32v gZ = _mm512_permutex2var_ps(float32v(1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1), hash, float32v(0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, -1, -1, -1, -1));
        //    float32v gW = _mm512_permutex2var_ps(float32v(1, 1, 1, 1, -1, -1, -1, -1, 1, 1, 1, 1, -1, -1, -1, -1), hash, float32v(1, 1, 1, 1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0));
        //
        //    return FS.FMulAdd_f32(gX, fX, FS.FMulAdd_f32(fY, gY, FS.FMulAdd_f32(fZ, gZ, fW * gW)));
        //}

        public int32v HashPrimes(int32v seed, int32v x, int32v y)
        {
            int32v hash = seed;
            hash = F.Xor(hash, F.Xor(x, y));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        public int32v HashPrimes(int32v seed, int32v x, int32v y, int32v z)
        {
            int32v hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, z)));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        public int32v HashPrimes(int32v seed, int32v x, int32v y, int32v z, int32v w)
        {
            int32v hash = seed;
            hash = F.Xor(hash, F.Xor(x, F.Xor(y, F.Xor(z, w))));

            hash = F.Mul(hash, F.Broad_i32(0x27d4eb2d));
            return F.Xor(F.RightShift(hash, 15), hash);
        }

        //public static int32v HashPrimesHB(int32v seed, P...primedPos )
        //{
        //    int32v hash = seed;
        //    hash ^= (primedPos ^ ...);
        //
        //    hash = hash.Mul(FS.Broad_i32(0x27d4eb2d));
        //    return hash;
        //}
        //
        //public static float32v GetValueCoord(int32v seed, P...primedPos )
        //{
        //    int32v hash = seed;
        //    hash ^= (primedPos ^ ...);
        //
        //    hash = hash.Mul(hash.Mul(FS.Broad_i32(0x27d4eb2d)));
        //    return FS.Converti32_f32(hash).Mul(FS.Broad_f32(1.0f / int.MaxValue));
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v Lerp(float32v a, float32v b, float32v t)
        {
            return F.FMulAdd_f32(t, F.Sub(b, a), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v InterpHermite(float32v t)
        {
            return F.Mul(F.Mul(t, t), F.FNMulAdd_f32(t, F.Broad_f32(2), F.Broad_f32(3)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v InterpQuintic(float32v t)
        {
            return F.Mul(F.Mul(F.Mul(t, t), t), F.FMulAdd_f32(t, F.FMulAdd_f32(t, F.Broad_f32(6), F.Broad_f32(-15)), F.Broad_f32(10)));
        }

        //public float32v CalcDistance(DistanceFunction distFunc, float32v dX, P...d )
        //{
        //    switch (distFunc)
        //    {
        //        default:
        //        case DistanceFunction::Euclidean:
        //        {
        //            float32v distSqr = dX * dX;
        //            ((distSqr = FS.FMulAdd_f32(d, d, distSqr)), ...);
        //
        //            return FS.InvSqrt_f32(distSqr) * distSqr;
        //        }
        //
        //        case DistanceFunction::EuclideanSquared:
        //        {
        //            float32v distSqr = dX * dX;
        //            ((distSqr = FS.FMulAdd_f32(d, d, distSqr)), ...);
        //
        //            return distSqr;
        //        }
        //
        //        case DistanceFunction::Manhattan:
        //        {
        //            float32v dist = FS.Abs_f32(dX);
        //            dist += (FS.Abs_f32(d) + ...);
        //
        //            return dist;
        //        }
        //
        //        case DistanceFunction::Hybrid:
        //        {
        //            float32v both = FS.FMulAdd_f32(dX, dX, FS.Abs_f32(dX));
        //            ((both += FS.FMulAdd_f32(d, d, FS.Abs_f32(d))), ...);
        //
        //            return both;
        //        }
        //
        //        case DistanceFunction::MaxAxis:
        //        {
        //            float32v max = FS.Abs_f32(dX);
        //            ((max = FS.Max_f32(FS.Abs_f32(d), max)), ...);
        //
        //            return max;
        //        }
        //    }
        //}
    }
}
