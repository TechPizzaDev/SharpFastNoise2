using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public struct DistanceMaxAxis<f32, i32, F> : IDistanceFunction<f32, i32, F>
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<f32, i32, F>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 max = F.Abs(dX);
            max = F.Max(F.Abs(dY), max);
            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 max = F.Abs(dX);
            max = F.Max(F.Abs(dY), max);
            max = F.Max(F.Abs(dZ), max);
            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 max = F.Abs(dX);
            max = F.Max(F.Abs(dY), max);
            max = F.Max(F.Abs(dZ), max);
            max = F.Max(F.Abs(dW), max);
            return max;
        }
    }
}
