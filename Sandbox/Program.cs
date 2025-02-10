using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using SharpFastNoise2;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;
using ShellProgressBar;
using SixLabors.ImageSharp;

namespace Sandbox;

class State
{
    public readonly TaskFactory TaskFactory;

    public int Seed = 1234;
    public int Width = 1024;
    public int Height = 1024;

    public float OffsetX = 0;

    public State(TaskFactory taskFactory)
    {
        TaskFactory = taskFactory;
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        string basePath = "NoiseTextures";

        var limitedScheduler = new LimitedConcurrencyLevelTaskScheduler(Math.Max(1, Environment.ProcessorCount / 2));

        State state = new(new TaskFactory(limitedScheduler));

        using ProgressBar topProgress = new(2, "Main", new ProgressBarOptions()
        {
            CollapseWhenFinished = true,
        });

        if (true)
        {
            using ChildProgressBar progress = topProgress.Spawn(4, basePath);
            await WriteAll(basePath, progress, state);
            topProgress.Tick();
        }

        if (true)
        {
            string tileableBasePath = "TileableNoiseTextures";

            string path = Path.Combine(tileableBasePath, "CellularValue");
            using ChildProgressBar progress = topProgress.Spawn(4, path);
            await WriteTileableCellularValue<Vector512<float>, Vector512<int>, Avx512Functions>(path, progress, state);
            await WriteTileableCellularValue<Vector256<float>, Vector256<int>, Avx2Functions>(path, progress, state);
            await WriteTileableCellularValue<Vector128<float>, Vector128<int>, Sse2Functions>(path, progress, state);
            await WriteTileableCellularValue<float, int, ScalarFunctions>(path, progress, state);
            topProgress.Tick();
        }
    }

    static async Task WriteTileableCellularValue<f32, i32, F>(string basePath, ProgressBarBase topProgress, State state)
        where F : IFunctionList<f32, i32, F>
    {
        using ChildProgressBar progress = topProgress.Spawn(6, "Distance");
        var tasks = new[]
        {
            WriteFor<DistanceEuclidean<f32, i32, F>>(),
            WriteFor<DistanceEuclideanEstimate<f32, i32, F>>(),
            WriteFor<DistanceEuclideanSquared<f32, i32, F>>(),
            WriteFor<DistanceManhattan<f32, i32, F>>(),
            WriteFor<DistanceHybrid<f32, i32, F>>(),
            WriteFor<DistanceMaxAxis<f32, i32, F>>()
        };
        await Task.WhenAll(tasks);
        topProgress.Tick();

        Task WriteFor<D>()
            where D : IDistanceFunction<f32, i32, F>
        {
            string dpath = Path.Combine(basePath, GetDistanceName(typeof(D)));
            return WriteTileable<f32,i32,CellularValue<f32,i32,F,D>>(dpath,progress,state,generator: new());
        }
    }

