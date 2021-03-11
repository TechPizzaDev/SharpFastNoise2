using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public readonly struct FVectorF128 : IFVector<FVectorF128>
    {
        public readonly Vector128<float> Value;

        public FVectorF128(Vector128<float> value)
        {
            Value = value;
        }

        public int Count => Vector128<float>.Count;

        public FVectorF128 Add(FVectorF128 rhs) => new(Sse.Add(Value, rhs.Value));

        public FVectorF128 And(FVectorF128 rhs) => new(Sse.And(Value, rhs.Value));

        public FVectorI128 AsInt32() => UnsafeR.As<FVectorF128, FVectorI128>(this);

        public FVectorF128 Complement() => new(Sse.Xor(Value, Vector128<float>.AllBitsSet));

        public FVectorF128 Div(FVectorF128 rhs) => throw new NotSupportedException();

        public FVectorF128 Equal(FVectorF128 other) => new(Sse.CompareEqual(Value, other.Value));

        public FVectorF128 GreaterThan(FVectorF128 rhs) => new(Sse.CompareGreaterThan(Value, rhs.Value));

        public FVectorF128 GreaterThanOrEqual(FVectorF128 rhs) => new(Sse.CompareGreaterThanOrEqual(Value, rhs.Value));

        public FVectorF128 Incremented() => new(Vector128.Create(0f, 1, 2, 3));

        public FVectorF128 LeftShift(byte rhs) => throw new NotSupportedException();

        public FVectorF128 LessThan(FVectorF128 rhs) => new(Sse.CompareLessThan(Value, rhs.Value));

        public FVectorF128 LessThanOrEqual(FVectorF128 rhs) => new(Sse.CompareLessThanOrEqual(Value, rhs.Value));

        public FVectorF128 Mul(FVectorF128 rhs) => new(Sse.Multiply(Value, rhs.Value));

        public FVectorF128 Negate() => new(Sse.Xor(Value, Vector128.Create(0x80000000).AsSingle()));

        public FVectorF128 NotEqual(FVectorF128 other) => new(Sse.CompareNotEqual(Value, other.Value));

        public FVectorF128 Or(FVectorF128 rhs) => new(Sse.Or(Value, rhs.Value));

        public FVectorF128 RightShift(byte rhs) => throw new NotSupportedException();

        public FVectorF128 Sub(FVectorF128 rhs) => new(Sse.Subtract(Value, rhs.Value));

        public FVectorF128 Xor(FVectorF128 rhs) => new(Sse.Xor(Value, rhs.Value));
    }
}
