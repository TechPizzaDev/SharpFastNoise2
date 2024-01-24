using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;
using SharpFastNoise2.Functions;

namespace Benchmarks
{
    using OpenSimplex2Noise16 = OpenSimplex2<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>;

    public class OpenSimplex2_16_Avx512 : BenchNoiseBase<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>
    {
        [Benchmark]
        public void OpenSimplex2_2Dx16()
        {
            Generate2D<OpenSimplex2Noise16>(Count);
        }
    
        [Benchmark]
        public void OpenSimplex2_3Dx16()
        {
            Generate3D<OpenSimplex2Noise16>(Count);
        }
    }
}