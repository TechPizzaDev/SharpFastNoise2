
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator4D<F, I> : INoiseGenerator4D, INoiseGeneratorAbstract
    {
        public F Gen(F x, F y, F z, F w, I seed);
    }
}
