using System;

namespace SharpFastNoise2
{
    using f32 = Single;
    using i32 = Int32;
    using m32 = Int32;

    public struct SimplexScalar :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>
    {
        private Simplex<m32, f32, i32, ScalarFunctions> _noise;

        public static bool IsSupported => ScalarFunctions.IsSupported;

        public static int Count => Simplex<m32, f32, i32, ScalarFunctions>.Count;

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
