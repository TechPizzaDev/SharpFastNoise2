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

        public OutputMinMax() : this(float.PositiveInfinity, float.NegativeInfinity)
        {
        }

        public void Apply(float min, float max)
        {
            Min = MathF.Min(Min, min);
            Max = MathF.Max(Max, max);
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
