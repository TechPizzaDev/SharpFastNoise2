using System;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using PerlinNoise1 = Perlin<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>;

    public class BenchPerlinScalar : BenchNoiseBase<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>
    {
        [Params(16 * 16, 16 * 16 * 16/*, 32 * 32 * 32*/)]
        public int Count { get; set; }

        [Benchmark]
        public void Perlin1Dx1()
        {
            throw new NotImplementedException();
            //Generate1D<PerlinNoise1>(Count);
        }

        [Benchmark]
        public void Perlin2Dx1()
        {
            Generate2D<PerlinNoise1>(Count);
        }

        [Benchmark]
        public void Perlin3Dx1()
        {
            Generate3D<PerlinNoise1>(Count);
        }

        [Benchmark]
        public void Perlin4Dx1()
        {
            Generate4D<PerlinNoise1>(Count);
        }
    }
}