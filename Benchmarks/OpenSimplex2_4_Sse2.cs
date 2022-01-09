using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using OpenSimplex2Noise4 = OpenSimplex2<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>;
    
    public class OpenSimplex2_4_Sse2 : BenchNoiseBase<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>
    {
        [Benchmark]
        public void OpenSimplex2_2Dx4()
        {
            Generate2D<OpenSimplex2Noise4>(Count);
        }
    
        [Benchmark]
        public void OpenSimplex2_3Dx4()
        {
            Generate3D<OpenSimplex2Noise4>(Count);
        }
    }
}