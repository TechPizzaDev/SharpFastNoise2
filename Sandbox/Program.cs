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

namespace Sandbox;

class Program
{
    static void Main(string[] args)
    {
        string basePath = "NoiseTextures";

        int width = 1024;
        int height = 1024;
        int seed = 1234;
        float offsetX = 0;

        if (true)
        {
            WriteAll(basePath, seed, width, height, offsetX);
        }

        if (true)
        {
            string tileableBasePath = "TileableNoiseTextures";

            string path = Path.Combine(tileableBasePath, "CellularValue");
            WriteTileableCellularValue<Vector512<float>, Vector512<int>, Avx512Functions>(path, seed, width, height);
            WriteTileableCellularValue<Vector256<float>, Vector256<int>, Avx2Functions>(path, seed, width, height);
            WriteTileableCellularValue<Vector128<float>, Vector128<int>, Sse2Functions>(path, seed, width, height);
            WriteTileableCellularValue<float, int, ScalarFunctions>(path, seed, width, height);
        }
    }

    static void WriteTileableCellularValue<f32, i32, F>(string basePath, int seed, int width, int height)
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
            string dpath = Path.Combine(basePath, GetDistanceName(typeof(D)));
            WriteTileable<f32, i32, CellularValue<f32, i32, F, D>>(dpath, generator: new(), seed, width, height);
        }
    }

    static void WriteAll(string basePath, int seed, int width, int height, float offsetX)
    {
        if (true)
        {
            string path = Path.Combine(basePath, "CellularValue");
            WriteCellularValue<Vector512<float>, Vector512<int>, Avx512Functions>(path, seed, width, height, offsetX);
            WriteCellularValue<Vector256<float>, Vector256<int>, Avx2Functions>(path, seed, width, height, offsetX);
            WriteCellularValue<Vector128<float>, Vector128<int>, Sse2Functions>(path, seed, width, height, offsetX);
            WriteCellularValue<float, int, ScalarFunctions>(path, seed, width, height, offsetX);
        }

        if (true)
        {
            string path = Path.Combine(basePath, "Perlin");
            WritePerlin<Vector512<float>, Vector512<int>, Avx512Functions>(path, seed, width, height, offsetX);
            WritePerlin<Vector256<float>, Vector256<int>, Avx2Functions>(path, seed, width, height, offsetX);
            WritePerlin<Vector128<float>, Vector128<int>, Sse2Functions>(path, seed, width, height, offsetX);
            WritePerlin<float, int, ScalarFunctions>(path, seed, width, height, offsetX);
        }

        if (true)
        {
            string path = Path.Combine(basePath, "Simplex");
            WriteSimplex<Vector512<float>, Vector512<int>, Avx512Functions>(path, seed, width, height, offsetX);
            WriteSimplex<Vector256<float>, Vector256<int>, Avx2Functions>(path, seed, width, height, offsetX);
            WriteSimplex<Vector128<float>, Vector128<int>, Sse2Functions>(path, seed, width, height, offsetX);
            WriteSimplex<float, int, ScalarFunctions>(path, seed, width, height, offsetX);
        }

        if (true)
        {
            string path = Path.Combine(basePath, "OpenSimplex2");
            WriteOpenSimplex2<Vector512<float>, Vector512<int>, Avx512Functions>(path, seed, width, height, offsetX);
            WriteOpenSimplex2<Vector256<float>, Vector256<int>, Avx2Functions>(path, seed, width, height, offsetX);
            WriteOpenSimplex2<Vector128<float>, Vector128<int>, Sse2Functions>(path, seed, width, height, offsetX);
            WriteOpenSimplex2<float, int, ScalarFunctions>(path, seed, width, height, offsetX);
        }
    }

    private static void WriteCellularValue<f32, i32, F>(
        string basePath, int seed, int width, int height, float offsetX)
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
            string dpath = Path.Combine(basePath, GetDistanceName(typeof(D)));
            Write<f32, i32, F, CellularValue<f32, i32, F, D>>(dpath, generator: new(), seed, offsetX, width, height);
        }
    }

    private static void WritePerlin<f32, i32, F>(
        string basePath, int seed, int width, int height, float offsetX)
        where F : IFunctionList<f32, i32, F>
    {
        Write<f32, i32, F, Perlin<f32, i32, F>>(basePath, generator: new(), seed, offsetX, width, height);
    }

    private static void WriteSimplex<f32, i32, F>(
        string basePath, int seed, int width, int height, float offsetX)
        where F : IFunctionList<f32, i32, F>
    {
        Write<f32, i32, F, Simplex<f32, i32, F>>(basePath, generator: new(), seed, offsetX, width, height);
    }

    private static void WriteOpenSimplex2<f32, i32, F>(
        string basePath, int seed, int width, int height, float offsetX)
        where F : IFunctionList<f32, i32, F>
    {
        Write<f32, i32, F, OpenSimplex2<f32, i32, F>>(basePath, generator: new(), seed, offsetX, width, height);
    }

    static void Save(Image image, string? path)
    {
        if (path == null)
        {
            return;
        }

        string? parent = Path.GetDirectoryName(path);
        if (parent != null)
        {
            Directory.CreateDirectory(parent);
        }

        image.Save(path + ".png");
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
            string path = Path.Join(basePath, $"4Dx{G.UnitSize}");
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
            Save(image, path);
            w.Stop();
            Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
        }

        if (generator is INoiseGenerator3D<f32, i32> gen3D)
        {
            string path = Path.Join(basePath, $"3Dx{G.UnitSize}");
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
            Save(image, path);
            w.Stop();
            Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
        }

        if (generator is INoiseGenerator2D<f32, i32> gen2D)
        {
            string path = Path.Join(basePath, $"2Dx{G.UnitSize}");
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
            Save(image, path);
            w.Stop();
            Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
        }

        if (generator is INoiseGenerator1D<f32, i32> gen1D)
        {
            string path = Path.Join(basePath, $"1Dx{G.UnitSize}");
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
            Save(image, path);
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
        string path = Path.Join(basePath, $"x{G.UnitSize}");

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
        Save(image, path);
        w.Stop();
        Console.WriteLine($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");
    }

    static string GetDistanceName(Type type)
    {
        string name = type.Name;
        string prefix = "Distance";
        if (name.StartsWith(prefix))
        {
            name = name.Substring(prefix.Length);
        }

        int lastTick = name.LastIndexOf('`');
        if (lastTick != -1)
        {
            name = name.Substring(0, lastTick);
        }
        return name;
    }
}
