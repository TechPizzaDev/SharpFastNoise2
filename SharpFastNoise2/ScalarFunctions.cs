using System;
using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    using float32v = FVectorF32;
    using int32v = FVectorI32;
    using mask32v = FVectorI32;

    public struct ScalarFunctions : IFunctionList<mask32v, float32v, int32v>
    {
        public float32v Broad_f32(float value)
        {
            return new float32v(value);
        }

        public int32v Broad_i32(int value)
        {
            return new int32v(value);
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

        // Store

        public void Store_f32(ref byte p, float32v a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_i32(ref byte p, int32v a)
        {
            Unsafe.WriteUnaligned(ref p, a);
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

        public float32v BitwiseShiftRightZX_f32(float32v a, byte b)
        {
            return Casti32_f32((int)((uint)Castf32_i32(a).Value >> b));
        }

        public int32v BitwiseShiftRightZX_i32(int32v a, byte b)
        {
            return (int)((uint)a.Value >> b);
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
            float xhalf = 0.5f * (float)a;
            a = Casti32_f32(0x5f3759df - ((int)Castf32_i32(a) >> 1));
            a *= (1.5f - xhalf * (float)a * (float)a);
            return a;
        }

        public float32v Reciprocal_f32(float32v a)
        {
            // pow( pow(x,-0.5), 2 ) = pow( x, -1 ) = 1.0 / x
            a = Casti32_f32((int)(0xbe6eb3beU - (uint)(int)Castf32_i32(a)) >> 1);
            return a * a;
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
            return m != 0 ? a : new int32v(0);
        }

        public float32v Mask_f32(float32v a, mask32v m)
        {
            return m != 0 ? a : new float32v(0);
        }

        public int32v NMask_i32(int32v a, mask32v m)
        {
            return m != 0 ? new int32v(0) : a;
        }

        public float32v NMask_f32(float32v a, mask32v m)
        {
            return m != 0 ? new float32v(0) : a;
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
    }
}
