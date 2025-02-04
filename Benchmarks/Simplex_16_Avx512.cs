using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;
using SharpFastNoise2.Functions;

namespace Benchmarks
{
    using SimplexNoise16 = Simplex<Vector512<float>, Vector512<int>, Avx512Functions>;

    public class Simplex_16_Avx512 : BenchNoiseBase<Vector512<float>, Vector512<int>, Avx512Functions>
    {
        [Benchmark]
        public void Simplex_2Dx16()
        {
            Generate2D<SimplexNoise16>(Count);
        }
    
        [Benchmark]
        public void Simplex_3Dx16()
        {
            Generate3D<SimplexNoise16>(Count);
        }
    
        [Benchmark]
        public void Simplex_4Dx16()
        {
            Generate4D<SimplexNoise16>(Count);
        }
    }
}