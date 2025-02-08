using System;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Generators
{
    public static class NoiseGenerator2DHelper
    {
        public static OutputMinMax GenUniformGrid<f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            int xStart,
            int yStart,
            int xSize,
            int ySize,
            float frequency,
            int seed)
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator2D<f32, i32>
        {
            f32 freqV = F.Broad(frequency);
            i32 seedV = F.Broad(seed);

            i32 xIdx = F.Add(F.Broad(xStart), F.Incremented_i32());
            i32 yIdx = F.Broad(yStart);

            i32 xSizeV = F.Broad(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad(-1)));

            int xStep = xSize;
            destination = destination.Slice(0, xStep * ySize);

            (xIdx, yIdx) = GeneratorHelper.AxisReset<f32, i32, F>(true, xIdx, yIdx, xMax, xSizeV, xStep);

            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);

            while (destination.Length >= F.Count)
            {
                f32 xPos = F.Mul(F.Convert_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Convert_f32(yIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, seedV);
                F.Store(destination, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                xIdx = F.Add(xIdx, F.Broad(F.Count));
                destination = destination.Slice(F.Count);

                (xIdx, yIdx) = GeneratorHelper.AxisReset<f32, i32, F>(false, xIdx, yIdx, xMax, xSizeV, xStep);
            }

            f32 finalGen = default;
            if (destination.Length > 0)
            {
                f32 xPos = F.Mul(F.Convert_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Convert_f32(yIdx), freqV);

                finalGen = generator.Gen(xPos, yPos, seedV);
            }
            return GeneratorHelper.DoRemaining<f32, i32, F>(destination, min, max, finalGen);
        }

        public static OutputMinMax GenPositionArray<f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            float xOffset,
            float yOffset,
            int seed)
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator2D<f32, i32>
        {
            i32 seedV = F.Broad(seed);
            f32 xOffsetV = F.Broad(xOffset);
            f32 yOffsetV = F.Broad(yOffset);

            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);

            while (destination.Length >= F.Count)
            {
                f32 xPos = F.Add(xOffsetV, F.Load(xPosArray));
                f32 yPos = F.Add(yOffsetV, F.Load(yPosArray));

                f32 gen = generator.Gen(xPos, yPos, seedV);
                F.Store(destination, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                xPosArray = xPosArray.Slice(F.Count);
                yPosArray = yPosArray.Slice(F.Count);
                destination = destination.Slice(F.Count);
            }

            f32 finalGen = default;
            if (destination.Length > 0)
            {
                f32 xPos = F.Add(xOffsetV, F.LoadOrZero(xPosArray));
                f32 yPos = F.Add(yOffsetV, F.LoadOrZero(yPosArray));

                finalGen = generator.Gen(xPos, yPos, seedV);
            }
            return GeneratorHelper.DoRemaining<f32, i32, F>(destination, min, max, finalGen);
        }

        public static float GenSingle<f32, i32, F, G>(
            ref G generator,
            float x,
            float y,
            int seed)
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator2D<f32, i32>
        {
            return F.Extract0(generator.Gen(
                F.Broad(x),
                F.Broad(y),
                F.Broad(seed)));
        }
    }
}
