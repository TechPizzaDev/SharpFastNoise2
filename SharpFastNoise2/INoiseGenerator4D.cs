
namespace SharpFastNoise2
{
    public interface INoiseGenerator4D<float32v, int32v> : INoiseGenerator
    {
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z, float32v w);
    }
}
