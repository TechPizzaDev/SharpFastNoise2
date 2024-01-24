
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator4D<F, I> : INoiseGenerator4D
    {
        public F Gen(F x, F y, F z, F w, I seed);
    }
}
