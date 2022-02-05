
namespace SharpFastNoise2
{
    public interface INoiseGenerator1D<f, i> : INoiseGenerator
    {
        public f Gen(i seed, f x);
    }
}
