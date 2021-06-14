
namespace SharpFastNoise2
{
    public interface INoiseGenerator3D<mask32v, float32v, int32v> : INoiseGenerator
        where mask32v : unmanaged, IFMask<mask32v>
        where float32v : unmanaged, IFVector<float32v, mask32v>
        where int32v : unmanaged, IFVector<int32v, mask32v>
    {
        public float32v Gen(int32v seed, float32v x, float32v y, float32v z);
    }
}
