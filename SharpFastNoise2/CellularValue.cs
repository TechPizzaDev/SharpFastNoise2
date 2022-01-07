using System;
using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct CellularValue<mask32v, float32v, int32v, TFunctions> :
        INoiseGenerator2D<float32v, int32v>,
        //INoiseGenerator3D<float32v, int32v>,
        INoiseGenerator4D<float32v, int32v>
        where mask32v : unmanaged
        where float32v : unmanaged
        where int32v : unmanaged
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private const int kMaxDistanceCount = 4;

        private static TFunctions F = default;
        private static Utils<mask32v, float32v, int32v, TFunctions> Utils = default;

        private int _valueIndex;

        public static int Count => TFunctions.Count;

        public DistanceFunction DistanceFunction { get; set; }

        public int ValueIndex
        {
            get => _valueIndex;
            set => _valueIndex = Math.Min(Math.Max(value, 0), kMaxDistanceCount - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public unsafe float32v Gen(int32v seed, float32v x, float32v y)
        {
            float32v jitter = F.Mul(F.Broad_f32(Cellular.kJitter2D), F.Broad_f32(1f)); //this->GetSourceValue(mJitterModifier, seed, x, y);
            float32v* value = stackalloc float32v[kMaxDistanceCount];
            float32v* distance = stackalloc float32v[kMaxDistanceCount];

            float32v initDistValue = F.Broad_f32(float.PositiveInfinity);
            for (int i = 0; i < kMaxDistanceCount; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            int32v xc = F.Add(F.Convertf32_i32(x), F.Broad_i32(-1));
            int32v ycBase = F.Add(F.Convertf32_i32(y), F.Broad_i32(-1));

            float32v xcf = F.Sub(F.Converti32_f32(xc), x);
            float32v ycfBase = F.Sub(F.Converti32_f32(ycBase), y);

            xc = F.Mul(xc, F.Broad_i32(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad_i32(Primes.Y));

            for (int xi = 0; xi < 3; xi++)
            {
                float32v ycf = ycfBase;
                int32v yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    int32v hash = Utils.HashPrimesHB(seed, xc, yc);
                    float32v xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad_i32(0xffff))), F.Broad_f32(0xffff / 2.0f));
                    float32v yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 16), F.Broad_i32(0xffff))), F.Broad_f32(0xffff / 2.0f));

                    float32v invMag = F.Mul(jitter, F.InvSqrt_f32(F.FMulAdd_f32(xd, xd, F.Mul(yd, yd))));
                    xd = F.FMulAdd_f32(xd, invMag, xcf);
                    yd = F.FMulAdd_f32(yd, invMag, ycf);

                    float32v newCellValue = F.Mul(F.Broad_f32((float)(1.0 / int.MaxValue)), F.Converti32_f32(hash));
                    float32v newDistance = Utils.CalcDistance(DistanceFunction, xd, yd);

                    for (int i = 0; ; i++)
                    {
                        mask32v closer = F.LessThan(newDistance, distance[i]);

                        float32v localDistance = distance[i];
                        float32v localCellValue = value[i];

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
        public unsafe float32v Gen(int32v seed, float32v x, float32v y, float32v z, float32v w)
        {
            float32v jitter = F.Mul(F.Broad_f32(Cellular.kJitter4D), F.Broad_f32(1)); //this->GetSourceValue(mJitterModifier, seed, x, y, z, w);
            float32v* value = stackalloc float32v[kMaxDistanceCount];
            float32v* distance = stackalloc float32v[kMaxDistanceCount];

            float32v initDistValue = F.Broad_f32(float.PositiveInfinity);
            for (int i = 0; i < kMaxDistanceCount; i++)
            {
                value[i] = initDistValue;
                distance[i] = initDistValue;
            }

            int32v xc = F.Add(F.Convertf32_i32(x), F.Broad_i32(-1));
            int32v ycBase = F.Add(F.Convertf32_i32(y), F.Broad_i32(-1));
            int32v zcBase = F.Add(F.Convertf32_i32(z), F.Broad_i32(-1));
            int32v wcBase = F.Add(F.Convertf32_i32(w), F.Broad_i32(-1));

            float32v xcf = F.Sub(F.Converti32_f32(xc), x);
            float32v ycfBase = F.Sub(F.Converti32_f32(ycBase), y);
            float32v zcfBase = F.Sub(F.Converti32_f32(zcBase), z);
            float32v wcfBase = F.Sub(F.Converti32_f32(wcBase), w);

            xc = F.Mul(xc, F.Broad_i32(Primes.X));
            ycBase = F.Mul(ycBase, F.Broad_i32(Primes.Y));
            zcBase = F.Mul(zcBase, F.Broad_i32(Primes.Z));
            wcBase = F.Mul(wcBase, F.Broad_i32(Primes.W));

            for (int xi = 0; xi < 3; xi++)
            {
                float32v ycf = ycfBase;
                int32v yc = ycBase;
                for (int yi = 0; yi < 3; yi++)
                {
                    float32v zcf = zcfBase;
                    int32v zc = zcBase;
                    for (int zi = 0; zi < 3; zi++)
                    {
                        float32v wcf = wcfBase;
                        int32v wc = wcBase;
                        for (int wi = 0; wi < 3; wi++)
                        {
                            int32v hash = Utils.HashPrimesHB(seed, xc, yc, zc, wc);
                            float32v xd = F.Sub(F.Converti32_f32(F.And(hash, F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));
                            float32v yd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 8), F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));
                            float32v zd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 16), F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));
                            float32v wd = F.Sub(F.Converti32_f32(F.And(F.RightShift(hash, 24), F.Broad_i32(0xff))), F.Broad_f32(0xff / 2.0f));

                            float32v invMag = F.Mul(jitter, F.InvSqrt_f32(
                                F.FMulAdd_f32(xd, xd, F.FMulAdd_f32(yd, yd, F.FMulAdd_f32(zd, zd, F.Mul(wd, wd))))));

                            xd = F.FMulAdd_f32(xd, invMag, xcf);
                            yd = F.FMulAdd_f32(yd, invMag, ycf);
                            zd = F.FMulAdd_f32(zd, invMag, zcf);
                            wd = F.FMulAdd_f32(wd, invMag, wcf);

                            float32v newCellValue = F.Mul(F.Broad_f32((float)(1.0 / int.MaxValue)), F.Converti32_f32(hash));
                            float32v newDistance = Utils.CalcDistance(DistanceFunction, xd, yd, zd, wd);

                            for (int i = 0; ; i++)
                            {
                                mask32v closer = F.LessThan(newDistance, distance[i]);

                                float32v localDistance = distance[i];
                                float32v localCellValue = value[i];

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
