using System.Runtime.Intrinsics;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    using f32 = Vector128<float>;
    using i32 = Vector128<int>;
    using m32 = Vector128<int>;

    public struct SimplexSse2 :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>
    {
        private Simplex<m32, f32, i32, Sse2Functions> _noise;

        public static bool IsSupported => Sse2Functions.IsSupported;

        public static int Count => Simplex<m32, f32, i32, Sse2Functions>.Count;

        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            return _noise.Gen(x, y, seed);
        }

        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            return _noise.Gen(x, y, z, seed);
        }
    }
}
