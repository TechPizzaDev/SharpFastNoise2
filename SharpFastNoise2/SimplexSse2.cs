﻿using System.Runtime.Intrinsics;

namespace SharpFastNoise2
{
    using float32v = Vector128<float>;
    using int32v = Vector128<int>;
    using mask32v = Vector128<int>;

    public struct SimplexSse2 :
        INoiseGenerator2D<float32v, int32v>,
        INoiseGenerator3D<float32v, int32v>
    {
        private Simplex<mask32v, float32v, int32v, Sse2Functions> _noise;

        public static bool IsSupported => Sse2Functions.IsSupported;

        public int Count => _noise.Count;

        public float32v Gen(int32v seed, float32v x, float32v y)
        {
            return _noise.Gen(seed, x, y);
        }

        public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        {
            return _noise.Gen(seed, x, y, z);
        }
    }
}
