using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct DistanceManhattan<m32, f32, i32, F> : IDistanceFunction<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 dist = F.Abs_f32(dX);
            dist = F.Add(dist, F.Abs_f32(dY));
            return dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 dist = F.Abs_f32(dX);
            dist = F.Add(dist, F.Abs_f32(dY));
            dist = F.Add(dist, F.Abs_f32(dZ));
            return dist;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 dist = F.Abs_f32(dX);
            dist = F.Add(dist, F.Abs_f32(dY));
            dist = F.Add(dist, F.Abs_f32(dZ));
            dist = F.Add(dist, F.Abs_f32(dW));
            return dist;
        }
    }
}
