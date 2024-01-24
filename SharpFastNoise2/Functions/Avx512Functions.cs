using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2;

namespace SharpFastNoise2.Functions
{
    using f32 = Vector512<float>;
    using i32 = Vector512<int>;
    using m32 = Vector512<int>;

    public struct Avx512Functions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => Avx512F.IsSupported && Avx512DQ.IsSupported;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad_f32(float value)
        {
            return Vector512.Create(value);
        }

        public static i32 Broad_i32(int value)
        {
            return Vector512.Create(value);
        }

        // Load

        public static f32 Load_f32(ref readonly float p)
        {
            return Vector512.LoadUnsafe(in p);
        }

        public static i32 Load_i32(ref readonly int p)
        {
            return Vector512.LoadUnsafe(in p);
        }

        // Incremented

        public static f32 Incremented_f32()
        {
            return Vector512.Create(0f, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
        }

        public static i32 Incremented_i32()
        {
            return Vector512.Create(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
        }

        // Store

        public static void Store_f32(ref float p, f32 a)
        {
            a.StoreUnsafe(ref p);
        }

        public static void Store_i32(ref int p, i32 a)
        {
            a.StoreUnsafe(ref p);
        }

        // Extract

        public static float Extract0_f32(f32 a)
        {
            return a.ToScalar();
        }

        public static int Extract0_i32(i32 a)
        {
            return a.ToScalar();
        }

        public static float Extract_f32(f32 a, int idx)
        {
            return a.GetElement(idx);
        }

        public static int Extract_i32(i32 a, int idx)
        {
            return a.GetElement(idx);
        }

        // Cast

        public static f32 Casti32_f32(i32 a)
        {
            return Unsafe.BitCast<i32, f32>(a);
        }

        public static i32 Castf32_i32(f32 a)
        {
            return Unsafe.BitCast<f32, i32>(a);
        }

        // Convert

        public static f32 Converti32_f32(i32 a)
        {
            return Avx512F.ConvertToVector512Single(a);
        }

        public static i32 Convertf32_i32(f32 a)
        {
            return Avx512F.ConvertToVector512Int32(a);
        }

        // Select

        public static f32 Select_f32(m32 m, f32 a, f32 b)
        {
            return Avx512F.BlendVariable(b, a, m.AsSingle());
        }

        public static i32 Select_i32(m32 m, i32 a, i32 b)
        {
            return Avx512F.BlendVariable(b, a, m);
        }

        // Min, Max

        public static f32 Min_f32(f32 a, f32 b)
        {
            return Avx512F.Min(a, b);
        }

        public static f32 Max_f32(f32 a, f32 b)
        {
            return Avx512F.Max(a, b);
        }

        public static i32 Min_i32(i32 a, i32 b)
        {
            return Avx512F.Min(a, b);
        }

        public static i32 Max_i32(i32 a, i32 b)
        {
            return Avx512F.Max(a, b);
        }

        // Bitwise       

        public static f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Avx512DQ.AndNot(b, a);
        }

        public static i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return Avx512F.AndNot(b, a);
        }

