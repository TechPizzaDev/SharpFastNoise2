using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public readonly struct FVectorF32 : IFVector<FVectorF32, FVectorI32>
    {
        public readonly float Value;

        public FVectorF32(float value)
        {
            Value = value;
        }

        public int Count => 1;

        public FVectorF32 Add(FVectorF32 rhs) => new(Value + rhs.Value);

        public FVectorF32 And(FVectorF32 rhs) => AsInt32().And(rhs.AsInt32()).AsSingle();

        public FVectorI32 AsInt32() => Unsafe.As<FVectorF32, FVectorI32>(ref Unsafe.AsRef(this));

        public FVectorF32 Complement() => AsInt32().Complement().AsSingle();

        public FVectorF32 Div(FVectorF32 rhs) => new(Value / rhs.Value);

        public FVectorI32 Equal(FVectorF32 rhs) => new((Value == rhs.Value).AsInt32());

        public FVectorI32 GreaterThan(FVectorF32 rhs) => new((Value > rhs.Value).AsInt32());

        public FVectorI32 GreaterThanOrEqual(FVectorF32 rhs) => new((Value >= rhs.Value).AsInt32());

        public FVectorF32 LeftShift(byte rhs) => AsInt32().LeftShift(rhs).AsSingle();

        public FVectorI32 LessThan(FVectorF32 rhs) => new((Value < rhs.Value).AsInt32());

        public FVectorI32 LessThanOrEqual(FVectorF32 rhs) => new((Value <= rhs.Value).AsInt32());

        public FVectorF32 Mul(FVectorF32 rhs) => new(Value * rhs.Value);

        public FVectorF32 Negate() => new(-Value);

        public FVectorI32 NotEqual(FVectorF32 rhs) => new((Value != rhs.Value).AsInt32());

        public FVectorF32 Or(FVectorF32 rhs) => AsInt32().Or(rhs.AsInt32()).AsSingle();

        public FVectorF32 RightShift(byte rhs) => AsInt32().RightShift(rhs).AsSingle();

        public FVectorF32 Sub(FVectorF32 rhs) => new(Value - rhs.Value);

        public FVectorF32 Xor(FVectorF32 rhs) => AsInt32().Xor(rhs.AsInt32()).AsSingle();

        public static implicit operator float(FVectorF32 vector) => vector.Value;

        public static implicit operator FVectorF32(float vector) => new(vector);

        public override string ToString()
        {
            return $"<{Value}>";
        }
    }
}
