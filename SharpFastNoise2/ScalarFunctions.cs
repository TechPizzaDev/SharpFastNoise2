using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using float32v = Single;
    using int32v = Int32;
    using mask32v = Int32;

    public struct ScalarFunctions : IFunctionList<mask32v, float32v, int32v>
    {
        public static bool IsSupported => true;

        public int Count => 1;

        // Broadcast

        public float32v Broad_f32(float value)
        {
            return value;
        }

        public int32v Broad_i32(int value)
        {
            return value;
        }

        // Load

        public float32v Load_f32(ref byte p)
        {
            return Unsafe.ReadUnaligned<float32v>(ref p);
        }

        public int32v Load_i32(ref byte p)
        {
            return Unsafe.ReadUnaligned<int32v>(ref p);
        }

        // Incremented

        public float32v Incremented_f32()
        {
            return 0;
        }

        public int32v Incremented_i32()
        {
            return 0;
        }

        // Store

        public void Store_f32(ref byte p, float32v a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_i32(ref byte p, int32v a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_f32(ref float p, float32v a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref p), a);
        }

        public void Store_i32(ref int p, int32v a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref p), a);
        }

        // Extract

        public float Extract0_f32(float32v a)
        {
            return a;
        }

        public int Extract0_i32(int32v a)
        {
            return a;
        }

        public float Extract_f32(float32v a, int idx)
        {
            return a;
        }

        public int Extract_i32(int32v a, int idx)
        {
            return a;
        }

        // Cast

        public float32v Casti32_f32(int32v a)
        {
            return Unsafe.As<int32v, float32v>(ref a);
        }

        public int32v Castf32_i32(float32v a)
        {
            return Unsafe.As<float32v, int32v>(ref a);
        }

        // Convert

        public float32v Converti32_f32(int32v a)
        {
            return (float)a;
        }

        public int32v Convertf32_i32(float32v a)
        {
            return (int)MathF.Round(a);
        }

        // Select

        public float32v Select_f32(mask32v m, float32v a, float32v b)
        {
            return m != 0 ? a : b;
        }

        public int32v Select_i32(mask32v m, int32v a, int32v b)
        {
            return m != 0 ? a : b;
        }

        // Min, Max

        public float32v Min_f32(float32v a, float32v b)
        {
            return MathF.Min(a, b);
        }

        public float32v Max_f32(float32v a, float32v b)
        {
            return MathF.Max(a, b);
        }

        public int32v Min_i32(int32v a, int32v b)
        {
            return Math.Min(a, b);
        }

        public int32v Max_i32(int32v a, int32v b)
        {
            return Math.Max(a, b);
        }

        // Bitwise       

        public float32v BitwiseAndNot_f32(float32v a, float32v b)
        {
            return Casti32_f32(Castf32_i32(a) & ~Castf32_i32(b));
        }

        public int32v BitwiseAndNot_i32(int32v a, int32v b)
        {
            return a & ~b;
        }

        public mask32v BitwiseAndNot_m32(mask32v a, mask32v b)
        {
            return a & ~b;
        }

        public float32v BitwiseShiftRightZX_f32(float32v a, byte b)
        {
            return Casti32_f32((int)((uint)Castf32_i32(a) >> b));
        }

        public int32v BitwiseShiftRightZX_i32(int32v a, byte b)
        {
            return (int)((uint)a >> b);
        }

        // Abs

        public float32v Abs_f32(float32v a)
        {
            return MathF.Abs(a);
        }

        public int32v Abs_i32(int32v a)
        {
            return Math.Abs(a);
        }

        // Float math

        public float32v Sqrt_f32(float32v a)
        {
            return MathF.Sqrt(a);
        }

        public float32v InvSqrt_f32(float32v a)
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

        public float32v Reciprocal_f32(float32v a)
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

        public float32v Floor_f32(float32v a)
        {
            return MathF.Floor(a);
        }

        public float32v Ceil_f32(float32v a)
        {
            return MathF.Ceiling(a);
        }

        public float32v Round_f32(float32v a)
        {
            return MathF.Round(a);
        }

        // Mask

        public int32v Mask_i32(int32v a, mask32v m)
        {
            return m != 0 ? a : 0;
        }

        public float32v Mask_f32(float32v a, mask32v m)
        {
            return m != 0 ? a : 0;
        }

        public int32v NMask_i32(int32v a, mask32v m)
        {
            return m != 0 ? 0 : a;
        }

        public float32v NMask_f32(float32v a, mask32v m)
        {
            return m != 0 ? 0 : a;
        }

        public bool AnyMask_bool(mask32v m)
        {
            return m != 0;
        }

        // FMA

        public float32v FMulAdd_f32(float32v a, float32v b, float32v c)
        {
            return MathF.FusedMultiplyAdd(a, b, c);
        }

        public float32v FNMulAdd_f32(float32v a, float32v b, float32v c)
        {
            return -(a * b) + c;
        }

        // Generic math

        public float32v Add(float32v lhs, float32v rhs) => lhs + rhs;
        public float32v And(float32v lhs, float32v rhs) => AsSingle(And(AsInt32(lhs), AsInt32(rhs)));
        public int32v AsInt32(float32v lhs) => Unsafe.As<float32v, int32v>(ref lhs);
        public float32v Complement(float32v lhs) => AsSingle(Complement(AsInt32(lhs)));
        public float32v Div(float32v lhs, float32v rhs) => lhs / rhs;
        public int32v Equal(float32v lhs, float32v rhs) => (lhs == rhs).AsInt32();
        public int32v GreaterThan(float32v lhs, float32v rhs) => (lhs > rhs).AsInt32();
        public int32v GreaterThanOrEqual(float32v lhs, float32v rhs) => (lhs >= rhs).AsInt32();
        public float32v LeftShift(float32v lhs, byte rhs) => AsSingle(LeftShift(AsInt32(lhs), rhs));
        public int32v LessThan(float32v lhs, float32v rhs) => (lhs < rhs).AsInt32();
        public int32v LessThanOrEqual(float32v lhs, float32v rhs) => (lhs <= rhs).AsInt32();
        public float32v Mul(float32v lhs, float32v rhs) => lhs * rhs;
        public float32v Negate(float32v lhs) => -lhs;
        public int32v NotEqual(float32v lhs, float32v rhs) => (lhs != rhs).AsInt32();
        public float32v Or(float32v lhs, float32v rhs) => AsSingle(Or(AsInt32(lhs), AsInt32(rhs)));
        public float32v RightShift(float32v lhs, byte rhs) => AsSingle(RightShift(AsInt32(lhs), rhs));
        public float32v Sub(float32v lhs, float32v rhs) => lhs - rhs;
        public float32v Xor(float32v lhs, float32v rhs) => AsSingle(Xor(AsInt32(lhs), AsInt32(rhs)));

        public int32v Add(int32v lhs, int32v rhs) => lhs + rhs;
        public int32v And(int32v lhs, int32v rhs) => lhs & rhs;
        public float32v AsSingle(int32v lhs) => Unsafe.As<int32v, float32v>(ref lhs);
        public int32v Complement(int32v lhs) => ~lhs;
        public int32v Div(int32v lhs, int32v rhs) => lhs / rhs;
        public int32v Equal(int32v lhs, int32v rhs) => (lhs == rhs).AsInt32();
        public int32v GreaterThan(int32v lhs, int32v rhs) => (lhs > rhs).AsInt32();
        public int32v GreaterThanOrEqual(int32v lhs, int32v rhs) => (lhs >= rhs).AsInt32();
        public int32v LeftShift(int32v lhs, byte rhs) => lhs << rhs;
        public int32v LessThan(int32v lhs, int32v rhs) => (lhs < rhs).AsInt32();
        public int32v LessThanOrEqual(int32v lhs, int32v rhs) => (lhs <= rhs).AsInt32();
        public int32v Mul(int32v lhs, int32v rhs) => lhs * rhs;
        public int32v Negate(int32v lhs) => -lhs;
        public int32v NotEqual(int32v lhs, int32v rhs) => (lhs != rhs).AsInt32();
        public int32v Or(int32v lhs, int32v rhs) => lhs | rhs;
        public int32v RightShift(int32v lhs, byte rhs) => lhs >> rhs;
        public int32v Sub(int32v lhs, int32v rhs) => lhs - rhs;
        public int32v Xor(int32v lhs, int32v rhs) => lhs ^ rhs;
    }
}
