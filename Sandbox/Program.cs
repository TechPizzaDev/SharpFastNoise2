using System;
using System.Diagnostics;
using System.IO;
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
                    CellularValue<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions,
                        DistanceEuclidean<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);

                WriteTileable<
                    Vector256<float>, Vector256<int>,
                    CellularValue<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                        DistanceEuclidean<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);

                WriteTileable<
                    Vector128<float>, Vector128<int>,
                    CellularValue<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                        DistanceEuclidean<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    1024,
                    1024);

                WriteTileable<
                    float, int,
                    CellularValue<int, float, int, ScalarFunctions,
                        DistanceEuclidean<int, float, int, ScalarFunctions>>>(
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

                float pi = MathF.PI;
                var noise = new CellularValue<int, float, int, ScalarFunctions,
                    DistanceEuclidean<int, float, int, ScalarFunctions>>();

                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        float s = x / 256f;
                        float t = y / 256f;

                        float nx = Utils<int, float, int, ScalarFunctions>.Cos_f32(s * 2 * pi) * dx / (2 * pi);
                        float ny = Utils<int, float, int, ScalarFunctions>.Cos_f32(t * 2 * pi) * dy / (2 * pi);
                        float nz = Utils<int, float, int, ScalarFunctions>.Sin_f32(s * 2 * pi) * dx / (2 * pi);
                        float nw = Utils<int, float, int, ScalarFunctions>.Sin_f32(t * 2 * pi) * dy / (2 * pi);

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
                    where D : IDistanceFunction<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>
                {
                    string dpath = Path.Combine(basePath, $"CellularValue_{{0}}_{typeof(D).Name}");

                    Write<
                        Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                        CellularValue<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions, D>>(
                        dpath,
                        generator: new(),
                        seed,
                        offsetX,
                        width,
                        height);
                }

                WriteDistanceFunc<DistanceEuclidean<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceEuclideanSquared<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceManhattan<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceHybrid<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>();
                WriteDistanceFunc<DistanceMaxAxis<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>();

                string path = Path.Combine(basePath, $"CellularValue_{{0}}");

                Write<
                    Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions,
                    CellularValue<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions,
                        DistanceEuclidean<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    CellularValue<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                        DistanceEuclidean<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    CellularValue<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                        DistanceEuclidean<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    int, float, int, ScalarFunctions,
                    CellularValue<int, float, int, ScalarFunctions,
                        DistanceEuclidean<int, float, int, ScalarFunctions>>>(
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
                    Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions,
                    Perlin<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    Perlin<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    Perlin<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    int, float, int, ScalarFunctions,
                    Perlin<int, float, int, ScalarFunctions>>(
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
                    Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions,
                    Simplex<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    Simplex<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    Simplex<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    int, float, int, ScalarFunctions,
                    Simplex<int, float, int, ScalarFunctions>>(
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
                    Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions,
                    OpenSimplex2<Vector512<int>, Vector512<float>, Vector512<int>, Avx512Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    OpenSimplex2<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    OpenSimplex2<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);

                Write<
                    int, float, int, ScalarFunctions,
                    OpenSimplex2<int, float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    width,
                    height);
            }
        }

        public static void Write<m32, f32, i32, F, G>(
            string basePath,
            G generator,
            int seed,
            float offsetX,
            int width,
            int height)
            where m32 : unmanaged
            where f32 : unmanaged
            where i32 : unmanaged
            where F : IFunctionList<m32, f32, i32, F>
            where G : INoiseGeneratorAbstract
        {
            using Image<L16> image = new(width, height);
            Stopwatch w = new();

            var vSeed = F.Broad_i32(seed);
            var vIncrementX = F.Div(F.Add(F.Incremented_f32(), F.Broad_f32(offsetX)), F.Broad_f32(32));

            if (generator is INoiseGenerator4D<f32, i32> gen4D)
            {
                string path = string.Format(basePath, $"4Dx{G.UnitSize}") + ".png";
                Console.Write($"Generating \"{path}\" ");

                w.Restart();
                for (int y = 0; y < image.Height; y++)
                {
                    f32 vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += G.UnitSize)
                    {
                        f32 vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        f32 noise = gen4D.Gen(vx, vy, default, default, vSeed);

                        for (int i = 0; i < G.UnitSize; i++)
                        {
                            image[x + i, y] = new L16((ushort) ((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

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
                for (int y = 0; y < image.Height; y++)
                {
                    f32 vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += G.UnitSize)
                    {
                        f32 vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        f32 noise = gen3D.Gen(vx, vy, default, vSeed);

                        for (int i = 0; i < G.UnitSize; i++)
                        {
                            image[x + i, y] = new L16((ushort) ((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

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
                for (int y = 0; y < image.Height; y++)
                {
                    f32 vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += G.UnitSize)
                    {
                        f32 vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        f32 noise = gen2D.Gen(vx, vy, vSeed);

                        for (int i = 0; i < G.UnitSize; i++)
                        {
                            image[x + i, y] = new L16((ushort) ((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

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
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x += G.UnitSize)
                    {
                        f32 vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        f32 noise = gen1D.Gen(vx, vSeed);

                        for (int i = 0; i < G.UnitSize; i++)
                        {
                            image[x + i, y] = new L16((ushort) ((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

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
            where f32 : unmanaged
            where i32 : unmanaged
            where G : INoiseGenerator4D<f32, i32>
        {
            float[] dst = new float[width * height];
            string path = string.Format(basePath, $"x{G.UnitSize}") + ".png";

            Console.Write("Generating ");
            Stopwatch w = new();

            w.Start();
            generator.GenTileable2D(dst.AsSpan(), width, height, 1f / 32f, seed);
            w.Stop();

            Console.Write($"({w.Elapsed.TotalMilliseconds,6:0.0}ms) \"{path}\" ");

            w.Restart();
            using Image<L16> image = new(width, height);

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<L16> row = accessor.GetRowSpan(y).Slice(0, width);
                    Span<float> dstSlice = dst.AsSpan(y * width, width);

                    for (int x = 0; x < row.Length; x++)
                    {
                        float value = dstSlice[x] + 1;
                        row[x] = new L16((ushort) (value * 0.5f * ushort.MaxValue));
                    }
                }
            });

            image.Save(path);
            w.Stop();

            Console.WriteLine($"({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
        }
    }
}
