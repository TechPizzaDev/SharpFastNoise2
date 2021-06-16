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
                string path = Path.Combine(basePath, "Perlin");

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
                string path = Path.Combine(basePath, "Simplex");

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
                string path = Path.Combine(basePath, "OpenSimplex2");

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
            where TGen : struct
        {
            TFunctions F = new();
            using Image<L16> image = new Image<L16>(width, height);

            var vSeed = F.Broad_i32(seed);
            var vIncrementX = F.Div(F.Add(F.Incremented_f32(), F.Broad_f32(offsetX)), F.Broad_f32(32));

            if (generator is INoiseGenerator4D<float32v, int32v> gen4D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += gen4D.Count)
                    {
                        float32v noise = gen4D.Gen(vSeed, F.Add(F.Broad_f32(x / 32f), vIncrementX), vy, default, default);

                        for (int i = 0; i < gen4D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_4Dx{gen4D.Count}.png");
            }

            if (generator is INoiseGenerator3D<float32v, int32v> gen3D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += gen3D.Count)
                    {
                        float32v noise = gen3D.Gen(vSeed, F.Add(F.Broad_f32(x / 32f), vIncrementX), vy, default);

                        for (int i = 0; i < gen3D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_3Dx{gen3D.Count}.png");
            }

            if (generator is INoiseGenerator2D<float32v, int32v> gen2D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = F.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += gen2D.Count)
                    {
                        float32v noise = gen2D.Gen(vSeed, F.Add(F.Broad_f32(x / 32f), vIncrementX), vy);

                        for (int i = 0; i < gen2D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_2Dx{gen2D.Count}.png");
            }

            if (generator is INoiseGenerator1D<float32v, int32v> gen1D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x += gen1D.Count)
                    {
                        float32v noise = gen1D.Gen(vSeed, F.Add(F.Broad_f32(x / 32f), vIncrementX));

                        for (int i = 0; i < gen1D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((F.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_1Dx{gen1D.Count}.png");
            }
        }
    }
}
