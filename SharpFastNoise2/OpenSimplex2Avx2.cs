using System.Runtime.Intrinsics;

namespace SharpFastNoise2
{
    using float32v = Vector256<float>;
    using int32v = Vector256<int>;
    using mask32v = Vector256<int>;

    public struct OpenSimplex2Avx2 :
        INoiseGenerator2D<float32v, int32v>,
        INoiseGenerator3D<float32v, int32v>
    {
        private OpenSimplex2<mask32v, float32v, int32v, Avx2Functions> _noise;

        public static bool IsSupported => Avx2Functions.IsSupported;

        public static int Count => OpenSimplex2<mask32v, float32v, int32v, Avx2Functions>.Count;

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
