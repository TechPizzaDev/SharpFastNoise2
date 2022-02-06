
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator3D<f, i> : INoiseGenerator3D
    {
        public f Gen(f x, f y, f z, i seed);
    }
}
