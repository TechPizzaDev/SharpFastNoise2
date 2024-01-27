
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator2D<F, I> : INoiseGenerator2D, INoiseGeneratorAbstract
    {
        public F Gen(F x, F y, I seed);
    }
}
