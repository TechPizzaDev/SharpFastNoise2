﻿using System;
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
            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);
            f32 freqV = F.Broad(frequency);
            i32 seedV = F.Broad(seed);

            i32 xIdx = F.Broad(xStart);
            i32 yIdx = F.Broad(yStart);
            i32 zIdx = F.Broad(zStart);
            i32 wIdx = F.Broad(wStart);

            i32 xSizeV = F.Broad(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad(-1)));
            i32 ySizeV = F.Broad(ySize);
            i32 yMax = F.Add(ySizeV, F.Add(yIdx, F.Broad(-1)));
            i32 zSizeV = F.Broad(zSize);
            i32 zMax = F.Add(zSizeV, F.Add(zIdx, F.Broad(-1)));

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
                f32 xPos = F.Mul(F.Convert_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Convert_f32(yIdx), freqV);
                f32 zPos = F.Mul(F.Convert_f32(zIdx), freqV);
                f32 wPos = F.Mul(F.Convert_f32(wIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
                F.Store(ref noiseOut, index, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                index += (nuint)F.Count;
                xIdx = F.Add(xIdx, F.Broad(F.Count));

                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref xIdx, ref yIdx, xMax, xSizeV, xStep);
                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref yIdx, ref zIdx, yMax, ySizeV, yStep);
                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref zIdx, ref wIdx, zMax, zSizeV, zStep);
            }

            {
                f32 xPos = F.Add(F.Convert_f32(xIdx), freqV);
                f32 yPos = F.Add(F.Convert_f32(yIdx), freqV);
                f32 zPos = F.Add(F.Convert_f32(zIdx), freqV);
                f32 wPos = F.Add(F.Convert_f32(wIdx), freqV);

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
            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);
            i32 seedV = F.Broad(seed);
            f32 xOffsetV = F.Broad(xOffset);
            f32 yOffsetV = F.Broad(yOffset);
            f32 zOffsetV = F.Broad(zOffset);
            f32 wOffsetV = F.Broad(wOffset);

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            ref float xPosRef = ref MemoryMarshal.GetReference(xPosArray);
            ref float yPosRef = ref MemoryMarshal.GetReference(yPosArray);
            ref float zPosRef = ref MemoryMarshal.GetReference(zPosArray);
            ref float wPosRef = ref MemoryMarshal.GetReference(wPosArray);

            nuint count = (nuint)destination.Length;
            nuint index = 0;
            while (index <= count - (nuint)F.Count)
            {
                f32 xPos = F.Add(xOffsetV, F.Load(ref xPosRef, index));
                f32 yPos = F.Add(yOffsetV, F.Load(ref yPosRef, index));
                f32 zPos = F.Add(zOffsetV, F.Load(ref zPosRef, index));
                f32 wPos = F.Add(wOffsetV, F.Load(ref wPosRef, index));

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, seedV);
                F.Store(ref noiseOut, index, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                index += (nuint)F.Count;
            }

            {
                f32 xPos = F.Add(xOffsetV, F.Load(ref xPosRef, index));
                f32 yPos = F.Add(yOffsetV, F.Load(ref yPosRef, index));
                f32 zPos = F.Add(zOffsetV, F.Load(ref zPosRef, index));
                f32 wPos = F.Add(wOffsetV, F.Load(ref wPosRef, index));

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
            return F.Extract0(generator.Gen(
                F.Broad(x),
                F.Broad(y),
                F.Broad(z),
                F.Broad(w),
                F.Broad(seed)));
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
            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);

            i32 xIdx = F.Broad(0);
            i32 yIdx = F.Broad(0);

            i32 xSizeV = F.Broad(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad(-1)));
            i32 vSeed = F.Broad(seed);

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            nuint totalValues = (nuint)xSize * (nuint)ySize;
            nuint index = 0;

            float pi2Recip = 0.15915493667f;
            float xSizePi = xSize * pi2Recip;
            float ySizePi = ySize * pi2Recip;
            f32 xFreq = F.Broad(frequency * xSizePi);
            f32 yFreq = F.Broad(frequency * ySizePi);
            f32 xMul = F.Broad(1 / xSizePi);
            f32 yMul = F.Broad(1 / ySizePi);

            xIdx = F.Add(xIdx, F.Incremented_i32());

            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref xIdx, ref yIdx, xMax, xSizeV, (nuint)xSize);

            while (index <= totalValues - (nuint)F.Count)
            {
                f32 xF = F.Mul(F.Convert_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Convert_f32(yIdx), yMul);
                
                (f32 xFSin, f32 xFCos) = Utils<m32, f32, i32, F>.SinCos_f32(xF);
                (f32 yFSin, f32 yFCos) = Utils<m32, f32, i32, F>.SinCos_f32(yF);

                f32 xPos = F.Mul(xFCos, xFreq);
                f32 yPos = F.Mul(yFCos, yFreq);
                f32 zPos = F.Mul(xFSin, xFreq);
                f32 wPos = F.Mul(yFSin, yFreq);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);
                F.Store(ref noiseOut, index, gen);

                min = F.Min(min, gen);
                max = F.Max(max, gen);

                index += (uint)F.Count;
                xIdx = F.Add(xIdx, F.Broad(F.Count));

                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref xIdx, ref yIdx, xMax, xSizeV, (nuint)xSize);
            }

            {
                f32 xF = F.Mul(F.Convert_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Convert_f32(yIdx), yMul);
                
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
