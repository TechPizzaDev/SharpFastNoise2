using System;
using System.Diagnostics.CodeAnalysis;

namespace SharpFastNoise2.Functions
{
    public partial interface IFunctionList<f32, i32, F>
        where F : IFunctionList<f32, i32, F>
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

        static abstract f32 Load(ReadOnlySpan<float> p);
        static abstract i32 Load(ReadOnlySpan<int> p);

        // Incremented

        static abstract f32 Incremented_f32();
        static abstract i32 Incremented_i32();

        // Store

        static abstract void Store(ref float p, f32 a);
        static abstract void Store(ref int p, i32 a);

        static abstract void Store(ref float p, nuint elementOffset, f32 a);
        static abstract void Store(ref int p, nuint elementOffset, i32 a);

        static abstract void Store(Span<float> p, f32 a);
        static abstract void Store(Span<int> p, i32 a);

        // Extract

        static abstract float Extract0(f32 a);
        static abstract int Extract0(i32 a);

        static abstract float Extract(f32 a, int idx);
        static abstract int Extract(i32 a, int idx);

        // Cast

        static abstract f32 Cast_f32(i32 a);
        static abstract i32 Cast_i32(f32 a);

        // Convert

        static abstract f32 Convert_f32(i32 a);
        static abstract i32 Convert_i32(f32 a);

        // Select

        static abstract f32 Select(f32 m, f32 a, f32 b);
        static abstract i32 Select(i32 m, i32 a, i32 b);

        // Min

        static abstract f32 Min(f32 a, f32 b);
        static abstract i32 Min(i32 a, i32 b);

        // Max

        static abstract f32 Max(f32 a, f32 b);
        static abstract i32 Max(i32 a, i32 b);

        // Bitwise

        static abstract f32 AndNot(f32 a, f32 b);
        static abstract i32 AndNot(i32 a, i32 b);

        static abstract f32 ShiftRightLogical(f32 a, [ConstantExpected] byte b);
        static abstract i32 ShiftRightLogical(i32 a, [ConstantExpected] byte b);

        // Abs

        static abstract f32 Abs(f32 a);
        static abstract i32 Abs(i32 a);

        // Float math

        static abstract f32 Sqrt(f32 a);
        static abstract f32 ReciprocalSqrt(f32 a);

        static abstract f32 Reciprocal(f32 a);

        // Rounding

        static abstract f32 Floor(f32 a);
        static abstract f32 Ceiling(f32 a);
        static abstract f32 Round(f32 a);

        // Mask

        static abstract i32 Mask(i32 a, i32 m);
        static abstract f32 Mask(f32 a, f32 m);

        static abstract i32 NMask(i32 a, i32 m);
        static abstract f32 NMask(f32 a, f32 m);

        static abstract bool AnyMask(i32 m);
        static abstract bool AllMask(i32 m);

        // Masked float

        static virtual f32 MaskAdd(f32 a, f32 b, f32 m) => F.Add(a, F.Mask(b, m));
        static virtual f32 MaskSub(f32 a, f32 b, f32 m) => F.Sub(a, F.Mask(b, m));
        static virtual f32 MaskMul(f32 a, f32 b, f32 m) => F.Mul(a, F.Mask(b, m));

        // NMasked float

        static virtual f32 NMaskAdd(f32 a, f32 b, f32 m) => F.Add(a, F.NMask(b, m));
        static virtual f32 NMaskSub(f32 a, f32 b, f32 m) => F.Sub(a, F.NMask(b, m));
        static virtual f32 NMaskMul(f32 a, f32 b, f32 m) => F.Mul(a, F.NMask(b, m));

        // Masked int32

        static virtual i32 MaskAdd(i32 a, i32 b, i32 m) => F.Add(a, F.Mask(b, m));
        static virtual i32 MaskSub(i32 a, i32 b, i32 m) => F.Sub(a, F.Mask(b, m));
        static virtual i32 MaskMul(i32 a, i32 b, i32 m) => F.Mul(a, F.Mask(b, m));

        static virtual i32 MaskIncrement(i32 a, i32 m) => F.MaskSub(a, F.Broad(-1), m);
        static virtual i32 MaskDecrement(i32 a, i32 m) => F.MaskAdd(a, F.Broad(-1), m);

        // NMasked int32

        static virtual i32 NMaskAdd(i32 a, i32 b, i32 m) => F.Add(a, F.NMask(b, m));
        static virtual i32 NMaskSub(i32 a, i32 b, i32 m) => F.Sub(a, F.NMask(b, m));
        static virtual i32 NMaskMul(i32 a, i32 b, i32 m) => F.Mul(a, F.NMask(b, m));

        // FMA

        static abstract f32 FMulAdd(f32 a, f32 b, f32 c);
        static abstract f32 FNMulAdd(f32 a, f32 b, f32 c);

        // Float math

        static abstract f32 Add(f32 lhs, f32 rhs);
        static abstract f32 And(f32 lhs, f32 rhs);
        static abstract f32 Div(f32 lhs, f32 rhs);
        static abstract f32 Not(f32 lhs);
        static abstract f32 Equal(f32 lhs, f32 rhs);
        static abstract f32 GreaterThan(f32 lhs, f32 rhs);
        static abstract f32 GreaterThanOrEqual(f32 lhs, f32 rhs);
        static abstract f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs);
        static abstract f32 LessThan(f32 lhs, f32 rhs);
        static abstract f32 LessThanOrEqual(f32 lhs, f32 rhs);
        static abstract f32 Mul(f32 lhs, f32 rhs);
        static abstract f32 Negate(f32 lhs);
        static abstract f32 NotEqual(f32 lhs, f32 rhs);
        static abstract f32 Or(f32 lhs, f32 rhs);
        static abstract f32 RightShift(f32 lhs, [ConstantExpected] byte rhs);
        static abstract f32 Sub(f32 lhs, f32 rhs);
        static abstract f32 Xor(f32 lhs, f32 rhs);

        // Int math

        static abstract i32 Add(i32 lhs, i32 rhs);
        static abstract i32 And(i32 lhs, i32 rhs);
        static abstract i32 Div(i32 lhs, i32 rhs);
        static abstract i32 Not(i32 lhs);
        static abstract i32 Equal(i32 lhs, i32 rhs);
        static abstract i32 GreaterThan(i32 lhs, i32 rhs);
        static abstract i32 GreaterThanOrEqual(i32 lhs, i32 rhs);
        static abstract i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs);
        static abstract i32 LessThan(i32 lhs, i32 rhs);
        static abstract i32 LessThanOrEqual(i32 lhs, i32 rhs);
        static abstract i32 Mul(i32 lhs, i32 rhs);
        static abstract i32 Negate(i32 lhs);
        static abstract i32 NotEqual(i32 lhs, i32 rhs);
        static abstract i32 Or(i32 lhs, i32 rhs);
        static abstract i32 RightShift(i32 lhs, [ConstantExpected] byte rhs);
        static abstract i32 Sub(i32 lhs, i32 rhs);
        static abstract i32 Xor(i32 lhs, i32 rhs);
    }
}
