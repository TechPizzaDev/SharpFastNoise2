namespace SharpFastNoise2
{
    public interface IFunctionList<m32, f32, i32>
    {
        static abstract bool IsSupported { get; }

        static abstract int Count { get; }

        // Broadcast

        public f32 Broad_f32(float value);

        public i32 Broad_i32(int value);

        // Load

        public f32 Load_f32(ref byte p);

        public i32 Load_i32(ref byte p);

        // Incremented

        public f32 Incremented_f32();

        public i32 Incremented_i32();

        // Store

        public void Store_f32(ref byte p, f32 a);

        public void Store_i32(ref byte p, i32 a);

        public void Store_f32(ref float p, f32 a);

        public void Store_i32(ref int p, i32 a);

        // Extract

        public float Extract0_f32(f32 a);

        public int Extract0_i32(i32 a);

        public float Extract_f32(f32 a, int idx);

        public int Extract_i32(i32 a, int idx);

        // Cast

        public f32 Casti32_f32(i32 a);

        public i32 Castf32_i32(f32 a);

        // Convert

        public f32 Converti32_f32(i32 a);

        public i32 Convertf32_i32(f32 a);

        // Select

        public f32 Select_f32(m32 m, f32 a, f32 b);

        public i32 Select_i32(m32 m, i32 a, i32 b);

        // Min, Max

        public f32 Min_f32(f32 a, f32 b);

        public f32 Max_f32(f32 a, f32 b);

        public i32 Min_i32(i32 a, i32 b);

        public i32 Max_i32(i32 a, i32 b);

        // Bitwise       

        public f32 BitwiseAndNot_f32(f32 a, f32 b);

        public i32 BitwiseAndNot_i32(i32 a, i32 b);

        public m32 BitwiseAndNot_m32(m32 a, m32 b);

        public f32 BitwiseShiftRightZX_f32(f32 a, byte b);

        public i32 BitwiseShiftRightZX_i32(i32 a, byte b);

        // Abs

        public f32 Abs_f32(f32 a);

        public i32 Abs_i32(i32 a);

        // Float math

        public f32 Sqrt_f32(f32 a);

        public f32 InvSqrt_f32(f32 a);

        public f32 Reciprocal_f32(f32 a);

        // Floor, Ceil, Round

        public f32 Floor_f32(f32 a);

        public f32 Ceil_f32(f32 a);

        public f32 Round_f32(f32 a);

        // Mask

        public i32 Mask_i32(i32 a, m32 m);

        public f32 Mask_f32(f32 a, m32 m);

        public i32 NMask_i32(i32 a, m32 m);

        public f32 NMask_f32(f32 a, m32 m);

        public bool AnyMask_bool(m32 m);

        //FMA

        public f32 FMulAdd_f32(f32 a, f32 b, f32 c);

        public f32 FNMulAdd_f32(f32 a, f32 b, f32 c);

        public f32 Add(f32 lhs, f32 rhs);
        public f32 And(f32 lhs, f32 rhs);
        public i32 AsInt32(f32 lhs);
        public f32 Div(f32 lhs, f32 rhs);
        public f32 Complement(f32 lhs);
        public m32 Equal(f32 lhs, f32 rhs);
        public m32 GreaterThan(f32 lhs, f32 rhs);
        public m32 GreaterThanOrEqual(f32 lhs, f32 rhs);
        public f32 LeftShift(f32 lhs, byte rhs);
        public m32 LessThan(f32 lhs, f32 rhs);
        public m32 LessThanOrEqual(f32 lhs, f32 rhs);
        public f32 Mul(f32 lhs, f32 rhs);
        public f32 Negate(f32 lhs);
        public m32 NotEqual(f32 lhs, f32 rhs);
        public f32 Or(f32 lhs, f32 rhs);
        public f32 RightShift(f32 lhs, byte rhs);
        public f32 Sub(f32 lhs, f32 rhs);
        public f32 Xor(f32 lhs, f32 rhs);

        public i32 Add(i32 lhs, i32 rhs);
        public i32 And(i32 lhs, i32 rhs);
        public f32 AsSingle(i32 lhs);
        public i32 Div(i32 lhs, i32 rhs);
        public i32 Complement(i32 lhs);
        public m32 Equal(i32 lhs, i32 rhs);
        public m32 GreaterThan(i32 lhs, i32 rhs);
        public m32 GreaterThanOrEqual(i32 lhs, i32 rhs);
        public i32 LeftShift(i32 lhs, byte rhs);
        public m32 LessThan(i32 lhs, i32 rhs);
        public m32 LessThanOrEqual(i32 lhs, i32 rhs);
        public i32 Mul(i32 lhs, i32 rhs);
        public i32 Negate(i32 lhs);
        public m32 NotEqual(i32 lhs, i32 rhs);
        public i32 Or(i32 lhs, i32 rhs);
        public i32 RightShift(i32 lhs, byte rhs);
        public i32 Sub(i32 lhs, i32 rhs);
        public i32 Xor(i32 lhs, i32 rhs);

        public m32 And(m32 lhs, m32 rhs);
        public m32 Complement(m32 lhs);
        public m32 Or(m32 lhs, m32 rhs);
    }
}
