using System.Runtime.Intrinsics;

namespace SharpFastNoise2
{
    using f32 = Vector128<float>;
    using i32 = Vector128<int>;
    using m32 = Vector128<int>;

    public struct OpenSimplex2Sse2 :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>
    {
        private OpenSimplex2<m32, f32, i32, Sse2Functions> _noise;

        public static bool IsSupported => Sse2Functions.IsSupported;

        public static int Count => OpenSimplex2<m32, f32, i32, Sse2Functions>.Count;

        public f32 Gen(i32 seed, f32 x, f32 y)
        {
            return _noise.Gen(seed, x, y);
        }

        public f32 Gen(i32 seed, f32 x, f32 y, f32 z)
        {
            return _noise.Gen(seed, x, y, z);
        }
    }
}
