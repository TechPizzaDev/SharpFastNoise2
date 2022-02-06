
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator2D<f, i> : INoiseGenerator2D
    {
        public f Gen(f x, f y, i seed);
    }
}
