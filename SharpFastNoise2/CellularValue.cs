using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Generators;

namespace SharpFastNoise2
{
    public struct CellularValue<m32, f32, i32, F> :
        INoiseGenerator2D<f32, i32>,
        //INoiseGenerator3D<f32, i32>,
        INoiseGenerator4D<f32, i32>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where F : unmanaged, IFunctionList<m32, f32, i32>
    {
        private const int kMaxDistanceCount = 4;

        private int _valueIndex;

        public static int Count => F.Count;

        public DistanceFunction DistanceFunction { get; set; }

        public int ValueIndex
        {
            get => _valueIndex;
            set => _valueIndex = Math.Min(Math.Max(value, 0), kMaxDistanceCount - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public unsafe f32 Gen(f32 x, f32 y, i32 seed)
        {
            f32 jitter = F.Mul(F.Broad_f32(Cellular.kJitter2D), F.Broad_f32(1f)); //this->GetSourceValue(mJitterModifier, seed, x, y);
            f32* value = stackalloc f32[kMaxDistanceCount];
            f32* distance = stackalloc f32[kMaxDistanceCount];

            f32 initDistValue = F.Broad_f32(float.PositiveInfinity);
            for (int i = 0; i < kMaxDistanceCount; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            i32 xc = F.Add(F.Convertf32_i32(x), F.Broad_i32(-1));
            i32 ycBase = F.Add(F.Convertf32_i32(y), F.Broad_i32(-1));

            f32 xcf = F.Sub(F.Converti32_f32(xc), x);
            f32 ycfBase = F.Sub(F.Converti32_f32(ycBase), y);

            xc = F.Mul(xc, F.Broad_i32(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad_i32(Primes.Y));

            for (int xi = 0; xi < 3; xi++)
            {
                f32 ycf = ycfBase;
                i32 yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    i32 hash = Utils<m32, f32, i32, F>.HashPrimesHB(seed, xc, yc);
                    f32 xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad_i32(0xffff))), F.Broad_f32(0xffff / 2.0f));
                    f32 yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 16), F.Broad_i32(0xffff))), F.Broad_f32(0xffff / 2.0f));

                    f32 invMag = F.Mul(jitter, F.InvSqrt_f32(F.FMulAdd_f32(xd, xd, F.Mul(yd, yd))));
                    xd = F.FMulAdd_f32(xd, invMag, xcf);
                    yd = F.FMulAdd_f32(yd, invMag, ycf);

                    f32 newCellValue = F.Mul(F.Broad_f32((float)(1.0 / int.MaxValue)), F.Converti32_f32(hash));
                    f32 newDistance = Utils<m32, f32, i32, F>.CalcDistance(DistanceFunction, xd, yd);

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

                    ycf = F.Add(ycf, F.Broad_f32(1));
                    yc = F.Add(yc, F.Broad_i32(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad_f32(1));
                xc = F.Add(xc, F.Broad_i32(Primes.X));
            }

            return value[_valueIndex];
        }

        //[MethodImpl(MethodImplOptions.AggressiveOptimization)]
        //public float32v Gen(int32v seed, float32v x, float32v y, float32v z)
        //{
        //}

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public unsafe f32 Gen(f32 x, f32 y, f32 z, f32 w, i32 seed)
        {
            f32 jitter = F.Mul(F.Broad_f32(Cellular.kJitter4D), F.Broad_f32(1)); //this->GetSourceValue(mJitterModifier, seed, x, y, z, w);
            f32* value = stackalloc f32[kMaxDistanceCount];
            f32* distance = stackalloc f32[kMaxDistanceCount];

            f32 initDistValue = F.Broad_f32(float.PositiveInfinity);
            for (int i = 0; i < kMaxDistanceCount; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            i32 xc = F.Add(F.Convertf32_i32(x), F.Broad_i32(-1));
            i32 ycBase = F.Add(F.Convertf32_i32(y), F.Broad_i32(-1));
            i32 zcBase = F.Add(F.Convertf32_i32(z), F.Broad_i32(-1));
            i32 wcBase = F.Add(F.Convertf32_i32(w), F.Broad_i32(-1));

            f32 xcf = F.Sub(F.Converti32_f32(xc), x);
            f32 ycfBase = F.Sub(F.Converti32_f32(ycBase), y);
            f32 zcfBase = F.Sub(F.Converti32_f32(zcBase), z);
            f32 wcfBase = F.Sub(F.Converti32_f32(wcBase), w);

            xc = F.Mul(xc, F.Broad_i32(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad_i32(Primes.Y));
            zcBase = F.Mul(zcBase, F.Broad_i32(Primes.Z));
            wcBase = F.Mul(wcBase, F.Broad_i32(Primes.W));

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
                            f32 xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));
                            f32 yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 8), F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));
                            f32 zd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 16), F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));
                            f32 wd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 24), F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));

                            f32 invMag = F.Mul(jitter, F.InvSqrt_f32(
                                F.FMulAdd_f32(xd, xd, F.FMulAdd_f32(yd, yd, F.FMulAdd_f32(zd, zd, F.Mul(wd, wd))))));

                            xd = F.FMulAdd_f32(xd, invMag, xcf);
                            yd = F.FMulAdd_f32(yd, invMag, ycf);
                            zd = F.FMulAdd_f32(zd, invMag, zcf);
                            wd = F.FMulAdd_f32(wd, invMag, wcf);

                            f32 newCellValue = F.Mul(F.Broad_f32((float)(1.0 / int.MaxValue)), F.Converti32_f32(hash));
                            f32 newDistance = Utils<m32, f32, i32, F>.CalcDistance(DistanceFunction, xd, yd, zd, wd);

                            for (int i = 0; ; i++)
                            {
                                m32 closer = F.LessThan(newDistance, distance[i]);

                                f32 localDistance = distance[i];
                                f32 localCellValue = value[i];

                                distance[i] = F.Select_f32(closer, newDistance, distance[i]);
                                value[i] = F.Select_f32(closer, newCellValue, value[i]);

                                if (i > ValueIndex)
                                {
                                    break;
                                }

                                newDistance = F.Select_f32(closer, localDistance, newDistance);
                                newCellValue = F.Select_f32(closer, localCellValue, newCellValue);
                            }

                            wcf = F.Add(wcf, F.Broad_f32(1));
                            wc = F.Add(wc, F.Broad_i32(Primes.W));
                        }
                        zcf = F.Add(zcf, F.Broad_f32(1));
                        zc = F.Add(zc, F.Broad_i32(Primes.Z));
                    }
                    ycf = F.Add(ycf, F.Broad_f32(1));
                    yc = F.Add(yc, F.Broad_i32(Primes.Y));
                }
                xcf = F.Add(xcf, F.Broad_f32(1));
                xc = F.Add(xc, F.Broad_i32(Primes.X));
            }

            return value[ValueIndex];
        }
    }
}
