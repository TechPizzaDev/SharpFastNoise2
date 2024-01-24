using System;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    using f32 = Single;
    using i32 = Int32;
    using m32 = Int32;

    public struct OpenSimplex2Scalar :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>
    {
        private OpenSimplex2<m32, f32, i32, ScalarFunctions> _noise;

        public static bool IsSupported => ScalarFunctions.IsSupported;

        public static int Count => OpenSimplex2<m32, f32, i32, ScalarFunctions>.Count;

        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            return _noise.Gen(x, y, seed);
        }

        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            return _noise.Gen(x, y, z, seed);
        }

        public OutputMinMax GenPositionArray2D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            float xOffset,
            float yOffset,
            int seed)
        {
            return _noise.GenPositionArray2D(destination, xPosArray, yPosArray, xOffset, yOffset, seed);
        }

        public OutputMinMax GenPositionArray3D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            int seed)
        {
            return _noise.GenPositionArray3D(destination, xPosArray, yPosArray, zPosArray, xOffset, yOffset, zOffset, seed);
        }

        public float GenSingle2D(float x, float y, int seed)
        {
            return _noise.GenSingle2D(x, y, seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return _noise.GenSingle3D(x, y, z, seed);
        }

        public OutputMinMax GenUniformGrid2D(
            Span<float> destination,
            int xStart, int yStart,
            int xSize, int ySize,
            float frequency, int seed)
        {
            return _noise.GenUniformGrid2D(destination, xStart, yStart, xSize, ySize, frequency, seed);
        }

        public OutputMinMax GenUniformGrid3D(
            Span<float> destination,
            int xStart, int yStart, int zStart,
            int xSize, int ySize, int zSize,
            float frequency, int seed)
        {
            return _noise.GenUniformGrid3D(destination, xStart, yStart, zStart, xSize, ySize, zSize, frequency, seed);
        }
    }
}
