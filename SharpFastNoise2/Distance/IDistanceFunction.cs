﻿using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Distance
{
    public interface IDistanceFunction<m32, f32, i32, F>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32, F>
    {
        static abstract f32 CalcDistance(f32 dX, f32 dY);

        static abstract f32 CalcDistance(f32 dX, f32 dY, f32 dZ);

        static abstract f32 CalcDistance(f32 dX, f32 dY, f32 dZ, f32 dW);
    }
}
