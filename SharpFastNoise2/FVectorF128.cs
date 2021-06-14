using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public readonly struct FVectorF128 : IFVector<FVectorF128, FVectorI128>
    {
        public readonly Vector128<float> Value;

        public FVectorF128(Vector128<float> value)
        {
            Value = value;
        }

        public FVectorF128(float value)
        {
            Value = Vector128.Create(value);
        }

        public FVectorF128(float x, float y, float z, float w)
        {
            Value = Vector128.Create(x, y, z, w);
        }

        public int Count => Vector128<float>.Count;

        public FVectorF128 Add(FVectorF128 rhs) => new(Sse.Add(Value, rhs.Value));

        public FVectorF128 And(FVectorF128 rhs) => new(Sse.And(Value, rhs.Value));

        public FVectorI128 AsInt32() => Unsafe.As<FVectorF128, FVectorI128>(ref Unsafe.AsRef(this));

        public FVectorF128 Complement() => new(Sse.Xor(Value, Vector128<float>.AllBitsSet));

        public FVectorF128 Div(FVectorF128 rhs) => throw new NotSupportedException();

        public FVectorI128 Equal(FVectorF128 rhs) => new(Sse.CompareEqual(Value, rhs.Value).AsInt32());

        public FVectorI128 GreaterThan(FVectorF128 rhs) => new(Sse.CompareGreaterThan(Value, rhs.Value).AsInt32());

        public FVectorI128 GreaterThanOrEqual(FVectorF128 rhs) => new(Sse.CompareGreaterThanOrEqual(Value, rhs.Value).AsInt32());

        public FVectorF128 Incremented() => new(Vector128.Create(0f, 1, 2, 3));

        public FVectorF128 LeftShift(byte rhs) => throw new NotSupportedException();

        public FVectorI128 LessThan(FVectorF128 rhs) => new(Sse.CompareLessThan(Value, rhs.Value).AsInt32());

        public FVectorI128 LessThanOrEqual(FVectorF128 rhs) => new(Sse.CompareLessThanOrEqual(Value, rhs.Value).AsInt32());

        public FVectorF128 Mul(FVectorF128 rhs) => new(Sse.Multiply(Value, rhs.Value));

        public FVectorF128 Negate() => new(Sse.Xor(Value, Vector128.Create(0x80000000).AsSingle()));

        public FVectorI128 NotEqual(FVectorF128 rhs) => new(Sse.CompareNotEqual(Value, rhs.Value).AsInt32());

        public FVectorF128 Or(FVectorF128 rhs) => new(Sse.Or(Value, rhs.Value));

        public FVectorF128 RightShift(byte rhs) => throw new NotSupportedException();

        public FVectorF128 Sub(FVectorF128 rhs) => new(Sse.Subtract(Value, rhs.Value));

        public FVectorF128 Xor(FVectorF128 rhs) => new(Sse.Xor(Value, rhs.Value));

        public static implicit operator Vector128<float>(FVectorF128 vector) => vector.Value;

        public static implicit operator FVectorF128(Vector128<float> vector) => new(vector);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
