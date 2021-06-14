using System;
using System.Diagnostics;
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
            //int hash = new Utils<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>().HashPrimes(1, 3, 4, 5);

            if (true)
            {
                var perlin32 = new Simplex<FVectorI32, FVectorF32, FVectorI32, ScalarFunctions>();

                if (true)
                {
                    using Image<L16> image32_4d = new Image<L16>(128, 128);

                    for (int y = 0; y < 128; y++)
                    {
                        for (int x = 0; x < 128; x++)
                        {
                            float noise = perlin32.Gen(1234, x / 32f, y / 32f, 0, 0);

                            image32_4d[x, y] = new L16((ushort)((noise + 1) * 0.5f * ushort.MaxValue));
                        }
                    }

                    image32_4d.Save("32_4d.png");
                }

                if (true)
                {
                    using Image<L16> image32_3d = new Image<L16>(128, 128);

                    for (int y = 0; y < 128; y++)
                    {
                        for (int x = 0; x < 128; x++)
                        {
                            float noise = perlin32.Gen(1234, x / 32f, y / 32f, 0);

                            image32_3d[x, y] = new L16((ushort)((noise + 1) * 0.5f * ushort.MaxValue));
                        }
                    }

                    image32_3d.Save("32_3d.png");
                }

                if (true)
                {
                    using Image<L16> image32 = new Image<L16>(128, 128);

                    for (int y = 0; y < 128; y++)
                    {
                        for (int x = 0; x < 128; x++)
                        {
                            float noise = perlin32.Gen(1234, x / 32f, y / 32f);

                            image32[x, y] = new L16((ushort)((noise + 1) * 0.5f * ushort.MaxValue));
                        }
                    }

                    image32.Save("32_2d.png");
                }
            }

            {
                var perlin128 = new Simplex<FVectorI128, FVectorF128, FVectorI128, SseFunctions>();

                if (true)
                {
                    using Image<L16> image128_4d = new Image<L16>(128, 128);

                    FVectorI128 seed = new(1234);
                    FVectorF128 increment = new(0 / 32f, 1 / 32f, 2 / 32f, 3 / 32f);

                    for (int y = 0; y < 128; y++)
                    {
                        FVectorF128 vy = new(y / 32f);

                        for (int x = 0; x < 128; x += 4)
                        {
                            FVectorF128 noise = perlin128.Gen(seed, new FVectorF128(x / 32f).Add(increment), vy, default, default);

                            image128_4d[x + 0, y] = new L16((ushort)((noise.Value.GetElement(0) + 1) * 0.5f * ushort.MaxValue));
                            image128_4d[x + 1, y] = new L16((ushort)((noise.Value.GetElement(1) + 1) * 0.5f * ushort.MaxValue));
                            image128_4d[x + 2, y] = new L16((ushort)((noise.Value.GetElement(2) + 1) * 0.5f * ushort.MaxValue));
                            image128_4d[x + 3, y] = new L16((ushort)((noise.Value.GetElement(3) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }

                    image128_4d.Save("128_4d.png");
                }

                if (true)
                {
                    using Image<L16> image128 = new Image<L16>(128, 128);

                    FVectorI128 seed = new(1234);
                    FVectorF128 increment = new(0 / 32f, 1 / 32f, 2 / 32f, 3 / 32f);

                    for (int y = 0; y < 128; y++)
                    {
                        FVectorF128 vy = new(y / 32f);

                        for (int x = 0; x < 128; x += 4)
                        {
                            FVectorF128 noise = perlin128.Gen(seed, new FVectorF128(x / 32f).Add(increment), vy, default);

                            image128[x + 0, y] = new L16((ushort)((noise.Value.GetElement(0) + 1) * 0.5f * ushort.MaxValue));
                            image128[x + 1, y] = new L16((ushort)((noise.Value.GetElement(1) + 1) * 0.5f * ushort.MaxValue));
                            image128[x + 2, y] = new L16((ushort)((noise.Value.GetElement(2) + 1) * 0.5f * ushort.MaxValue));
                            image128[x + 3, y] = new L16((ushort)((noise.Value.GetElement(3) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }

                    image128.Save("128_3d.png");
                }

                if (true)
                {
                    using Image<L16> image128 = new Image<L16>(128, 128);

                    FVectorI128 seed = new(1234);
                    FVectorF128 increment = new(0 / 32f, 1 / 32f, 2 / 32f, 3 / 32f);

                    for (int y = 0; y < 128; y++)
                    {
                        FVectorF128 vy = new(y / 32f);

                        for (int x = 0; x < 128; x += 4)
                        {
                            FVectorF128 noise = perlin128.Gen(seed, new FVectorF128(x / 32f).Add(increment), vy);

                            image128[x + 0, y] = new L16((ushort)((noise.Value.GetElement(0) + 1) * 0.5f * ushort.MaxValue));
                            image128[x + 1, y] = new L16((ushort)((noise.Value.GetElement(1) + 1) * 0.5f * ushort.MaxValue));
                            image128[x + 2, y] = new L16((ushort)((noise.Value.GetElement(2) + 1) * 0.5f * ushort.MaxValue));
                            image128[x + 3, y] = new L16((ushort)((noise.Value.GetElement(3) + 1) * 0.5f * ushort.MaxValue));
                        }
                    }

                    image128.Save("128_2d.png");
                }
            }
        }
    }
}
