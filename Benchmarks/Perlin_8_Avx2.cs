using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;
using SharpFastNoise2.Functions;

namespace Benchmarks
{
    using PerlinNoise8 = Perlin<Vector256<float>, Vector256<int>, Avx2Functions>;

    public class Perlin_8_Avx2 : BenchNoiseBase<Vector256<float>, Vector256<int>, Avx2Functions>
    {
        [Benchmark]
        public void Perlin_2Dx8()
        {
            Generate2D<PerlinNoise8>(Count);
        }
    
        [Benchmark]
        public void Perlin_3Dx8()
        {
            Generate3D<PerlinNoise8>(Count);
        }
    
        [Benchmark]
        public void Perlin_4Dx8()
        {
            Generate4D<PerlinNoise8>(Count);
        }
    }
}