
namespace SharpFastNoise2
{
    public interface INoiseGenerator4D<f, i> : INoiseGenerator
    {
        public f Gen(i seed, f x, f y, f z, f w);
    }
}
