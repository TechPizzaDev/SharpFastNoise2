using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public struct DistanceEuclideanSquared<f32, i32, F> : IDistanceFunction<f32, i32, F>
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<f32, i32, F>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY)
        {
            f32 distSqr = F.Mul(dX, dX);
            distSqr = F.FMulAdd(dY, dY, distSqr);
            return distSqr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ)
        {
            f32 distSqr = F.Mul(dX, dX);
            distSqr = F.FMulAdd(dY, dY, distSqr);
            distSqr = F.FMulAdd(dZ, dZ, distSqr);
            return distSqr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW)
        {
            f32 distSqr = F.Mul(dX, dX);
            distSqr = F.FMulAdd(dY, dY, distSqr);
            distSqr = F.FMulAdd(dZ, dZ, distSqr);
            distSqr = F.FMulAdd(dW, dW, distSqr);
            return distSqr;
        }
    }
}
