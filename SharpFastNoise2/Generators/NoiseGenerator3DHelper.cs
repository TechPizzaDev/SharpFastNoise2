using System;
using System.Runtime.InteropServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Generators
{
    public static class NoiseGenerator3DHelper
    {
        public static OutputMinMax GenUniformGrid<m32, f32, i32, F, G>(
            ref G generator,
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int xSize,
            int ySize,
            int zSize,
            float frequency,
            int seed)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator3D<f32, i32>
        {
            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);
            f32 freqV = F.Broad(frequency);
            i32 seedV = F.Broad(seed);

            i32 xIdx = F.Broad(xStart);
            i32 yIdx = F.Broad(yStart);
            i32 zIdx = F.Broad(zStart);

            i32 xSizeV = F.Broad(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad(-1)));
            i32 ySizeV = F.Broad(ySize);
            i32 yMax = F.Add(ySizeV, F.Add(yIdx, F.Broad(-1)));

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            nuint xStep = (nuint)xSize;
            nuint yStep = xStep * (nuint)ySize;
            nuint totalValues = yStep * (nuint)zSize;
            nuint index = 0;

            xIdx = F.Add(xIdx, F.Incremented_i32());

            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref xIdx, ref yIdx, xMax, xSizeV, xStep);
            GeneratorHelper.AxisReset<m32, f32, i32, F>(true, ref yIdx, ref zIdx, yMax, ySizeV, yStep);

            while (index <= totalValues - (nuint)F.Count)
            {
                f32 xPos = F.Mul(F.Converti32_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Converti32_f32(yIdx), freqV);
                f32 zPos = F.Mul(F.Converti32_f32(zIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, seedV);
                F.Store(ref noiseOut, index, gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (nuint)F.Count;
                xIdx = F.Add(xIdx, F.Broad(F.Count));

                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref xIdx, ref yIdx, xMax, xSizeV, xStep);
                GeneratorHelper.AxisReset<m32, f32, i32, F>(false, ref yIdx, ref zIdx, yMax, ySizeV, yStep);
            }

            {
                f32 xPos = F.Add(F.Converti32_f32(xIdx), freqV);
                f32 yPos = F.Add(F.Converti32_f32(yIdx), freqV);
                f32 zPos = F.Add(F.Converti32_f32(zIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, seedV);

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
            float xOffset,
            float yOffset,
            float zOffset,
            int seed)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator3D<f32, i32>
        {
            f32 min = F.Broad(float.PositiveInfinity);
            f32 max = F.Broad(float.NegativeInfinity);
            i32 seedV = F.Broad(seed);
            f32 xOffsetV = F.Broad(xOffset);
            f32 yOffsetV = F.Broad(yOffset);
            f32 zOffsetV = F.Broad(zOffset);

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            ref float xPosRef = ref MemoryMarshal.GetReference(xPosArray);
            ref float yPosRef = ref MemoryMarshal.GetReference(yPosArray);
            ref float zPosRef = ref MemoryMarshal.GetReference(zPosArray);

            nuint count = (nuint)destination.Length;
            nuint index = 0;
            while (index <= count - (nuint)F.Count)
            {
                f32 xPos = F.Add(xOffsetV, F.Load(ref xPosRef, index));
                f32 yPos = F.Add(yOffsetV, F.Load(ref yPosRef, index));
                f32 zPos = F.Add(zOffsetV, F.Load(ref zPosRef, index));

                f32 gen = generator.Gen(xPos, yPos, zPos, seedV);
                F.Store(ref noiseOut, index, gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (nuint)F.Count;
            }

            {
                f32 xPos = F.Add(xOffsetV, F.Load(ref xPosRef, index));
                f32 yPos = F.Add(yOffsetV, F.Load(ref yPosRef, index));
                f32 zPos = F.Add(zOffsetV, F.Load(ref zPosRef, index));

                f32 gen = generator.Gen(xPos, yPos, zPos, seedV);

                return GeneratorHelper.DoRemaining<m32, f32, i32, F>(
                    ref noiseOut, count, index, min, max, gen);
            }
        }

        public static float GenSingle<m32, f32, i32, F, G>(
            ref G generator,
            float x, 
            float y, 
            float z,
            int seed)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGenerator3D<f32, i32>
        {
            return F.Extract0(generator.Gen(
                F.Broad(x),
                F.Broad(y),
                F.Broad(z),
                F.Broad(seed)));
        }
    }
}
