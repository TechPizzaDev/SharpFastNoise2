using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public readonly struct FVectorI256 : IFMask<FVectorI256>, IFVector<FVectorI256, FVectorI256>
    {
        public readonly Vector256<int> Value;

        public FVectorI256(Vector256<int> value)
        {
            Value = value;
        }

        public FVectorI256(int value)
        {
            Value = Vector256.Create(value);
        }

        public FVectorI256(int e0, int e1, int e2, int e3, int e4, int e5, int e6, int e7)
        {
            Value = Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7);
        }

        public int Count => Vector256<int>.Count;

        public FVectorI256 Add(FVectorI256 rhs) => Avx2.Add(Value, rhs.Value);

        public FVectorI256 And(FVectorI256 rhs) => Avx2.And(Value, rhs.Value);

        public FVectorF256 AsSingle() => Unsafe.As<FVectorI256, FVectorF256>(ref Unsafe.AsRef(this));

        public FVectorI256 Complement() => Avx2.Xor(Value, Vector256<int>.AllBitsSet);

        public FVectorI256 Div(FVectorI256 rhs) => throw new NotSupportedException();

        public FVectorI256 Equal(FVectorI256 rhs) => Avx2.CompareEqual(Value, rhs.Value);

        public FVectorI256 GreaterThan(FVectorI256 rhs) => Avx2.CompareGreaterThan(Value, rhs.Value);

        public FVectorI256 GreaterThanOrEqual(FVectorI256 rhs) => throw new NotSupportedException();

        public FVectorI256 LeftShift(byte rhs) => Avx2.ShiftLeftLogical(Value, rhs);

        public FVectorI256 LessThan(FVectorI256 rhs) => Avx2.CompareGreaterThan(rhs.Value, Value);

        public FVectorI256 LessThanOrEqual(FVectorI256 rhs) => throw new NotSupportedException();

        public FVectorI256 Mul(FVectorI256 rhs) => Avx2.MultiplyLow(Value, rhs.Value);

        public FVectorI256 Negate() => Avx2.Subtract(Vector256<int>.Zero, Value);

        public FVectorI256 NotEqual(FVectorI256 other) => AsSingle().NotEqual(other.AsSingle());

        public FVectorI256 Or(FVectorI256 rhs) => Avx2.Or(Value, rhs.Value);

        public FVectorI256 RightShift(byte rhs) => Avx2.ShiftRightArithmetic(Value, rhs);

        public FVectorI256 Sub(FVectorI256 rhs) => Avx2.Subtract(Value, rhs.Value);

        public FVectorI256 Xor(FVectorI256 rhs) => Avx2.Xor(Value, rhs.Value);

        public static implicit operator Vector256<int>(FVectorI256 vector) => vector.Value;

        public static implicit operator FVectorI256(Vector256<int> vector) => new(vector);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
