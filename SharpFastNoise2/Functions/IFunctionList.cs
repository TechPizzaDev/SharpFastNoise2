using System.Diagnostics.CodeAnalysis;

namespace SharpFastNoise2.Functions
{
    public partial interface IFunctionList<m32, f32, i32, F>
        where F : IFunctionList<m32, f32, i32, F>
    {
        static abstract bool IsSupported { get; }

        static abstract int Count { get; }

        // Broadcast

        static abstract f32 Broad(float value);
        static abstract i32 Broad(int value);

        // Load

        static abstract f32 Load(ref readonly float p);
        static abstract i32 Load(ref readonly int p);

        static abstract f32 Load(ref readonly float p, nuint elementOffset);
        static abstract i32 Load(ref readonly int p, nuint elementOffset);

        // Incremented

        static abstract f32 Incremented_f32();
        static abstract i32 Incremented_i32();

        // Store

        static abstract void Store(ref float p, f32 a);
        static abstract void Store(ref int p, i32 a);

        static abstract void Store(ref float p, nuint elementOffset, f32 a);
        static abstract void Store(ref int p, nuint elementOffset, i32 a);

        // Extract

        static abstract float Extract0(f32 a);
        static abstract int Extract0(i32 a);

        static abstract float Extract(f32 a, int idx);
        static abstract int Extract(i32 a, int idx);

        // Cast

        static abstract f32 Casti32_f32(i32 a);
        static abstract i32 Castf32_i32(f32 a);

        // Convert

        static abstract f32 Converti32_f32(i32 a);
        static abstract i32 Convertf32_i32(f32 a);

        // Select

        static abstract f32 Select_f32(m32 m, f32 a, f32 b);
        static abstract i32 Select_i32(m32 m, i32 a, i32 b);

        // Min

        static abstract f32 Min_f32(f32 a, f32 b);
        static abstract i32 Min_i32(i32 a, i32 b);

        // Max

        static abstract f32 Max_f32(f32 a, f32 b);
        static abstract i32 Max_i32(i32 a, i32 b);

        // Bitwise

        static abstract f32 BitwiseAndNot_f32(f32 a, f32 b);
        static abstract i32 BitwiseAndNot_i32(i32 a, i32 b);
        static abstract m32 BitwiseAndNot_m32(m32 a, m32 b);

        static abstract f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b);
        static abstract i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b);

        // Abs

        static abstract f32 Abs_f32(f32 a);
        static abstract i32 Abs_i32(i32 a);

        // Float math

        static abstract f32 Sqrt_f32(f32 a);
        static abstract f32 InvSqrt_f32(f32 a);

        static abstract f32 Reciprocal_f32(f32 a);

        // Rounding

        static abstract f32 Floor_f32(f32 a);
        static abstract f32 Ceil_f32(f32 a);
        static abstract f32 Round_f32(f32 a);

        // Mask

        static abstract i32 Mask_i32(i32 a, m32 m);
        static abstract f32 Mask_f32(f32 a, m32 m);

        static abstract i32 NMask_i32(i32 a, m32 m);
        static abstract f32 NMask_f32(f32 a, m32 m);

        static abstract bool AnyMask_bool(m32 m);
        static abstract bool AllMask_bool(m32 m);

        // Masked float

        static virtual f32 MaskedAdd_f32(f32 a, f32 b, m32 m) => F.Add(a, F.Mask_f32(b, m));
        static virtual f32 MaskedSub_f32(f32 a, f32 b, m32 m) => F.Sub(a, F.Mask_f32(b, m));
        static virtual f32 MaskedMul_f32(f32 a, f32 b, m32 m) => F.Mul(a, F.Mask_f32(b, m));

        // NMasked float

        static virtual f32 NMaskedAdd_f32(f32 a, f32 b, m32 m) => F.Add(a, F.NMask_f32(b, m));
        static virtual f32 NMaskedSub_f32(f32 a, f32 b, m32 m) => F.Sub(a, F.NMask_f32(b, m));
        static virtual f32 NMaskedMul_f32(f32 a, f32 b, m32 m) => F.Mul(a, F.NMask_f32(b, m));

        // Masked int32

        static virtual i32 MaskedAdd_i32(i32 a, i32 b, m32 m) => F.Add(a, F.Mask_i32(b, m));
        static virtual i32 MaskedSub_i32(i32 a, i32 b, m32 m) => F.Sub(a, F.Mask_i32(b, m));
        static virtual i32 MaskedMul_i32(i32 a, i32 b, m32 m) => F.Mul(a, F.Mask_i32(b, m));

        static virtual i32 MaskedIncrement_i32(i32 a, m32 m) => F.MaskedSub_i32(a, F.Broad(-1), m);
        static virtual i32 MaskedDecrement_i32(i32 a, m32 m) => F.MaskedAdd_i32(a, F.Broad(-1), m);

        // NMasked int32

        static virtual i32 NMaskedAdd_i32(i32 a, i32 b, m32 m) => F.Add(a, F.NMask_i32(b, m));
        static virtual i32 NMaskedSub_i32(i32 a, i32 b, m32 m) => F.Sub(a, F.NMask_i32(b, m));
        static virtual i32 NMaskedMul_i32(i32 a, i32 b, m32 m) => F.Mul(a, F.NMask_i32(b, m));

        // FMA

        static abstract f32 FMulAdd_f32(f32 a, f32 b, f32 c);
        static abstract f32 FNMulAdd_f32(f32 a, f32 b, f32 c);

        // Float math

        static abstract f32 Add(f32 lhs, f32 rhs);
        static abstract f32 And(f32 lhs, f32 rhs);
        static abstract f32 Div(f32 lhs, f32 rhs);
        static abstract f32 Complement(f32 lhs);
        static abstract m32 Equal(f32 lhs, f32 rhs);
        static abstract m32 GreaterThan(f32 lhs, f32 rhs);
        static abstract m32 GreaterThanOrEqual(f32 lhs, f32 rhs);
        static abstract f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs);
        static abstract m32 LessThan(f32 lhs, f32 rhs);
        static abstract m32 LessThanOrEqual(f32 lhs, f32 rhs);
        static abstract f32 Mul(f32 lhs, f32 rhs);
        static abstract f32 Negate(f32 lhs);
        static abstract m32 NotEqual(f32 lhs, f32 rhs);
        static abstract f32 Or(f32 lhs, f32 rhs);
        static abstract f32 RightShift(f32 lhs, [ConstantExpected] byte rhs);
        static abstract f32 Sub(f32 lhs, f32 rhs);
        static abstract f32 Xor(f32 lhs, f32 rhs);

        // Int math

        static abstract i32 Add(i32 lhs, i32 rhs);
        static abstract i32 And(i32 lhs, i32 rhs);
        static abstract i32 Div(i32 lhs, i32 rhs);
        static abstract i32 Complement(i32 lhs);
        static abstract m32 Equal(i32 lhs, i32 rhs);
        static abstract m32 GreaterThan(i32 lhs, i32 rhs);
        static abstract m32 GreaterThanOrEqual(i32 lhs, i32 rhs);
        static abstract i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs);
        static abstract m32 LessThan(i32 lhs, i32 rhs);
        static abstract m32 LessThanOrEqual(i32 lhs, i32 rhs);
        static abstract i32 Mul(i32 lhs, i32 rhs);
        static abstract i32 Negate(i32 lhs);
        static abstract m32 NotEqual(i32 lhs, i32 rhs);
        static abstract i32 Or(i32 lhs, i32 rhs);
        static abstract i32 RightShift(i32 lhs, [ConstantExpected] byte rhs);
        static abstract i32 Sub(i32 lhs, i32 rhs);
        static abstract i32 Xor(i32 lhs, i32 rhs);

        // Mask math

        static abstract m32 And(m32 lhs, m32 rhs);
        static abstract m32 Complement(m32 lhs);
        static abstract m32 Or(m32 lhs, m32 rhs);
    }
}
