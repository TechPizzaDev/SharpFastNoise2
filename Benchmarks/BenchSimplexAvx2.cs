using System;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using SimplexNoise8 = Simplex<FVectorI256, FVectorF256, FVectorI256, Avx2Functions>;

    public class BenchSimplexAvx2 : BenchNoiseBase<FVectorI256, FVectorF256, FVectorI256, Avx2Functions>
    {
        [Params(16 * 16, 16 * 16 * 16/*, 32 * 32 * 32*/)]
        public int Count { get; set; }

        [Benchmark]
        public void Simplex1Dx8()
        {
            throw new NotImplementedException();
            //Generate1D<SimplexNoise8>(Count);
        }

        [Benchmark]
        public void Simplex2Dx8()
        {
            Generate2D<SimplexNoise8>(Count);
        }

        [Benchmark]
        public void Simplex3Dx8()
        {
            Generate3D<SimplexNoise8>(Count);
        }

        [Benchmark]
        public void Simplex4Dx8()
        {
            Generate4D<SimplexNoise8>(Count);
        }
    }
}