using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
            where F : IFunctionList<m32, f32, i32>
            where G : INoiseGenerator3D<f32, i32>
        {
            f32 min = F.Broad_f32(float.PositiveInfinity);
            f32 max = F.Broad_f32(float.NegativeInfinity);
            f32 freqV = F.Broad_f32(frequency);
            i32 seedV = F.Broad_i32(seed);

            i32 xIdx = F.Broad_i32(xStart);
            i32 yIdx = F.Broad_i32(yStart);
            i32 zIdx = F.Broad_i32(zStart);

            i32 xSizeV = F.Broad_i32(xSize);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad_i32(-1)));
            i32 ySizeV = F.Broad_i32(ySize);
            i32 yMax = F.Add(ySizeV, F.Add(yIdx, F.Broad_i32(-1)));

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            nuint xStep = (nuint)xSize;
            nuint yStep = xStep * (nuint)ySize;
            nuint totalValues = yStep * (nuint)zSize;
            nuint index = 0;

            xIdx = F.Add(xIdx, F.Incremented_i32());

            GeneratorHelper.AxisResetTrue<m32, f32, i32, F>(ref xIdx, ref yIdx, xMax, xSizeV, xStep);
            GeneratorHelper.AxisResetTrue<m32, f32, i32, F>(ref yIdx, ref zIdx, yMax, ySizeV, yStep);

            while (index <= totalValues - (nuint)F.Count)
            {
                f32 xPos = F.Mul(F.Converti32_f32(xIdx), freqV);
                f32 yPos = F.Mul(F.Converti32_f32(yIdx), freqV);
                f32 zPos = F.Mul(F.Converti32_f32(zIdx), freqV);

                f32 gen = generator.Gen(xPos, yPos, zPos, seedV);
                F.Store_f32(ref Unsafe.Add(ref noiseOut, index), gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (nuint)F.Count;
                xIdx = F.Add(xIdx, F.Broad_i32(F.Count));

                GeneratorHelper.AxisResetFalse<m32, f32, i32, F>(ref xIdx, ref yIdx, xMax, xSizeV, xStep);
                GeneratorHelper.AxisResetFalse<m32, f32, i32, F>(ref yIdx, ref zIdx, yMax, ySizeV, yStep);
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
            where F : IFunctionList<m32, f32, i32>
            where G : INoiseGenerator3D<f32, i32>
        {
            f32 min = F.Broad_f32(float.PositiveInfinity);
            f32 max = F.Broad_f32(float.NegativeInfinity);
            i32 seedV = F.Broad_i32(seed);
            f32 xOffsetV = F.Broad_f32(xOffset);
            f32 yOffsetV = F.Broad_f32(yOffset);
            f32 zOffsetV = F.Broad_f32(zOffset);

            ref float noiseOut = ref MemoryMarshal.GetReference(destination);
            ref float xPosRef = ref MemoryMarshal.GetReference(xPosArray);
            ref float yPosRef = ref MemoryMarshal.GetReference(yPosArray);
            ref float zPosRef = ref MemoryMarshal.GetReference(zPosArray);

            nuint count = (nuint)destination.Length;
            nuint index = 0;
            while (index <= count - (nuint)F.Count)
            {
                f32 xPos = F.Add(xOffsetV, F.Load_f32(ref Unsafe.Add(ref xPosRef, index)));
                f32 yPos = F.Add(yOffsetV, F.Load_f32(ref Unsafe.Add(ref yPosRef, index)));
                f32 zPos = F.Add(zOffsetV, F.Load_f32(ref Unsafe.Add(ref zPosRef, index)));

                f32 gen = generator.Gen(xPos, yPos, zPos, seedV);
                F.Store_f32(ref Unsafe.Add(ref noiseOut, index), gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += (nuint)F.Count;
            }

            {
                f32 xPos = F.Add(xOffsetV, F.Load_f32(ref Unsafe.Add(ref xPosRef, index)));
                f32 yPos = F.Add(yOffsetV, F.Load_f32(ref Unsafe.Add(ref yPosRef, index)));
                f32 zPos = F.Add(zOffsetV, F.Load_f32(ref Unsafe.Add(ref zPosRef, index)));

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
            where F : IFunctionList<m32, f32, i32>
            where G : INoiseGenerator3D<f32, i32>
        {
            return F.Extract0_f32(generator.Gen(
                F.Broad_f32(x),
                F.Broad_f32(y),
                F.Broad_f32(z),
                F.Broad_i32(seed)));
        }
    }
}
