using System;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Generators
{
    public static class NoiseGenerator4DHelper
    {
        public static OutputMinMax GenUniformGrid<f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int wStart,
            int xSize,
            int ySize,
            int zSize,
            int wSize,
            float frequency,
            int seed)
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            f32 freqV = F.Broad(frequency);
            i32 seedV = F.Broad(seed);

            i32 xIdx = F.Add(F.Broad(xStart), F.Incremented_i32());
            i32 yIdx = F.Broad(yStart);
            i32 zIdx = F.Broad(zStart);
            i32 wIdx = F.Broad(wStart);

            i32 icn1 = F.Broad(-1);
            i32 xSizeV = F.Broad(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, icn1));
            i32 ySizeV = F.Broad(ySize);
            i32 yMax = F.Add(ySizeV, F.Add(yIdx, icn1));
            i32 zSizeV = F.Broad(zSize);
            i32 zMax = F.Add(zSizeV, F.Add(zIdx, icn1));

            int xStep = xSize;
            int yStep = xStep * ySize;
            int zStep = yStep * zSize;
            destination = destination.Slice(0, zStep * wSize);

            (xIdx, yIdx) = GeneratorHelper.AxisReset<f32, i32, F>(true, xIdx, yIdx, xMax, xSizeV, xStep);
            (yIdx, zIdx) = GeneratorHelper.AxisReset<f32, i32, F>(true, yIdx, zIdx, yMax, ySizeV, yStep);
            (zIdx, wIdx) = GeneratorHelper.AxisReset<f32, i32, F>(true, zIdx, wIdx, zMax, zSizeV, zStep);

            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);

            while (destination.Length >= F.Count)
            {
                f32 xPos = F.Mul(F.Convert_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Convert_f32(yIdx), freqV);
                f32 zPos = F.Mul(F.Convert_f32(zIdx), freqV);
                f32 wPos = F.Mul(F.Convert_f32(wIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
                F.Store(destination, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                xIdx = F.Add(xIdx, F.Broad(F.Count));
                destination = destination.Slice(F.Count);

                (xIdx, yIdx) = GeneratorHelper.AxisReset<f32, i32, F>(false, xIdx, yIdx, xMax, xSizeV, xStep);
                (yIdx, zIdx) = GeneratorHelper.AxisReset<f32, i32, F>(false, yIdx, zIdx, yMax, ySizeV, yStep);
                (zIdx, wIdx) = GeneratorHelper.AxisReset<f32, i32, F>(false, zIdx, wIdx, zMax, zSizeV, zStep);
            }

            f32 finalGen = F.Broad(0f);
            if (destination.Length > 0)
            {
                f32 xPos = F.Mul(F.Convert_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Convert_f32(yIdx), freqV);
                f32 zPos = F.Mul(F.Convert_f32(zIdx), freqV);
                f32 wPos = F.Mul(F.Convert_f32(wIdx), freqV);

                finalGen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
            }
            return GeneratorHelper.DoRemaining<f32, i32, F>(destination, min, max, finalGen);
        }

        public static OutputMinMax GenPositionArray<f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            ReadOnlySpan<float> wPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            float wOffset,
            int seed)
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            i32 seedV = F.Broad(seed);
            f32 xOffsetV = F.Broad(xOffset);
            f32 yOffsetV = F.Broad(yOffset);
            f32 zOffsetV = F.Broad(zOffset);
            f32 wOffsetV = F.Broad(wOffset);

            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);

            while (destination.Length >= F.Count)
            {
                f32 xPos = F.Add(xOffsetV, F.Load(xPosArray));
                f32 yPos = F.Add(yOffsetV, F.Load(yPosArray));
                f32 zPos = F.Add(zOffsetV, F.Load(zPosArray));
                f32 wPos = F.Add(wOffsetV, F.Load(wPosArray));

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
                F.Store(destination, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                xPosArray = xPosArray.Slice(F.Count);
                yPosArray = yPosArray.Slice(F.Count);
                zPosArray = zPosArray.Slice(F.Count);
                wPosArray = wPosArray.Slice(F.Count);
                destination = destination.Slice(F.Count);
            }

            f32 finalGen = F.Broad(0f);
            if (destination.Length > 0)
            {
                f32 xPos = F.Add(xOffsetV, F.LoadOrZero(xPosArray));
                f32 yPos = F.Add(yOffsetV, F.LoadOrZero(yPosArray));
                f32 zPos = F.Add(zOffsetV, F.LoadOrZero(zPosArray));
                f32 wPos = F.Add(wOffsetV, F.LoadOrZero(wPosArray));

                finalGen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
            }
            return GeneratorHelper.DoRemaining<f32, i32, F>(destination, min, max, finalGen);
        }

        public static float GenSingle<f32, i32, F, G>(
            ref G generator,
            float x,
            float y,
            float z,
            float w,
            int seed)
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            return F.Extract0(generator.Gen(
                F.Broad(x),
                F.Broad(y),
                F.Broad(z),
                F.Broad(w),
                F.Broad(seed)));
        }

        public static OutputMinMax GenTileable<f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            int xSize,
            int ySize,
            float frequency,
            int seed)
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            i32 xIdx = F.Incremented_i32();
            i32 yIdx = xIdx;

            i32 xSizeV = F.Broad(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad(-1)));
            i32 vSeed = F.Broad(seed);

            float xSizePi = xSize / MathF.Tau;
            float ySizePi = ySize / MathF.Tau;
            f32 xFreq = F.Broad(frequency * xSizePi);
            f32 yFreq = F.Broad(frequency * ySizePi);
            f32 xMul = F.Broad(MathF.Tau / xSize);
            f32 yMul = F.Broad(MathF.Tau / ySize);

            destination = destination.Slice(0, xSize * ySize);

            (xIdx, yIdx) = GeneratorHelper.AxisReset<f32, i32, F>(true, xIdx, yIdx, xMax, xSizeV, xSize);

            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);

            while (destination.Length >= F.Count)
            {
                f32 xF = F.Mul(F.Convert_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Convert_f32(yIdx), yMul);

                (f32 xFSin, f32 xFCos) = Utils<f32, i32, F>.SinCos_f32(xF);
                (f32 yFSin, f32 yFCos) = Utils<f32, i32, F>.SinCos_f32(yF);

                f32 xPos = F.Mul(xFCos, xFreq);
                f32 yPos = F.Mul(yFCos, yFreq);
                f32 zPos = F.Mul(xFSin, xFreq);
                f32 wPos = F.Mul(yFSin, yFreq);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);
                F.Store(destination, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                xIdx = F.Add(xIdx, F.Broad(F.Count));
                destination = destination.Slice(F.Count);

                (xIdx, yIdx) = GeneratorHelper.AxisReset<f32, i32, F>(false, xIdx, yIdx, xMax, xSizeV, xSize);
            }

            f32 finalGen = default;
            if (destination.Length > 0)
            {
                f32 xF = F.Mul(F.Convert_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Convert_f32(yIdx), yMul);

                (f32 xFSin, f32 xFCos) = Utils<f32, i32, F>.SinCos_f32(xF);
                (f32 yFSin, f32 yFCos) = Utils<f32, i32, F>.SinCos_f32(yF);

                f32 xPos = F.Mul(xFCos, xFreq);
                f32 yPos = F.Mul(yFCos, yFreq);
                f32 zPos = F.Mul(xFSin, xFreq);
                f32 wPos = F.Mul(yFSin, yFreq);

                finalGen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);
            }
            return GeneratorHelper.DoRemaining<f32, i32, F>(destination, min, max, finalGen);
        }
    }
}
