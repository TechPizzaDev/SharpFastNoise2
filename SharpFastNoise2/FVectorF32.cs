namespace SharpFastNoise2
{
    public readonly struct FVectorF32 : IFVector<FVectorF32>
    {
        public readonly float Value;

        public FVectorF32(float value)
        {
            Value = value;
        }

        public int Count => 1;

        public FVectorF32 Add(FVectorF32 rhs) => new(Value + rhs.Value);

        public FVectorF32 And(FVectorF32 rhs) => AsInt32().And(rhs.AsInt32()).AsSingle();

        public FVectorI32 AsInt32() => UnsafeR.As<FVectorF32, FVectorI32>(this);

        public FVectorF32 Complement() => AsInt32().Complement().AsSingle();

        public FVectorF32 Div(FVectorF32 rhs) => new(Value / rhs.Value);

        public FVectorF32 Equal(FVectorF32 other) => new((Value == other.Value).AsSingle());

        public FVectorF32 GreaterThan(FVectorF32 rhs) => new((Value > rhs.Value).AsSingle());

        public FVectorF32 GreaterThanOrEqual(FVectorF32 rhs) => new((Value >= rhs.Value).AsSingle());

        public FVectorF32 Incremented() => new(0);

        public FVectorF32 LeftShift(byte rhs) => AsInt32().LeftShift(rhs).AsSingle();

        public FVectorF32 LessThan(FVectorF32 rhs) => new((Value < rhs.Value).AsSingle());

        public FVectorF32 LessThanOrEqual(FVectorF32 rhs) => new((Value <= rhs.Value).AsSingle());

        public FVectorF32 Mul(FVectorF32 rhs) => new(Value * rhs.Value);

        public FVectorF32 Negate() => new(-Value);

        public FVectorF32 NotEqual(FVectorF32 other) => new((Value != other.Value).AsSingle());

        public FVectorF32 Or(FVectorF32 rhs) => AsInt32().Or(rhs.AsInt32()).AsSingle();

        public FVectorF32 RightShift(byte rhs) => AsInt32().RightShift(rhs).AsSingle();

        public FVectorF32 Sub(FVectorF32 rhs) => new(Value - rhs.Value);

        public FVectorF32 Xor(FVectorF32 rhs) => AsInt32().Xor(rhs.AsInt32()).AsSingle();
    }
}
