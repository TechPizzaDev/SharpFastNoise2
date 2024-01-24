
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator2D<F, I> : INoiseGenerator2D
    {
        public F Gen(F x, F y, I seed);
    }
}
