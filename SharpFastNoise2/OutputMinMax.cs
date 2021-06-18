using System;

namespace SharpFastNoise2
{
    public struct OutputMinMax
    {
        public float Min;
        public float Max;

        public OutputMinMax(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public static OutputMinMax CreateInfiniteBounds()
        {
            return new OutputMinMax(float.PositiveInfinity, float.NegativeInfinity);
        }

        public void Apply(float min, float max)
        {
            Min = MathF.Min(min, min);
            Max = MathF.Max(max, max);
        }

        public void Apply(float v)
        {
            Apply(v, v);
        }

        public void Apply(OutputMinMax v)
        {
            Apply(v.Min, v.Max);
        }
    }
}
