using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    [InlineArray(Count)]
    file struct DistanceArray<T>
    {
        public const int Count = 4;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0051 // Remove unused private members
        private T _e0;
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore IDE0044 // Add readonly modifier
    }

    public struct CellularValue<m32, f32, i32, F, D> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : IFunctionList<m32, f32, i32, F>
        where D : IDistanceFunction<m32, f32, i32, F>
    {
        private int _valueIndex;

        public static int UnitSize => F.Count;

        readonly int INoiseGenerator.UnitSize => F.Count;

        public int ValueIndex
        {
            get => _valueIndex;
            set => _valueIndex = Math.Min(Math.Max(value, 0), DistanceArray<f32>.Count - 1);
        }

        public f32 Gen(f32 x, f32 y, i32 seed)
        {
            f32 jitter = F.Mul(F.Broad(Cellular.kJitter2D), F.Broad(1f)); //this->GetSourceValue(mJitterModifier, seed, x, y);
            DistanceArray<f32> value = new();
            DistanceArray<f32> distance = new();

            f32 initDistValue = F.Broad(float.PositiveInfinity);
            for (int i = 0; i < DistanceArray<f32>.Count; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            i32 xc = F.Add(F.Convertf32_i32(x), F.Broad(-1));
            i32 ycBase = F.Add(F.Convertf32_i32(y), F.Broad(-1));

            f32 xcf = F.Sub(F.Converti32_f32(xc), x);
            f32 ycfBase = F.Sub(F.Converti32_f32(ycBase), y);

            xc = F.Mul(xc, F.Broad(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad(Primes.Y));

            for (int xi = 0; xi < 3; xi++)
            {
                f32 ycf = ycfBase;
                i32 yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    i32 hash = Utils<m32, f32, i32, F>.HashPrimesHB(seed, xc, yc);
                    f32 xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad(0xffff))), F.Broad(0xffff / 2.0f));
                    f32 yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 16), F.Broad(0xffff))), F.Broad(0xffff / 2.0f));

                    f32 invMag = F.Mul(jitter, F.InvSqrt_f32(F.FMulAdd_f32(xd, xd, F.Mul(yd, yd))));
                    xd = F.FMulAdd_f32(xd, invMag, xcf);
                    yd = F.FMulAdd_f32(yd, invMag, ycf);

                    f32 newCellValue = F.Mul(F.Broad((float) (1.0 / int.MaxValue)), F.Converti32_f32(hash));
                    f32 newDistance = D.CalcDistance(xd, yd);

                    for (int i = 0; ; i++)
                    {
                        m32 closer = F.LessThan(newDistance, distance[i]);

                        f32 localDistance = distance[i];
                        f32 localCellValue = value[i];

                        distance[i] = F.Select_f32(closer, newDistance, distance[i]);
                        value[i] = F.Select_f32(closer, newCellValue, value[i]);

                        if (i > _valueIndex)
                        {
                            break;
                        }

                        newDistance = F.Select_f32(closer, localDistance, newDistance);
                        newCellValue = F.Select_f32(closer, localCellValue, newCellValue);
                    }

                    ycf = F.Add(ycf, F.Broad((float)1));
                    yc = F.Add(yc, F.Broad(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad((float)1));
                xc = F.Add(xc, F.Broad(Primes.X));
            }

            return value[_valueIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            f32 jitter = F.Mul(F.Broad(Cellular.kJitter3D), F.Broad((float)1)); //this->GetSourceValue(mJitterModifier, seed, x, y, z);
            DistanceArray<f32> value = new();
            DistanceArray<f32> distance = new();

            f32 initDistValue = F.Broad(float.PositiveInfinity);
            for (int i = 0; i < DistanceArray<f32>.Count; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            i32 xc = F.Add(F.Convertf32_i32(x), F.Broad(-1));
            i32 ycBase = F.Add(F.Convertf32_i32(y), F.Broad(-1));
            i32 zcBase = F.Add(F.Convertf32_i32(z), F.Broad(-1));

            f32 xcf = F.Sub(F.Converti32_f32(xc), x);
            f32 ycfBase = F.Sub(F.Converti32_f32(ycBase), y);
            f32 zcfBase = F.Sub(F.Converti32_f32(zcBase), z);

            xc = F.Mul(xc, F.Broad(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad(Primes.Y));
            zcBase = F.Mul(zcBase, F.Broad(Primes.Z));

            for (int xi = 0; xi < 3; xi++)
            {
                f32 ycf = ycfBase;
                i32 yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    f32 zcf = zcfBase;
                    i32 zc = zcBase;
                    for (int zi = 0; zi < 3; zi++)
                    {
                        i32 hash = Utils<m32, f32, i32, F>.HashPrimesHB(seed, xc, yc, zc);
                        f32 xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad(0x3ff))), F.Broad(0x3ff / 2.0f));
                        f32 yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 10), F.Broad(0x3ff))), F.Broad(0x3ff / 2.0f));
                        f32 zd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 20), F.Broad(0x3ff))), F.Broad(0x3ff / 2.0f));

                        f32 invMag = F.Mul(jitter, F.InvSqrt_f32(F.FMulAdd_f32(xd, xd, F.FMulAdd_f32(yd, yd, F.Mul(zd, zd)))));
                        xd = F.FMulAdd_f32(xd, invMag, xcf);
                        yd = F.FMulAdd_f32(yd, invMag, ycf);
                        zd = F.FMulAdd_f32(zd, invMag, zcf);

                        f32 newCellValue = F.Mul(F.Broad((float) (1.0 / int.MaxValue)), F.Converti32_f32(hash));
                        f32 newDistance = D.CalcDistance(xd, yd, zd);

                        for (int i = 0; ; i++)
                        {
                            m32 closer = F.LessThan(newDistance, distance[i]);

                            f32 localDistance = distance[i];
                            f32 localCellValue = value[i];

                            distance[i] = F.Select_f32(closer, newDistance, distance[i]);
                            value[i] = F.Select_f32(closer, newCellValue, value[i]);

                            if (i > _valueIndex)
                            {
                                break;
                            }

                            newDistance = F.Select_f32(closer, localDistance, newDistance);
                            newCellValue = F.Select_f32(closer, localCellValue, newCellValue);
                        }

                        zcf = F.Add(zcf, F.Broad((float)1));
                        zc = F.Add(zc, F.Broad(Primes.Z));
                    }
                    ycf = F.Add(ycf, F.Broad((float)1));
                    yc = F.Add(yc, F.Broad(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad((float)1));
                xc = F.Add(xc, F.Broad(Primes.X));
            }

            return value[_valueIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
        {
            f32 jitter = F.Mul(F.Broad(Cellular.kJitter4D), F.Broad((float)1)); //this->GetSourceValue(mJitterModifier, seed, x, y, z, w);
            DistanceArray<f32> value = new();
            DistanceArray<f32> distance = new();

            f32 initDistValue = F.Broad(float.PositiveInfinity);
            for (int i = 0; i < DistanceArray<f32>.Count; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            i32 xc = F.Add(F.Convertf32_i32(x), F.Broad(-1));
            i32 ycBase = F.Add(F.Convertf32_i32(y), F.Broad(-1));
            i32 zcBase = F.Add(F.Convertf32_i32(z), F.Broad(-1));
            i32 wcBase = F.Add(F.Convertf32_i32(w), F.Broad(-1));

            f32 xcf = F.Sub(F.Converti32_f32(xc), x);
            f32 ycfBase = F.Sub(F.Converti32_f32(ycBase), y);
            f32 zcfBase = F.Sub(F.Converti32_f32(zcBase), z);
            f32 wcfBase = F.Sub(F.Converti32_f32(wcBase), w);

            xc = F.Mul(xc, F.Broad(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad(Primes.Y));
            zcBase = F.Mul(zcBase, F.Broad(Primes.Z));
            wcBase = F.Mul(wcBase, F.Broad(Primes.W));

            for (int xi = 0; xi < 3; xi++)
            {
                f32 ycf = ycfBase;
                i32 yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    f32 zcf = zcfBase;
                    i32 zc = zcBase;
                    for (int zi = 0; zi < 3; zi++)
                    {
                        f32 wcf = wcfBase;
                        i32 wc = wcBase;
                        for (int wi = 0; wi < 3; wi++)
                        {
                            i32 hash = Utils<m32, f32, i32, F>.HashPrimesHB(seed, xc, yc, zc, wc);
                            f32 xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad(0xff))), F.Broad(0xff / 2.0f));
                            f32 yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 8), F.Broad(0xff))), F.Broad(0xff / 2.0f));
                            f32 zd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 16), F.Broad(0xff))), F.Broad(0xff / 2.0f));
                            f32 wd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 24), F.Broad(0xff))), F.Broad(0xff / 2.0f));

                            f32 invMag = F.Mul(jitter, F.InvSqrt_f32(
                                F.FMulAdd_f32(xd, xd, F.FMulAdd_f32(yd, yd, F.FMulAdd_f32(zd, zd, F.Mul(wd, wd))))));

                            xd = F.FMulAdd_f32(xd, invMag, xcf);
                            yd = F.FMulAdd_f32(yd, invMag, ycf);
                            zd = F.FMulAdd_f32(zd, invMag, zcf);
                            wd = F.FMulAdd_f32(wd, invMag, wcf);

                            f32 newCellValue = F.Mul(F.Broad((float) (1.0 / int.MaxValue)), F.Converti32_f32(hash));
                            f32 newDistance = D.CalcDistance(xd, yd, zd, wd);

                            for (int i = 0; ; i++)
                            {
                                m32 closer = F.LessThan(newDistance, distance[i]);

                                f32 localDistance = distance[i];
                                f32 localCellValue = value[i];

                                distance[i] = F.Select_f32(closer, newDistance, distance[i]);
                                value[i] = F.Select_f32(closer, newCellValue, value[i]);

                                if (i > _valueIndex)
                                {
                                    break;
                                }

                                newDistance = F.Select_f32(closer, localDistance, newDistance);
                                newCellValue = F.Select_f32(closer, localCellValue, newCellValue);
                            }

                            wcf = F.Add(wcf, F.Broad((float)1));
                            wc = F.Add(wc, F.Broad(Primes.W));
                        }
                        zcf = F.Add(zcf, F.Broad((float)1));
                        zc = F.Add(zc, F.Broad(Primes.Z));
                    }
                    ycf = F.Add(ycf, F.Broad((float)1));
                    yc = F.Add(yc, F.Broad(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad((float)1));
                xc = F.Add(xc, F.Broad(Primes.X));
            }

            return value[_valueIndex];
        }

        public OutputMinMax GenUniformGrid2D(
            Span<float> destination,
            int xStart,
            int yStart,
            int xSize,
            int ySize,
            float frequency,
            int seed)
        {
            return NoiseGenerator2DHelper.GenUniformGrid<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xStart,
                yStart,
                xSize,
                ySize,
                frequency,
                seed);
        }

        public OutputMinMax GenUniformGrid3D(
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int xSize,
            int ySize,
            int zSize,
            float frequency,
            int seed)
        {
            return NoiseGenerator3DHelper.GenUniformGrid<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xStart,
                yStart,
                zStart,
                xSize,
                ySize,
                zSize,
                frequency,
                seed);
        }

        public OutputMinMax GenUniformGrid4D(
            Span<float> destination,
            int xStart,
            int yStart,
            int zStart,
            int wStart,
            int xSize,
            int ySize,
            int zSize,
            int wSize,
            float frequency,
            int seed)
        {
            return NoiseGenerator4DHelper.GenUniformGrid<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xStart,
                yStart,
                zStart,
                wStart,
                xSize,
                ySize,
                zSize,
                wSize,
                frequency,
                seed);
        }

        public OutputMinMax GenPositionArray2D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            float xOffset,
            float yOffset,
            int seed)
        {
            return NoiseGenerator2DHelper.GenPositionArray<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xPosArray,
                yPosArray,
                xOffset,
                yOffset,
                seed);
        }

        public OutputMinMax GenPositionArray3D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            int seed)
        {
            return NoiseGenerator3DHelper.GenPositionArray<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xPosArray,
                yPosArray,
                zPosArray,
                xOffset,
                yOffset,
                zOffset,
                seed);
        }

        public OutputMinMax GenPositionArray4D(
            Span<float> destination,
            ReadOnlySpan<float> xPosArray,
            ReadOnlySpan<float> yPosArray,
            ReadOnlySpan<float> zPosArray,
            ReadOnlySpan<float> wPosArray,
            float xOffset,
            float yOffset,
            float zOffset,
            float wOffset,
            int seed)
        {
            return NoiseGenerator4DHelper.GenPositionArray<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xPosArray,
                yPosArray,
                zPosArray,
                wPosArray,
                xOffset,
                yOffset,
                zOffset,
                wOffset,
                seed);
        }

        public float GenSingle2D(float x, float y, int seed)
        {
            return NoiseGenerator2DHelper.GenSingle<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                x,
                y,
                seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return NoiseGenerator3DHelper.GenSingle<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                x,
                y,
                z,
                seed);
        }

        public float GenSingle4D(float x, float y, float z, float w, int seed)
        {
            return NoiseGenerator4DHelper.GenSingle<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                x,
                y,
                z,
                w,
                seed);
        }

        public OutputMinMax GenTileable2D(
            Span<float> destination,
            int xSize,
            int ySize,
            float frequency,
            int seed)
        {
            return NoiseGenerator4DHelper.GenTileable<m32, f32, i32, F, CellularValue<m32, f32, i32, F, D>>(
                ref this,
                destination,
                xSize,
                ySize,
                frequency,
                seed);
        }
    }
}
