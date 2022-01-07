using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using f32 = Vector256<float>;
    using i32 = Vector256<int>;
    using m32 = Vector256<int>;

    public struct Avx2Functions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => Avx2.IsSupported;

        public static int Count => 8;

        // Broadcast

        public f32 Broad_f32(float value)
        {
            return Vector256.Create(value);
        }

        public i32 Broad_i32(int value)
        {
            return Vector256.Create(value);
        }

        // Load

        public f32 Load_f32(ref byte p)
        {
            return Unsafe.ReadUnaligned<f32>(ref p);
        }

        public i32 Load_i32(ref byte p)
        {
            return Unsafe.ReadUnaligned<i32>(ref p);
        }

        // Incremented

        public f32 Incremented_f32()
        {
            return Vector256.Create(0f, 1, 2, 3, 4, 5, 6, 7);
        }

        public i32 Incremented_i32()
        {
            return Vector256.Create(0, 1, 2, 3, 4, 5, 6, 7);
        }

        // Store

        public void Store_f32(ref byte p, f32 a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_i32(ref byte p, i32 a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_f32(ref float p, f32 a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref p), a);
        }

        public void Store_i32(ref int p, i32 a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref p), a);
        }

        // Extract

        public float Extract0_f32(f32 a)
        {
            return a.ToScalar();
        }

        public int Extract0_i32(i32 a)
        {
            return a.ToScalar();
        }

        public float Extract_f32(f32 a, int idx)
        {
            return a.GetElement(idx);
        }

        public int Extract_i32(i32 a, int idx)
        {
            return a.GetElement(idx);
        }

        // Cast

        public f32 Casti32_f32(i32 a)
        {
            return Unsafe.As<i32, f32>(ref a);
        }

        public i32 Castf32_i32(f32 a)
        {
            return Unsafe.As<f32, i32>(ref a);
        }

        // Convert

        public f32 Converti32_f32(i32 a)
        {
            return Avx.ConvertToVector256Single(a);
        }

        public i32 Convertf32_i32(f32 a)
        {
            return Avx.ConvertToVector256Int32(a);
        }

        // Select

        public f32 Select_f32(m32 m, f32 a, f32 b)
        {
            return Avx.BlendVariable(b, a, m.AsSingle());
        }

        public i32 Select_i32(m32 m, i32 a, i32 b)
        {
            return Avx2.BlendVariable(b, a, m);
        }

        // Min, Max

        public f32 Min_f32(f32 a, f32 b)
        {
            return Avx.Min(a, b);
        }

        public f32 Max_f32(f32 a, f32 b)
        {
            return Avx.Max(a, b);
        }

        public i32 Min_i32(i32 a, i32 b)
        {
            return Avx2.Min(a, b);
        }

        public i32 Max_i32(i32 a, i32 b)
        {
            return Avx2.Max(a, b);
        }

        // Bitwise       

        public f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Avx.AndNot(b, a);
        }

        public i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return Avx2.AndNot(b, a);
        }

        public m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return Avx2.AndNot(b, a);
        }

        public f32 BitwiseShiftRightZX_f32(f32 a, byte b)
        {
            return Casti32_f32(Avx2.ShiftRightLogical(Castf32_i32(a), b));
        }

        public i32 BitwiseShiftRightZX_i32(i32 a, byte b)
        {
            return Avx2.ShiftRightLogical(a, b);
        }

        // Abs

        public f32 Abs_f32(f32 a)
        {
            i32 intMax = Avx2.ShiftRightLogical(Vector256<int>.AllBitsSet, 1);
            return And(a, intMax.AsSingle());
        }

        public i32 Abs_i32(i32 a)
        {
            return Avx2.Abs(a).AsInt32();
        }

        // Float math

        public f32 Sqrt_f32(f32 a)
        {
            return Avx.Sqrt(a);
        }

        public f32 InvSqrt_f32(f32 a)
        {
            return Avx.ReciprocalSqrt(a);
        }

        public f32 Reciprocal_f32(f32 a)
        {
            return Avx.Reciprocal(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        public f32 Floor_f32(f32 a)
        {
            return Avx.RoundToNegativeInfinity(a);
        }

        public f32 Ceil_f32(f32 a)
        {
            return Avx.RoundToPositiveInfinity(a);
        }

        public f32 Round_f32(f32 a)
        {
            return Avx.RoundToNearestInteger(a);
        }

        // Mask

        public i32 Mask_i32(i32 a, m32 m)
        {
            return And(a, m);
        }

        public f32 Mask_f32(f32 a, m32 m)
        {
            return And(a, m.AsSingle());
        }

        public i32 NMask_i32(i32 a, m32 m)
        {
            return Avx2.AndNot(m, a);
        }

        public f32 NMask_f32(f32 a, m32 m)
        {
            return Avx.AndNot(m.AsSingle(), a);
        }

        public bool AnyMask_bool(m32 m)
        {
            return !Avx.TestZ(m, m);
        }

        // FMA

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 FMulAdd_f32(f32 a, f32 b, f32 c)
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
        public f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
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

        public f32 Add(f32 lhs, f32 rhs) => Avx.Add(lhs, rhs);
        public f32 And(f32 lhs, f32 rhs) => Avx.And(lhs, rhs);
        public i32 AsInt32(f32 lhs) => lhs.AsInt32();
        public f32 Complement(f32 lhs) => Avx.Xor(lhs, f32.AllBitsSet);
        public f32 Div(f32 lhs, f32 rhs) => Avx.Divide(lhs, rhs);
        public m32 Equal(f32 lhs, f32 rhs) => Avx.CompareEqual(lhs, rhs).AsInt32();
        public m32 GreaterThan(f32 lhs, f32 rhs) => Avx.CompareGreaterThan(lhs, rhs).AsInt32();
        public m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public f32 LeftShift(f32 lhs, byte rhs) => throw new NotSupportedException();
        public m32 LessThan(f32 lhs, f32 rhs) => Avx.CompareLessThan(lhs, rhs).AsInt32();
        public m32 LessThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public f32 Mul(f32 lhs, f32 rhs) => Avx.Multiply(lhs, rhs);
        public f32 Negate(f32 lhs) => Avx.Xor(lhs, Vector256.Create(0x80000000).AsSingle());
        public m32 NotEqual(f32 lhs, f32 rhs) => Avx.CompareNotEqual(lhs, rhs).AsInt32();
        public f32 Or(f32 lhs, f32 rhs) => Avx.Or(lhs, rhs);
        public f32 RightShift(f32 lhs, byte rhs) => throw new NotSupportedException();
        public f32 Sub(f32 lhs, f32 rhs) => Avx.Subtract(lhs, rhs);
        public f32 Xor(f32 lhs, f32 rhs) => Avx.Xor(lhs, rhs);

        public i32 Add(i32 lhs, i32 rhs) => Avx2.Add(lhs, rhs);
        public i32 And(i32 lhs, i32 rhs) => Avx2.And(lhs, rhs);
        public f32 AsSingle(i32 lhs) => lhs.AsSingle();
        public i32 Complement(i32 lhs) => Avx2.Xor(lhs, i32.AllBitsSet);
        public i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public m32 Equal(i32 lhs, i32 rhs) => Avx2.CompareEqual(lhs, rhs);
        public m32 GreaterThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(lhs, rhs);
        public m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public i32 LeftShift(i32 lhs, byte rhs) => Avx2.ShiftLeftLogical(lhs, rhs);
        public m32 LessThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(rhs, lhs);
        public m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public i32 Mul(i32 lhs, i32 rhs) => Avx2.MultiplyLow(lhs, rhs);
        public i32 Negate(i32 lhs) => Avx2.Subtract(i32.Zero, lhs);
        public m32 NotEqual(i32 lhs, i32 rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public i32 Or(i32 lhs, i32 rhs) => Avx2.Or(lhs, rhs);
        public i32 RightShift(i32 lhs, byte rhs) => Avx2.ShiftRightArithmetic(lhs, rhs);
        public i32 Sub(i32 lhs, i32 rhs) => Avx2.Subtract(lhs, rhs);
        public i32 Xor(i32 lhs, i32 rhs) => Avx2.Xor(lhs, rhs);
    }
}
