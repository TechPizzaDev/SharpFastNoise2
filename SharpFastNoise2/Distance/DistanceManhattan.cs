using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public struct DistanceManhattan<f32, i32, F> : IDistanceFunction<f32, i32, F>
        where F : IFunctionList<f32, i32, F>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 dist = F.Abs(dX);
            dist = F.Add(dist, F.Abs(dY));
            return dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 dist = F.Abs(dX);
            dist = F.Add(dist, F.Abs(dY));
            dist = F.Add(dist, F.Abs(dZ));
            return dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 dist = F.Abs(dX);
            dist = F.Add(dist, F.Abs(dY));
            dist = F.Add(dist, F.Abs(dZ));
            dist = F.Add(dist, F.Abs(dW));
            return dist;
        }
    }
}
