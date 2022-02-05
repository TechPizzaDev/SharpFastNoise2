
namespace SharpFastNoise2
{
    public interface INoiseGenerator2D<f, i> : INoiseGenerator
    {
        public f Gen(i seed, f x, f y);
    }
}
