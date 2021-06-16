
namespace SharpFastNoise2
{
    public interface INoiseGenerator1D<float32v, int32v> : INoiseGenerator
    {
        public float32v Gen(int32v seed, float32v x);
    }
}
