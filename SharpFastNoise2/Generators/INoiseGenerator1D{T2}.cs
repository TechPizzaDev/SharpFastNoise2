
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator1D<F, I> : INoiseGenerator1D
    {
        public F Gen(F x, I seed);
    }
}
