
namespace SharpFastNoise2
{
    public interface INoiseGenerator2D<float32v, int32v> : INoiseGenerator
    {
        public float32v Gen(int32v seed, float32v x, float32v y);
    }
}
