using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2.Functions;

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
        
        public void Apply<m32, f32, i32, F>(f32 min, f32 max)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
        {
            float scalarMin = F.MinAcross(min);
            float scalarMax = F.MaxAcross(max);
            Apply(scalarMin, scalarMax);
        }

        public void Apply(float min, float max)
        {
            if (Sse.IsSupported)
            {
                Min = Sse.MinScalar(Vector128.CreateScalarUnsafe(Min), Vector128.CreateScalarUnsafe(min)).ToScalar();
                Max = Sse.MaxScalar(Vector128.CreateScalarUnsafe(Max), Vector128.CreateScalarUnsafe(max)).ToScalar();
            }
            else
            {
                Min = MathF.Min(Min, min);
                Max = MathF.Max(Max, max);
            }
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
