using System;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2.Generators
{
    public static class GeneratorHelper
    {
        public static OutputMinMax DoRemaining<f32, i32, F>(
            Span<float> destination,
            f32 min,
            f32 max,
            f32 finalGen)
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<f32, i32, F>
        {
            OutputMinMax minMax = new();
            minMax.Apply<f32, i32, F>(min, max);

            for (int i = 0; i < destination.Length; i++)
            {
                float f = F.Extract(finalGen, i);
                destination[i] = f;
                minMax.Apply(f);
            }

            return minMax;
        }

        public static (i32 A, i32 B) AxisReset<f32, i32, F>(
            bool initial, i32 aIdx, i32 bIdx, i32 aMax, i32 aSize, int aStep)
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<f32, i32, F>
        {
            for (int resetLoop = initial ? aStep : 0; resetLoop < F.Count; resetLoop += aStep)
            {
                i32 aReset = F.GreaterThan(aIdx, aMax);
                bIdx = F.MaskIncrement(bIdx, aReset);
                aIdx = F.MaskSub(aIdx, aSize, aReset);
            }
            return (aIdx, bIdx);
        }
    }
}
