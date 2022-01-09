using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using OpenSimplex2Noise8 = OpenSimplex2<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>;
    
    public class OpenSimplex2_8_Avx2 : BenchNoiseBase<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>
    {
        [Benchmark]
        public void OpenSimplex2_2Dx8()
        {
            Generate2D<OpenSimplex2Noise8>(Count);
        }
    
        [Benchmark]
        public void OpenSimplex2_3Dx8()
        {
            Generate3D<OpenSimplex2Noise8>(Count);
        }
    }
}