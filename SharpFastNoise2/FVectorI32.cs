namespace SharpFastNoise2
{
    public readonly struct FVectorI32 : IFMask<FVectorI32>, IFVector<FVectorI32, FVectorI32>
    {
        public readonly int Value;

        public FVectorI32(int value)
        {
            Value = value;
        }

        public int Count => 1;

        public FVectorI32 Add(FVectorI32 rhs) => new(Value + rhs.Value);

        public FVectorI32 And(FVectorI32 rhs) => new(Value & rhs.Value);

        public FVectorF32 AsSingle() => UnsafeR.As<FVectorI32, FVectorF32>(this);

        public FVectorI32 Complement() => new(~Value);

        public FVectorI32 Div(FVectorI32 rhs) => new(Value / rhs.Value);

        public FVectorI32 Equal(FVectorI32 rhs) => new((Value == rhs.Value).AsInt32());

        public FVectorI32 GreaterThan(FVectorI32 rhs) => new((Value > rhs.Value).AsInt32());

        public FVectorI32 GreaterThanOrEqual(FVectorI32 rhs) => new((Value >= rhs.Value).AsInt32());

        public FVectorI32 Incremented() => new(0);

        public FVectorI32 LeftShift(byte rhs) => new(Value << rhs);

        public FVectorI32 LessThan(FVectorI32 rhs) => new((Value < rhs.Value).AsInt32());

        public FVectorI32 LessThanOrEqual(FVectorI32 rhs) => new((Value <= rhs.Value).AsInt32());

        public FVectorI32 Mul(FVectorI32 rhs) => new(Value * rhs.Value);

        public FVectorI32 Negate() => new(-Value);

        public FVectorI32 NotEqual(FVectorI32 rhs) => new((Value != rhs.Value).AsInt32());

        public FVectorI32 Or(FVectorI32 rhs) => new(Value | rhs.Value);

        public FVectorI32 RightShift(byte rhs) => new(Value >> rhs);

        public FVectorI32 Sub(FVectorI32 rhs) => new(Value - rhs.Value);

        public FVectorI32 Xor(FVectorI32 rhs) => new(Value ^ rhs.Value);

        public static implicit operator int(FVectorI32 vector) => vector.Value;

        public static implicit operator FVectorI32(int vector) => new(vector);

        public override string ToString()
        {
            return $"<{Value}>";
        }
    }
}
