using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using float32v = FVectorF128;
    using int32v = FVectorI128;
    using mask32v = FVectorI128;

    public struct Sse2Functions : IFunctionList<mask32v, float32v, int32v>
    {
        // Broadcast

        public float32v Broad_f32(float value)
        {
            return new float32v(Vector128.Create(value));
        }

        public int32v Broad_i32(int value)
        {
            return new int32v(Vector128.Create(value));
        }

        // Load

        public float32v Load_f32(ref byte p)
        {
            return Unsafe.ReadUnaligned<float32v>(ref p);
        }

        public int32v Load_i32(ref byte p)
        {
            return Unsafe.ReadUnaligned<int32v>(ref p);
        }

        // Incremented

        public float32v Incremented_f32()
        {
            return new(Vector128.Create(0f, 1, 2, 3));
        }

        public int32v Incremented_i32()
        {
            return new(Vector128.Create(0, 1, 2, 3));
        }

        // Store

        public void Store_f32(ref byte p, float32v a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_i32(ref byte p, int32v a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        // Extract

        public float Extract0_f32(float32v a)
        {
            return a.Value.ToScalar();
        }

        public int Extract0_i32(int32v a)
        {
            return a.Value.ToScalar();
        }

        public float Extract_f32(float32v a, int idx)
        {
            return a.Value.GetElement(idx);
        }

        public int Extract_i32(int32v a, int idx)
        {
            return a.Value.GetElement(idx);
        }

        // Cast

        public float32v Casti32_f32(int32v a)
        {
            return Unsafe.As<int32v, float32v>(ref a);
        }

        public int32v Castf32_i32(float32v a)
        {
            return Unsafe.As<float32v, int32v>(ref a);
        }

        // Convert

        public float32v Converti32_f32(int32v a)
        {
            return Sse2.ConvertToVector128Single(a);
        }

        public int32v Convertf32_i32(float32v a)
        {
            return Sse2.ConvertToVector128Int32(a);
        }

        // Select

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v Select_f32(mask32v m, float32v a, float32v b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.BlendVariable(b, a, m.AsSingle());
            }
            else
            {
                return b.Xor(m.AsSingle().And(a.Xor(b)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int32v Select_i32(mask32v m, int32v a, int32v b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.BlendVariable(b, a, m);
            }
            else
            {
                return b.Xor(m.And(a.Xor(b)));
            }
        }

        // Min, Max

        public float32v Min_f32(float32v a, float32v b)
        {
            return Sse.Min(a, b);
        }

        public float32v Max_f32(float32v a, float32v b)
        {
            return Sse.Max(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int32v Min_i32(int32v a, int32v b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.Min(a, b);
            }
            else
            {
                return Select_i32(a.LessThan(b), a, b);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int32v Max_i32(int32v a, int32v b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.Max(a, b);
            }
            else
            {
                return Select_i32(a.GreaterThan(b), a, b);
            }
        }

        // Bitwise       

        public float32v BitwiseAndNot_f32(float32v a, float32v b)
        {
            return Sse.AndNot(b, a);
        }

        public int32v BitwiseAndNot_i32(int32v a, int32v b)
        {
            return Sse2.AndNot(b, a);
        }

        public float32v BitwiseShiftRightZX_f32(float32v a, byte b)
        {
            return Casti32_f32(Sse2.ShiftRightLogical(Castf32_i32(a), b));
        }

        public int32v BitwiseShiftRightZX_i32(int32v a, byte b)
        {
            return Sse2.ShiftRightLogical(a, b);
        }

        // Abs

        public float32v Abs_f32(float32v a)
        {
            int32v intMax = Sse2.ShiftRightLogical(Vector128<int>.AllBitsSet, 1);
            return a.And(intMax.AsSingle());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int32v Abs_i32(int32v a)
        {
            if (Ssse3.IsSupported)
            {
                return Ssse3.Abs(a).AsInt32();
            }
            else
            {
                int32v signMask = Sse2.ShiftRightArithmetic(a, 31);
                return a.Xor(signMask).Sub(signMask);
            }
        }

        // Float math

        public float32v Sqrt_f32(float32v a)
        {
            return Sse.Sqrt(a);
        }

        public float32v InvSqrt_f32(float32v a)
        {
            return Sse.ReciprocalSqrt(a);
        }

        public float32v Reciprocal_f32(float32v a)
        {
            return Sse.Reciprocal(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v Floor_f32(float32v a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNegativeInfinity(a);
            }
            else
            {
                Vector128<float> f1 = Vector128.Create(1f);
                Vector128<float> fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                Vector128<float> cmp = Sse.CompareLessThan(a, fval);
                return Sse.Subtract(fval, Sse.And(cmp, f1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v Ceil_f32(float32v a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToPositiveInfinity(a);
            }
            else
            {
                Vector128<float> f1 = Vector128.Create(1f);
                Vector128<float> fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                Vector128<float> cmp = Sse.CompareLessThan(fval, a);
                return Sse.Add(fval, Sse.And(cmp, f1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v Round_f32(float32v a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNearestInteger(a);
            }
            else
            {
                unchecked
                {
                    float32v aSign = a.And(new int32v((int)0x80000000).AsSingle());
                    float32v v = a.Add(aSign.Or(new float32v(0.5f)));
                    return Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(v));
                }
            }
        }

        // Mask

        public int32v Mask_i32(int32v a, mask32v m)
        {
            return a.And(m);
        }

        public float32v Mask_f32(float32v a, mask32v m)
        {
            return a.And(m.AsSingle());
        }

        public int32v NMask_i32(int32v a, mask32v m)
        {
            return Sse2.AndNot(m, a);
        }

        public float32v NMask_f32(float32v a, mask32v m)
        {
            return Sse.AndNot(m.AsSingle(), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AnyMask_bool(mask32v m)
        {
            if (Sse41.IsSupported)
            {
                return !Sse41.TestZ(m, m);
            }
            else
            {
                return Sse.MoveMask(m.AsSingle()) != 0;
            }
        }

        // FMA

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v FMulAdd_f32(float32v a, float32v b, float32v c)
        {
            if (Fma.IsSupported)
            {
                return Fma.MultiplyAdd(a, b, c);
            }
            else
            {
                return a.Mul(b).Add(c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float32v FNMulAdd_f32(float32v a, float32v b, float32v c)
        {
            if (Fma.IsSupported)
            {
                return Fma.MultiplyAddNegated(a, b, c);
            }
            else
            {
                return a.Mul(b).Negate().Add(c);
            }
        }
    }
}
