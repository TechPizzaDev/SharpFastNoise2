
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator4D<f, i> : INoiseGenerator
    {
        public f Gen(f x, f y, f z, f w, i seed);
    }
}
