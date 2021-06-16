using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using float32v = Vector128<float>;
    using int32v = Vector128<int>;
    using mask32v = Vector128<int>;

    public struct Sse2Functions : IFunctionList<mask32v, float32v, int32v>
    {
        public int Count => 4;

        // Broadcast

        public float32v Broad_f32(float value)
        {
            return Vector128.Create(value);
        }

        public int32v Broad_i32(int value)
        {
            return Vector128.Create(value);
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
            return Vector128.Create(0f, 1, 2, 3);
        }

        public int32v Incremented_i32()
        {
            return Vector128.Create(0, 1, 2, 3);
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
            return a.ToScalar();
        }

        public int Extract0_i32(int32v a)
        {
            return a.ToScalar();
        }

        public float Extract_f32(float32v a, int idx)
        {
            return a.GetElement(idx);
        }

        public int Extract_i32(int32v a, int idx)
        {
            return a.GetElement(idx);
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
                return Xor(b, And(m.AsSingle(), Xor(a, b)));
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
                return Xor(b, And(m, Xor(a, b)));
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
                return Select_i32(LessThan(a, b), a, b);
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
                return Select_i32(GreaterThan(a, b), a, b);
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
            int32v intMax = Sse2.ShiftRightLogical(mask32v.AllBitsSet, 1);
            return And(a, intMax.AsSingle());
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
                return Sub(Xor(a, signMask), signMask);
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
                float32v f1 = Vector128.Create(1f);
                float32v fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                float32v cmp = Sse.CompareLessThan(a, fval);
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
                float32v f1 = Vector128.Create(1f);
                float32v fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                float32v cmp = Sse.CompareLessThan(fval, a);
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
                    float32v aSign = And(a, Broad_i32((int)0x80000000).AsSingle());
                    float32v v = Add(a, Or(aSign, Broad_f32(0.5f)));
                    return Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(v));
                }
            }
        }

        // Mask

        public int32v Mask_i32(int32v a, mask32v m)
        {
            return And(a, m);
        }

        public float32v Mask_f32(float32v a, mask32v m)
        {
            return And(a, m.AsSingle());
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
                return Add(Mul(a, b), c);
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
                return Add(Negate(Mul(a, b)), c);
            }
        }

        // Generic math

        public float32v Add(float32v lhs, float32v rhs) => Sse.Add(lhs, rhs);
        public float32v And(float32v lhs, float32v rhs) => Sse.And(lhs, rhs);
        public int32v AsInt32(float32v lhs) => lhs.AsInt32();
        public float32v Complement(float32v lhs) => Sse.Xor(lhs, float32v.AllBitsSet);
        public float32v Div(float32v lhs, float32v rhs) => Sse.Divide(lhs, rhs);
        public mask32v Equal(float32v lhs, float32v rhs) => Sse.CompareEqual(lhs, rhs).AsInt32();
        public mask32v GreaterThan(float32v lhs, float32v rhs) => Sse.CompareGreaterThan(lhs, rhs).AsInt32();
        public mask32v GreaterThanOrEqual(float32v lhs, float32v rhs) => Sse.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public float32v LeftShift(float32v lhs, byte rhs) => throw new NotSupportedException();
        public mask32v LessThan(float32v lhs, float32v rhs) => Sse.CompareLessThan(lhs, rhs).AsInt32();
        public mask32v LessThanOrEqual(float32v lhs, float32v rhs) => Sse.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public float32v Mul(float32v lhs, float32v rhs) => Sse.Multiply(lhs, rhs);
        public float32v Negate(float32v lhs) => Sse.Xor(lhs, Vector128.Create(0x80000000).AsSingle());
        public mask32v NotEqual(float32v lhs, float32v rhs) => Sse.CompareNotEqual(lhs, rhs).AsInt32();
        public float32v Or(float32v lhs, float32v rhs) => Sse.Or(lhs, rhs);
        public float32v RightShift(float32v lhs, byte rhs) => throw new NotSupportedException();
        public float32v Sub(float32v lhs, float32v rhs) => Sse.Subtract(lhs, rhs);
        public float32v Xor(float32v lhs, float32v rhs) => Sse.Xor(lhs, rhs);

        public int32v Add(int32v lhs, int32v rhs) => Sse2.Add(lhs, rhs);
        public int32v And(int32v lhs, int32v rhs) => Sse2.And(lhs, rhs);
        public float32v AsSingle(int32v lhs) => lhs.AsSingle();
        public int32v Complement(int32v lhs) => Sse2.Xor(lhs, int32v.AllBitsSet);
        public int32v Div(int32v lhs, int32v rhs) => throw new NotSupportedException();
        public mask32v Equal(int32v lhs, int32v rhs) => Sse2.CompareEqual(lhs, rhs);
        public mask32v GreaterThan(int32v lhs, int32v rhs) => Sse2.CompareGreaterThan(lhs, rhs);
        public mask32v GreaterThanOrEqual(int32v lhs, int32v rhs) => throw new NotSupportedException();
        public int32v LeftShift(int32v lhs, byte rhs) => Sse2.ShiftLeftLogical(lhs, rhs);
        public mask32v LessThan(int32v lhs, int32v rhs) => Sse2.CompareLessThan(lhs, rhs);
        public mask32v LessThanOrEqual(int32v lhs, int32v rhs) => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int32v Mul(int32v lhs, int32v rhs)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.MultiplyLow(lhs, rhs);
            }
            else
            {
                var tmp1 = Sse2.Multiply(lhs.AsUInt32(), rhs.AsUInt32()); // mul 2,0
                var tmp2 = Sse2.Multiply(
                    Sse2.ShiftRightLogical128BitLane(lhs, 4).AsUInt32(),
                    Sse2.ShiftRightLogical128BitLane(rhs, 4).AsUInt32()); // mul 3,1

                const byte control = 8; // _MM_SHUFFLE(0,0,2,0)
                return Sse2.UnpackLow(
                    Sse2.Shuffle(tmp1.AsInt32(), control),
                    Sse2.Shuffle(tmp2.AsInt32(), control)); // shuffle results to [63..0] and pack
            }
        }

        public int32v Negate(int32v lhs) => Sse2.Subtract(int32v.Zero, lhs);
        public mask32v NotEqual(int32v lhs, int32v rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public int32v Or(int32v lhs, int32v rhs) => Sse2.Or(lhs, rhs);
        public int32v RightShift(int32v lhs, byte rhs) => Sse2.ShiftRightArithmetic(lhs, rhs);
        public int32v Sub(int32v lhs, int32v rhs) => Sse2.Subtract(lhs, rhs);
        public int32v Xor(int32v lhs, int32v rhs) => Sse2.Xor(lhs, rhs);
    }
}
