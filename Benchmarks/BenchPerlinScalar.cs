using System;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using SimplexNoise1 = Simplex<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>;

    public class BenchSimplexScalar : BenchNoiseBase<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>
    {
        [Params(16 * 16, 16 * 16 * 16/*, 32 * 32 * 32*/)]
        public int Count { get; set; }

        [Benchmark]
        public void Simplex1Dx1()
        {
            throw new NotImplementedException();
            //Generate1D<SimplexNoise1>(Count);
        }

        [Benchmark]
        public void Simplex2Dx1()
        {
            Generate2D<SimplexNoise1>(Count);
        }

        [Benchmark]
        public void Simplex3Dx1()
        {
            Generate3D<SimplexNoise1>(Count);
        }

        [Benchmark]
        public void Simplex4Dx1()
        {
            Generate4D<SimplexNoise1>(Count);
        }
    }
}