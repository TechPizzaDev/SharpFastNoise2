namespace SharpFastNoise2
{
    public interface IFVector<T, M>
        where T : IFVector<T, M>
        where M : IFMask<M>
    {
        int Count { get; }

        T Add(T rhs);
        T And(T rhs);
        T Div(T rhs);
        T Complement();
        M Equal(T other);
        M GreaterThan(T rhs);
        M GreaterThanOrEqual(T rhs);
        T Incremented();
        T LeftShift(byte rhs);
        M LessThan(T rhs);
        M LessThanOrEqual(T rhs);
        T Mul(T rhs);
        T Negate();
        M NotEqual(T other);
        T Or(T rhs);
        T RightShift(byte rhs);
        T Sub(T rhs);
        T Xor(T rhs);
    }
}
