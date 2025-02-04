using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2;

namespace SharpFastNoise2.Functions
{
    using f32 = Vector128<float>;
    using i32 = Vector128<int>;

    public struct Sse2Functions : IFunctionList<f32, i32, Sse2Functions>
    {
        public static bool IsSupported => Sse2.IsSupported || Vector128.IsHardwareAccelerated;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad(float value) => Vector128.Create(value);
        public static i32 Broad(int value) => Vector128.Create(value);

        // Load

        public static f32 Load(ref readonly float p) => Vector128.LoadUnsafe(in p);
        public static i32 Load(ref readonly int p) => Vector128.LoadUnsafe(in p);

        public static f32 Load(ref readonly float p, nuint elementOffset) => Vector128.LoadUnsafe(in p, elementOffset);
        public static i32 Load(ref readonly int p, nuint elementOffset) => Vector128.LoadUnsafe(in p, elementOffset);

        public static f32 Load(ReadOnlySpan<float> p) => Vector128.Create(p);
        public static i32 Load(ReadOnlySpan<int> p) => Vector128.Create(p);

        // Incremented

        public static f32 Incremented_f32() => Vector128.Create(0f, 1, 2, 3);
        public static i32 Incremented_i32() => Vector128.Create(0, 1, 2, 3);

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

