using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2;

namespace SharpFastNoise2.Functions
{
    using static Gradient;
    using f32 = Vector256<float>;
    using i32 = Vector256<int>;
    using m32 = Vector256<uint>;

    public struct Avx2Functions : IFunctionList<m32, f32, i32, Avx2Functions>
    {
        public static bool IsSupported => Avx2.IsSupported;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad(float value) => Vector256.Create(value);
        public static i32 Broad(int value) => Vector256.Create(value);

        // Load

        public static f32 Load(ref readonly float p) => Vector256.LoadUnsafe(in p);
        public static i32 Load(ref readonly int p) => Vector256.LoadUnsafe(in p);

        public static f32 Load(ref readonly float p, nuint elementOffset) => Vector256.LoadUnsafe(in p, elementOffset);
        public static i32 Load(ref readonly int p, nuint elementOffset) => Vector256.LoadUnsafe(in p, elementOffset);

        // Incremented

        public static f32 Incremented_f32() => Vector256.Create(0f, 1, 2, 3, 4, 5, 6, 7);
        public static i32 Incremented_i32() => Vector256.Create(0, 1, 2, 3, 4, 5, 6, 7);

        // Store

        public static void Store(ref float p, f32 a) => a.StoreUnsafe(ref p);
        public static void Store(ref int p, i32 a) => a.StoreUnsafe(ref p);

        public static void Store(ref float p, nuint elementOffset, f32 a) => a.StoreUnsafe(ref p, elementOffset);
        public static void Store(ref int p, nuint elementOffset, i32 a) => a.StoreUnsafe(ref p, elementOffset);

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

        public static f32 Select(m32 m, f32 a, f32 b) => Avx.BlendVariable(b, a, m.AsSingle());
        public static i32 Select(m32 m, i32 a, i32 b) => Avx2.BlendVariable(b, a, m.AsInt32());

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
        public static m32 AndNot(m32 a, m32 b) => Avx2.AndNot(b, a);

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

        public static f32 Floor_f32(f32 a) => Avx.RoundToNegativeInfinity(a);
        public static f32 Ceil_f32(f32 a) => Avx.RoundToPositiveInfinity(a);
        public static f32 Round_f32(f32 a) => Avx.RoundToNearestInteger(a);

        // Mask

        public static i32 Mask_i32(i32 a, m32 m) => a & m.AsInt32();
        public static f32 Mask_f32(f32 a, m32 m) => a & m.AsSingle();

        public static i32 NMask_i32(i32 a, m32 m) => Avx2.AndNot(m.AsInt32(), a);
        public static f32 NMask_f32(f32 a, m32 m) => Avx.AndNot(m.AsSingle(), a);

        public static bool AnyMask_bool(m32 m) => !Avx.TestZ(m, m);
        public static bool AllMask_bool(m32 m) => m.ExtractMostSignificantBits() == 0xFF;

        public static i32 MaskedIncrement_i32(i32 a, m32 m) => a - m.AsInt32();
        public static i32 MaskedDecrement_i32(i32 a, m32 m) => a + m.AsInt32();

        // FMA

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 FMulAdd_f32(f32 a, f32 b, f32 c)
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
        public static f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
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
        public static f32 Complement(f32 lhs) => Vector256.OnesComplement(lhs);
        public static f32 Div(f32 lhs, f32 rhs) => Avx.Divide(lhs, rhs);
        public static m32 Equal(f32 lhs, f32 rhs) => Avx.CompareEqual(lhs, rhs).AsUInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => Avx.CompareGreaterThan(lhs, rhs).AsUInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareGreaterThanOrEqual(lhs, rhs).AsUInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static m32 LessThan(f32 lhs, f32 rhs) => Avx.CompareLessThan(lhs, rhs).AsUInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareLessThanOrEqual(lhs, rhs).AsUInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => Avx.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => Vector256.Negate(lhs);
        public static m32 NotEqual(f32 lhs, f32 rhs) => Avx.CompareNotEqual(lhs, rhs).AsUInt32();
        public static f32 Or(f32 lhs, f32 rhs) => Avx.Or(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Avx.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Avx.Xor(lhs, rhs);

        // Int math

        public static i32 Add(i32 lhs, i32 rhs) => Avx2.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Avx2.And(lhs, rhs);
        public static i32 Complement(i32 lhs) => Vector256.OnesComplement(lhs);
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static m32 Equal(i32 lhs, i32 rhs) => Avx2.CompareEqual(lhs, rhs).AsUInt32();
        public static m32 GreaterThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(lhs, rhs).AsUInt32();
        public static m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => lhs << rhs;
        public static m32 LessThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(rhs, lhs).AsUInt32();
        public static m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => Avx2.MultiplyLow(lhs, rhs);
        public static i32 Negate(i32 lhs) => Vector256.Negate(lhs);
        public static m32 NotEqual(i32 lhs, i32 rhs) => Avx.CompareNotEqual(lhs.AsSingle(), rhs.AsSingle()).AsUInt32();
        public static i32 Or(i32 lhs, i32 rhs) => Avx2.Or(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => lhs >> rhs;
        public static i32 Sub(i32 lhs, i32 rhs) => Avx2.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Avx2.Xor(lhs, rhs);

        // Mask math

        public static m32 And(m32 lhs, m32 rhs) => lhs & rhs;
        public static m32 Complement(m32 lhs) => ~lhs;
        public static m32 Or(m32 lhs, m32 rhs) => lhs | rhs;

        // Gradient dot fancy

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDotFancy(i32 hash, f32 fX, f32 fY)
        {
            i32 index = Avx.ConvertToVector256Int32(
                Avx.ConvertToVector256Single(hash & Vector256.Create(0x3FFFFF)) * Vector256.Create(1.3333333333333333f));

            f32 gX = Avx2.PermuteVar8x32(Vector256.Create(ROOT3, ROOT3, 2, 2, 1, -1, 0, 0), index);
            f32 gY = Avx2.PermuteVar8x32(Vector256.Create(1, -1, 0, 0, ROOT3, ROOT3, 2, 2), index);

            // Bit-8 = Flip sign of a + b
            return FMulAdd_f32(gX, fX, fY * gY) ^ ((index >> 3) << 31).AsSingle();
        }

        // Gradient dot

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 GetGradientDot(i32 hash, f32 fX, f32 fY)
        {
            f32 gX = Avx2.PermuteVar8x32(Vector256.Create(1 + ROOT2, -1 - ROOT2, 1 + ROOT2, -1 - ROOT2, 1, -1, 1, -1), hash);
            f32 gY = Avx2.PermuteVar8x32(Vector256.Create(1, 1, -1, -1, 1 + ROOT2, 1 + ROOT2, -1 - ROOT2, -1 - ROOT2), hash);

            return FMulAdd_f32(gX, fX, fY * gY);
        }
    }
}
