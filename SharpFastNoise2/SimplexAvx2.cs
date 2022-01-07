using System.Runtime.Intrinsics;

namespace SharpFastNoise2
{
    using f32 = Vector256<float>;
    using i32 = Vector256<int>;
    using m32 = Vector256<int>;

    public struct SimplexAvx2 :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>
    {
        private Simplex<m32, f32, i32, Avx2Functions> _noise;

        public static bool IsSupported => Avx2Functions.IsSupported;

        public static int Count => Simplex<m32, f32, i32, Avx2Functions>.Count;

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
