using System;
using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using PerlinNoise4 = Perlin<FVectorI128, FVectorF128, FVectorI128, SseFunctions>;

    public class BenchPerlinSse : BenchNoiseBase<FVectorI128, FVectorF128, FVectorI128, SseFunctions>
    {
        [Params(16 * 16, 16 * 16 * 16/*, 32 * 32 * 32*/)]
        public int Count { get; set; }

        [Benchmark]
        public void Perlin1Dx4()
        {
            throw new NotImplementedException();
            //Generate1D<PerlinNoise4>(Count);
        }

        [Benchmark]
        public void Perlin2Dx4()
        {
            Generate2D<PerlinNoise4>(Count);
        }

        [Benchmark]
        public void Perlin3Dx4()
        {
            Generate3D<PerlinNoise4>(Count);
        }

        [Benchmark]
        public void Perlin4Dx4()
        {
            Generate4D<PerlinNoise4>(Count);
        }
    }
}