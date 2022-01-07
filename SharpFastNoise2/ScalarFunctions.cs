using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using f32 = Single;
    using i32 = Int32;
    using m32 = Int32;

    public struct ScalarFunctions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => true;

        public static int Count => 1;

        // Broadcast

        public f32 Broad_f32(float value)
        {
            return value;
        }

        public i32 Broad_i32(int value)
        {
            return value;
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
            return 0;
        }

        public i32 Incremented_i32()
        {
            return 0;
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
            return a;
        }

        public int Extract0_i32(i32 a)
        {
            return a;
        }

        public float Extract_f32(f32 a, int idx)
        {
            return a;
        }

        public int Extract_i32(i32 a, int idx)
        {
            return a;
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
            return (float)a;
        }

        public i32 Convertf32_i32(f32 a)
        {
            return (int)MathF.Round(a);
        }

        // Select

        public f32 Select_f32(m32 m, f32 a, f32 b)
        {
            return m != 0 ? a : b;
        }

        public i32 Select_i32(m32 m, i32 a, i32 b)
        {
            return m != 0 ? a : b;
        }

        // Min, Max

        public f32 Min_f32(f32 a, f32 b)
        {
            return MathF.Min(a, b);
        }

        public f32 Max_f32(f32 a, f32 b)
        {
            return MathF.Max(a, b);
        }

        public i32 Min_i32(i32 a, i32 b)
        {
            return Math.Min(a, b);
        }

        public i32 Max_i32(i32 a, i32 b)
        {
            return Math.Max(a, b);
        }

        // Bitwise       

        public f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Casti32_f32(Castf32_i32(a) & ~Castf32_i32(b));
        }

        public i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return a & ~b;
        }

        public m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return a & ~b;
        }

        public f32 BitwiseShiftRightZX_f32(f32 a, byte b)
        {
            return Casti32_f32((int)((uint)Castf32_i32(a) >> b));
        }

        public i32 BitwiseShiftRightZX_i32(i32 a, byte b)
        {
            return (int)((uint)a >> b);
        }

        // Abs

        public f32 Abs_f32(f32 a)
        {
            return MathF.Abs(a);
        }

        public i32 Abs_i32(i32 a)
        {
            return Math.Abs(a);
        }

        // Float math

        public f32 Sqrt_f32(f32 a)
        {
            return MathF.Sqrt(a);
        }

        public f32 InvSqrt_f32(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalSqrtScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
            }
            else
            {
                float xhalf = 0.5f * (float)a;
                a = Casti32_f32(0x5f3759df - (Castf32_i32(a) >> 1));
                a *= 1.5f - xhalf * (float)a * (float)a;
                return a;
            }
        }

        public f32 Reciprocal_f32(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
            }
            else
            {
                // pow( pow(x,-0.5), 2 ) = pow( x, -1 ) = 1.0 / x
                a = Casti32_f32((int)(0xbe6eb3beU - (uint)Castf32_i32(a)) >> 1);
                return a * a;
            }
        }

        // Floor, Ceil, Round

        public f32 Floor_f32(f32 a)
        {
            return MathF.Floor(a);
        }

        public f32 Ceil_f32(f32 a)
        {
            return MathF.Ceiling(a);
        }

        public f32 Round_f32(f32 a)
        {
            return MathF.Round(a);
        }

        // Mask

        public i32 Mask_i32(i32 a, m32 m)
        {
            return m != 0 ? a : 0;
        }

        public f32 Mask_f32(f32 a, m32 m)
        {
            return m != 0 ? a : 0;
        }

        public i32 NMask_i32(i32 a, m32 m)
        {
            return m != 0 ? 0 : a;
        }

        public f32 NMask_f32(f32 a, m32 m)
        {
            return m != 0 ? 0 : a;
        }

        public bool AnyMask_bool(m32 m)
        {
            return m != 0;
        }

        // FMA

        public f32 FMulAdd_f32(f32 a, f32 b, f32 c)
        {
            return MathF.FusedMultiplyAdd(a, b, c);
        }

        public f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
        {
            return -(a * b) + c;
        }

        // Generic math

        public f32 Add(f32 lhs, f32 rhs) => lhs + rhs;
        public f32 And(f32 lhs, f32 rhs) => AsSingle(And(AsInt32(lhs), AsInt32(rhs)));
        public i32 AsInt32(f32 lhs) => Unsafe.As<f32, i32>(ref lhs);
        public f32 Complement(f32 lhs) => AsSingle(Complement(AsInt32(lhs)));
        public f32 Div(f32 lhs, f32 rhs) => lhs / rhs;
        public i32 Equal(f32 lhs, f32 rhs) => (lhs == rhs).AsInt32();
        public i32 GreaterThan(f32 lhs, f32 rhs) => (lhs > rhs).AsInt32();
        public i32 GreaterThanOrEqual(f32 lhs, f32 rhs) => (lhs >= rhs).AsInt32();
        public f32 LeftShift(f32 lhs, byte rhs) => AsSingle(LeftShift(AsInt32(lhs), rhs));
        public i32 LessThan(f32 lhs, f32 rhs) => (lhs < rhs).AsInt32();
        public i32 LessThanOrEqual(f32 lhs, f32 rhs) => (lhs <= rhs).AsInt32();
        public f32 Mul(f32 lhs, f32 rhs) => lhs * rhs;
        public f32 Negate(f32 lhs) => -lhs;
        public i32 NotEqual(f32 lhs, f32 rhs) => (lhs != rhs).AsInt32();
        public f32 Or(f32 lhs, f32 rhs) => AsSingle(Or(AsInt32(lhs), AsInt32(rhs)));
        public f32 RightShift(f32 lhs, byte rhs) => AsSingle(RightShift(AsInt32(lhs), rhs));
        public f32 Sub(f32 lhs, f32 rhs) => lhs - rhs;
        public f32 Xor(f32 lhs, f32 rhs) => AsSingle(Xor(AsInt32(lhs), AsInt32(rhs)));

        public i32 Add(i32 lhs, i32 rhs) => lhs + rhs;
        public i32 And(i32 lhs, i32 rhs) => lhs & rhs;
        public f32 AsSingle(i32 lhs) => Unsafe.As<i32, f32>(ref lhs);
        public i32 Complement(i32 lhs) => ~lhs;
        public i32 Div(i32 lhs, i32 rhs) => lhs / rhs;
        public i32 Equal(i32 lhs, i32 rhs) => (lhs == rhs).AsInt32();
        public i32 GreaterThan(i32 lhs, i32 rhs) => (lhs > rhs).AsInt32();
        public i32 GreaterThanOrEqual(i32 lhs, i32 rhs) => (lhs >= rhs).AsInt32();
        public i32 LeftShift(i32 lhs, byte rhs) => lhs << rhs;
        public i32 LessThan(i32 lhs, i32 rhs) => (lhs < rhs).AsInt32();
        public i32 LessThanOrEqual(i32 lhs, i32 rhs) => (lhs <= rhs).AsInt32();
        public i32 Mul(i32 lhs, i32 rhs) => lhs * rhs;
        public i32 Negate(i32 lhs) => -lhs;
        public i32 NotEqual(i32 lhs, i32 rhs) => (lhs != rhs).AsInt32();
        public i32 Or(i32 lhs, i32 rhs) => lhs | rhs;
        public i32 RightShift(i32 lhs, byte rhs) => lhs >> rhs;
        public i32 Sub(i32 lhs, i32 rhs) => lhs - rhs;
        public i32 Xor(i32 lhs, i32 rhs) => lhs ^ rhs;
    }
}
