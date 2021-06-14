using BenchmarkDotNet.Attributes;
using SharpFastNoise2;

namespace Benchmarks
{
    using PerlinNoise1 = Perlin<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>;
    using PerlinNoise4 = Perlin<FVectorI128, FVectorF128, FVectorI128, SseFunctions>;

    public class BenchPerlin
    {
        private const int Seed = 1234;
        private const float Mul = 0.01f;

        [Params(16 * 16, 16 * 16 * 16/*, 32 * 32 * 32*/)]
        public int Count { get; set; }

        private void Generate1D<mask32v, float32v, int32v, TFunc, TGen>()
            where mask32v : unmanaged, IFMask<mask32v>
            where float32v : unmanaged, IFVector<float32v, mask32v>
            where int32v : unmanaged, IFVector<int32v, mask32v>
            where TFunc : IFunctionList<mask32v, float32v, int32v>, new()
            where TGen : INoiseGenerator1D<mask32v, float32v, int32v>, new()
        {
            var noise = new TGen();
            var fn = new TFunc();
            var seed = fn.Broad_i32(Seed);
            for (int x = 0; x < Count; x += noise.Count)
            {
                noise.Gen(seed, fn.Broad_f32(x * Mul));
            }
        }

        private void Generate2D<mask32v, float32v, int32v, TFunc, TGen>()
            where mask32v : unmanaged, IFMask<mask32v>
            where float32v : unmanaged, IFVector<float32v, mask32v>
            where int32v : unmanaged, IFVector<int32v, mask32v>
            where TFunc : IFunctionList<mask32v, float32v, int32v>, new()
            where TGen : INoiseGenerator2D<mask32v, float32v, int32v>, new()
        {
            var noise = new TGen();
            var fn = new TFunc();
            var seed = fn.Broad_i32(Seed);
            for (int x = 0; x < Count; x += noise.Count)
            {
                noise.Gen(seed, fn.Broad_f32(x * Mul), default);
            }
        }

        private void Generate3D<mask32v, float32v, int32v, TFunc, TGen>()
            where mask32v : unmanaged, IFMask<mask32v>
            where float32v : unmanaged, IFVector<float32v, mask32v>
            where int32v : unmanaged, IFVector<int32v, mask32v>
            where TFunc : IFunctionList<mask32v, float32v, int32v>, new()
            where TGen : INoiseGenerator3D<mask32v, float32v, int32v>, new()
        {
            var noise = new TGen();
            var fn = new TFunc();
            var seed = fn.Broad_i32(Seed);
            for (int x = 0; x < Count; x += noise.Count)
            {
                noise.Gen(seed, fn.Broad_f32(x * Mul), default, default);
            }
        }

        private void Generate4D<mask32v, float32v, int32v, TFunc, TGen>()
            where mask32v : unmanaged, IFMask<mask32v>
            where float32v : unmanaged, IFVector<float32v, mask32v>
            where int32v : unmanaged, IFVector<int32v, mask32v>
            where TFunc : IFunctionList<mask32v, float32v, int32v>, new()
            where TGen : INoiseGenerator4D<mask32v, float32v, int32v>, new()
        {
            var noise = new TGen();
            var fn = new TFunc();
            var seed = fn.Broad_i32(Seed);
            for (int x = 0; x < Count; x += noise.Count)
            {
                noise.Gen(seed, fn.Broad_f32(x * Mul), default, default, default);
            }
        }

        [Benchmark]
        public void Perlin1Dx1()
        {
            Generate1D<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions, PerlinNoise1>();
        }

        [Benchmark]
        public void Perlin2Dx1()
        {
            Generate2D<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions, PerlinNoise1>();
        }

        [Benchmark]
        public void Perlin3Dx1()
        {
            Generate3D<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions, PerlinNoise1>();
        }

        [Benchmark]
        public void Perlin4Dx1()
        {
            Generate4D<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions, PerlinNoise1>();
        }



        [Benchmark]
        public void Perlin1Dx4()
        {
            Generate1D<FVectorI128, FVectorF128, FVectorI128, SseFunctions, PerlinNoise4>();
        }

        [Benchmark]
        public void Perlin2Dx4()
        {
            Generate2D<FVectorI128, FVectorF128, FVectorI128, SseFunctions, PerlinNoise4>();
        }

        [Benchmark]
        public void Perlin3Dx4()
        {
            Generate3D<FVectorI128, FVectorF128, FVectorI128, SseFunctions, PerlinNoise4>();
        }

        [Benchmark]
        public void Perlin4Dx4()
        {
            Generate4D<FVectorI128, FVectorF128, FVectorI128, SseFunctions, PerlinNoise4>();
        }
    }
}
