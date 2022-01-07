
namespace SharpFastNoise2
{
    public interface INoiseGenerator1D<f32, i32> : INoiseGenerator
    {
        public f32 Gen(i32 seed, f32 x);
    }
}
