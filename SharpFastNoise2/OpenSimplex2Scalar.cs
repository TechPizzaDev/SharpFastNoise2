using System;

namespace SharpFastNoise2
{
    using float32v = Single;
    using int32v = Int32;
    using mask32v = Int32;

    public struct OpenSimplex2Scalar :
        INoiseGenerator2D<float32v, int32v>,
        INoiseGenerator3D<float32v, int32v>
    {
        private OpenSimplex2<mask32v, float32v, int32v, ScalarFunctions> _noise;

        public static bool IsSupported => ScalarFunctions.IsSupported;

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
