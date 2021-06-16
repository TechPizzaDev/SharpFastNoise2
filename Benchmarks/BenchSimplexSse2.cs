using System;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using SimplexNoise4 = Simplex<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>;
    
    public class BenchSimplexSse2 : BenchNoiseBase<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>
    {
        [Params(16 * 16, 16 * 16 * 16, 32 * 32 * 32)]
        public int Count { get; set; }
    
        //[Benchmark]
        //public void Simplex1Dx4()
        //{
        //    throw new NotImplementedException();
        //    //Generate1D<SimplexNoise4>(Count);
        //}
    
        [Benchmark]
        public void Simplex2Dx4()
        {
            Generate2D<SimplexNoise4>(Count);
        }
    
        [Benchmark]
        public void Simplex3Dx4()
        {
            Generate3D<SimplexNoise4>(Count);
        }
    
        [Benchmark]
        public void Simplex4Dx4()
        {
            Generate4D<SimplexNoise4>(Count);
        }
    }
}