namespace SharpFastNoise2
{
    public interface IFMask<T>
        where T : IFMask<T>
    {
        int Count { get; }

        T And(T rhs);
        T Complement();
        T Or(T rhs);
    }
}
