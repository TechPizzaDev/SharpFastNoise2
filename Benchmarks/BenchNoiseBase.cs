using BenchmarkDotNet.Attributes;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace Benchmarks
{
    public class BenchNoiseBase<m32, f32, i32, F>
        where F : IFunctionList<m32, f32, i32, F>
    {
        [Params(
            16 * 16,
            16 * 16 * 16,
            32 * 32 * 32,
            128 * 128 * 128)]
        public int Count { get; set; }

        private const int Seed = 1234;
        private const float Mul = 0.01f;

        public void Generate1D<G>(int count)
            where G : INoiseGenerator1D<f32, i32>, new()
        {
            Generate1D(new G(), count);
        }

        public void Generate1D<G>(G noise, int count)
            where G : INoiseGenerator1D<f32, i32>
        {
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(F.Broad_f32(x * Mul), seed);
            }
        }

        public void Generate2D<G>(int count)
            where G : INoiseGenerator2D<f32, i32>, new()
        {
            Generate2D(new G(), count);
        }

        public void Generate2D<G>(G noise, int count)
            where G : INoiseGenerator2D<f32, i32>
        {
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(F.Broad_f32(x * Mul), default, seed);
            }
        }

        public void Generate3D<G>(int count)
            where G : INoiseGenerator3D<f32, i32>, new()
        {
            Generate3D(new G(), count);
        }

        public void Generate3D<G>(G noise, int count)
            where G : INoiseGenerator3D<f32, i32>
        {
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(F.Broad_f32(x * Mul), default, default, seed);
            }
        }

        public void Generate4D<G>(int count)
            where G : INoiseGenerator4D<f32, i32>, new()
        {
            Generate4D(new G(), count);
        }

        public void Generate4D<G>(G noise, int count)
            where G : INoiseGenerator4D<f32, i32>
        {
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(F.Broad_f32(x * Mul), default, default, default, seed);
            }
        }
    }
}