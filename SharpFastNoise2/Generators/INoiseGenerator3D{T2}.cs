
namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator3D<F, I> : INoiseGenerator3D
    {
        public F Gen(F x, F y, F z, I seed);
    }
}
