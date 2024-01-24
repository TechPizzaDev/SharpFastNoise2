using BenchmarkDotNet.Attributes;
using SharpFastNoise2;
using SharpFastNoise2.Functions;

namespace Benchmarks
{
    using SimplexNoise1 = Simplex<int, float, int, ScalarFunctions>;

    public class Simplex_1_Scalar : BenchNoiseBase<int, float, int, ScalarFunctions>
    {
        [Benchmark]
        public void Simplex_2Dx1()
        {
            Generate2D<SimplexNoise1>(Count);
        }

        [Benchmark]
        public void Simplex_3Dx1()
        {
            Generate3D<SimplexNoise1>(Count);
        }

        [Benchmark]
        public void Simplex_4Dx1()
        {
            Generate4D<SimplexNoise1>(Count);
        }
    }
}