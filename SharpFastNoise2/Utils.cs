
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public struct Utils<mask32v, float32v, int32v, TFunctions>
        where mask32v : IFMask<mask32v>
        where float32v : IFVector<float32v, mask32v>
        where int32v : IFVector<int32v, mask32v>
        where TFunctions : struct, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions FS;

        public const float ROOT2 = 1.4142135623730950488f;
        public const float ROOT3 = 1.7320508075688772935f;

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level<FastSIMD::Level_AVX2>* = nullptr>
        //public float32v GetGradientDotFancy(int32v hash, float32v fX, float32v fY)
        //{
        //    int32v index = FS.Convertf32_i32(FS.Converti32_f32(hash.And(FS.Broad_i32(0x3FFFFF))).Mul(FS.Broad_f32(1.3333333333333333f)));
        //
        //    // Bit-4 = Choose X Y ordering
        //    mask32v xy;
        //
        //    if constexpr(FS::SIMD_Level == FastSIMD::Level_Scalar)
        //    {
        //        xy = int32_t(index & FS.Broad_i32(1 << 2)) != 0;
        //    }
        //    else
        //    {
        //        xy = index << 29;
        //
        //        if constexpr(FS::SIMD_Level < FastSIMD::Level_SSE41)
        //        {
        //            xy >>= 31;
        //        }
        //    }
        //
        //    float32v a = FS.Select_f32(xy, fY, fX);
        //    float32v b = FS.Select_f32(xy, fX, fY);
        //
        //    // Bit-1 = b flip sign
        //    b ^= FS.Casti32_f32(index << 31);
        //
        //    // Bit-2 = Mul a by 2 or Root3
        //    mask32v aMul2;
        //
        //    if constexpr(FS::SIMD_Level == FastSIMD::Level_Scalar)
        //    {
        //        aMul2 = int32_t(index & FS.Broad_i32(1 << 1)) != 0;
        //    }
        //    else
        //    {
        //        aMul2 = (index << 30) >> 31;
        //    }
        //
        //    a *= FS.Select_f32(aMul2, float32v(2), float32v(ROOT3));
        //    // b zero value if a mul 2
        //    b = FS.NMask_f32(b, aMul2);
        //
        //    // Bit-8 = Flip sign of a + b
        //    return (a + b) ^ FS.Casti32_f32((index >> 3) << 31);
        //}

        //template<typename SIMD = FS, std::enable_if_t<SIMD::SIMD_Level == FastSIMD::Level_AVX2>* = nullptr>
        //FS.INLINE static float32v GetGradientDotFancy(int32v hash, float32v fX, float32v fY)
        //{
        //    int32v index = FS.Convertf32_i32(FS.Converti32_f32(hash & FS.Broad_i32(0x3FFFFF)) * float32v(1.3333333333333333f));
        //
        //    float32v gX = _mm256_permutevar8x32_ps(float32v(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0), index);
        //    float32v gY = _mm256_permutevar8x32_ps(float32v(1, -1, 0, 0, ROOT3, ROOT3, 2, 2), index);
        //
        //    // Bit-8 = Flip sign of a + b
        //    return FS.FMulAdd_f32(gX, fX, fY * gY) ^ FS.Casti32_f32((index >> 3) << 31);
        //}

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

        public float32v GetGradientDot(int32v hash, float32v fX, float32v fY)
        {
            if (hash.Count == Vector256<int>.Count)
            {
                Vector256<float> gX = Avx2.PermuteVar8x32(
                    Vector256.Create(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1),
                    Unsafe.As<int32v, Vector256<int>>(ref hash));

                Vector256<float> gY = Avx2.PermuteVar8x32(
                    Vector256.Create(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2),
                    Unsafe.As<int32v, Vector256<int>>(ref hash));

                return FS.FMulAdd_f32(
                    Unsafe.As<Vector256<float>, float32v>(ref gX),
                    fX,
                    fY.Mul(Unsafe.As<Vector256<float>, float32v>(ref gY)));
            }
            else
            {
                // ( 1+R2, 1 ) ( -1-R2, 1 ) ( 1+R2, -1 ) ( -1-R2, -1 )
                // ( 1, 1+R2 ) ( 1, -1-R2 ) ( -1, 1+R2 ) ( -1, -1-R2 )

                int32v bit1 = hash.LeftShift(31);
                int32v bit2 = hash.RightShift(1).LeftShift(31);
                mask32v mbit4;

                if (hash.Count == 1)
                {
                    mbit4 = hash.And(FS.Broad_i32(1 << 2)).NotEqual(FS.Broad_i32(0));
                }
                else
                {
                    int32v bit4 = hash.LeftShift(29);
                    if (!Sse41.IsSupported)
                    {
                        bit4 = bit4.RightShift(31);
                    }
                    mbit4 = Unsafe.As<int32v, mask32v>(ref bit4);
                }

                fX = fX.Xor(FS.Casti32_f32(bit1));
                fY = fY.Xor(FS.Casti32_f32(bit2));

                float32v a = FS.Select_f32(mbit4, fY, fX);
                float32v b = FS.Select_f32(mbit4, fX, fY);

                return FS.FMulAdd_f32(FS.Broad_f32(1.0f + ROOT2), a, b);
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
            int32v hasha13 = hash.And(FS.Broad_i32(13));

            //if h < 8 then x, else y
            float32v u = FS.Select_f32(hasha13.LessThan(FS.Broad_i32(8)), fX, fY);

            //if h < 4 then y else if h is 12 or 14 then x else z
            float32v v = FS.Select_f32(hasha13.Equal(FS.Broad_i32(12)), fX, fZ);
            v = FS.Select_f32(hasha13.LessThan(FS.Broad_i32(2)), fY, v);

            //if h1 then -u else u
            //if h2 then -v else v
            float32v h1 = FS.Casti32_f32(hash.LeftShift(31));
            float32v h2 = FS.Casti32_f32(hash.And(FS.Broad_i32(2)).LeftShift(30));
            //then add them
            return u.Xor(h1).Add(v.Xor(h2));
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
            int32v p = hash.And(FS.Broad_i32(3 << 3));

            float32v a = FS.Select_f32(p.GreaterThan(FS.Broad_i32(0)), fX, fY);
            float32v b;

            if (Sse41.IsSupported)
            {
                b = FS.Select_f32(p.GreaterThan(FS.Broad_i32(1 << 3)), fY, fZ);
            }
            else
            {
                int32v mask = hash.LeftShift(27);
                b = FS.Select_f32(Unsafe.As<int32v, mask32v>(ref mask), fY, fZ);
            }

            float32v c = FS.Select_f32(p.GreaterThan(FS.Broad_i32(2 << 3)), fZ, fW);

            unchecked
            {
                float32v aSign = FS.Casti32_f32(hash.LeftShift(31));
                float32v bSign = FS.Casti32_f32(hash.LeftShift(30).And(FS.Broad_i32((int)0x80000000)));
                float32v cSign = FS.Casti32_f32(hash.LeftShift(29).And(FS.Broad_i32((int)0x80000000)));
                return a.Xor(aSign).Add(b.Xor(bSign)).Add(c.Xor(cSign));
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
            hash = hash.Xor(x.Xor(y));

            hash = hash.Mul(FS.Broad_i32(0x27d4eb2d));
            return hash.RightShift(15).Xor(hash);
        }

        public int32v HashPrimes(int32v seed, int32v x, int32v y, int32v z)
        {
            int32v hash = seed;
            hash = hash.Xor(x.Xor(y.Xor(z)));

            hash = hash.Mul(FS.Broad_i32(0x27d4eb2d));
            return hash.RightShift(15).Xor(hash);
        }

        public int32v HashPrimes(int32v seed, int32v x, int32v y, int32v z, int32v w)
        {
            int32v hash = seed;
            hash = hash.Xor(x.Xor(y.Xor(z.Xor(w))));

            hash = hash.Mul(FS.Broad_i32(0x27d4eb2d));
            return hash.RightShift(15).Xor(hash);
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

        public float32v Lerp(float32v a, float32v b, float32v t)
        {
            return FS.FMulAdd_f32(t, b.Sub(a), a);
        }

        public float32v InterpHermite(float32v t)
        {
            return t.Mul(t).Mul(FS.FNMulAdd_f32(t, FS.Broad_f32(2), FS.Broad_f32(3)));
        }

        public float32v InterpQuintic(float32v t)
        {
            return t.Mul(t).Mul(t).Mul(FS.FMulAdd_f32(t, FS.FMulAdd_f32(t, FS.Broad_f32(6), FS.Broad_f32(-15)), FS.Broad_f32(10)));
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
