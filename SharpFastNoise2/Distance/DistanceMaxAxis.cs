using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public struct DistanceMaxAxis<m32, f32, i32, F> : IDistanceFunction<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32, F>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 max = F.Abs_f32(dX);
            max = F.Max(F.Abs_f32(dY), max);
            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 max = F.Abs_f32(dX);
            max = F.Max(F.Abs_f32(dY), max);
            max = F.Max(F.Abs_f32(dZ), max);
            return max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 max = F.Abs_f32(dX);
            max = F.Max(F.Abs_f32(dY), max);
            max = F.Max(F.Abs_f32(dZ), max);
            max = F.Max(F.Abs_f32(dW), max);
            return max;
        }
    }
}
