using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SharpFastNoise2
{
    public static class GeneratorTileableExtensions
    {
        private static void ThrowNotEnoughSpace(string spanName)
        {
            throw new ArgumentException("The destination is too small.", spanName);
        }

        public static OutputMinMax GenTileable2D<TGenerator>(
            this TGenerator generator,
            Span<float> noiseOut,
            int width,
            int height,
            float frequency,
            int seed)
            where TGenerator : INoiseGenerator4D<float, int>
        {
            return generator.GenTileable2D<int, float, int, ScalarFunctions, TGenerator>(
                noiseOut, width, height, frequency, seed);
        }

        public static OutputMinMax GenTileable2D<mask32v, float32v, int32v, TFunctions, TGenerator>(
            this TGenerator generator,
            Span<float> noiseOut,
            int width,
            int height,
            float frequency,
            int seed)
            where mask32v : unmanaged
            where float32v : unmanaged
            where int32v : unmanaged
            where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
            where TGenerator : INoiseGenerator4D<float32v, int32v>
        {
            TFunctions F = new();
            FastSimd<mask32v, float32v, int32v, TFunctions> FSS = new();

            int totalValues = width * height;
            if (noiseOut.Length < totalValues)
                ThrowNotEnoughSpace(nameof(noiseOut));

            float32v min = F.Broad_f32(float.PositiveInfinity);
            float32v max = F.Broad_f32(float.NegativeInfinity);

            int32v xIdx = F.Broad_i32(0);
            int32v yIdx = F.Broad_i32(0);

            int32v xSizeV = F.Broad_i32(width); 
            int32v xMax = F.Add(xSizeV, F.Add(xIdx, F.Broad_i32(-1)));
            int32v vSeed = F.Broad_i32(seed);

            int index = 0;
            ref float dst = ref MemoryMarshal.GetReference(noiseOut);

            float pi2Recip = 0.15915493667f;
            float xSizePi = width * pi2Recip;
            float ySizePi = height * pi2Recip;
            float32v xFreq = F.Broad_f32(frequency * xSizePi);
            float32v yFreq = F.Broad_f32(frequency * ySizePi);
            float32v xMul = F.Broad_f32(1 / xSizePi);
            float32v yMul = F.Broad_f32(1 / ySizePi);

            xIdx = F.Add(xIdx, F.Incremented_i32());

            while (index < totalValues - F.Count)
            {
                float32v xF = F.Mul(F.Converti32_f32(xIdx), xMul);
                float32v yF = F.Mul(F.Converti32_f32(yIdx), yMul);

                float32v xPos = F.Mul(FSS.Cos_f32(xF), xFreq);
                float32v yPos = F.Mul(FSS.Cos_f32(yF), yFreq);
                float32v zPos = F.Mul(FSS.Sin_f32(xF), xFreq);
                float32v wPos = F.Mul(FSS.Sin_f32(yF), yFreq);

                float32v gen = generator.Gen(vSeed, xPos, yPos, zPos, wPos);
                F.Store_f32(ref Unsafe.Add(ref dst, index), gen);

                min = F.Min_f32(min, gen);
                max = F.Max_f32(max, gen);

                index += F.Count;
                xIdx = F.Add(xIdx, F.Broad_i32(F.Count));

                mask32v xReset = F.GreaterThan(xIdx, xMax);
                yIdx = FSS.MaskedIncrement_i32(yIdx, xReset);
                xIdx = FSS.MaskedSub_i32(xIdx, xSizeV, xReset);
            }

            {
                float32v xF = F.Mul(F.Converti32_f32(xIdx), xMul);
                float32v yF = F.Mul(F.Converti32_f32(yIdx), yMul);

                float32v xPos = F.Mul(FSS.Cos_f32(xF), xFreq);
                float32v yPos = F.Mul(FSS.Cos_f32(yF), yFreq);
                float32v zPos = F.Mul(FSS.Sin_f32(xF), xFreq);
                float32v wPos = F.Mul(FSS.Sin_f32(yF), yFreq);

                float32v gen = generator.Gen(vSeed, xPos, yPos, zPos, wPos);

                return DoRemaining<mask32v, float32v, int32v, TFunctions>(
                    ref dst, totalValues, index, min, max, gen);
            }
        }

        private static OutputMinMax DoRemaining<mask32v, float32v, int32v, TFunctions>(
            ref float noiseOut,
            int totalValues,
            int index,
            float32v min,
            float32v max,
            float32v finalGen)
            where mask32v : unmanaged
            where float32v : unmanaged
            where int32v : unmanaged
            where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
        {
            TFunctions F = new();

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
                    ref Unsafe.As<float32v, byte>(ref finalGen),
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
