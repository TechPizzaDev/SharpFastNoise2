using System;
using System.Runtime.CompilerServices;
using SharpFastNoise2.Functions;

namespace SharpFastNoise2
{
    public partial struct Utils<f32, i32, F>
        where F : IFunctionList<f32, i32, F>
    {
        // Trig

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (f32 Sign, f32 Y) SignSinCos(f32 value)
        {
            f32 tau = F.Broad(MathF.Tau);
            f32 y = F.Sub(value, F.Mul(tau, F.Round(F.Div(value, tau))));

            f32 halfPi = F.Broad(0.5f * MathF.PI);
            f32 signBit = F.Broad(-0f);
            f32 gHalfPi = F.GreaterThan(y, halfPi);
            f32 lHalfPi = F.LessThan(y, F.Xor(halfPi, signBit));

            f32 sign = F.Mask(signBit, F.Or(gHalfPi, lHalfPi));
            f32 yRhs = F.Xor(y, sign);

            f32 pi = F.Broad(MathF.PI);
            f32 yG = F.MaskAdd(yRhs, pi, gHalfPi);
            f32 yL = F.MaskSub(yRhs, pi, lHalfPi);

            f32 ySum = F.Select(lHalfPi, yL, yG);
            return (sign, ySum);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static f32 Cos(f32 y2, f32 sign)
        {
            // 10-degree minimax approximation
            f32 cosv = F.FMulAdd(F.Broad(-2.6051615e-07f), y2, F.Broad(2.4760495e-05f));
            cosv = F.FMulAdd(cosv, y2, F.Broad(-0.0013888378f));
            cosv = F.FMulAdd(cosv, y2, F.Broad(0.041666638f));
            cosv = F.FMulAdd(cosv, y2, F.Broad(-0.5f));
            cosv = F.Xor(F.FMulAdd(cosv, y2, F.Broad(1.0f)), sign);
            return cosv;
        }

        public static f32 Cos_f32(f32 value)
        {
            (f32 sign, f32 y) = SignSinCos(value);
            f32 y2 = F.Mul(y, y);
            f32 cosv = Cos(y2, sign);
            return cosv;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static f32 Sin(f32 y, f32 y2)
        {
            // 11-degree minimax approximation
            f32 sinv = F.FMulAdd(F.Broad(-2.3889859e-08f), y2, F.Broad(2.7525562e-06f));
            sinv = F.FMulAdd(sinv, y2, F.Broad(-0.00019840874f));
            sinv = F.FMulAdd(sinv, y2, F.Broad(0.0083333310f));
            sinv = F.FMulAdd(sinv, y2, F.Broad(-0.16666667f));
            sinv = F.Mul(y, F.FMulAdd(sinv, y2, F.Broad(1.0f)));
            return sinv;
        }

        public static f32 Sin_f32(f32 value)
        {
            (_, f32 y) = SignSinCos(value);
            f32 y2 = F.Mul(y, y);
            f32 sinv = Sin(y, y2);
            return sinv;
        }

        public static (f32 Sin, f32 Cos) SinCos_f32(f32 value)
        {
            (f32 sign, f32 y) = SignSinCos(value);
            f32 y2 = F.Mul(y, y);
            f32 sinv = Sin(y, y2);
            f32 cosv = Cos(y2, sign);
            return (sinv, cosv);
        }

        public static f32 Exp_f32(f32 x)
        {
            f32 limit = F.Broad(88.3762626647949f);
            x = F.Min(x, limit);
            x = F.Max(x, F.Negate(limit));

            // express exp(x) as exp(g + n*log(2))
            f32 fx = F.FMulAdd(x, F.Broad(1.44269504088896341f), F.Broad(0.5f));

            f32 flr = F.Floor(fx);
            fx = F.MaskSub(flr, F.Broad(1f), F.GreaterThan(flr, fx));

            x = F.FNMulAdd(fx, F.Broad(0.693359375f), x);
            x = F.FNMulAdd(fx, F.Broad(-2.12194440e-4f), x);

            f32 y = F.Broad(1.9875691500E-4f);
            y = F.FMulAdd(y, x, F.Broad(1.3981999507E-3f));
            y = F.FMulAdd(y, x, F.Broad(8.3334519073E-3f));
            y = F.FMulAdd(y, x, F.Broad(4.1665795894E-2f));
            y = F.FMulAdd(y, x, F.Broad(1.6666665459E-1f));
            y = F.FMulAdd(y, x, F.Broad(5.0000001201E-1f));
            y = F.FMulAdd(y, F.Mul(x, x), F.Add(x, F.Broad(1f)));

            // build 2^n
            i32 i = F.Convert_i32(fx);
            // another two AVX2 instructions
            i = F.Add(i, F.Broad(0x7f));
            i = F.LeftShift(i, 23);
            f32 pow2n = F.Cast_f32(i);

            return F.Mul(y, pow2n);
        }

        public static f32 Log_f32(f32 x)
        {
            f32 validMask = F.GreaterThan(x, F.Broad(0f));

            x = F.Max(x, F.Cast_f32(F.Broad(0x00800000)));  // cut off denormalized stuff

            // can be done with AVX2
            i32 i = F.ShiftRightLogical(F.Cast_i32(x), 23);

            // keep only the fractional part 
            x = F.And(x, F.Cast_f32(F.Broad(~0x7f800000)));
            x = F.Or(x, F.Broad(0.5f));

            // this is again another AVX2 instruction
            i = F.Sub(i, F.Broad(0x7f));
            f32 e = F.Convert_f32(i);

            f32 fc1 = F.Broad(1f);
            e = F.Add(e, fc1);

            f32 mask = F.LessThan(x, F.Broad(0.707106781186547524f));
            x = F.MaskAdd(x, x, mask);
            x = F.Sub(x, fc1);
            e = F.MaskSub(e, fc1, mask);

            f32 y = F.Broad(7.0376836292E-2f);
            y = F.FMulAdd(y, x, F.Broad(-1.1514610310E-1f));
            y = F.FMulAdd(y, x, F.Broad(1.1676998740E-1f));
            y = F.FMulAdd(y, x, F.Broad(-1.2420140846E-1f));
            y = F.FMulAdd(y, x, F.Broad(1.4249322787E-1f));
            y = F.FMulAdd(y, x, F.Broad(-1.6668057665E-1f));
            y = F.FMulAdd(y, x, F.Broad(2.0000714765E-1f));
            y = F.FMulAdd(y, x, F.Broad(-2.4999993993E-1f));
            y = F.FMulAdd(y, x, F.Broad(3.3333331174E-1f));
            y = F.Mul(y, x);

            f32 xx = F.Mul(x, x);
            y = F.Mul(y, xx);
            y = F.Mul(y, F.Mul(e, F.Broad(-2.12194440e-4f)));
            y = F.FNMulAdd(xx, F.Broad(0.5f), y);

            x = F.Add(x, y);
            x = F.FMulAdd(e, F.Broad(0.693359375f), x);

            return F.Mask(x, validMask);
        }

        public static f32 Pow_f32(f32 value, f32 pow)
        {
            return Exp_f32(F.Mul(pow, Log_f32(value)));
        }
    }
}
