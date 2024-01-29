using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Generators
{
    public static class GeneratorHelper
    {
        public static OutputMinMax DoRemaining<m32, f32, i32, F>(
            ref float destination,
            nuint totalValues,
            nuint index,
            f32 min,
            f32 max,
            f32 finalGen)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
        {
            OutputMinMax minMax = new();
            int remaining = (int)(totalValues - index);
            ref float noiseOut = ref Unsafe.Add(ref destination, index);

            for (int i = 0; i < remaining; i++)
            {
                float f = F.Extract_f32(finalGen, i);
                Unsafe.Add(ref noiseOut, i) = f;
                minMax.Apply(f);
            }

            for (int i = 0; i < F.Count; i++)
            {
                minMax.Apply(F.Extract_f32(min, i), F.Extract_f32(max, i));
            }

            return minMax;
        }

        public static void AxisResetTrue<m32, f32, i32, F>(
            ref i32 aIdx, ref i32 bIdx, i32 aMax, i32 aSize, nuint aStep)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
        {
            for (nuint resetLoop = aStep; resetLoop < (nuint)F.Count; resetLoop += aStep)
            {
                m32 aReset = F.GreaterThan(aIdx, aMax);
                bIdx = Utils<m32, f32, i32, F>.MaskedIncrement_i32(bIdx, aReset);
                aIdx = Utils<m32, f32, i32, F>.MaskedSub_i32(aIdx, aSize, aReset);
            }
        }

        public static void AxisResetFalse<m32, f32, i32, F>(
            ref i32 aIdx, ref i32 bIdx, i32 aMax, i32 aSize, nuint aStep)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
        {
            for (nuint resetLoop = 0; resetLoop < (nuint)F.Count; resetLoop += aStep)
            {
                m32 aReset = F.GreaterThan(aIdx, aMax);
                bIdx = Utils<m32, f32, i32, F>.MaskedIncrement_i32(bIdx, aReset);
                aIdx = Utils<m32, f32, i32, F>.MaskedSub_i32(aIdx, aSize, aReset);
            }
        }
    }
}
