using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public interface IDistanceFunction<f32, i32, F>
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<f32, i32, F>
    {
        static abstract f32 CalcDistance(f32 dX, f32 dY);

        static abstract f32 CalcDistance(f32 dX, f32 dY, f32 dZ);

        static abstract f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW);
    }
}
