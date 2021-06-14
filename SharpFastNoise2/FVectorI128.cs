using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    public readonly struct FVectorI128 : IFMask<FVectorI128>, IFVector<FVectorI128, FVectorI128>
    {
        public readonly Vector128<int> Value;

        public FVectorI128(Vector128<int> value)
        {
            Value = value;
        }

        public FVectorI128(int value)
        {
            Value = Vector128.Create(value);
        }

        public FVectorI128(int x, int y, int z, int w)
        {
            Value = Vector128.Create(x, y, z, w);
        }

        public int Count => Vector128<int>.Count;

        public FVectorI128 Add(FVectorI128 rhs) => new(Sse2.Add(Value, rhs.Value));

        public FVectorI128 And(FVectorI128 rhs) => new(Sse2.And(Value, rhs.Value));

        public FVectorF128 AsSingle() => UnsafeR.As<FVectorI128, FVectorF128>(this);

        public FVectorI128 Complement() => new(Sse2.Xor(Value, Vector128<int>.AllBitsSet));

        public FVectorI128 Div(FVectorI128 rhs) => throw new NotSupportedException();

        public FVectorI128 Equal(FVectorI128 rhs) => new(Sse2.CompareEqual(Value, rhs.Value));

        public FVectorI128 GreaterThan(FVectorI128 rhs) => new(Sse2.CompareGreaterThan(Value, rhs.Value));

        public FVectorI128 GreaterThanOrEqual(FVectorI128 rhs) => throw new NotSupportedException();

        public FVectorI128 Incremented() => new(Vector128.Create(0, 1, 2, 3));

        public FVectorI128 LeftShift(byte rhs) => new(Sse2.ShiftLeftLogical(Value, rhs));

        public FVectorI128 LessThan(FVectorI128 rhs) => new(Sse2.CompareLessThan(Value, rhs.Value));

        public FVectorI128 LessThanOrEqual(FVectorI128 rhs) => throw new NotSupportedException();

        public FVectorI128 Mul(FVectorI128 rhs)
        {
            if (Sse41.IsSupported)
            {
                return new(Sse41.MultiplyLow(Value, rhs.Value));
            }
            else
            {
                var tmp1 = Sse2.Multiply(Value.AsUInt32(), rhs.Value.AsUInt32()); // mul 2,0
                var tmp2 = Sse2.Multiply(
                    Sse2.ShiftRightLogical128BitLane(Value, 4).AsUInt32(),
                    Sse2.ShiftRightLogical128BitLane(rhs.Value, 4).AsUInt32()); // mul 3,1

                const byte control = 8; // _MM_SHUFFLE(0,0,2,0)
                return new(Sse2.UnpackLow(
                    Sse2.Shuffle(tmp1.AsInt32(), control),
                    Sse2.Shuffle(tmp2.AsInt32(), control))); // shuffle results to [63..0] and pack
            }
        }

        public FVectorI128 Negate() => new(Sse2.Subtract(Vector128<int>.Zero, Value));

        public FVectorI128 NotEqual(FVectorI128 other) => AsSingle().NotEqual(other.AsSingle());

        public FVectorI128 Or(FVectorI128 rhs) => new(Sse2.Or(Value, rhs.Value));

        public FVectorI128 RightShift(byte rhs) => new(Sse2.ShiftRightArithmetic(Value, rhs));

        public FVectorI128 Sub(FVectorI128 rhs) => new(Sse2.Subtract(Value, rhs.Value));

        public FVectorI128 Xor(FVectorI128 rhs) => new(Sse2.Xor(Value, rhs.Value));

        public static implicit operator Vector128<int>(FVectorI128 vector) => vector.Value;

        public static implicit operator FVectorI128(Vector128<int> vector) => new(vector);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
