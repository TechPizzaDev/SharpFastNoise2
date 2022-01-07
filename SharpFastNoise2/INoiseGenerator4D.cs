
namespace SharpFastNoise2
{
    public interface INoiseGenerator4D<f32, i32> : INoiseGenerator
    {
        public f32 Gen(i32 seed, f32 x, f32 y, f32 z, f32 w);
    }
}