        public static f32 Convert_f32(i32 a) => Vector128.ConvertToSingle(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 Convert_i32(f32 a)
        {
            if (Sse2.IsSupported)
            {
                return Sse2.ConvertToVector128Int32(a);
            }
            else
            {
                // More accurate hack:
                // f32 sign = a & Vector128.Create(-0.0f);
                // f32 val_2p23_f32 = sign | Vector128.Create(8388608.0f);
                // val_2p23_f32 = (a + val_2p23_f32) - val_2p23_f32;
                // return val_2p23_f32 | sign;

                f32 aSign = a & Vector128.Create(-0f);
                f32 v = a & (aSign | Vector128.Create(0.5f));
                return Vector128.ConvertToInt32(v);
            }
        }

        // Select

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Select(f32 m, f32 a, f32 b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.BlendVariable(b, a, m);
            }
            else
            {
                return Vector128.ConditionalSelect(m, a, b);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 Select(i32 m, i32 a, i32 b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.BlendVariable(b, a, m);
            }
            else
            {
                return Vector128.ConditionalSelect(m, a, b);
            }
        }

        // Min

        public static f32 Min(f32 a, f32 b) => Vector128.Min(a, b);
        public static i32 Min(i32 a, i32 b) => Vector128.Min(a, b);

        // Min/Max-Across based on
        //  https://stackoverflow.com/questions/6996764/fastest-way-to-do-horizontal-sse-vector-sum-or-other-reduction/35270026#35270026

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinAcross(f32 a)
        {
            f32 v1 = Vector128.Shuffle(a, Vector128.Create(2, 3, 0, 1));
            f32 v2 = Vector128.Min(a, v1);
            f32 v3 = Vector128.Shuffle(v2, Vector128.Create(1, 0, 0, 0));
            f32 v4 = Sse.IsSupported ? Sse.MinScalar(v2, v3) : Vector128.Min(v2, v3);
            return v4.ToScalar();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinAcross(i32 a)
        {
            i32 v1 = Vector128.Shuffle(a, Vector128.Create(2, 3, 0, 1));
            i32 v2 = Vector128.Min(a, v1);
            i32 v3 = Vector128.Shuffle(v2, Vector128.Create(1, 0, 0, 0));
            i32 v4 = Vector128.Min(v2, v3);
            return v4.ToScalar();
        }

        // Max

        public static f32 Max(f32 a, f32 b) => Vector128.Max(a, b);
        public static i32 Max(i32 a, i32 b) => Vector128.Max(a, b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxAcross(f32 a)
        {
            f32 v1 = Vector128.Shuffle(a, Vector128.Create(2, 3, 0, 1));
            f32 v2 = Vector128.Max(a, v1);
            f32 v3 = Vector128.Shuffle(v2, Vector128.Create(1, 0, 0, 0));
            f32 v4 = Sse.IsSupported ? Sse.MaxScalar(v2, v3) : Vector128.Max(v2, v3);
            return v4.ToScalar();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxAcross(i32 a)
        {
            i32 v1 = Vector128.Shuffle(a, Vector128.Create(2, 3, 0, 1));
            i32 v2 = Vector128.Max(a, v1);
            i32 v3 = Vector128.Shuffle(v2, Vector128.Create(1, 0, 0, 0));
            i32 v4 = Vector128.Max(v2, v3);
            return v4.ToScalar();
        }

        // Bitwise

        public static f32 AndNot(f32 a, f32 b) => Vector128.AndNot(a, b);
        public static i32 AndNot(i32 a, i32 b) => Vector128.AndNot(a, b);

        public static f32 ShiftRightLogical(f32 a, [ConstantExpected] byte b) => a >>> b;
        public static i32 ShiftRightLogical(i32 a, [ConstantExpected] byte b) => a >>> b;

        // Abs

        public static f32 Abs(f32 a) => Vector128.Abs(a);
        public static i32 Abs(i32 a) => Vector128.Abs(a);

        // Float math

        public static f32 Sqrt(f32 a) => Vector128.Sqrt(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 ReciprocalSqrt(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalSqrt(a);
            }
            else
            {
                return Vector128.Create(1f) / Vector128.Sqrt(a);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Reciprocal(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.Reciprocal(a);
            }
            else
            {
                return Vector128.Create(1f) / a;
            }
        }

        // Rounding: http://dss.stephanierct.com/DevBlog/?p=8

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Floor(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNegativeInfinity(a);
            }
            else
            {
                f32 fval = Vector128.ConvertToSingle(Vector128.ConvertToInt32(a));
                f32 cmp = Vector128.LessThan(a, fval);
                return fval - (cmp & Vector128.Create(1f));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Ceiling(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToPositiveInfinity(a);
            }
            else
            {
                f32 fval = Vector128.ConvertToSingle(Vector128.ConvertToInt32(a));
                f32 cmp = Vector128.LessThan(fval, a);
                return fval + (cmp & Vector128.Create(1f));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Round(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNearestInteger(a);
            }
            else
            {
                return Vector128.ConvertToSingle(Convert_i32(a));
            }
        }

        // Mask

        public static i32 Mask(i32 a, i32 m) => a & m;
        public static f32 Mask(f32 a, f32 m) => a & m;

        public static i32 NMask(i32 a, i32 m) => Vector128.AndNot(a, m);
        public static f32 NMask(f32 a, f32 m) => Vector128.AndNot(a, m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyMask(i32 m)
        {
            if (Sse41.IsSupported)
            {
                return !Sse41.TestZ(m, m);
            }
            else
            {
                return m.ExtractMostSignificantBits() != 0;
            }
        }

        public static bool AllMask(i32 m) => m.ExtractMostSignificantBits() == 0xF;

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

        public static f32 Add(f32 lhs, f32 rhs) => Vector128.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Vector128.BitwiseAnd(lhs, rhs);
        public static f32 Complement(f32 lhs) => Vector128.OnesComplement(lhs);
        public static f32 Div(f32 lhs, f32 rhs) => Vector128.Divide(lhs, rhs);
        public static f32 Equal(f32 lhs, f32 rhs) => Vector128.Equals(lhs, rhs);
        public static f32 GreaterThan(f32 lhs, f32 rhs) => Vector128.GreaterThan(lhs, rhs);
        public static f32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Vector128.GreaterThanOrEqual(lhs, rhs);
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 LessThan(f32 lhs, f32 rhs) => Vector128.LessThan(lhs, rhs);
        public static f32 LessThanOrEqual(f32 lhs, f32 rhs) => Vector128.LessThanOrEqual(lhs, rhs);
        public static f32 Mul(f32 lhs, f32 rhs) => Vector128.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => Vector128.Negate(lhs);
        public static f32 NotEqual(f32 lhs,f32 rhs) => ~Vector128.Equals(lhs,rhs);

        public static f32 Or(f32 lhs, f32 rhs) => Vector128.BitwiseOr(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Vector128.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Vector128.Xor(lhs, rhs);

        // Int math

        public static i32 Add(i32 lhs, i32 rhs) => Vector128.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Vector128.BitwiseAnd(lhs, rhs);
        public static i32 Complement(i32 lhs) => Vector128.OnesComplement(lhs);
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Equal(i32 lhs, i32 rhs) => Vector128.Equals(lhs, rhs);
        public static i32 GreaterThan(i32 lhs, i32 rhs) => Vector128.GreaterThan(lhs, rhs);
        public static i32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => Vector128.ShiftLeft(lhs, rhs);
        public static i32 LessThan(i32 lhs, i32 rhs) => Vector128.LessThan(lhs, rhs);
        public static i32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => lhs * rhs;
        public static i32 Negate(i32 lhs) => Vector128.Negate(lhs);
        public static i32 NotEqual(i32 lhs,i32 rhs) => ~Vector128.Equals(lhs,rhs);

        public static i32 Or(i32 lhs, i32 rhs) => Vector128.BitwiseOr(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => Vector128.ShiftRightArithmetic(lhs, rhs);
        public static i32 Sub(i32 lhs, i32 rhs) => Vector128.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Vector128.Xor(lhs, rhs);
    }
}
