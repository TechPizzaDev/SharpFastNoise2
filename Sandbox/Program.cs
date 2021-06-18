using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2;
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
                WriteAll(basePath, seed, offsetX);
            }

            if (true)
            {
                string tileableBasePath = "TileableNoiseTextures";
                Directory.CreateDirectory(tileableBasePath);

                string path = Path.Combine(tileableBasePath, "CellularValue_{0}");

                WriteTileable<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    CellularValue<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    256,
                    256);

                WriteTileable<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    CellularValue<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    256,
                    256);
                
                WriteTileable<
                    int, float, int, ScalarFunctions,
                    CellularValue<int, float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    256,
                    256);
            }

            if (false)
            {
                using var img = new Image<L16>(256, 256);

                float dx = 8;
                float dy = 8;

                float pi = MathF.PI;
                var noise = new CellularValue<int, float, int, ScalarFunctions>();
                var fss = new FastSimd<int, float, int, ScalarFunctions>();

                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        float s = x / 256f;
                        float t = y / 256f;

                        float nx = fss.Cos_f32(s * 2 * pi) * dx / (2 * pi);
                        float ny = fss.Cos_f32(t * 2 * pi) * dy / (2 * pi);
                        float nz = fss.Sin_f32(s * 2 * pi) * dx / (2 * pi);
                        float nw = fss.Sin_f32(t * 2 * pi) * dy / (2 * pi);

                        img[x, y] = new L16((ushort)((noise.Gen(1234, nx, ny, nz, nw) + 1f) * 0.5f * ushort.MaxValue));
                    }
                }

                img.Save("what.png");
            }
        }

        static void WriteAll(string basePath, int seed, float offsetX)
        {
            if (true)
            {
                var distFuncs = Enum.GetValues<DistanceFunction>();

                foreach (DistanceFunction distFunc in distFuncs)
                {
                    string dpath = Path.Combine(basePath, $"CellularValue_{{0}}_{distFunc}");

                    Write<
                        Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                        CellularValue<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                        dpath,
                        generator: new() { DistanceFunction = distFunc },
                        seed,
                        offsetX,
                        256,
                        256);
                }

                string path = Path.Combine(basePath, $"CellularValue_{{0}}");

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    CellularValue<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);

                Write<
                    int, float, int, ScalarFunctions,
                    CellularValue<int, float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);
            }
            
            if (true)
            {
                string path = Path.Combine(basePath, "Perlin_{0}");

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    Perlin<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    Perlin<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);

                Write<
                    int, float, int, ScalarFunctions,
                    Perlin<int, float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "Simplex_{0}");

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    Simplex<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);

                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    Simplex<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);

                Write<
                    int, float, int, ScalarFunctions,
                    Simplex<int, float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);
            }

            if (true)
            {
                string path = Path.Combine(basePath, "OpenSimplex2_{0}");

                Write<
                    Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions,
                    OpenSimplex2<Vector256<int>, Vector256<float>, Vector256<int>, Avx2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);


                Write<
                    Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions,
                    OpenSimplex2<Vector128<int>, Vector128<float>, Vector128<int>, Sse2Functions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);

                Write<
                    int, float, int, ScalarFunctions,
                    OpenSimplex2<int, float, int, ScalarFunctions>>(
                    path,
                    generator: new(),
                    seed,
                    offsetX,
                    256,
                    256);
            }
        }

        public static void Write<mask32v, float32v, int32v, TFunctions, TGen>(
            string basePath,
            TGen generator,
            int seed,
            float offsetX,
            int width,
            int height)
            where mask32v : unmanaged
            where float32v : unmanaged
            where int32v : unmanaged
            where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
            where TGen : INoiseGenerator
        {
            TFunctions F = new();
            using Image<L16> image = new Image<L16>(width, height);

            var vSeed = F.Broad_i32(seed);
            var vIncrementX = F.Div(F.Add(F.Incremented_f32(), F.Broad_f32(offsetX)), F.Broad_f32(32));

            if (generator is INoiseGenerator4D<float32v, int32v> gen4D)
            {
                string path = string.Format(basePath, $"4Dx{generator.Count}") + ".png";
                Console.WriteLine("Writing " + path);

                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += generator.Count)
                    {
                        float32v vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        float32v noise = gen4D.Gen(vSeed, vx, vy, default, default);

                        for (int i = 0; i < generator.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(path);
            }

            if (generator is INoiseGenerator3D<float32v, int32v> gen3D)
            {
                string path = string.Format(basePath, $"3Dx{generator.Count}") + ".png";
                Console.WriteLine("Writing " + path);

                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += generator.Count)
                    {
                        float32v vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        float32v noise = gen3D.Gen(vSeed, vx, vy, default);

                        for (int i = 0; i < generator.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(path);
            }

            if (generator is INoiseGenerator2D<float32v, int32v> gen2D)
            {
                string path = string.Format(basePath, $"2Dx{generator.Count}") + ".png";
                Console.WriteLine("Writing " + path);

                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += generator.Count)
                    {
                        float32v vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        float32v noise = gen2D.Gen(vSeed, vx, vy);

                        for (int i = 0; i < generator.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(path);
            }

            if (generator is INoiseGenerator1D<float32v, int32v> gen1D)
            {
                string path = string.Format(basePath, $"1Dx{generator.Count}") + ".png";
                Console.WriteLine("Writing " + path);

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x += generator.Count)
                    {
                        float32v vx = F.Add(F.Broad_f32(x / 32f), vIncrementX);
                        float32v noise = gen1D.Gen(vSeed, vx);

                        for (int i = 0; i < generator.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(path);
            }
        }


        public static void WriteTileable<mask32v, float32v, int32v, TFunctions, TGen>(
            string basePath,
            TGen generator,
            int seed,
            int width,
            int height)
            where mask32v : unmanaged
            where float32v : unmanaged
            where int32v : unmanaged
            where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
            where TGen : INoiseGenerator4D<float32v, int32v>
        {
            float[] dst = new float[width * height];

            generator.GenTileable2D<mask32v, float32v, int32v, TFunctions, TGen>(
                dst.AsSpan(), width, height, 1f / 32f, seed);

            using Image<L16> image = new Image<L16>(width, height);

            string path = string.Format(basePath, $"x{generator.Count}") + ".png";
            Console.WriteLine("Writing " + path);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    float value = dst[y * image.Width + x] + 1;
                    image[x, y] = new L16((ushort)(value * 0.5f * ushort.MaxValue));
                }
            }

            image.Save(path);
        }
    }
}
