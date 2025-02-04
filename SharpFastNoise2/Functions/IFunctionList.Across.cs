using System;
using System.Runtime.CompilerServices;

namespace SharpFastNoise2.Functions
{
    public partial interface IFunctionList<f32, i32, F>
        where F : IFunctionList<f32, i32, F>
    {
        // Min

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static virtual float MinAcross(f32 a)
        {
            float v = F.Extract0(a);
            for (int i = 1; i < F.Count; i++)
            {
                v = MathF.Min(v, F.Extract(a, i));
            }
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static virtual int MinAcross(i32 a)
        {
            int v = F.Extract0(a);
            for (int i = 1; i < F.Count; i++)
            {
                v = Math.Min(v, F.Extract(a, i));
            }
            return v;
        }

        // Max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static virtual float MaxAcross(f32 a)
        {
            float v = F.Extract0(a);
            for (int i = 1; i < F.Count; i++)
            {
                v = MathF.Max(v, F.Extract(a, i));
            }
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static virtual int MaxAcross(i32 a)
        {
            int v = F.Extract0(a);
            for (int i = 1; i < F.Count; i++)
            {
                v = Math.Max(v, F.Extract(a, i));
            }
            return v;
        }
    }
}
