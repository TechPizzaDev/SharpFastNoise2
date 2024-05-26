using System;
using System.Diagnostics.CodeAnalysis;
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

        // Incremented

        public static f32 Incremented_f32() => 0;

        public static i32 Incremented_i32() => 0;

        // Store

        public static void Store_f32(ref float p, f32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.As<f32, byte>(ref p), a);

        public static void Store_i32(ref int p, i32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.As<i32, byte>(ref p), a);

        public static void Store_f32(ref float p, nuint elementOffset, f32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref Unsafe.As<f32, byte>(ref p), elementOffset), a);

        public static void Store_i32(ref int p, nuint elementOffset, i32 a) =>
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref Unsafe.As<i32, byte>(ref p), elementOffset), a);

        // Extract

        public static float Extract0_f32(f32 a) => a;
        public static int Extract0_i32(i32 a) => a;

        public static float Extract_f32(f32 a, int idx) => a;
        public static int Extract_i32(i32 a, int idx) => a;

        // Cast

        public static f32 Casti32_f32(i32 a) => Unsafe.BitCast<i32, f32>(a);
        public static i32 Castf32_i32(f32 a) => Unsafe.BitCast<f32, i32>(a);

        // Convert

        public static f32 Converti32_f32(i32 a) => a;
        public static i32 Convertf32_i32(f32 a) => (int) MathF.Round(a);

        // Select

        public static f32 Select_f32(m32 m, f32 a, f32 b) => m != 0 ? a : b;
        public static i32 Select_i32(m32 m, i32 a, i32 b) => m != 0 ? a : b;

        // Min

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Min_f32(f32 a, f32 b)
        {
            if (Sse.IsSupported)
            {
                return Sse.MinScalar(Vector128.CreateScalarUnsafe(a), Vector128.CreateScalarUnsafe(b)).ToScalar();
            }
            return MathF.Min(a, b);
        }

        public static i32 Min_i32(i32 a, i32 b) => Math.Min(a, b);
        
        // Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Max_f32(f32 a, f32 b)
        {
            if (Sse.IsSupported)
            {
                return Sse.MaxScalar(Vector128.CreateScalarUnsafe(a), Vector128.CreateScalarUnsafe(b)).ToScalar();
            }
            return MathF.Max(a, b);
        }

        public static i32 Max_i32(i32 a, i32 b) => Math.Max(a, b);
        
        // Bitwise       

        public static f32 BitwiseAndNot_f32(f32 a, f32 b) => Casti32_f32(Castf32_i32(a) & ~Castf32_i32(b));
        public static i32 BitwiseAndNot_i32(i32 a, i32 b) => a & ~b;
        public static m32 BitwiseAndNot_m32(m32 a, m32 b) => a & ~b;

        public static f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b) => Casti32_f32(Castf32_i32(a) >>> b);
        public static i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b) => a >>> b;

        // Abs

        public static f32 Abs_f32(f32 a) => MathF.Abs(a);
        public static i32 Abs_i32(i32 a) => a < 0 ? -a : a;

        // Float math

        public static f32 Sqrt_f32(f32 a) => MathF.Sqrt(a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 InvSqrt_f32(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalSqrtScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
            }

            float xhalf = 0.5f * (float) a;
            a = Casti32_f32(0x5f3759df - (Castf32_i32(a) >> 1));
            a *= 1.5f - xhalf * (float) a * (float) a;
            return a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Reciprocal_f32(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
            }

            // pow( pow(x,-0.5), 2 ) = pow( x, -1 ) = 1.0 / x
            a = Casti32_f32((int) (0xbe6eb3beU - (uint) Castf32_i32(a)) >> 1);
            return a * a;
        }

        // Rounding

        public static f32 Floor_f32(f32 a) => MathF.Floor(a);
        public static f32 Ceil_f32(f32 a) => MathF.Ceiling(a);
        public static f32 Round_f32(f32 a) => MathF.Round(a);

        // Mask

        public static i32 Mask_i32(i32 a, m32 m) => m != 0 ? a : 0;
        public static f32 Mask_f32(f32 a, m32 m) => m != 0 ? a : 0;

        public static i32 NMask_i32(i32 a, m32 m) => m != 0 ? 0 : a;
        public static f32 NMask_f32(f32 a, m32 m) => m != 0 ? 0 : a;
        
        public static bool AnyMask_bool(m32 m) => m != 0;
        public static bool AllMask_bool(m32 m) => m == m32.MaxValue;

        public static i32 MaskedIncrement_i32(i32 a, m32 m) => a - (int) m;
        public static i32 MaskedDecrement_i32(i32 a, m32 m) => a + (int) m;

        // FMA

        public static f32 FMulAdd_f32(f32 a, f32 b, f32 c) => MathF.FusedMultiplyAdd(a, b, c);
        public static f32 FNMulAdd_f32(f32 a, f32 b, f32 c) => -(a * b) + c;

        // Float math

        public static f32 Add(f32 lhs, f32 rhs) => lhs + rhs;
        public static f32 And(f32 lhs, f32 rhs) => Casti32_f32(And(Castf32_i32(lhs), Castf32_i32(rhs)));
        public static f32 Complement(f32 lhs) => Casti32_f32(Complement(Castf32_i32(lhs)));
        public static f32 Div(f32 lhs, f32 rhs) => lhs / rhs;
        public static m32 Equal(f32 lhs, f32 rhs) => (lhs == rhs).AsUInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => (lhs > rhs).AsUInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => (lhs >= rhs).AsUInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => Casti32_f32(LeftShift(Castf32_i32(lhs), rhs));
        public static m32 LessThan(f32 lhs, f32 rhs) => (lhs < rhs).AsUInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => (lhs <= rhs).AsUInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => lhs * rhs;
        public static f32 Negate(f32 lhs) => -lhs;
        public static m32 NotEqual(f32 lhs, f32 rhs) => (lhs != rhs).AsUInt32();
        public static f32 Or(f32 lhs, f32 rhs) => Casti32_f32(Or(Castf32_i32(lhs), Castf32_i32(rhs)));
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => Casti32_f32(RightShift(Castf32_i32(lhs), rhs));
        public static f32 Sub(f32 lhs, f32 rhs) => lhs - rhs;
        public static f32 Xor(f32 lhs, f32 rhs) => Casti32_f32(Xor(Castf32_i32(lhs), Castf32_i32(rhs)));

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
