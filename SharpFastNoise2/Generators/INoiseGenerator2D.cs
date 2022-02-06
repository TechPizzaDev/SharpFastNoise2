using System;

namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator2D : INoiseGenerator
    {
        OutputMinMax GenUniformGrid2D(
            Span<float> destination,
            int xStart,
            int yStart,
            int xSize,
            int ySize,
            float frequency,
            int seed);

        OutputMinMax GenPositionArray2D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            float xOffset,
            float yOffset,
            int seed);

        float GenSingle2D(float x, float y, int seed);
    }
}
