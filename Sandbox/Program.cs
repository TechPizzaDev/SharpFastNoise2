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

            if (true)
            {
                string simplexPath = Path.Combine(basePath, "Simplex");

                Write<FVectorI128, FVectorF128, FVectorI128, SseFunctions, Simplex<FVectorI128, FVectorF128, FVectorI128, SseFunctions>>(
                    basePath: simplexPath,
                    generator: new(),
                    seed: new FVectorI128(1234),
                    increment: new(0 / 32f, 1 / 32f, 2 / 32f, 3 / 32f),
                    256,
                    256);

                Write<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions, Simplex<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>>(
                    basePath: simplexPath,
                    generator: new(),
                    seed: new FVectorI32(1234),
                    increment: new(0 / 32f),
                    256,
                    256);
            }

            if (true)
            {
                string openSimplex2Path = Path.Combine(basePath, "OpenSimplex2");

                float off = 0;

                Write<FVectorI128, FVectorF128, FVectorI128, SseFunctions, OpenSimplex2<FVectorI128, FVectorF128, FVectorI128, SseFunctions>>(
                    basePath: openSimplex2Path,
                    generator: new(),
                    seed: new FVectorI128(1234),
                    increment: new(off / 32f, (off + 1) / 32f, (off + 2) / 32f, (off + 3) / 32f),
                    256,
                    256);

                Write<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions, OpenSimplex2<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>>(
                    basePath: openSimplex2Path,
                    generator: new(),
                    seed: new FVectorI32(1234),
                    increment: new(off / 32f),
                    256,
                    256);
            }
        }

        public static void Write<mask32v, float32v, int32v, TFunctions, TGen>(
            string basePath,
            TGen generator,
            int32v seed,
            float32v increment,
            int width,
            int height)
            where mask32v : unmanaged, IFMask<mask32v>
            where float32v : unmanaged, IFVector<float32v, mask32v>
            where int32v : unmanaged, IFVector<int32v, mask32v>
            where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
            where TGen : struct
        {
            TFunctions funcs = new();
            using Image<L16> image = new Image<L16>(width, height);

            if (generator is INoiseGenerator4D<mask32v, float32v, int32v> gen4D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = funcs.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += gen4D.Count)
                    {
                        float32v noise = gen4D.Gen(seed, funcs.Broad_f32(x / 32f).Add(increment), vy, default, default);

                        for (int i = 0; i < gen4D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((funcs.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_4Dx{gen4D.Count}.png");
            }

            if (generator is INoiseGenerator3D<mask32v, float32v, int32v> gen3D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = funcs.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += gen3D.Count)
                    {
                        float32v noise = gen3D.Gen(seed, funcs.Broad_f32(x / 32f).Add(increment), vy, default);

                        for (int i = 0; i < gen3D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((funcs.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_3Dx{gen3D.Count}.png");
            }

            if (generator is INoiseGenerator2D<mask32v, float32v, int32v> gen2D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    float32v vy = funcs.Broad_f32(y / 32f);

                    for (int x = 0; x < image.Width; x += gen2D.Count)
                    {
                        float32v noise = gen2D.Gen(seed, funcs.Broad_f32(x / 32f).Add(increment), vy);

                        for (int i = 0; i < gen2D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((funcs.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_2Dx{gen2D.Count}.png");
            }

            if (generator is INoiseGenerator1D<mask32v, float32v, int32v> gen1D)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x += gen1D.Count)
                    {
                        float32v noise = gen1D.Gen(seed, funcs.Broad_f32(x / 32f).Add(increment));

                        for (int i = 0; i < gen1D.Count; i++)
                        {
                            image[x + i, y] = new L16((ushort)((funcs.Extract_f32(noise, i) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }
                }

                image.Save(basePath + $"_1Dx{gen1D.Count}.png");
            }
        }
    }
}
