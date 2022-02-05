
namespace SharpFastNoise2
{
    public interface INoiseGenerator3D<f, i> : INoiseGenerator
    {
        public f Gen(i seed, f x, f y, f z);
    }
}
