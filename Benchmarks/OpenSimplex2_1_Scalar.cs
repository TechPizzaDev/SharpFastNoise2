using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using OpenSimplex2Noise1 = OpenSimplex2<int, float, int, ScalarFunctions>;

    public class OpenSimplex2_1_Scalar : BenchNoiseBase<int, float, int, ScalarFunctions>
    {
        [Benchmark]
        public void OpenSimplex2_2Dx1()
        {
            Generate2D<OpenSimplex2Noise1>(Count);
        }

        [Benchmark]
        public void OpenSimplex2_3Dx1()
        {
            Generate3D<OpenSimplex2Noise1>(Count);
        }
    }
}