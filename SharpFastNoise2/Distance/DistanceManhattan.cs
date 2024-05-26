using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public struct DistanceManhattan<m32, f32, i32, F> : IDistanceFunction<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32, F>
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
