using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2.Functions
{
    using f32 = Single;
    using i32 = Int32;
    using m32 = Int32;

    public struct ScalarFunctions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => true;

        public static int Count => 1;

        // Broadcast

        public static f32 Broad_f32(float value)
        {
            return value;
        }

        public static i32 Broad_i32(int value)
        {
            return value;
        }

        // Load

        public static f32 Load_f32(ref readonly float p)
        {
            return Unsafe.ReadUnaligned<f32>(ref Unsafe.As<float, byte>(ref Unsafe.AsRef(in p)));
        }

        public static i32 Load_i32(ref readonly int p)
        {
            return Unsafe.ReadUnaligned<i32>(ref Unsafe.As<int, byte>(ref Unsafe.AsRef(in p)));
        }

        // Incremented

        public static f32 Incremented_f32()
        {
            return 0;
        }

        public static i32 Incremented_i32()
        {
            return 0;
        }

        // Store

        public static void Store_f32(ref float p, f32 a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref p), a);
        }

        public static void Store_i32(ref int p, i32 a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref p), a);
        }

        // Extract

        public static float Extract0_f32(f32 a)
        {
            return a;
        }

        public static int Extract0_i32(i32 a)
        {
            return a;
        }

        public static float Extract_f32(f32 a, int idx)
        {
            return a;
        }

        public static int Extract_i32(i32 a, int idx)
        {
            return a;
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
            return a;
        }

        public static i32 Convertf32_i32(f32 a)
        {
            return (int) MathF.Round(a);
        }

        // Select

        public static f32 Select_f32(m32 m, f32 a, f32 b)
        {
            return m != 0 ? a : b;
        }

        public static i32 Select_i32(m32 m, i32 a, i32 b)
        {
            return m != 0 ? a : b;
        }

        // Min, Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Min_f32(f32 a, f32 b)
        {
            if (Sse.IsSupported)
            {
                return Sse.MinScalar(Vector128.CreateScalarUnsafe(a), Vector128.CreateScalarUnsafe(b)).ToScalar();
            }
            return MathF.Min(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Max_f32(f32 a, f32 b)
        {
            if (Sse.IsSupported)
            {
                return Sse.MaxScalar(Vector128.CreateScalarUnsafe(a), Vector128.CreateScalarUnsafe(b)).ToScalar();
            }
            return MathF.Max(a, b);
        }

        public static i32 Min_i32(i32 a, i32 b)
        {
            return Math.Min(a, b);
        }

        public static i32 Max_i32(i32 a, i32 b)
        {
            return Math.Max(a, b);
        }

        // Bitwise       

        public static f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Casti32_f32(Castf32_i32(a) & ~Castf32_i32(b));
        }

        public static i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return a & ~b;
        }

        public static m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return a & ~b;
        }

        public static f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b)
        {
            return Casti32_f32((int) ((uint) Castf32_i32(a) >> b));
        }

        public static i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b)
        {
            return (int) ((uint) a >> b);
        }

        // Abs

        public static f32 Abs_f32(f32 a)
        {
            return MathF.Abs(a);
        }

        public static i32 Abs_i32(i32 a)
        {
            return Math.Abs(a);
        }

        // Float math

        public static f32 Sqrt_f32(f32 a)
        {
            return MathF.Sqrt(a);
        }

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

        // Floor, Ceil, Round

        public static f32 Floor_f32(f32 a)
        {
            return MathF.Floor(a);
        }

        public static f32 Ceil_f32(f32 a)
        {
            return MathF.Ceiling(a);
        }

        public static f32 Round_f32(f32 a)
        {
            return MathF.Round(a);
        }

        // Mask

        public static i32 Mask_i32(i32 a, m32 m)
        {
            return m != 0 ? a : 0;
        }

        public static f32 Mask_f32(f32 a, m32 m)
        {
            return m != 0 ? a : 0;
        }

        public static i32 NMask_i32(i32 a, m32 m)
        {
            return m != 0 ? 0 : a;
        }

        public static f32 NMask_f32(f32 a, m32 m)
        {
            return m != 0 ? 0 : a;
        }

        public static bool AnyMask_bool(m32 m)
        {
            return m != 0;
        }

        // FMA

        public static f32 FMulAdd_f32(f32 a, f32 b, f32 c)
        {
            return MathF.FusedMultiplyAdd(a, b, c);
        }

        public static f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
        {
            return -(a * b) + c;
        }

        // Generic math

        public static f32 Add(f32 lhs, f32 rhs) => lhs + rhs;
        public static f32 And(f32 lhs, f32 rhs) => AsSingle(And(AsInt32(lhs), AsInt32(rhs)));
        public static i32 AsInt32(f32 lhs) => Unsafe.BitCast<f32, i32>(lhs);
        public static f32 Complement(f32 lhs) => AsSingle(Complement(AsInt32(lhs)));
        public static f32 Div(f32 lhs, f32 rhs) => lhs / rhs;
        public static i32 Equal(f32 lhs, f32 rhs) => (lhs == rhs).AsInt32();
        public static i32 GreaterThan(f32 lhs, f32 rhs) => (lhs > rhs).AsInt32();
        public static i32 GreaterThanOrEqual(f32 lhs, f32 rhs) => (lhs >= rhs).AsInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => AsSingle(LeftShift(AsInt32(lhs), rhs));
        public static i32 LessThan(f32 lhs, f32 rhs) => (lhs < rhs).AsInt32();
        public static i32 LessThanOrEqual(f32 lhs, f32 rhs) => (lhs <= rhs).AsInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => lhs * rhs;
        public static f32 Negate(f32 lhs) => -lhs;
        public static i32 NotEqual(f32 lhs, f32 rhs) => (lhs != rhs).AsInt32();
        public static f32 Or(f32 lhs, f32 rhs) => AsSingle(Or(AsInt32(lhs), AsInt32(rhs)));
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => AsSingle(RightShift(AsInt32(lhs), rhs));
        public static f32 Sub(f32 lhs, f32 rhs) => lhs - rhs;
        public static f32 Xor(f32 lhs, f32 rhs) => AsSingle(Xor(AsInt32(lhs), AsInt32(rhs)));

        public static i32 Add(i32 lhs, i32 rhs) => lhs + rhs;
        public static i32 And(i32 lhs, i32 rhs) => lhs & rhs;
        public static f32 AsSingle(i32 lhs) => Unsafe.BitCast<i32, f32>(lhs);
        public static i32 Complement(i32 lhs) => ~lhs;
        public static i32 Div(i32 lhs, i32 rhs) => lhs / rhs;
        public static i32 Equal(i32 lhs, i32 rhs) => (lhs == rhs).AsInt32();
        public static i32 GreaterThan(i32 lhs, i32 rhs) => (lhs > rhs).AsInt32();
        public static i32 GreaterThanOrEqual(i32 lhs, i32 rhs) => (lhs >= rhs).AsInt32();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => lhs << rhs;
        public static i32 LessThan(i32 lhs, i32 rhs) => (lhs < rhs).AsInt32();
        public static i32 LessThanOrEqual(i32 lhs, i32 rhs) => (lhs <= rhs).AsInt32();
        public static i32 Mul(i32 lhs, i32 rhs) => lhs * rhs;
        public static i32 Negate(i32 lhs) => -lhs;
        public static i32 NotEqual(i32 lhs, i32 rhs) => (lhs != rhs).AsInt32();
        public static i32 Or(i32 lhs, i32 rhs) => lhs | rhs;
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => lhs >> rhs;
        public static i32 Sub(i32 lhs, i32 rhs) => lhs - rhs;
        public static i32 Xor(i32 lhs, i32 rhs) => lhs ^ rhs;
    }
}
