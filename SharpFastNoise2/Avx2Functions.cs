using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using float32v = Vector256<float>;
    using int32v = Vector256<int>;
    using mask32v = Vector256<int>;

    public struct Avx2Functions : IFunctionList<mask32v, float32v, int32v>
    {
        public static bool IsSupported => Avx2.IsSupported;

        public int Count => 8;

        // Broadcast

        public float32v Broad_f32(float value)
        {
            return Vector256.Create(value);
        }

        public int32v Broad_i32(int value)
        {
            return Vector256.Create(value);
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
            return Vector256.Create(0f, 1, 2, 3, 4, 5, 6, 7);
        }

        public int32v Incremented_i32()
        {
            return Vector256.Create(0, 1, 2, 3, 4, 5, 6, 7);
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

        public void Store_f32(ref float p, float32v a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref p), a);
        }

        public void Store_i32(ref int p, int32v a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref p), a);
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
            return Avx.ConvertToVector256Single(a);
        }

        public int32v Convertf32_i32(float32v a)
        {
            return Avx.ConvertToVector256Int32(a);
        }

        // Select

        public float32v Select_f32(mask32v m, float32v a, float32v b)
        {
            return Avx.BlendVariable(b, a, m.AsSingle());
        }

        public int32v Select_i32(mask32v m, int32v a, int32v b)
        {
            return Avx2.BlendVariable(b, a, m);
        }

        // Min, Max

        public float32v Min_f32(float32v a, float32v b)
        {
            return Avx.Min(a, b);
        }

        public float32v Max_f32(float32v a, float32v b)
        {
            return Avx.Max(a, b);
        }

        public int32v Min_i32(int32v a, int32v b)
        {
            return Avx2.Min(a, b);
        }

        public int32v Max_i32(int32v a, int32v b)
        {
            return Avx2.Max(a, b);
        }

        // Bitwise       

        public float32v BitwiseAndNot_f32(float32v a, float32v b)
        {
            return Avx.AndNot(b, a);
        }

        public int32v BitwiseAndNot_i32(int32v a, int32v b)
        {
            return Avx2.AndNot(b, a);
        }

        public mask32v BitwiseAndNot_m32(mask32v a, mask32v b)
        {
            return Avx2.AndNot(b, a);
        }

        public float32v BitwiseShiftRightZX_f32(float32v a, byte b)
        {
            return Casti32_f32(Avx2.ShiftRightLogical(Castf32_i32(a), b));
        }

        public int32v BitwiseShiftRightZX_i32(int32v a, byte b)
        {
            return Avx2.ShiftRightLogical(a, b);
        }

        // Abs

        public float32v Abs_f32(float32v a)
        {
            int32v intMax = Avx2.ShiftRightLogical(Vector256<int>.AllBitsSet, 1);
            return And(a, intMax.AsSingle());
        }

        public int32v Abs_i32(int32v a)
        {
            return Avx2.Abs(a).AsInt32();
        }

        // Float math

        public float32v Sqrt_f32(float32v a)
        {
            return Avx.Sqrt(a);
        }

        public float32v InvSqrt_f32(float32v a)
        {
            return Avx.ReciprocalSqrt(a);
        }

        public float32v Reciprocal_f32(float32v a)
        {
            return Avx.Reciprocal(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        public float32v Floor_f32(float32v a)
        {
            return Avx.RoundToNegativeInfinity(a);
        }

        public float32v Ceil_f32(float32v a)
        {
            return Avx.RoundToPositiveInfinity(a);
        }

        public float32v Round_f32(float32v a)
        {
            return Avx.RoundToNearestInteger(a);
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
            return Avx2.AndNot(m, a);
        }

        public float32v NMask_f32(float32v a, mask32v m)
        {
            return Avx.AndNot(m.AsSingle(), a);
        }

        public bool AnyMask_bool(mask32v m)
        {
            return !Avx.TestZ(m, m);
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

        public float32v Add(float32v lhs, float32v rhs) => Avx.Add(lhs, rhs);
        public float32v And(float32v lhs, float32v rhs) => Avx.And(lhs, rhs);
        public int32v AsInt32(float32v lhs) => lhs.AsInt32();
        public float32v Complement(float32v lhs) => Avx.Xor(lhs, float32v.AllBitsSet);
        public float32v Div(float32v lhs, float32v rhs) => Avx.Divide(lhs, rhs);
        public mask32v Equal(float32v lhs, float32v rhs) => Avx.CompareEqual(lhs, rhs).AsInt32();
        public mask32v GreaterThan(float32v lhs, float32v rhs) => Avx.CompareGreaterThan(lhs, rhs).AsInt32();
        public mask32v GreaterThanOrEqual(float32v lhs, float32v rhs) => Avx.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public float32v LeftShift(float32v lhs, byte rhs) => throw new NotSupportedException();
        public mask32v LessThan(float32v lhs, float32v rhs) => Avx.CompareLessThan(lhs, rhs).AsInt32();
        public mask32v LessThanOrEqual(float32v lhs, float32v rhs) => Avx.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public float32v Mul(float32v lhs, float32v rhs) => Avx.Multiply(lhs, rhs);
        public float32v Negate(float32v lhs) => Avx.Xor(lhs, Vector256.Create(0x80000000).AsSingle());
        public mask32v NotEqual(float32v lhs, float32v rhs) => Avx.CompareNotEqual(lhs, rhs).AsInt32();
        public float32v Or(float32v lhs, float32v rhs) => Avx.Or(lhs, rhs);
        public float32v RightShift(float32v lhs, byte rhs) => throw new NotSupportedException();
        public float32v Sub(float32v lhs, float32v rhs) => Avx.Subtract(lhs, rhs);
        public float32v Xor(float32v lhs, float32v rhs) => Avx.Xor(lhs, rhs);

        public int32v Add(int32v lhs, int32v rhs) => Avx2.Add(lhs, rhs);
        public int32v And(int32v lhs, int32v rhs) => Avx2.And(lhs, rhs);
        public float32v AsSingle(int32v lhs) => lhs.AsSingle();
        public int32v Complement(int32v lhs) => Avx2.Xor(lhs, int32v.AllBitsSet);
        public int32v Div(int32v lhs, int32v rhs) => throw new NotSupportedException();
        public mask32v Equal(int32v lhs, int32v rhs) => Avx2.CompareEqual(lhs, rhs);
        public mask32v GreaterThan(int32v lhs, int32v rhs) => Avx2.CompareGreaterThan(lhs, rhs);
        public mask32v GreaterThanOrEqual(int32v lhs, int32v rhs) => throw new NotSupportedException();
        public int32v LeftShift(int32v lhs, byte rhs) => Avx2.ShiftLeftLogical(lhs, rhs);
        public mask32v LessThan(int32v lhs, int32v rhs) => Avx2.CompareGreaterThan(rhs, lhs);
        public mask32v LessThanOrEqual(int32v lhs, int32v rhs) => throw new NotSupportedException();
        public int32v Mul(int32v lhs, int32v rhs) => Avx2.MultiplyLow(lhs, rhs);
        public int32v Negate(int32v lhs) => Avx2.Subtract(int32v.Zero, lhs);
        public mask32v NotEqual(int32v lhs, int32v rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public int32v Or(int32v lhs, int32v rhs) => Avx2.Or(lhs, rhs);
        public int32v RightShift(int32v lhs, byte rhs) => Avx2.ShiftRightArithmetic(lhs, rhs);
        public int32v Sub(int32v lhs, int32v rhs) => Avx2.Subtract(lhs, rhs);
        public int32v Xor(int32v lhs, int32v rhs) => Avx2.Xor(lhs, rhs);
    }
}
