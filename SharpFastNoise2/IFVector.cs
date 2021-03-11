namespace SharpFastNoise2
{
    public interface IFVector<T>
        where T : IFVector<T>
    {
        int Count { get; }

        T Add(T rhs);
        T And(T rhs);
        T Div(T rhs);
        T Complement();
        T Equal(T other);
        T GreaterThan(T rhs);
        T GreaterThanOrEqual(T rhs);
        T Incremented();
        T LeftShift(byte rhs);
        T LessThan(T rhs);
        T LessThanOrEqual(T rhs);
        T Mul(T rhs);
        T Negate();
        T NotEqual(T other);
        T Or(T rhs);
        T RightShift(byte rhs);
        T Sub(T rhs);
        T Xor(T rhs);
    }
}
