namespace SharpFastNoise2
{
    public interface IFunctionList<mask32v, float32v, int32v>
    {
        static abstract bool IsSupported { get; }

        static abstract int Count { get; }

        // Broadcast

        public float32v Broad_f32(float value);

        public int32v Broad_i32(int value);

        // Load

        public float32v Load_f32(ref byte p);

        public int32v Load_i32(ref byte p);

        // Incremented

        public float32v Incremented_f32();

        public int32v Incremented_i32();

        // Store

        public void Store_f32(ref byte p, float32v a);

        public void Store_i32(ref byte p, int32v a);

        public void Store_f32(ref float p, float32v a);

        public void Store_i32(ref int p, int32v a);

        // Extract

        public float Extract0_f32(float32v a);

        public int Extract0_i32(int32v a);

        public float Extract_f32(float32v a, int idx);

        public int Extract_i32(int32v a, int idx);

        // Cast

        public float32v Casti32_f32(int32v a);

        public int32v Castf32_i32(float32v a);

        // Convert

        public float32v Converti32_f32(int32v a);

        public int32v Convertf32_i32(float32v a);

        // Select

        public float32v Select_f32(mask32v m, float32v a, float32v b);

        public int32v Select_i32(mask32v m, int32v a, int32v b);

        // Min, Max

        public float32v Min_f32(float32v a, float32v b);

        public float32v Max_f32(float32v a, float32v b);

        public int32v Min_i32(int32v a, int32v b);

        public int32v Max_i32(int32v a, int32v b);

        // Bitwise       

        public float32v BitwiseAndNot_f32(float32v a, float32v b);

        public int32v BitwiseAndNot_i32(int32v a, int32v b);

        public mask32v BitwiseAndNot_m32(mask32v a, mask32v b);

        public float32v BitwiseShiftRightZX_f32(float32v a, byte b);

        public int32v BitwiseShiftRightZX_i32(int32v a, byte b);

        // Abs

        public float32v Abs_f32(float32v a);

        public int32v Abs_i32(int32v a);

        // Float math

        public float32v Sqrt_f32(float32v a);

        public float32v InvSqrt_f32(float32v a);

        public float32v Reciprocal_f32(float32v a);

        // Floor, Ceil, Round

        public float32v Floor_f32(float32v a);

        public float32v Ceil_f32(float32v a);

        public float32v Round_f32(float32v a);

        // Mask

        public int32v Mask_i32(int32v a, mask32v m);

        public float32v Mask_f32(float32v a, mask32v m);

        public int32v NMask_i32(int32v a, mask32v m);

        public float32v NMask_f32(float32v a, mask32v m);

        public bool AnyMask_bool(mask32v m);

        //FMA

        public float32v FMulAdd_f32(float32v a, float32v b, float32v c);

        public float32v FNMulAdd_f32(float32v a, float32v b, float32v c);

        public float32v Add(float32v lhs, float32v rhs);
        public float32v And(float32v lhs, float32v rhs);
        public int32v AsInt32(float32v lhs);
        public float32v Div(float32v lhs, float32v rhs);
        public float32v Complement(float32v lhs);
        public mask32v Equal(float32v lhs, float32v rhs);
        public mask32v GreaterThan(float32v lhs, float32v rhs);
        public mask32v GreaterThanOrEqual(float32v lhs, float32v rhs);
        public float32v LeftShift(float32v lhs, byte rhs);
        public mask32v LessThan(float32v lhs, float32v rhs);
        public mask32v LessThanOrEqual(float32v lhs, float32v rhs);
        public float32v Mul(float32v lhs, float32v rhs);
        public float32v Negate(float32v lhs);
        public mask32v NotEqual(float32v lhs, float32v rhs);
        public float32v Or(float32v lhs, float32v rhs);
        public float32v RightShift(float32v lhs, byte rhs);
        public float32v Sub(float32v lhs, float32v rhs);
        public float32v Xor(float32v lhs, float32v rhs);

        public int32v Add(int32v lhs, int32v rhs);
        public int32v And(int32v lhs, int32v rhs);
        public float32v AsSingle(int32v lhs);
        public int32v Div(int32v lhs, int32v rhs);
        public int32v Complement(int32v lhs);
        public mask32v Equal(int32v lhs, int32v rhs);
        public mask32v GreaterThan(int32v lhs, int32v rhs);
        public mask32v GreaterThanOrEqual(int32v lhs, int32v rhs);
        public int32v LeftShift(int32v lhs, byte rhs);
        public mask32v LessThan(int32v lhs, int32v rhs);
        public mask32v LessThanOrEqual(int32v lhs, int32v rhs);
        public int32v Mul(int32v lhs, int32v rhs);
        public int32v Negate(int32v lhs);
        public mask32v NotEqual(int32v lhs, int32v rhs);
        public int32v Or(int32v lhs, int32v rhs);
        public int32v RightShift(int32v lhs, byte rhs);
        public int32v Sub(int32v lhs, int32v rhs);
        public int32v Xor(int32v lhs, int32v rhs);

        public mask32v And(mask32v lhs, mask32v rhs);
        public mask32v Complement(mask32v lhs);
        public mask32v Or(mask32v lhs, mask32v rhs);
    }
}
