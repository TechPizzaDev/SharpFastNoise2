using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;
using SharpFastNoise2.Functions;

namespace Benchmarks
{
    using PerlinNoise16 = Perlin<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>;

    public class Perlin_16_Avx512 : BenchNoiseBase<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>
    {
        [Benchmark]
        public void Perlin_2Dx16()
        {
            Generate2D<PerlinNoise16>(Count);
        }
    
        [Benchmark]
        public void Perlin_3Dx16()
        {
            Generate3D<PerlinNoise16>(Count);
        }
    
        [Benchmark]
        public void Perlin_4Dx16()
        {
            Generate4D<PerlinNoise16>(Count);
        }
    }
}