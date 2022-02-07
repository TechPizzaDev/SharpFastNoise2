using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct DistanceEuclideanSquared<m32, f32, i32, F> : IDistanceFunction<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 distSqr = F.Mul(dX, dX);
            distSqr = F.FMulAdd_f32(dY, dY, distSqr);
            return distSqr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 distSqr = F.Mul(dX, dX);
            distSqr = F.FMulAdd_f32(dY, dY, distSqr);
            distSqr = F.FMulAdd_f32(dZ, dZ, distSqr);
            return distSqr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 distSqr = F.Mul(dX, dX);
            distSqr = F.FMulAdd_f32(dY, dY, distSqr);
            distSqr = F.FMulAdd_f32(dZ, dZ, distSqr);
            distSqr = F.FMulAdd_f32(dW, dW, distSqr);
            return distSqr;
        }
    }
}