    static async Task WriteAll(string basePath, ProgressBarBase topProgress, State state)
    {
        if (true)
        {
            string path = Path.Combine(basePath, "CellularValue");
            using ChildProgressBar progress = topProgress.Spawn(4, path);
            await WriteCellularValue<Vector512<float>,Vector512<int>,Avx512Functions>(path,progress,state);
            await WriteCellularValue<Vector256<float>,Vector256<int>,Avx2Functions>(path,progress,state);
            await WriteCellularValue<Vector128<float>,Vector128<int>,Sse2Functions>(path,progress,state);
            await WriteCellularValue<float,int,ScalarFunctions>(path,progress,state);
            topProgress.Tick();
        }

        if (true)
        {
            string path = Path.Combine(basePath, "Perlin");
            using ChildProgressBar progress = topProgress.Spawn(4, path);
            var tasks = new[]
            {
                WritePerlin<Vector512<float>, Vector512<int>, Avx512Functions>(path, progress, state),
                WritePerlin<Vector256<float>, Vector256<int>, Avx2Functions>(path, progress, state),
                WritePerlin<Vector128<float>, Vector128<int>, Sse2Functions>(path, progress, state),
                WritePerlin<float, int, ScalarFunctions>(path, progress, state)
            };
            await Task.WhenAll(tasks);
            topProgress.Tick();
        }

        if (true)
        {
            string path = Path.Combine(basePath, "Simplex");
            using ChildProgressBar progress = topProgress.Spawn(4, path);
            var tasks = new[]
            {
                WriteSimplex<Vector512<float>, Vector512<int>, Avx512Functions>(path, progress, state),
                WriteSimplex<Vector256<float>, Vector256<int>, Avx2Functions>(path, progress, state),
                WriteSimplex<Vector128<float>, Vector128<int>, Sse2Functions>(path, progress, state),
                WriteSimplex<float, int, ScalarFunctions>(path, progress, state)
            };
            await Task.WhenAll(tasks);
            topProgress.Tick();
        }

        if (true)
        {
            string path = Path.Combine(basePath, "OpenSimplex2");
            using ChildProgressBar progress = topProgress.Spawn(4, path);
            var tasks = new[]
            {
                WriteOpenSimplex2<Vector512<float>, Vector512<int>, Avx512Functions>(path, progress, state),
                WriteOpenSimplex2<Vector256<float>, Vector256<int>, Avx2Functions>(path, progress, state),
                WriteOpenSimplex2<Vector128<float>, Vector128<int>, Sse2Functions>(path, progress, state),
                WriteOpenSimplex2<float, int, ScalarFunctions>(path, progress, state)
            };
            await Task.WhenAll(tasks);
            topProgress.Tick();
        }
    }

    private static async Task WriteCellularValue<f32, i32, F>(string basePath, ProgressBarBase topProgress, State state)
        where F : IFunctionList<f32, i32, F>
    {
        using ChildProgressBar progress = topProgress.Spawn(6, "Distance");
        var tasks = new[]
        {
            WriteFor<DistanceEuclidean<f32, i32, F>>(),
            WriteFor<DistanceEuclideanEstimate<f32, i32, F>>(),
            WriteFor<DistanceEuclideanSquared<f32, i32, F>>(),
            WriteFor<DistanceManhattan<f32, i32, F>>(),
            WriteFor<DistanceHybrid<f32, i32, F>>(),
            WriteFor<DistanceMaxAxis<f32, i32, F>>()
        };

        await Task.WhenAll(tasks);
        topProgress.Tick();

        Task WriteFor<D>()
            where D : IDistanceFunction<f32, i32, F>
        {
            string dpath = Path.Combine(basePath, GetDistanceName(typeof(D)));
            return Write<f32, i32, F, CellularValue<f32, i32, F, D>>(dpath, progress, state, generator: new());
        }
    }

    private static Task WritePerlin<f32, i32, F>(string basePath, ProgressBarBase progress, State state)
        where F : IFunctionList<f32, i32, F>
    {
        return Write<f32, i32, F, Perlin<f32, i32, F>>(basePath, progress, state, generator: new());
    }

    private static Task WriteSimplex<f32, i32, F>(string basePath, ProgressBarBase progress, State state)
        where F : IFunctionList<f32, i32, F>
    {
        return Write<f32, i32, F, Simplex<f32, i32, F>>(basePath, progress, state, generator: new());
    }

    private static Task WriteOpenSimplex2<f32, i32, F>(string basePath, ProgressBarBase progress, State state)
        where F : IFunctionList<f32, i32, F>
    {
        return Write<f32, i32, F, OpenSimplex2<f32, i32, F>>(basePath, progress, state, generator: new());
    }

    static async Task Save(Image image, string? path)
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

        await image.SaveAsync(path + ".png");
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

