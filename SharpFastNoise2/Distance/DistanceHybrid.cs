using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public struct DistanceHybrid<f32, i32, F> : IDistanceFunction<f32, i32, F>
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<f32, i32, F>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 both = F.FMulAdd(dX, dX, F.Abs(dX));
            both = F.Add(both, F.FMulAdd(dY, dY, F.Abs(dY)));
            return both;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 both = F.FMulAdd(dX, dX, F.Abs(dX));
            both = F.Add(both, F.FMulAdd(dY, dY, F.Abs(dY)));
            both = F.Add(both, F.FMulAdd(dZ, dZ, F.Abs(dZ)));
            return both;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 both = F.FMulAdd(dX, dX, F.Abs(dX));
            both = F.Add(both, F.FMulAdd(dY, dY, F.Abs(dY)));
            both = F.Add(both, F.FMulAdd(dZ, dZ, F.Abs(dZ)));
            both = F.Add(both, F.FMulAdd(dW, dW, F.Abs(dW)));
            return both;
        }
    }
}
