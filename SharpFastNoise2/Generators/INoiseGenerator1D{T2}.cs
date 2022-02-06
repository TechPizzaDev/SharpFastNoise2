
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator1D<f, i> : INoiseGenerator1D
    {
        public f Gen(f x, i seed);
    }
}