    public static async Task Write<f32, i32, F, G>(
        string basePath,
        ProgressBarBase progress,
        State state,
        G generator)
        where F : IFunctionList<f32, i32, F>
        where G : INoiseGeneratorAbstract
    {
        int seed = state.Seed;
        int width = state.Width;
        int height = state.Height;

        var tasks = new List<Task>();

        var vSeed = F.Broad(seed);
        var vIncrementX = F.Div(F.Add(F.Incremented_f32(), F.Broad(state.OffsetX)), F.Broad(32f));

        if (generator is INoiseGenerator4D<f32, i32> gen4D)
        {
            string path = Path.Join(basePath, $"4Dx{G.UnitSize}");
            ChildProgressBar pBar = progress.Spawn(state.Height, $"Generating \"{path}\" ");

            tasks.Add(state.TaskFactory.StartNew(async () =>
            {
                using Image<L32> image = new(width, height);

                Stopwatch w = new();
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
                        pBar.Tick();
                    }
                });
                w.Stop();
                pBar.Tick($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                await Save(image, path);
                w.Stop();
                pBar.Tick($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");

                pBar.Dispose();
            }));
        }

        if (generator is INoiseGenerator3D<f32, i32> gen3D)
        {
            string path = Path.Join(basePath, $"3Dx{G.UnitSize}");
            ChildProgressBar pBar = progress.Spawn(state.Height, $"Generating \"{path}\" ");

            tasks.Add(state.TaskFactory.StartNew(async () =>
            {
                using Image<L32> image = new(width, height);

                Stopwatch w = new();
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
                        pBar.Tick();
                    }
                });
                w.Stop();
                pBar.Tick($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                await Save(image, path);
                w.Stop();
                pBar.Tick($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");

                pBar.Dispose();
            }));
        }

        if (generator is INoiseGenerator2D<f32, i32> gen2D)
        {
            string path = Path.Join(basePath, $"2Dx{G.UnitSize}");
            ChildProgressBar pBar = progress.Spawn(state.Height, $"Generating \"{path}\" ");

            tasks.Add(state.TaskFactory.StartNew(async () =>
            {
                using Image<L32> image = new(width, height);

                Stopwatch w = new();
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
                        pBar.Tick();
                    }
                });
                w.Stop();
                pBar.Tick($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                await Save(image, path);
                w.Stop();
                pBar.Tick($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");

                pBar.Dispose();
            }));
        }

        if (generator is INoiseGenerator1D<f32, i32> gen1D)
        {
            string path = Path.Join(basePath, $"1Dx{G.UnitSize}");
            ChildProgressBar pBar = progress.Spawn(state.Height, $"Generating \"{path}\" ");

            tasks.Add(state.TaskFactory.StartNew(async () =>
            {
                using Image<L32> image = new(width, height);

                Stopwatch w = new();
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
                        pBar.Tick();
                    }
                });
                w.Stop();
                pBar.Tick($"({w.Elapsed.TotalMilliseconds,4:0.0}ms)");

                w.Restart();
                await Save(image, path);
                w.Stop();
                pBar.Tick($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");

                pBar.Dispose();
            }));
        }

        await Task.WhenAll(tasks);
        progress.Tick();
    }

    public static async Task WriteTileable<f32, i32, G>(
        string basePath,
        ProgressBarBase progress,
        State state,
        G generator)
        where G : INoiseGenerator4D<f32, i32>
    {
        string path = Path.Join(basePath, $"x{G.UnitSize}");
        ChildProgressBar pBar = progress.SpawnIndeterminate($"Generating \"{path}\" ");

        Task task = state.TaskFactory.StartNew(async () =>
        {
            int seed = state.Seed;
            int width = state.Width;
            int height = state.Height;
            float[] dst = new float[width * height];

            Stopwatch w = new();
            w.Start();
            generator.GenTileable2D(dst.AsSpan(), width, height, 1f / 32f, seed);
            w.Stop();
            pBar.Tick($"({w.Elapsed.TotalMilliseconds,4:0.0}ms gen)");

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
            pBar.Tick($" ({w.Elapsed.TotalMilliseconds:0.0}ms convert)");

            w.Restart();
            await Save(image, path);
            w.Stop();
            pBar.Tick($" ({w.Elapsed.TotalMilliseconds,4:0.0}ms encode)");

            pBar.Dispose();
        });

        await task;
        progress.Tick();
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
