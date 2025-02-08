using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpFastNoise2.Distance;
using SharpFastNoise2.Functions;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    [InlineArray(Count)]
    [StructLayout(LayoutKind.Sequential)]
    file struct DistanceArray<T>
    {
        public const int Count = 4;

        private T _e0;
    }

    public struct CellularValue<f32, i32, F, D> :
        INoiseGenerator2D<f32, i32>,
        INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where F : IFunctionList<f32, i32, F>
        where D : IDistanceFunction<f32, i32, F>
    {
        private int _valueIndex;

        public static int UnitSize => F.Count;

        readonly int INoiseGenerator.UnitSize => F.Count;

        public int ValueIndex
        {
            readonly get => _valueIndex;
            set => _valueIndex = Math.Min(Math.Max(value, 0), DistanceArray<f32>.Count - 1);
        }

        public readonly f32 Gen(f32 x, f32 y, i32 seed)
        {
            DistanceArray<f32> valueArray = new();
            DistanceArray<f32> distanceArray = new();

            int count = _valueIndex + 1;
            Span<f32> values = valueArray[..count];
            Span<f32> distances = distanceArray[..count];

            f32 initDistValue = F.Broad(float.PositiveInfinity);
            for (int i = 0; i < count; i++)
            {
                values[i] = initDistValue;
                distances[i] = initDistValue;
            }

            f32 jitter = F.Mul(F.Broad(Cellular.kJitter2D), F.Broad(1f)); //this->GetSourceValue(mJitterModifier, seed, x, y);
            Gen(values, distances, jitter, seed, x, y);

            return values[count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public readonly void Gen(Span<f32> values, Span<f32> distances, f32 jitter, i32 seed, f32 x, f32 y)
        {
            i32 icn1 = F.Broad(-1);
            i32 xc = F.Add(F.Convert_i32(x), icn1);
            i32 ycBase = F.Add(F.Convert_i32(y), icn1);

            f32 xcf = F.Sub(F.Convert_f32(xc), x);
            f32 ycfBase = F.Sub(F.Convert_f32(ycBase), y);

            xc = F.Mul(xc, F.Broad(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad(Primes.Y));

            for (int xi = 0; xi < 3; xi++)
            {
                f32 ycf = ycfBase;
                i32 yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    i32 hash = Utils<f32, i32, F>.HashPrimesHB(seed, xc, yc);
                    f32 halfMask = F.Broad(0xffff / 2.0f);
                    f32 xd = F.Sub(F.Convert_f32(F.And(hash, F.Broad(0xffff))), halfMask);
                    f32 yd = F.Sub(F.Convert_f32(F.RightShift(hash, 16)), halfMask);

                    f32 invMag = F.Mul(jitter, F.ReciprocalSqrt(F.FMulAdd(xd, xd, F.Mul(yd, yd))));
                    xd = F.FMulAdd(xd, invMag, xcf);
                    yd = F.FMulAdd(yd, invMag, ycf);

                    f32 newDistance = D.CalcDistance(xd, yd);
                    SelectCell(values, distances, hash, newDistance);

                    ycf = F.Add(ycf, F.Broad(1f));
                    yc = F.Add(yc, F.Broad(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad(1f));
                xc = F.Add(xc, F.Broad(Primes.X));
            }
        }

        public readonly f32 Gen(f32 x, f32 y, f32 z, i32 seed)
        {
            DistanceArray<f32> valueArray = new();
            DistanceArray<f32> distanceArray = new();

            int count = _valueIndex + 1;
            Span<f32> values = valueArray[..count];
            Span<f32> distances = distanceArray[..count];

            f32 initDistValue = F.Broad(float.PositiveInfinity);
            for (int i = 0; i < count; i++)
            {
                values[i] = initDistValue;
                distances[i] = initDistValue;
            }

            f32 jitter = F.Mul(F.Broad(Cellular.kJitter3D), F.Broad(1f)); //this->GetSourceValue(mJitterModifier, seed, x, y, z);
            Gen(values, distances, jitter, seed, x, y, z);

            return values[count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public readonly void Gen(Span<f32> values, Span<f32> distances, f32 jitter, i32 seed, f32 x, f32 y, f32 z)
        {
            i32 icn1 = F.Broad(-1);
            i32 xc = F.Add(F.Convert_i32(x), icn1);
            i32 ycBase = F.Add(F.Convert_i32(y), icn1);
            i32 zcBase = F.Add(F.Convert_i32(z), icn1);

            f32 xcf = F.Sub(F.Convert_f32(xc), x);
            f32 ycfBase = F.Sub(F.Convert_f32(ycBase), y);
            f32 zcfBase = F.Sub(F.Convert_f32(zcBase), z);

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
                        i32 hash = Utils<f32, i32, F>.HashPrimesHB(seed, xc, yc, zc);
                        i32 mask = F.Broad(0x3ff);
                        f32 halfMask = F.Broad(0x3ff / 2.0f);
                        f32 xd = F.Sub(F.Convert_f32(F.And(hash, mask)), halfMask);
                        f32 yd = F.Sub(F.Convert_f32(F.And(F.RightShift(hash, 10), mask)), halfMask);
                        f32 zd = F.Sub(F.Convert_f32(F.And(F.RightShift(hash, 20), mask)), halfMask);

                        f32 invMag = F.Mul(jitter, F.ReciprocalSqrt(F.FMulAdd(xd, xd, F.FMulAdd(yd, yd, F.Mul(zd, zd)))));
                        xd = F.FMulAdd(xd, invMag, xcf);
                        yd = F.FMulAdd(yd, invMag, ycf);
                        zd = F.FMulAdd(zd, invMag, zcf);

                        f32 newDistance = D.CalcDistance(xd, yd, zd);
                        SelectCell(values, distances, hash, newDistance);

                        zcf = F.Add(zcf, F.Broad(1f));
                        zc = F.Add(zc, F.Broad(Primes.Z));
                    }
                    ycf = F.Add(ycf, F.Broad(1f));
                    yc = F.Add(yc, F.Broad(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad(1f));
                xc = F.Add(xc, F.Broad(Primes.X));
            }
        }

        public readonly f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
        {
            DistanceArray<f32> valueArray = new();
            DistanceArray<f32> distanceArray = new();

            int count = _valueIndex + 1;
            Span<f32> values = valueArray[..count];
            Span<f32> distances = distanceArray[..count];

            f32 initDistValue = F.Broad(float.PositiveInfinity);
            for (int i = 0; i < count; i++)
            {
                values[i] = initDistValue;
                distances[i] = initDistValue;
            }

            f32 jitter = F.Mul(F.Broad(Cellular.kJitter4D), F.Broad(1f)); //this->GetSourceValue(mJitterModifier, seed, x, y, z, w);
            Gen(values, distances, jitter, seed, x, y, z, w);

            return values[count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public readonly void Gen(Span<f32> values, Span<f32> distances, f32 jitter, i32 seed, f32 x, f32 y, f32 z, f32 w)
        {
            i32 icn1 = F.Broad(-1);
            i32 xc = F.Add(F.Convert_i32(x), icn1);
            i32 ycBase = F.Add(F.Convert_i32(y), icn1);
            i32 zcBase = F.Add(F.Convert_i32(z), icn1);
            i32 wcBase = F.Add(F.Convert_i32(w), icn1);

            f32 xcf = F.Sub(F.Convert_f32(xc), x);
            f32 ycfBase = F.Sub(F.Convert_f32(ycBase), y);
            f32 zcfBase = F.Sub(F.Convert_f32(zcBase), z);
            f32 wcfBase = F.Sub(F.Convert_f32(wcBase), w);

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
                            i32 hash = Utils<f32, i32, F>.HashPrimesHB(seed, xc, yc, zc, wc);
                            i32 mask = F.Broad(0xff);
                            f32 halfMask = F.Broad(0xff / 2.0f);
                            f32 xd = F.Sub(F.Convert_f32(F.And(hash, mask)), halfMask);
                            f32 yd = F.Sub(F.Convert_f32(F.And(F.RightShift(hash, 8), mask)), halfMask);
                            f32 zd = F.Sub(F.Convert_f32(F.And(F.RightShift(hash, 16), mask)), halfMask);
                            f32 wd = F.Sub(F.Convert_f32(F.RightShift(hash, 24)), halfMask);

                            f32 invMag = F.Mul(jitter, F.ReciprocalSqrt(
                                F.FMulAdd(xd, xd, F.FMulAdd(yd, yd, F.FMulAdd(zd, zd, F.Mul(wd, wd))))));

                            xd = F.FMulAdd(xd, invMag, xcf);
                            yd = F.FMulAdd(yd, invMag, ycf);
                            zd = F.FMulAdd(zd, invMag, zcf);
                            wd = F.FMulAdd(wd, invMag, wcf);

                            f32 newDistance = D.CalcDistance(xd, yd, zd, wd);
                            SelectCell(values, distances, hash, newDistance);

                            wcf = F.Add(wcf, F.Broad(1f));
                            wc = F.Add(wc, F.Broad(Primes.W));
                        }
                        zcf = F.Add(zcf, F.Broad(1f));
                        zc = F.Add(zc, F.Broad(Primes.Z));
                    }
                    ycf = F.Add(ycf, F.Broad(1f));
                    yc = F.Add(yc, F.Broad(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad(1f));
                xc = F.Add(xc, F.Broad(Primes.X));
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void SelectCell(Span<f32> values, Span<f32> distances, i32 hash, f32 distance)
        {
            f32 newDistance = distance;
            f32 newValue = F.Mul(F.Broad((float) (1.0 / int.MaxValue)), F.Convert_f32(hash));

            for (int i = 0; i < values.Length && i < distances.Length; i++)
            {
                f32 localDistance = distances[i];
                f32 localCellValue = values[i];

                f32 closer = F.LessThan(newDistance, localDistance);

                distances[i] = F.Select(closer, newDistance, localDistance);
                values[i] = F.Select(closer, newValue, localCellValue);

                newDistance = F.Select(closer, localDistance, newDistance);
                newValue = F.Select(closer, localCellValue, newValue);
            }
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
            return NoiseGenerator2DHelper.GenUniformGrid<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator3DHelper.GenUniformGrid<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator4DHelper.GenUniformGrid<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator2DHelper.GenPositionArray<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator3DHelper.GenPositionArray<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator4DHelper.GenPositionArray<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator2DHelper.GenSingle<f32, i32, F, CellularValue<f32, i32, F, D>>(
                ref this,
                x,
                y,
                seed);
        }

        public float GenSingle3D(float x, float y, float z, int seed)
        {
            return NoiseGenerator3DHelper.GenSingle<f32, i32, F, CellularValue<f32, i32, F, D>>(
                ref this,
                x,
                y,
                z,
                seed);
        }

        public float GenSingle4D(float x, float y, float z, float w, int seed)
        {
            return NoiseGenerator4DHelper.GenSingle<f32, i32, F, CellularValue<f32, i32, F, D>>(
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
            return NoiseGenerator4DHelper.GenTileable<f32, i32, F, CellularValue<f32, i32, F, D>>(
                ref this,
                destination,
                xSize,
                ySize,
                frequency,
                seed);
        }
    }
}
