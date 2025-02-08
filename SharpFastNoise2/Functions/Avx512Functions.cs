using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2.Functions
{
    using f32 = Vector512<float>;
    using i32 = Vector512<int>;
    using static Gradient;

    public struct Avx512Functions : IFunctionList<f32, i32, Avx512Functions>
    {
        public static bool IsSupported => Avx512F.IsSupported && Avx512DQ.IsSupported;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad(float value) => Avx512F.BroadcastScalarToVector512(Vector128.CreateScalarUnsafe(value));
        public static i32 Broad(int value) => Avx512F.BroadcastScalarToVector512(Vector128.CreateScalarUnsafe(value));

        // Load

        public static f32 Load(ref readonly float p) => Vector512.LoadUnsafe(in p);
        public static i32 Load(ref readonly int p) => Vector512.LoadUnsafe(in p);

        public static f32 Load(ref readonly float p, nuint elementOffset) => Vector512.LoadUnsafe(in p, elementOffset);
        public static i32 Load(ref readonly int p, nuint elementOffset) => Vector512.LoadUnsafe(in p, elementOffset);

        public static f32 Load(ReadOnlySpan<float> p) => Vector512.Create(p);
        public static i32 Load(ReadOnlySpan<int> p) => Vector512.Create(p);

        // Incremented

        public static f32 Incremented_f32()
        {
#if NET9_0_OR_GREATER
            return f32.Indices;
#else
            return Vector512.Create(0f, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
#endif
        }

        public static i32 Incremented_i32()
        {
#if NET9_0_OR_GREATER
            return i32.Indices;
#else
            return Vector512.Create(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
#endif
        }

        // Store

        public static void Store(ref float p, f32 a) => a.StoreUnsafe(ref p);
        public static void Store(ref int p, i32 a) => a.StoreUnsafe(ref p);

        public static void Store(ref float p, nuint elementOffset, f32 a) => a.StoreUnsafe(ref p, elementOffset);
        public static void Store(ref int p, nuint elementOffset, i32 a) => a.StoreUnsafe(ref p, elementOffset);

        public static void Store(Span<float> p, f32 a) => a.CopyTo(p);
        public static void Store(Span<int> p, i32 a) => a.CopyTo(p);

        // Extract

        public static float Extract0(f32 a) => a.ToScalar();
        public static int Extract0(i32 a) => a.ToScalar();

        public static float Extract(f32 a, int idx) => a.GetElement(idx);
        public static int Extract(i32 a, int idx) => a.GetElement(idx);

        // Cast

        public static f32 Cast_f32(i32 a) => a.AsSingle();
        public static i32 Cast_i32(f32 a) => a.AsInt32();

        // Convert

        public static f32 Convert_f32(i32 a) => Avx512F.ConvertToVector512Single(a);
        public static i32 Convert_i32(f32 a) => Avx512F.ConvertToVector512Int32(a);

        // Select

        public static f32 Select(f32 m, f32 a, f32 b) => Avx512F.BlendVariable(b, a, m.AsSingle());
        public static i32 Select(i32 m, i32 a, i32 b) => Avx512F.BlendVariable(b, a, m.AsInt32());

        // Min

        public static f32 Min(f32 a, f32 b) => Avx512F.Min(a, b);
        public static i32 Min(i32 a, i32 b) => Avx512F.Min(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinAcross(f32 a) => Avx2Functions.MinAcross(Vector256.Min(a.GetLower(), a.GetUpper()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinAcross(i32 a) => Avx2Functions.MinAcross(Vector256.Min(a.GetLower(), a.GetUpper()));

        // Max

        public static f32 Max(f32 a, f32 b) => Avx512F.Max(a, b);
        public static i32 Max(i32 a, i32 b) => Avx512F.Max(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxAcross(f32 a) => Avx2Functions.MaxAcross(Vector256.Max(a.GetLower(), a.GetUpper()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxAcross(i32 a) => Avx2Functions.MaxAcross(Vector256.Max(a.GetLower(), a.GetUpper()));

        // Bitwise

        public static f32 AndNot(f32 a, f32 b) => Avx512DQ.AndNot(b, a);
        public static i32 AndNot(i32 a, i32 b) => Avx512F.AndNot(b, a);

        public static f32 ShiftRightLogical(f32 a, [ConstantExpected] byte b) => a >>> b;
        public static i32 ShiftRightLogical(i32 a, [ConstantExpected] byte b) => a >>> b;

        // Abs

        public static f32 Abs(f32 a) => Vector512.Abs(a);
        public static i32 Abs(i32 a) => Vector512.Abs(a);

        // Float math

        public static f32 Sqrt(f32 a) => Avx512F.Sqrt(a);
        public static f32 ReciprocalSqrt(f32 a) => Avx512F.ReciprocalSqrt14(a);

        public static f32 Reciprocal(f32 a) => Avx512F.Reciprocal14(a);

        // Rounding: http://dss.stephanierct.com/DevBlog/?p=8

        private const byte _MM_FROUND_TO_NEAREST_INT = 0x00;
        private const byte _MM_FROUND_TO_NEG_INT = 0x01;
        private const byte _MM_FROUND_TO_POS_INT = 0x02;
        private const byte _MM_FROUND_NO_EXC = 0x08;

        public static f32 Floor(f32 a) => Avx512F.RoundScale(a, _MM_FROUND_TO_NEG_INT | _MM_FROUND_NO_EXC);
        public static f32 Ceiling(f32 a) => Avx512F.RoundScale(a, _MM_FROUND_TO_POS_INT | _MM_FROUND_NO_EXC);
        public static f32 Round(f32 a) => Avx512F.RoundScale(a, _MM_FROUND_TO_NEAREST_INT | _MM_FROUND_NO_EXC);

        // Mask

        public static i32 Mask(i32 a, i32 m) =>
            // return _mm512_maskz_mov_epi32(m, a);
            Avx512F.And(m, a);

        public static f32 Mask(f32 a, f32 m) =>
            // return _mm512_maskz_mov_ps(m, a);
            Avx512DQ.And(m, a);

        public static i32 NMask(i32 a, i32 m) =>
            // return _mm512_maskz_mov_epi32( ~m, a );
            Avx512F.AndNot(m, a);

        public static f32 NMask(f32 a, f32 m) =>
            // return _mm512_maskz_mov_ps( ~m, a );
            Avx512DQ.AndNot(m, a);

        public static bool AnyMask(i32 m) => m.ExtractMostSignificantBits() != 0;
        public static bool AllMask(i32 m) => m.ExtractMostSignificantBits() == 0xFFFF;

        // Bit Ops

        public static int Log2(i32 a) => BitOperations.Log2(a.ExtractMostSignificantBits());
        public static int PopCount(i32 a) => BitOperations.PopCount(a.ExtractMostSignificantBits());

        public static int LeadingZeroCount(i32 a) => BitOperations.LeadingZeroCount(a.ExtractMostSignificantBits());
        public static int TrailingZeroCount(i32 a) => BitOperations.TrailingZeroCount(a.ExtractMostSignificantBits());

        // Masked float

        public static f32 MaskAdd(f32 a, f32 b, f32 m) => Avx512F.BlendVariable(a, a + b, m);
        public static f32 MaskSub(f32 a, f32 b, f32 m) => Avx512F.BlendVariable(a, a - b, m);
        public static f32 MaskMul(f32 a, f32 b, f32 m) => Avx512F.BlendVariable(a, a * b, m);

        public static i32 MaskIncrement(i32 a, i32 m) => Avx512F.BlendVariable(a, a - i32.One, m);
        public static i32 MaskDecrement(i32 a, i32 m) => Avx512F.BlendVariable(a, a + i32.One, m);

        // NMasked float

        public static f32 NMaskAdd(f32 a, f32 b, f32 m) => Avx512F.BlendVariable(a, a + b, ~m);
        public static f32 NMaskSub(f32 a, f32 b, f32 m) => Avx512F.BlendVariable(a, a - b, ~m);
        public static f32 NMaskMul(f32 a, f32 b, f32 m) => Avx512F.BlendVariable(a, a * b, ~m);

        // Masked int32

        public static i32 MaskAdd(i32 a, i32 b, i32 m) => Avx512F.BlendVariable(a, a + b, m);
        public static i32 MaskSub(i32 a, i32 b, i32 m) => Avx512F.BlendVariable(a, a - b, m);
        public static i32 MaskMul(i32 a, i32 b, i32 m) => Avx512F.BlendVariable(a, a * b, m);

        // NMasked int32

        public static i32 NMaskAdd(i32 a, i32 b, i32 m) => Avx512F.BlendVariable(a, a + b, ~m);
        public static i32 NMaskSub(i32 a, i32 b, i32 m) => Avx512F.BlendVariable(a, a - b, ~m);
        public static i32 NMaskMul(i32 a, i32 b, i32 m) => Avx512F.BlendVariable(a, a * b, ~m);

        // FMA

        public static f32 FMulAdd(f32 a, f32 b, f32 c) => Avx512F.FusedMultiplyAdd(a, b, c);
        public static f32 FNMulAdd(f32 a, f32 b, f32 c) => Avx512F.FusedMultiplyAddNegated(a, b, c);

        // Float math

        public static f32 Add(f32 lhs, f32 rhs) => Avx512F.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Avx512DQ.And(lhs, rhs);
        public static f32 Not(f32 lhs) => ~lhs;
        public static f32 Div(f32 lhs, f32 rhs) => Avx512F.Divide(lhs, rhs);
        public static f32 Equal(f32 lhs, f32 rhs) => Avx512F.CompareEqual(lhs, rhs);
        public static f32 GreaterThan(f32 lhs, f32 rhs) => Avx512F.CompareGreaterThan(lhs, rhs);
        public static f32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Avx512F.CompareGreaterThanOrEqual(lhs, rhs);
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 LessThan(f32 lhs, f32 rhs) => Avx512F.CompareLessThan(lhs, rhs);
        public static f32 LessThanOrEqual(f32 lhs, f32 rhs) => Avx512F.CompareLessThanOrEqual(lhs, rhs);
        public static f32 Mul(f32 lhs, f32 rhs) => Avx512F.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => -lhs;
        public static f32 NotEqual(f32 lhs, f32 rhs) => Avx512F.CompareNotEqual(lhs, rhs);
        public static f32 Or(f32 lhs, f32 rhs) => Avx512DQ.Or(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Avx512F.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Avx512DQ.Xor(lhs, rhs);

        // Int math

        public static i32 Add(i32 lhs, i32 rhs) => Avx512F.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Avx512F.And(lhs, rhs);
        public static i32 Not(i32 lhs) => ~lhs;
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Equal(i32 lhs, i32 rhs) => Avx512F.CompareEqual(lhs, rhs);
        public static i32 GreaterThan(i32 lhs, i32 rhs) => Avx512F.CompareGreaterThan(lhs, rhs);
        public static i32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => Avx512F.ShiftLeftLogical(lhs, rhs);
        public static i32 LessThan(i32 lhs, i32 rhs) => Avx512F.CompareGreaterThan(rhs, lhs);
        public static i32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => Avx512F.MultiplyLow(lhs, rhs);
        public static i32 Negate(i32 lhs) => -lhs;
        public static i32 NotEqual(i32 lhs, i32 rhs) => Avx512F.CompareNotEqual(lhs, rhs);
        public static i32 Or(i32 lhs, i32 rhs) => Avx512F.Or(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => Avx512F.ShiftRightArithmetic(lhs, rhs);
        public static i32 Sub(i32 lhs, i32 rhs) => Avx512F.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Avx512F.Xor(lhs, rhs);

        // Gradient dot fancy

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDotFancy(i32 hash, f32 fX, f32 fY)
        {
            i32 index = Avx512F.ConvertToVector512Int32(
                Avx512F.ConvertToVector512Single(hash & Broad(0x3FFFFF)) * Broad(1.3333333333333333f));

            f32 mX = Vector512.Create(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0, -ROOT3, -ROOT3, -2, -2, -1, 1, 0, 0);
            f32 gX = Avx512F.PermuteVar16x32(mX, index);

            f32 mY = Vector512.Create(1, -1, 0, 0, ROOT3, ROOT3, 2, 2, -1, 1, 0, 0, -ROOT3, -ROOT3, -2, -2);
            f32 gY = Avx512F.PermuteVar16x32(mY, index);

            return Avx512F.FusedMultiplyAdd(gX, fX, fY * gY);
        }

        // Gradient dot

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY)
        {
            var mX = Vector256.Create(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1);
            f32 gX = Avx512F.PermuteVar16x32(Vector512.Create(mX, mX), hash);

            var mY = Vector256.Create(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2);
            f32 gY = Avx512F.PermuteVar16x32(Vector512.Create(mY, mY), hash);

            return Avx512F.FusedMultiplyAdd(gX, fX, fY * gY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY, f32 fZ)
        {
            f32 gX = Avx512F.PermuteVar16x32(Vector512.Create(1f, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0, 1, 0, -1, 0), hash);
            f32 gY = Avx512F.PermuteVar16x32(Vector512.Create(1f, 1, -1, -1, 0, 0, 0, 0, 1, -1, 1, -1, 1, -1, 1, -1), hash);
            f32 gZ = Avx512F.PermuteVar16x32(Vector512.Create(0f, 0, 0, 0, 1, 1, -1, -1, 1, 1, -1, -1, 0, 1, 0, -1), hash);

            return Avx512F.FusedMultiplyAdd(gX, fX, Avx512F.FusedMultiplyAdd(fY, gY, fZ * gZ));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY, f32 fZ, f32 fW)
        {
            f32 gX = Avx512F.PermuteVar16x32x2(
                Vector512.Create(0f, 0, 0, 0, 0, 0, 0, 0, 1, -1, 1, -1, 1, -1, 1, -1),
                hash,
                Vector512.Create(1f, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1));

            f32 gY = Avx512F.PermuteVar16x32x2(
                Vector512.Create(1f, -1, 1, -1, 1, -1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0),
                hash,
                Vector512.Create(1f, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1));

            f32 gZ = Avx512F.PermuteVar16x32x2(
                Vector512.Create(1f, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, 1, -1, -1),
                hash,
                Vector512.Create(0f, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, -1, -1, -1, -1));

            f32 gW = Avx512F.PermuteVar16x32x2(
                Vector512.Create(1f, 1, 1, 1, -1, -1, -1, -1, 1, 1, 1, 1, -1, -1, -1, -1),
                hash,
                Vector512.Create(1f, 1, 1, 1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0));

            return Avx512F.FusedMultiplyAdd(
                gX, fX, Avx512F.FusedMultiplyAdd(fY, gY, Avx512F.FusedMultiplyAdd(fZ, gZ, fW * gW)));
        }
    }
}
