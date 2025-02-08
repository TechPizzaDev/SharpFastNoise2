using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2.Functions
{
    using static Gradient;
    using f32 = Vector256<float>;
    using i32 = Vector256<int>;

    public struct Avx2Functions : IFunctionList<f32, i32, Avx2Functions>
    {
        public static bool IsSupported => Avx2.IsSupported;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad(float value) => Avx2.BroadcastScalarToVector256(Vector128.CreateScalarUnsafe(value));
        public static i32 Broad(int value) => Avx2.BroadcastScalarToVector256(Vector128.CreateScalarUnsafe(value));

        // Load

        public static f32 Load(ref readonly float p) => Vector256.LoadUnsafe(in p);
        public static i32 Load(ref readonly int p) => Vector256.LoadUnsafe(in p);

        public static f32 Load(ref readonly float p, nuint elementOffset) => Vector256.LoadUnsafe(in p, elementOffset);
        public static i32 Load(ref readonly int p, nuint elementOffset) => Vector256.LoadUnsafe(in p, elementOffset);

        public static f32 Load(ReadOnlySpan<float> p) => Vector256.Create(p);
        public static i32 Load(ReadOnlySpan<int> p) => Vector256.Create(p);

        // Incremented

        public static f32 Incremented_f32()
        {
#if NET9_0_OR_GREATER
            return f32.Indices;
#else
            return Vector256.Create(0f, 1, 2, 3, 4, 5, 6, 7);
#endif
        }

        public static i32 Incremented_i32()
        {
#if NET9_0_OR_GREATER
            return i32.Indices;
#else
            return Vector256.Create(0, 1, 2, 3, 4, 5, 6, 7);
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

        public static f32 Convert_f32(i32 a) => Avx.ConvertToVector256Single(a);
        public static i32 Convert_i32(f32 a) => Avx.ConvertToVector256Int32(a);

        // Select

        public static f32 Select(f32 m, f32 a, f32 b) => Avx.BlendVariable(b, a, m.AsSingle());
        public static i32 Select(i32 m, i32 a, i32 b) => Avx2.BlendVariable(b, a, m.AsInt32());

        // Min

        public static f32 Min(f32 a, f32 b) => Avx.Min(a, b);
        public static i32 Min(i32 a, i32 b) => Avx2.Min(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinAcross(f32 a) => Sse2Functions.MinAcross(Vector128.Min(a.GetLower(), a.GetUpper()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinAcross(i32 a) => Sse2Functions.MinAcross(Vector128.Min(a.GetLower(), a.GetUpper()));

        // Max

        public static f32 Max(f32 a, f32 b) => Avx.Max(a, b);
        public static i32 Max(i32 a, i32 b) => Avx2.Max(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxAcross(f32 a) => Sse2Functions.MaxAcross(Vector128.Max(a.GetLower(), a.GetUpper()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxAcross(i32 a) => Sse2Functions.MaxAcross(Vector128.Max(a.GetLower(), a.GetUpper()));

        // Bitwise       

        public static f32 AndNot(f32 a, f32 b) => Avx.AndNot(b, a);
        public static i32 AndNot(i32 a, i32 b) => Avx2.AndNot(b, a);

        public static f32 ShiftRightLogical(f32 a, [ConstantExpected] byte b) => a >>> b;
        public static i32 ShiftRightLogical(i32 a, [ConstantExpected] byte b) => a >>> b;

        // Abs

        public static f32 Abs(f32 a) => Vector256.Abs(a);
        public static i32 Abs(i32 a) => Vector256.Abs(a);

        // Float math

        public static f32 Sqrt(f32 a) => Avx.Sqrt(a);
        public static f32 ReciprocalSqrt(f32 a) => Avx.ReciprocalSqrt(a);

        public static f32 Reciprocal(f32 a) => Avx.Reciprocal(a);

        // Rounding: http://dss.stephanierct.com/DevBlog/?p=8

        public static f32 Floor(f32 a) => Avx.RoundToNegativeInfinity(a);
        public static f32 Ceiling(f32 a) => Avx.RoundToPositiveInfinity(a);
        public static f32 Round(f32 a) => Avx.RoundToNearestInteger(a);

        // Mask

        public static i32 Mask(i32 a, i32 m) => a & m.AsInt32();
        public static f32 Mask(f32 a, f32 m) => a & m.AsSingle();

        public static i32 NMask(i32 a, i32 m) => Avx2.AndNot(m.AsInt32(), a);
        public static f32 NMask(f32 a, f32 m) => Avx.AndNot(m.AsSingle(), a);

        public static bool AnyMask(i32 m) => !Avx.TestZ(m, m);
        public static bool AllMask(i32 m) => m.ExtractMostSignificantBits() == 0xFF;

        // Bit Ops

        public static int Log2(i32 a) => BitOperations.Log2(a.ExtractMostSignificantBits());
        public static int PopCount(i32 a) => BitOperations.PopCount(a.ExtractMostSignificantBits());

        public static int LeadingZeroCount(i32 a) => BitOperations.LeadingZeroCount(a.ExtractMostSignificantBits());
        public static int TrailingZeroCount(i32 a) => BitOperations.TrailingZeroCount(a.ExtractMostSignificantBits());

        // Masked int32

        public static i32 MaskIncrement(i32 a, i32 m) => a - m.AsInt32();
        public static i32 MaskDecrement(i32 a, i32 m) => a + m.AsInt32();

        // FMA

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 FMulAdd(f32 a, f32 b, f32 c)
        {
            if (Fma.IsSupported)
            {
                return Fma.MultiplyAdd(a, b, c);
            }
            else
            {
                return (a * b) + c;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 FNMulAdd(f32 a, f32 b, f32 c)
        {
            if (Fma.IsSupported)
            {
                return Fma.MultiplyAddNegated(a, b, c);
            }
            else
            {
                return -(a * b) + c;
            }
        }

        // Float math

        public static f32 Add(f32 lhs, f32 rhs) => Avx.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Avx.And(lhs, rhs);
        public static f32 Not(f32 lhs) => ~lhs;
        public static f32 Div(f32 lhs, f32 rhs) => Avx.Divide(lhs, rhs);
        public static f32 Equal(f32 lhs, f32 rhs) => Avx.CompareEqual(lhs, rhs);
        public static f32 GreaterThan(f32 lhs, f32 rhs) => Avx.CompareGreaterThan(lhs, rhs);
        public static f32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareGreaterThanOrEqual(lhs, rhs);
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 LessThan(f32 lhs, f32 rhs) => Avx.CompareLessThan(lhs, rhs);
        public static f32 LessThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareLessThanOrEqual(lhs, rhs);
        public static f32 Mul(f32 lhs, f32 rhs) => Avx.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => -lhs;
        public static f32 NotEqual(f32 lhs, f32 rhs) => Avx.CompareNotEqual(lhs, rhs);
        public static f32 Or(f32 lhs, f32 rhs) => Avx.Or(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Avx.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Avx.Xor(lhs, rhs);

        // Int math

        public static i32 Add(i32 lhs, i32 rhs) => Avx2.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Avx2.And(lhs, rhs);
        public static i32 Not(i32 lhs) => ~lhs;
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Equal(i32 lhs, i32 rhs) => Avx2.CompareEqual(lhs, rhs);
        public static i32 GreaterThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(lhs, rhs);
        public static i32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => lhs << rhs;
        public static i32 LessThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(rhs, lhs);
        public static i32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => Avx2.MultiplyLow(lhs, rhs);
        public static i32 Negate(i32 lhs) => -lhs;
        public static i32 NotEqual(i32 lhs, i32 rhs) => ~Avx2.CompareEqual(lhs, rhs);
        public static i32 Or(i32 lhs, i32 rhs) => Avx2.Or(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => lhs >> rhs;
        public static i32 Sub(i32 lhs, i32 rhs) => Avx2.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Avx2.Xor(lhs, rhs);

        // Gradient dot fancy

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDotFancy(i32 hash, f32 fX, f32 fY)
        {
            i32 index = Avx.ConvertToVector256Int32(
                Avx.ConvertToVector256Single(hash & Broad(0x3FFFFF)) * Broad(1.3333333333333333f));

            f32 mX = Vector256.Create(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0);
            f32 gX = Avx2.PermuteVar8x32(mX, index);

            f32 mY = Vector256.Create(1, -1, 0, 0, ROOT3, ROOT3, 2, 2);
            f32 gY = Avx2.PermuteVar8x32(mY, index);

            // Bit-8 = Flip sign of a + b
            return FMulAdd(gX, fX, fY * gY) ^ ((index >> 3) << 31).AsSingle();
        }

        // Gradient dot

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY)
        {
            f32 mX = Vector256.Create(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1);
            f32 gX = Avx2.PermuteVar8x32(mX, hash);

            f32 mY = Vector256.Create(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2);
            f32 gY = Avx2.PermuteVar8x32(mY, hash);

            return FMulAdd(gX, fX, fY * gY);
        }
    }
}
