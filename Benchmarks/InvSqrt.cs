using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class InvSqrt
    {
        [Benchmark]
        [Arguments(64f)]
        public float FastApproxSse(float a)
        {
            return Sse.ReciprocalSqrtScalar(Vector128.CreateScalarUnsafe(a)).ToScalar();
        }

        [Benchmark]
        [Arguments(64f)]
        public float AccurateSse(float a)
        {
            return Sse.DivideScalar(
                Vector128.CreateScalarUnsafe(1f),
                Sse.SqrtScalar(Vector128.CreateScalarUnsafe(a)))
                .ToScalar();
        }

        [Benchmark]
        [Arguments(64f)]
        public float FastApprox(float a)
        {
            float xhalf = 0.5f * (float)a;
            a = Casti32_f32(0x5f3759df - (Castf32_i32(a) >> 1));
            a *= 1.5f - xhalf * (float)a * (float)a;
            return a;
        }

        [Benchmark]
        [Arguments(64f)]
        public float Accurate(float a)
        {
            return 1f / MathF.Sqrt(a);
        }

        public static float Casti32_f32(int a)
        {
            return Unsafe.As<int, float>(ref a);
        }

        public static int Castf32_i32(float a)
        {
            return Unsafe.As<float, int>(ref a);
        }
    }
}
