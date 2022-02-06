using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public static class GeneratorTileableExtensions
    {
        private static void ThrowNotEnoughSpace(string spanName)
        {
            throw new ArgumentException("The destination is too small.", spanName);
        }

        public static OutputMinMax GenTileable2D<G>(
            this G generator,
            Span<float> noiseOut,
            int width,
            int height,
            float frequency,
            int seed)
            where G : INoiseGenerator4D<float, int>
        {
            return generator.GenTileable2D<int, float, int, ScalarFunctions, G>(
                noiseOut, width, height, frequency, seed);
        }

        public static OutputMinMax GenTileable2D<m32, f32, i32, F, G>(
            this G generator,
            Span<float> noiseOut,
            int width,
            int height,
            float frequency,
            int seed)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : unmanaged, IFunctionList<m32, f32, i32>
            where G : INoiseGenerator4D<f32, i32>
        {
            int totalValues = width * height;
            if (noiseOut.Length < totalValues)
                ThrowNotEnoughSpace(nameof(noiseOut));

            f32 min = F.Broad_f32(float.PositiveInfinity);
            f32 max = F.Broad_f32(float.NegativeInfinity);

            i32 xIdx = F.Broad_i32(0);
            i32 yIdx = F.Broad_i32(0);

            i32 xSizeV = F.Broad_i32(width);
            i32 xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad_i32(-1)));
            i32 vSeed = F.Broad_i32(seed);

            int index = 0;
            ref float dst = ref MemoryMarshal.GetReference(noiseOut);

            float pi2Recip = 0.15915493667f;
            float xSizePi = width * pi2Recip;
            float ySizePi = height * pi2Recip;
            f32 xFreq = F.Broad_f32(frequency * xSizePi);
            f32 yFreq = F.Broad_f32(frequency * ySizePi);
            f32 xMul = F.Broad_f32(1 / xSizePi);
            f32 yMul = F.Broad_f32(1 / ySizePi);

            xIdx = F.Add(xIdx, F.Incremented_i32());

            while (index < totalValues - F.Count)
            {
                f32 xF = F.Mul(F.Converti32_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Converti32_f32(yIdx), yMul);

                f32 xPos = F.Mul(Utils<m32, f32, i32, F>.Cos_f32(xF), xFreq);
                f32 yPos = F.Mul(Utils<m32, f32, i32, F>.Cos_f32(yF), yFreq);
                f32 zPos = F.Mul(Utils<m32, f32, i32, F>.Sin_f32(xF), xFreq);
                f32 wPos = F.Mul(Utils<m32, f32, i32, F>.Sin_f32(yF), yFreq);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);
                F.Store_f32(ref Unsafe.Add(ref dst, index), gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += F.Count;
                xIdx = F.Add(xIdx, F.Broad_i32(F.Count));

                m32 xReset = F.GreaterThan(xIdx, xMax);
                yIdx = Utils<m32, f32, i32, F>.MaskedIncrement_i32(yIdx, xReset);
                xIdx = Utils<m32, f32, i32, F>.MaskedSub_i32(xIdx, xSizeV, xReset);
            }

            {
                f32 xF = F.Mul(F.Converti32_f32(xIdx), xMul);
                f32 yF = F.Mul(F.Converti32_f32(yIdx), yMul);

                f32 xPos = F.Mul(Utils<m32, f32, i32, F>.Cos_f32(xF), xFreq);
                f32 yPos = F.Mul(Utils<m32, f32, i32, F>.Cos_f32(yF), yFreq);
                f32 zPos = F.Mul(Utils<m32, f32, i32, F>.Sin_f32(xF), xFreq);
                f32 wPos = F.Mul(Utils<m32, f32, i32, F>.Sin_f32(yF), yFreq);

                f32 gen = generator.Gen(xPos, yPos, zPos, wPos, vSeed);

                return DoRemaining<m32, f32, i32, F>(
                    ref dst, totalValues, index, min, max, gen);
            }
        }

        private static OutputMinMax DoRemaining<m32, f32, i32, F>(
            ref float noiseOut,
            int totalValues,
            int index,
            f32 min,
            f32 max,
            f32 finalGen)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : unmanaged, IFunctionList<m32, f32, i32>
        {
            OutputMinMax minMax = default;
            int remaining = totalValues - index;

            if (remaining == F.Count)
            {
                F.Store_f32(ref Unsafe.Add(ref noiseOut, index), finalGen);

                min = F.Min_f32(min, finalGen);
                max = F.Max_f32(max, finalGen);
            }
            else
            {
                Unsafe.CopyBlockUnaligned(
                    ref Unsafe.As<float, byte>(ref Unsafe.Add(ref noiseOut, index)),
                    ref Unsafe.As<f32, byte>(ref finalGen),
                    (uint)remaining * sizeof(int));

                do
                {
                    minMax.Apply(Unsafe.Add(ref noiseOut, index));
                }
                while (++index < totalValues);
            }

            for (int i = 0; i < F.Count; i++)
            {
                minMax.Apply(F.Extract_f32(min, i), F.Extract_f32(max, i));
            }

            return minMax;
        }
    }
}
