using SharpFastNoise2;

namespace Benchmarks
{
    public class BenchNoiseBase<m32, f32, i32, F>
        where F : IFunctionList<m32, f32, i32>, new()
    {
        private const int Seed = 1234;
        private const float Mul = 0.01f;

        public void Generate1D<G>(int count)
            where G : INoiseGenerator1D<f32, i32>, new()
        {
            var noise = new G();
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(seed, F.Broad_f32(x * Mul));
            }
        }

        public void Generate2D<G>(int count)
            where G : INoiseGenerator2D<f32, i32>, new()
        {
            var noise = new G();
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(seed, F.Broad_f32(x * Mul), default);
            }
        }

        public void Generate3D<G>(int count)
            where G : INoiseGenerator3D<f32, i32>, new()
        {
            var noise = new G();
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(seed, F.Broad_f32(x * Mul), default, default);
            }
        }

        public void Generate4D<G>(int count)
            where G : INoiseGenerator4D<f32, i32>, new()
        {
            var noise = new G();
            var seed = F.Broad_i32(Seed);
            for (int x = 0; x < count; x += F.Count)
            {
                noise.Gen(seed, F.Broad_f32(x * Mul), default, default, default);
            }
        }
    }
}