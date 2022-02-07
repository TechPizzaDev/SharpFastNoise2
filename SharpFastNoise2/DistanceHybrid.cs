using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct DistanceHybrid<m32, f32, i32, F> : IDistanceFunction<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 both = F.FMulAdd_f32(dX, dX, F.Abs_f32(dX));
            both = F.Add(both, F.FMulAdd_f32(dY, dY, F.Abs_f32(dY)));
            return both;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 both = F.FMulAdd_f32(dX, dX, F.Abs_f32(dX));
            both = F.Add(both, F.FMulAdd_f32(dY, dY, F.Abs_f32(dY)));
            both = F.Add(both, F.FMulAdd_f32(dZ, dZ, F.Abs_f32(dZ)));
            return both;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 both = F.FMulAdd_f32(dX, dX, F.Abs_f32(dX));
            both = F.Add(both, F.FMulAdd_f32(dY, dY, F.Abs_f32(dY)));
            both = F.Add(both, F.FMulAdd_f32(dZ, dZ, F.Abs_f32(dZ)));
            both = F.Add(both, F.FMulAdd_f32(dW, dW, F.Abs_f32(dW)));
            return both;
        }
    }
}
