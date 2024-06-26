using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2.Functions
{
    using f32 = Single;
    using i32 = Int32;
    using m32 = UInt32;

    public struct ScalarFunctions : IFunctionList<m32, f32, i32, ScalarFunctions>
    {
        public static bool IsSupported => true;

        public static int Count => 1;

        // Broadcast

        public static f32 Broad(float value) => value;
        public static i32 Broad(int value) => value;

        // Load

        public static f32 Load(ref readonly float p) =>
            Unsafe.ReadUnaligned<f32>(in Unsafe.As<f32, byte>(ref Unsafe.AsRef(in p)));

        public static i32 Load(ref readonly int p) =>
            Unsafe.ReadUnaligned<i32>(in Unsafe.As<i32, byte>(ref Unsafe.AsRef(in p)));

        public static f32 Load(ref readonly float p, nuint elementOffset) =>
            Unsafe.ReadUnaligned<f32>(in Unsafe.Add(ref Unsafe.As<f32, byte>(ref Unsafe.AsRef(in p)), elementOffset));

        public static i32 Load(ref readonly int p, nuint elementOffset) =>
            Unsafe.ReadUnaligned<i32>(in Unsafe.Add(ref Unsafe.As<i32, byte>(ref Unsafe.AsRef(in p)), elementOffset));

        public static f32 Load(ReadOnlySpan<float> p) => p[0];

        public static i32 Load(ReadOnlySpan<int> p) => p[0];

        // Incremented

        public static f32 Incremented_f32() => 0;

        public static i32 Incremented_i32() => 0;

        // Store

        public static void Store(ref float p, f32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.As<f32, byte>(ref p), a);

        public static void Store(ref int p, i32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.As<i32, byte>(ref p), a);

        public static void Store(ref float p, nuint elementOffset, f32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref Unsafe.As<f32, byte>(ref p), elementOffset), a);

        public static void Store(ref int p, nuint elementOffset, i32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref Unsafe.As<i32, byte>(ref p), elementOffset), a);

        public static void Store(Span<float> p, f32 a) => p[0] = a;

        public static void Store(Span<int> p, i32 a) => p[0] = a;

        // Extract

        public static float Extract0(f32 a) => a;
        public static int Extract0(i32 a) => a;

        public static float Extract(f32 a, int idx) => a;
        public static int Extract(i32 a, int idx) => a;

        // Cast

        public static f32 Cast_f32(i32 a) => Unsafe.BitCast<i32, f32>(a);
        public static i32 Cast_i32(f32 a) => Unsafe.BitCast<f32, i32>(a);

        // Convert

        public static f32 Convert_f32(i32 a) => a;
        public static i32 Convert_i32(f32 a) => (int)MathF.Round(a);

        // Select

        public static f32 Select(m32 m, f32 a, f32 b) => m != 0 ? a : b;
        public static i32 Select(m32 m, i32 a, i32 b) => m != 0 ? a : b;

        // Min

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Min(f32 a, f32 b)
        {
            if (Sse.IsSupported)
            {
                return Sse.MinScalar(Vector128.CreateScalarUnsafe(a), Vector128.CreateScalarUnsafe(b)).ToScalar();
            }
            return MathF.Min(a, b);
        }

        public static i32 Min(i32 a, i32 b) => Math.Min(a, b);

        // Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Max(f32 a, f32 b)
        {
            if (Sse.IsSupported)
            {
                return Sse.MaxScalar(Vector128.CreateScalarUnsafe(a), Vector128.CreateScalarUnsafe(b)).ToScalar();
            }
            return MathF.Max(a, b);
        }

        public static i32 Max(i32 a, i32 b) => Math.Max(a, b);

        // Bitwise       

        public static f32 AndNot(f32 a, f32 b) => Cast_f32(Cast_i32(a) & ~Cast_i32(b));
        public static i32 AndNot(i32 a, i32 b) => a & ~b;
        public static m32 AndNot(m32 a, m32 b) => a & ~b;

        public static f32 ShiftRightLogical(f32 a, [ConstantExpected] byte b) => Cast_f32(Cast_i32(a) >>> b);
        public static i32 ShiftRightLogical(i32 a, [ConstantExpected] byte b) => a >>> b;

        // Abs

        public static f32 Abs(f32 a) => MathF.Abs(a);
        public static i32 Abs(i32 a) => a < 0 ? -a : a;

        // Float math

        public static f32 Sqrt(f32 a) => MathF.Sqrt(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 ReciprocalSqrt(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalSqrtScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
            }

            f32 xhalf = 0.5f * a;
            a = Cast_f32(0x5f3759df - (Cast_i32(a) >> 1));
            a *= 1.5f - xhalf * a * a;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Reciprocal(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
            }

            // pow( pow(x,-0.5), 2 ) = pow( x, -1 ) = 1.0 / x
            a = Cast_f32((int)(0xbe6eb3beU - (uint)Cast_i32(a)) >> 1);
            return a * a;
        }

        // Rounding

        public static f32 Floor(f32 a) => MathF.Floor(a);
        public static f32 Ceiling(f32 a) => MathF.Ceiling(a);
        public static f32 Round(f32 a) => MathF.Round(a);

        // Mask

        public static i32 Mask(i32 a, m32 m) => m != 0 ? a : 0;
        public static f32 Mask(f32 a, m32 m) => m != 0 ? a : 0;

        public static i32 NMask(i32 a, m32 m) => m != 0 ? 0 : a;
        public static f32 NMask(f32 a, m32 m) => m != 0 ? 0 : a;

        public static bool AnyMask(m32 m) => m != 0;
        public static bool AllMask(m32 m) => m == m32.MaxValue;

        // Bit Ops

        public static int Log2(m32 a) => BitOperations.Log2(a);
        public static int PopCount(m32 a) => BitOperations.PopCount(a);

        public static int LeadingZeroCount(m32 a) => BitOperations.LeadingZeroCount(a);
        public static int TrailingZeroCount(m32 a) => BitOperations.TrailingZeroCount(a);

        // Masked int32

        public static i32 MaskIncrement(i32 a, m32 m) => a - (int) m;
        public static i32 MaskDecrement(i32 a, m32 m) => a + (int) m;

        // FMA

        public static f32 FMulAdd(f32 a, f32 b, f32 c) => MathF.FusedMultiplyAdd(a, b, c);
        public static f32 FNMulAdd(f32 a, f32 b, f32 c) => -(a * b) + c;

        // Float math

        public static f32 Add(f32 lhs, f32 rhs) => lhs + rhs;
        public static f32 And(f32 lhs, f32 rhs) => Cast_f32(And(Cast_i32(lhs), Cast_i32(rhs)));
        public static f32 Complement(f32 lhs) => Cast_f32(Complement(Cast_i32(lhs)));
        public static f32 Div(f32 lhs, f32 rhs) => lhs / rhs;
        public static m32 Equal(f32 lhs, f32 rhs) => (lhs == rhs).AsUInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => (lhs > rhs).AsUInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => (lhs >= rhs).AsUInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => Cast_f32(LeftShift(Cast_i32(lhs), rhs));
        public static m32 LessThan(f32 lhs, f32 rhs) => (lhs < rhs).AsUInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => (lhs <= rhs).AsUInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => lhs * rhs;
        public static f32 Negate(f32 lhs) => -lhs;
        public static m32 NotEqual(f32 lhs, f32 rhs) => (lhs != rhs).AsUInt32();
        public static f32 Or(f32 lhs, f32 rhs) => Cast_f32(Or(Cast_i32(lhs), Cast_i32(rhs)));
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => Cast_f32(RightShift(Cast_i32(lhs), rhs));
        public static f32 Sub(f32 lhs, f32 rhs) => lhs - rhs;
        public static f32 Xor(f32 lhs, f32 rhs) => Cast_f32(Xor(Cast_i32(lhs), Cast_i32(rhs)));

        // Int math

        public static i32 Add(i32 lhs, i32 rhs) => lhs + rhs;
        public static i32 And(i32 lhs, i32 rhs) => lhs & rhs;
        public static i32 Complement(i32 lhs) => ~lhs;
        public static i32 Div(i32 lhs, i32 rhs) => lhs / rhs;
        public static m32 Equal(i32 lhs, i32 rhs) => (lhs == rhs).AsUInt32();
        public static m32 GreaterThan(i32 lhs, i32 rhs) => (lhs > rhs).AsUInt32();
        public static m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => (lhs >= rhs).AsUInt32();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => lhs << rhs;
        public static m32 LessThan(i32 lhs, i32 rhs) => (lhs < rhs).AsUInt32();
        public static m32 LessThanOrEqual(i32 lhs, i32 rhs) => (lhs <= rhs).AsUInt32();
        public static i32 Mul(i32 lhs, i32 rhs) => lhs * rhs;
        public static i32 Negate(i32 lhs) => -lhs;
        public static m32 NotEqual(i32 lhs, i32 rhs) => (lhs != rhs).AsUInt32();
        public static i32 Or(i32 lhs, i32 rhs) => lhs | rhs;
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => lhs >> rhs;
        public static i32 Sub(i32 lhs, i32 rhs) => lhs - rhs;
        public static i32 Xor(i32 lhs, i32 rhs) => lhs ^ rhs;

        // Mask math

        public static m32 And(m32 lhs, m32 rhs) => lhs & rhs;
        public static m32 Complement(m32 lhs) => ~lhs;
        public static m32 Or(m32 lhs, m32 rhs) => lhs | rhs;
    }
}
