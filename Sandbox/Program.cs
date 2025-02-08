using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using SharpFastNoise2;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;
using SixLabors.ImageSharp;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            string basePath = "NoiseTextures";
            Directory.CreateDirectory(basePath);

            int seed = 1234;
            float offsetX = 0;

            if (true)
            {
                WriteAll(basePath, 1024, 1024, seed, offsetX);
            }

            if (true)
            {
                string tileableBasePath = "TileableNoiseTextures";
                Directory.CreateDirectory(tileableBasePath);

                string path = Path.Combine(tileableBasePath, "CellularValue_{0}");

                WriteTileable<
                    Vector512<float>, Vector512<int>,
                    CellularValue<Vector512<float>, Vector512<int>, Avx512Functions,
                        DistanceEuclideanEstimate<Vector512<float>, Vector512<int>, Avx512Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);

                WriteTileable<
                    Vector256<float>, Vector256<int>,
                    CellularValue<Vector256<float>, Vector256<int>, Avx2Functions,
                        DistanceEuclideanEstimate<Vector256<float>, Vector256<int>, Avx2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);

                WriteTileable<
                    Vector128<float>, Vector128<int>,
                    CellularValue<Vector128<float>, Vector128<int>, Sse2Functions,
                        DistanceEuclideanEstimate<Vector128<float>, Vector128<int>, Sse2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);

                WriteTileable<
                    float, int,
                    CellularValue<float, int, ScalarFunctions,
                        DistanceEuclideanEstimate<float, int, ScalarFunctions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);
            }
        }

        static void WriteAll(string basePath, int width, int height, int seed, float offsetX)
        {
            if (true)
            {
                string path = Path.Combine(basePath, $"CellularValue_{{0}}");
                WriteCellularValue<Vector512<float>, Vector512<int>, Avx512Functions>(path, width, height, seed, offsetX);
                WriteCellularValue<Vector256<float>, Vector256<int>, Avx2Functions>(path, width, height, seed, offsetX);
                WriteCellularValue<Vector128<float>, Vector128<int>, Sse2Functions>(path, width, height, seed, offsetX);
                WriteCellularValue<float, int, ScalarFunctions>(path, width, height, seed, offsetX);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "Perlin_{0}");
                WritePerlin<Vector512<float>, Vector512<int>, Avx512Functions>(path, width, height, seed, offsetX);
                WritePerlin<Vector256<float>, Vector256<int>, Avx2Functions>(path, width, height, seed, offsetX);
                WritePerlin<Vector128<float>, Vector128<int>, Sse2Functions>(path, width, height, seed, offsetX);
                WritePerlin<float, int, ScalarFunctions>(path, width, height, seed, offsetX);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "Simplex_{0}");
                WriteSimplex<Vector512<float>, Vector512<int>, Avx512Functions>(path, width, height, seed, offsetX);
                WriteSimplex<Vector256<float>, Vector256<int>, Avx2Functions>(path, width, height, seed, offsetX);
                WriteSimplex<Vector128<float>, Vector128<int>, Sse2Functions>(path, width, height, seed, offsetX);
                WriteSimplex<float, int, ScalarFunctions>(path, width, height, seed, offsetX);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "OpenSimplex2_{0}");
                WriteOpenSimplex2<Vector512<float>, Vector512<int>, Avx512Functions>(path, width, height, seed, offsetX);
                WriteOpenSimplex2<Vector256<float>, Vector256<int>, Avx2Functions>(path, width, height, seed, offsetX);
                WriteOpenSimplex2<Vector128<float>, Vector128<int>, Sse2Functions>(path, width, height, seed, offsetX);
                WriteOpenSimplex2<float, int, ScalarFunctions>(path, width, height, seed, offsetX);
            }
        }

        private static void WriteCellularValue<f32, i32, F>(
            string basePath, int width, int height, int seed, float offsetX)
            where F : IFunctionList<f32, i32, F>
        {
            WriteFor<DistanceEuclidean<f32, i32, F>>();
            WriteFor<DistanceEuclideanEstimate<f32, i32, F>>();
            WriteFor<DistanceEuclideanSquared<f32, i32, F>>();
            WriteFor<DistanceManhattan<f32, i32, F>>();
            WriteFor<DistanceHybrid<f32, i32, F>>();
            WriteFor<DistanceMaxAxis<f32, i32, F>>();

            void WriteFor<D>()
                where D : IDistanceFunction<f32, i32, F>
            {
                string dpath = basePath + $"_{typeof(D).Name}";
                Write<f32, i32, F, CellularValue<f32, i32, F, D>>(dpath, generator: new(), seed, offsetX, width, height);
            }
        }

        private static void WritePerlin<f32, i32, F>(
            string basePath, int width, int height, int seed, float offsetX)
            where F : IFunctionList<f32, i32, F>
        {
            Write<f32, i32, F, Perlin<f32, i32, F>>(basePath, generator: new(), seed, offsetX, width, height);
        }

        private static void WriteSimplex<f32, i32, F>(
            string basePath, int width, int height, int seed, float offsetX)
            where F : IFunctionList<f32, i32, F>
        {
            Write<f32, i32, F, Simplex<f32, i32, F>>(basePath, generator: new(), seed, offsetX, width, height);
        }

        private static void WriteOpenSimplex2<f32, i32, F>(
            string basePath, int width, int height, int seed, float offsetX)
            where F : IFunctionList<f32, i32, F>
        {
            Write<f32, i32, F, OpenSimplex2<f32, i32, F>>(basePath, generator: new(), seed, offsetX, width, height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void StoreUnit<f32, i32, F>(Span<L32> span, f32 unit)
            where F : IFunctionList<f32, i32, F>
        {
            f32 scaled = F.Mul(F.Add(unit, F.Broad(1f)), F.Broad(uint.MaxValue / 2 + 0.5f));
            for (int i = 0; i < F.Count; i++)
            {
                span[i] = new L32(L32.UpscaleF32ToU32(F.Extract(scaled, i)));
            }
        }

        public static void Write<f32, i32, F, G>(
            string basePath,
            G generator,
            int seed,
            float offsetX,
            int width,
            int height)
            where F : IFunctionList<f32, i32, F>
            where G : INoiseGeneratorAbstract
        {
            using Image<L32> image = new(width, height);
            Stopwatch w = new();

            var vSeed = F.Broad(seed);
            var vIncrementX = F.Div(F.Add(F.Incremented_f32(), F.Broad(offsetX)), F.Broad(32f));

            if (generator is INoiseGenerator4D<f32, i32> gen4D)
            {
                string path = string.Format(basePath, $"4Dx{G.UnitSize}") + ".png";
                Console.Write($"Generating \"{path}\" ");

                w.Restart();
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<L32> row = accessor.GetRowSpan(y);
                        f32 vy = F.Broad(y / 32f);

                        for (int x = 0; x < row.Length; x += G.UnitSize)
                        {
                            f32 vx = F.Add(F.Broad(x / 32f), vIncrementX);
                            f32 noise = gen4D.Gen(vx, vy, F.Broad(0f), F.Broad(0f), vSeed);
                            StoreUnit<f32, i32, F>(row.Slice(x, F.Count), noise);
                        }
                    }
                });
                w.Stop();
                Console.Write($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                image.Save(path);
                w.Stop();
                Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
            }

            if (generator is INoiseGenerator3D<f32, i32> gen3D)
            {
                string path = string.Format(basePath, $"3Dx{G.UnitSize}") + ".png";
                Console.Write($"Generating \"{path}\" ");

                w.Restart();
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<L32> row = accessor.GetRowSpan(y);
                        f32 vy = F.Broad(y / 32f);

                        for (int x = 0; x < image.Width; x += G.UnitSize)
                        {
                            f32 vx = F.Add(F.Broad(x / 32f), vIncrementX);
                            f32 noise = gen3D.Gen(vx, vy, F.Broad(0f), vSeed);
                            StoreUnit<f32, i32, F>(row.Slice(x, F.Count), noise);
                        }
                    }
                });
                w.Stop();
                Console.Write($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                image.Save(path);
                w.Stop();
                Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
            }

            if (generator is INoiseGenerator2D<f32, i32> gen2D)
            {
                string path = string.Format(basePath, $"2Dx{G.UnitSize}") + ".png";
                Console.Write($"Generating \"{path}\" ");

                w.Restart();
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<L32> row = accessor.GetRowSpan(y);
                        f32 vy = F.Broad(y / 32f);

                        for (int x = 0; x < image.Width; x += G.UnitSize)
                        {
                            f32 vx = F.Add(F.Broad(x / 32f), vIncrementX);
                            f32 noise = gen2D.Gen(vx, vy, vSeed);
                            StoreUnit<f32, i32, F>(row.Slice(x, F.Count), noise);
                        }
                    }
                });
                w.Stop();
                Console.Write($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                image.Save(path);
                w.Stop();
                Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
            }

            if (generator is INoiseGenerator1D<f32, i32> gen1D)
            {
                string path = string.Format(basePath, $"1Dx{G.UnitSize}") + ".png";
                Console.Write($"Generating \"{path}\" ");

                w.Restart();
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<L32> row = accessor.GetRowSpan(y);

                        for (int x = 0; x < image.Width; x += G.UnitSize)
                        {
                            f32 vx = F.Add(F.Broad(x / 32f), vIncrementX);
                            f32 noise = gen1D.Gen(vx, vSeed);
                            StoreUnit<f32, i32, F>(row.Slice(x, F.Count), noise);
                        }
                    }
                });
                w.Stop();
                Console.Write($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                image.Save(path);
                w.Stop();
                Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
            }
        }

        public static void WriteTileable<f32, i32, G>(
            string basePath,
            G generator,
            int seed,
            int width,
            int height)
            where G : INoiseGenerator4D<f32, i32>
        {
            float[] dst = new float[width * height];
            string path = string.Format(basePath, $"x{G.UnitSize}") + ".png";

            Console.Write($"Generating \"{path}\" ");
            Stopwatch w = new();

            w.Start();
            generator.GenTileable2D(dst.AsSpan(), width, height, 1f / 32f, seed);
            w.Stop();
            Console.Write($"({w.Elapsed.TotalMilliseconds,4:0.0}ms gen)");

            using Image<L32> image = new(width, height);
            w.Restart();
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<L32> row = accessor.GetRowSpan(y).Slice(0, width);
                    Span<float> dstSlice = dst.AsSpan(y * width, width);

                    const float scale = uint.MaxValue / 2 + 0.5f;
                    for (int x = 0; x < row.Length; x++)
                    {
                        row[x] = new L32(L32.UpscaleF32ToU32((dstSlice[x] + 1f) * scale));
                    }
                }
            });
            w.Stop();
            Console.Write($" ({w.Elapsed.TotalMilliseconds:0.0}ms convert)");

            w.Restart();
            image.Save(path);
            w.Stop();
            Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
        }
    }
}
