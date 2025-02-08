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
using SixLabors.ImageSharp.PixelFormats;

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

            if (false)
            {
                using var img = new Image<L16>(256, 256);

                float dx = 8;
                float dy = 8;

                const float pi = MathF.PI;
                var noise = new CellularValue<float, int, ScalarFunctions,
                    DistanceEuclideanEstimate<float, int, ScalarFunctions>>();

                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        float s = x / 256f;
                        float t = y / 256f;

                        (float sSin, float sCos) = Utils<float, int, ScalarFunctions>.SinCos_f32(s * 2 * pi);
                        (float tSin, float tCos) = Utils<float, int, ScalarFunctions>.SinCos_f32(t * 2 * pi);

                        float nx = sCos * dx / (2 * pi);
                        float ny = tCos * dy / (2 * pi);
                        float nz = sSin * dx / (2 * pi);
                        float nw = tSin * dy / (2 * pi);

                        img[x, y] = new L16((ushort) ((noise.Gen(nx, ny, nz, nw, 1234) + 1f) * 0.5f * ushort.MaxValue));
                    }
                }

                img.Save("what.png");
            }
        }

        static void WriteAll(string basePath, int width, int height, int seed, float offsetX)
        {
            if (true)
            {
                void WriteDistanceFunc<D>()
                    where D : IDistanceFunction<Vector256<float>, Vector256<int>, Avx2Functions>
                {
                    string dpath = Path.Combine(basePath, $"CellularValue_{{0}}_{typeof(D).Name}");

                    Write<
                        Vector256<float>, Vector256<int>, Avx2Functions,
                        CellularValue<Vector256<float>, Vector256<int>, Avx2Functions, D>>(
                        dpath,
                        generator: new(),
                        seed,
                        offsetX,
                        width,
                        height);
                }

                WriteDistanceFunc<DistanceEuclidean<Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceEuclideanEstimate<Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceEuclideanSquared<Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceManhattan<Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceHybrid<Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceMaxAxis<Vector256<float>, Vector256<int>, Avx2Functions>>();

                string path = Path.Combine(basePath, $"CellularValue_{{0}}");

                Write<
                    Vector512<float>, Vector512<int>, Avx512Functions,
                    CellularValue<Vector512<float>, Vector512<int>, Avx512Functions,
                        DistanceEuclideanEstimate<Vector512<float>, Vector512<int>, Avx512Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<float>, Vector256<int>, Avx2Functions,
                    CellularValue<Vector256<float>, Vector256<int>, Avx2Functions,
                        DistanceEuclideanEstimate<Vector256<float>, Vector256<int>, Avx2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<float>, Vector128<int>, Sse2Functions,
                    CellularValue<Vector128<float>, Vector128<int>, Sse2Functions,
                        DistanceEuclideanEstimate<Vector128<float>, Vector128<int>, Sse2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    float, int, ScalarFunctions,
                    CellularValue<float, int, ScalarFunctions,
                        DistanceEuclideanEstimate<float, int, ScalarFunctions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "Perlin_{0}");

                Write<
                    Vector512<float>, Vector512<int>, Avx512Functions,
                    Perlin<Vector512<float>, Vector512<int>, Avx512Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<float>, Vector256<int>, Avx2Functions,
                    Perlin<Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<float>, Vector128<int>, Sse2Functions,
                    Perlin<Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    float, int, ScalarFunctions,
                    Perlin<float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "Simplex_{0}");

                Write<
                    Vector512<float>, Vector512<int>, Avx512Functions,
                    Simplex<Vector512<float>, Vector512<int>, Avx512Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<float>, Vector256<int>, Avx2Functions,
                    Simplex<Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<float>, Vector128<int>, Sse2Functions,
                    Simplex<Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    float, int, ScalarFunctions,
                    Simplex<float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "OpenSimplex2_{0}");

                Write<
                    Vector512<float>, Vector512<int>, Avx512Functions,
                    OpenSimplex2<Vector512<float>, Vector512<int>, Avx512Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<float>, Vector256<int>, Avx2Functions,
                    OpenSimplex2<Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<float>, Vector128<int>, Sse2Functions,
                    OpenSimplex2<Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    float, int, ScalarFunctions,
                    OpenSimplex2<float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);
            }
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
