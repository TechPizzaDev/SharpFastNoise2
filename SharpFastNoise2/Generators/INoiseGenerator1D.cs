using System;

namespace SharpFastNoise2.Generators
{
    public interface INoiseGenerator1D : INoiseGenerator
    {
        OutputMinMax GenUniformGrid1D(
            Span<float> destination,
            int xStart,
            int xSize,
            float frequency,
            int seed);

        OutputMinMax GenPositionArray1D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            float xOffset,
            int seed);

        float GenSingle1D(float x, int seed);
    }
}
