using System;

namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator3D : INoiseGenerator
    {
        OutputMinMax GenUniformGrid3D(
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int xSize,
            int ySize,
            int zSize,
            float frequency,
            int seed);

        OutputMinMax GenPositionArray3D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            int seed);

        float GenSingle3D(float x, float y, float z, int seed);
    }
}