        public static m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return Avx512F.AndNot(b, a);
        }

        public static f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b)
        {
            return Casti32_f32(Avx512F.ShiftRightLogical(Castf32_i32(a), b));
        }

        public static i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b)
        {
            return Avx512F.ShiftRightLogical(a, b);
        }

        // Abs

        public static f32 Abs_f32(f32 a)
        {
            return Vector512.Abs(a);
        }

        public static i32 Abs_i32(i32 a)
        {
            return Vector512.Abs(a);
        }

        // Float math

        public static f32 Sqrt_f32(f32 a)
        {
            return Avx512F.Sqrt(a);
        }

        public static f32 InvSqrt_f32(f32 a)
        {
            return Avx512F.ReciprocalSqrt14(a);
        }

        public static f32 Reciprocal_f32(f32 a)
        {
            return Avx512F.Reciprocal14(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        private const byte _MM_FROUND_TO_NEAREST_INT = 0x00;
        private const byte _MM_FROUND_TO_NEG_INT = 0x01;
        private const byte _MM_FROUND_TO_POS_INT = 0x02;
        private const byte _MM_FROUND_NO_EXC = 0x08;

        public static f32 Floor_f32(f32 a)
        {
            return Avx512F.RoundScale(a, _MM_FROUND_TO_NEG_INT | _MM_FROUND_NO_EXC);
        }

        public static f32 Ceil_f32(f32 a)
        {
            return Avx512F.RoundScale(a, _MM_FROUND_TO_POS_INT | _MM_FROUND_NO_EXC);
        }

        public static f32 Round_f32(f32 a)
        {
            return Avx512F.RoundScale(a, _MM_FROUND_TO_NEAREST_INT | _MM_FROUND_NO_EXC);
        }

        // Mask

        public static i32 Mask_i32(i32 a, m32 m)
        {
            // return _mm512_maskz_mov_epi32(m, a);
            return Avx512F.And(a, m);
        }

        public static f32 Mask_f32(f32 a, m32 m)
        {
            // return _mm512_maskz_mov_ps(m, a);
            return Avx512DQ.And(a, m.AsSingle());
        }

        public static i32 NMask_i32(i32 a, m32 m)
        {
            // return _mm512_maskz_mov_epi32( ~m, a );
            return Avx512F.AndNot(m, a);
        }

        public static f32 NMask_f32(f32 a, m32 m)
        {
            // return _mm512_maskz_mov_ps( ~m, a );
            return Avx512DQ.AndNot(m.AsSingle(), a);
        }

        public static bool AnyMask_bool(m32 m)
        {
            return m.ExtractMostSignificantBits() != 0;
        }

        // FMA

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 FMulAdd_f32(f32 a, f32 b, f32 c)
        {
            return Avx512F.FusedMultiplyAdd(a, b, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
        {
            return Avx512F.FusedMultiplyAddNegated(a, b, c);
        }

        // Generic math

        public static f32 Add(f32 lhs, f32 rhs) => Avx512F.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Avx512DQ.And(lhs, rhs);
        public static i32 AsInt32(f32 lhs) => lhs.AsInt32();
        public static f32 Complement(f32 lhs) => Vector512.OnesComplement(lhs);
        public static f32 Div(f32 lhs, f32 rhs) => Avx512F.Divide(lhs, rhs);
        public static m32 Equal(f32 lhs, f32 rhs) => Avx512F.CompareEqual(lhs, rhs).AsInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => Avx512F.CompareGreaterThan(lhs, rhs).AsInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Avx512F.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static m32 LessThan(f32 lhs, f32 rhs) => Avx512F.CompareLessThan(lhs, rhs).AsInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => Avx512F.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => Avx512F.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => Vector512.Negate(lhs);

        public static m32 NotEqual(f32 lhs, f32 rhs) => Avx512F.CompareNotEqual(lhs, rhs).AsInt32();
        public static f32 Or(f32 lhs, f32 rhs) => Avx512DQ.Or(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Avx512F.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Avx512DQ.Xor(lhs, rhs);

        public static i32 Add(i32 lhs, i32 rhs) => Avx512F.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Avx512F.And(lhs, rhs);
        public static f32 AsSingle(i32 lhs) => lhs.AsSingle();
        public static i32 Complement(i32 lhs) => Vector512.OnesComplement(lhs);
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static m32 Equal(i32 lhs, i32 rhs) => Avx512F.CompareEqual(lhs, rhs);
        public static m32 GreaterThan(i32 lhs, i32 rhs) => Avx512F.CompareGreaterThan(lhs, rhs);
        public static m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => Avx512F.ShiftLeftLogical(lhs, rhs);
        public static m32 LessThan(i32 lhs, i32 rhs) => Avx512F.CompareGreaterThan(rhs, lhs);
        public static m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => Avx512F.MultiplyLow(lhs, rhs);
        public static i32 Negate(i32 lhs) => Avx512F.Subtract(i32.Zero, lhs);
        public static m32 NotEqual(i32 lhs, i32 rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public static i32 Or(i32 lhs, i32 rhs) => Avx512F.Or(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => Avx512F.ShiftRightArithmetic(lhs, rhs);
        public static i32 Sub(i32 lhs, i32 rhs) => Avx512F.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Avx512F.Xor(lhs, rhs);
    }
}
