using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;
using SharpFastNoise2.Functions;

namespace Benchmarks
{
    using SimplexNoise8 = Simplex<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>;

    public class Simplex_8_Avx2 : BenchNoiseBase<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>
    {
        [Benchmark]
        public void Simplex_2Dx8()
        {
            Generate2D<SimplexNoise8>(Count);
        }
    
        [Benchmark]
        public void Simplex_3Dx8()
        {
            Generate3D<SimplexNoise8>(Count);
        }
    
        [Benchmark]
        public void Simplex_4Dx8()
        {
            Generate4D<SimplexNoise8>(Count);
        }
    }
}