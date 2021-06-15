using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public readonly struct FVectorF256 : IFVector<FVectorF256, FVectorI256>
    {
        public readonly Vector256<float> Value;

        public FVectorF256(Vector256<float> value)
        {
            Value = value;
        }

        public FVectorF256(float value)
        {
            Value = Vector256.Create(value);
        }

        public FVectorF256(float e0, float e1, float e2, float e3, float e4, float e5, float e6, float e7)
        {
            Value = Vector256.Create(e0, e1, e2, e3, e4, e5, e6, e7);
        }

        public int Count => Vector256<float>.Count;

        public FVectorF256 Add(FVectorF256 rhs) => Avx.Add(Value, rhs.Value);

        public FVectorF256 And(FVectorF256 rhs) => Avx.And(Value, rhs.Value);

        public FVectorI256 AsInt32() => Unsafe.As<FVectorF256, FVectorI256>(ref Unsafe.AsRef(this));

        public FVectorF256 Complement() => Avx.Xor(Value, Vector256<float>.AllBitsSet);

        public FVectorF256 Div(FVectorF256 rhs) => Avx.Divide(Value, rhs.Value);

        public FVectorI256 Equal(FVectorF256 rhs) => Avx.CompareEqual(Value, rhs.Value).AsInt32();

        public FVectorI256 GreaterThan(FVectorF256 rhs) => Avx.CompareGreaterThan(Value, rhs.Value).AsInt32();

        public FVectorI256 GreaterThanOrEqual(FVectorF256 rhs) => Avx.CompareGreaterThanOrEqual(Value, rhs.Value).AsInt32();

        public FVectorF256 LeftShift(byte rhs) => throw new NotSupportedException();

        public FVectorI256 LessThan(FVectorF256 rhs) => Avx.CompareLessThan(Value, rhs.Value).AsInt32();

        public FVectorI256 LessThanOrEqual(FVectorF256 rhs) => Avx.CompareLessThanOrEqual(Value, rhs.Value).AsInt32();

        public FVectorF256 Mul(FVectorF256 rhs) => Avx.Multiply(Value, rhs.Value);

        public FVectorF256 Negate() => Avx.Xor(Value, Vector256.Create(0x80000000).AsSingle());

        public FVectorI256 NotEqual(FVectorF256 rhs) => Avx.CompareNotEqual(Value, rhs.Value).AsInt32();

        public FVectorF256 Or(FVectorF256 rhs) => Avx.Or(Value, rhs.Value);

        public FVectorF256 RightShift(byte rhs) => throw new NotSupportedException();

        public FVectorF256 Sub(FVectorF256 rhs) => Avx.Subtract(Value, rhs.Value);

        public FVectorF256 Xor(FVectorF256 rhs) => Avx.Xor(Value, rhs.Value);

        public static implicit operator Vector256<float>(FVectorF256 vector) => vector.Value;

        public static implicit operator FVectorF256(Vector256<float> vector) => new(vector);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
