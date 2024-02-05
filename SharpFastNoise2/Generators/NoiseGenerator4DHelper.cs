using System;
using System.Runtime.InteropServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Generators
{
    public static class NoiseGenerator4DHelper
    {
        public static OutputMinMax GenUniformGrid<m32, f32, i32, F, G>(
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
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            f32 min = F.Broad_f32(float.PositiveInfinity);
            f32 max = F.Broad_f32(float.NegativeInfinity);
            f32 freqV = F.Broad_f32(frequency);
            i32 seedV = F.Broad_i32(seed);

            i32 xIdx = F.Broad_i32(xStart);
            i32 yIdx = F.Broad_i32(yStart);
            i32 zIdx = F.Broad_i32(zStart);
            i32 wIdx = F.Broad_i32(wStart);

            i32 xSizeV = F.Broad_i32(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad_i32(-1)));
            i32 ySizeV = F.Broad_i32(ySize);
            i32 yMax = F.Add(ySizeV, F.Add(yIdx, F.Broad_i32(-1)));
            i32 zSizeV = F.Broad_i32(zSize);
            i32 zMax = F.Add(zSizeV, F.Add(zIdx, F.Broad_i32(-1)));

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            nuint xStep = (nuint)xSize;
            nuint yStep = xStep * (nuint)ySize;
            nuint zStep = yStep * (nuint)zSize;
            nuint totalValues = zStep * (nuint)wSize;
            nuint index = 0;

            xIdx = F.Add(xIdx, F.Incremented_i32());

            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref xIdx, ref yIdx, xMax, xSizeV, xStep);
            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref yIdx, ref zIdx, yMax, ySizeV, yStep);
            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref zIdx, ref wIdx, zMax, zSizeV, zStep);

            while (index <= totalValues - (nuint)F.Count)
            {
                f32 xPos = F.Mul(F.Converti32_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Converti32_f32(yIdx), freqV);
                f32 zPos = F.Mul(F.Converti32_f32(zIdx), freqV);
                f32 wPos = F.Mul(F.Converti32_f32(wIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
                F.Store_f32(ref noiseOut, index, gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (nuint)F.Count;
                xIdx = F.Add(xIdx, F.Broad_i32(F.Count));

                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref xIdx, ref yIdx, xMax, xSizeV, xStep);
                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref yIdx, ref zIdx, yMax, ySizeV, yStep);
                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref zIdx, ref wIdx, zMax, zSizeV, zStep);
            }

            {
                f32 xPos = F.Add(F.Converti32_f32(xIdx), freqV);
                f32 yPos = F.Add(F.Converti32_f32(yIdx), freqV);
                f32 zPos = F.Add(F.Converti32_f32(zIdx), freqV);
                f32 wPos = F.Add(F.Converti32_f32(wIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);

                return GeneratorHelper.DoRemaining<m32, f32, i32, F>(
                    ref noiseOut, totalValues, index, min, max, gen);
            }
        }

        public static OutputMinMax GenPositionArray<m32, f32, i32, F, G>(
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
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            f32 min = F.Broad_f32(float.PositiveInfinity);
            f32 max = F.Broad_f32(float.NegativeInfinity);
            i32 seedV = F.Broad_i32(seed);
            f32 xOffsetV = F.Broad_f32(xOffset);
            f32 yOffsetV = F.Broad_f32(yOffset);
            f32 zOffsetV = F.Broad_f32(zOffset);
            f32 wOffsetV = F.Broad_f32(wOffset);

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            ref float xPosRef = ref MemoryMarshal.GetReference(xPosArray);
            ref float yPosRef = ref MemoryMarshal.GetReference(yPosArray);
            ref float zPosRef = ref MemoryMarshal.GetReference(zPosArray);
            ref float wPosRef = ref MemoryMarshal.GetReference(wPosArray);

            nuint count = (nuint)destination.Length;
            nuint index = 0;
            while (index <= count - (nuint)F.Count)
            {
                f32 xPos = F.Add(xOffsetV, F.Load_f32(ref xPosRef, index));
                f32 yPos = F.Add(yOffsetV, F.Load_f32(ref yPosRef, index));
                f32 zPos = F.Add(zOffsetV, F.Load_f32(ref zPosRef, index));
                f32 wPos = F.Add(wOffsetV, F.Load_f32(ref wPosRef, index));

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
                F.Store_f32(ref noiseOut, index, gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (nuint)F.Count;
            }

            {
                f32 xPos = F.Add(xOffsetV, F.Load_f32(ref xPosRef, index));
                f32 yPos = F.Add(yOffsetV, F.Load_f32(ref yPosRef, index));
                f32 zPos = F.Add(zOffsetV, F.Load_f32(ref zPosRef, index));
                f32 wPos = F.Add(wOffsetV, F.Load_f32(ref wPosRef, index));

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);

                return GeneratorHelper.DoRemaining<m32, f32, i32, F>(
                    ref noiseOut, count, index, min, max, gen);
            }
        }

        public static float GenSingle<m32, f32, i32, F, G>(
            ref G generator,
            float x,
            float y,
            float z,
            float w,
            int seed)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            return F.Extract0_f32(generator.Gen(
                F.Broad_f32(x),
                F.Broad_f32(y),
                F.Broad_f32(z),
                F.Broad_f32(w),
                F.Broad_i32(seed)));
        }

        public static OutputMinMax GenTileable<m32, f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            int xSize,
            int ySize,
            float frequency,
            int seed)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator4D<f32, i32>
        {
            f32 min = F.Broad_f32(float.PositiveInfinity);
            f32 max = F.Broad_f32(float.NegativeInfinity);

            i32 xIdx = F.Broad_i32(0);
            i32 yIdx = F.Broad_i32(0);

            i32 xSizeV = F.Broad_i32(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad_i32(-1)));
            i32 vSeed = F.Broad_i32(seed);

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            nuint totalValues = (nuint)xSize * (nuint)ySize;
            nuint index = 0;

            float pi2Recip = 0.15915493667f;
            float xSizePi = xSize * pi2Recip;
            float ySizePi = ySize * pi2Recip;
            f32 xFreq = F.Broad_f32(frequency * xSizePi);
            f32 yFreq = F.Broad_f32(frequency * ySizePi);
            f32 xMul = F.Broad_f32(1 / xSizePi);
            f32 yMul = F.Broad_f32(1 / ySizePi);

            xIdx = F.Add(xIdx, F.Incremented_i32());

            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref xIdx, ref yIdx, xMax, xSizeV, (nuint)xSize);

            while (index <= totalValues - (nuint)F.Count)
            {
                f32 xF = F.Mul(F.Converti32_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Converti32_f32(yIdx), yMul);
                
                (f32 xFSin, f32 xFCos) = Utils<m32, f32, i32, F>.SinCos_f32(xF);
                (f32 yFSin, f32 yFCos) = Utils<m32, f32, i32, F>.SinCos_f32(yF);

                f32 xPos = F.Mul(xFCos, xFreq);
                f32 yPos = F.Mul(yFCos, yFreq);
                f32 zPos = F.Mul(xFSin, xFreq);
                f32 wPos = F.Mul(yFSin, yFreq);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);
                F.Store_f32(ref noiseOut, index, gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (uint)F.Count;
                xIdx = F.Add(xIdx, F.Broad_i32(F.Count));

                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref xIdx, ref yIdx, xMax, xSizeV, (nuint)xSize);
            }

            {
                f32 xF = F.Mul(F.Converti32_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Converti32_f32(yIdx), yMul);
                
                (f32 xFSin, f32 xFCos) = Utils<m32, f32, i32, F>.SinCos_f32(xF);
                (f32 yFSin, f32 yFCos) = Utils<m32, f32, i32, F>.SinCos_f32(yF);

                f32 xPos = F.Mul(xFCos, xFreq);
                f32 yPos = F.Mul(yFCos, yFreq);
                f32 zPos = F.Mul(xFSin, xFreq);
                f32 wPos = F.Mul(yFSin, yFreq);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);

                return GeneratorHelper.DoRemaining<m32, f32, i32, F>(
                    ref noiseOut, totalValues, index, min, max, gen);
            }
        }
    }
}
