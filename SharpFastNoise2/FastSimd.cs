using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct FastSimd<mask32v, float32v, int32v, TFunctions>
        where mask32v : unmanaged
        where float32v : unmanaged
        where int32v : unmanaged
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions F = default;

        // Masked float

        public float32v MaskedAdd_f32(float32v a, float32v b, mask32v m)
        {
            return F.Add(a, F.Mask_f32(b, m));
        }

        public float32v MaskedSub_f32(float32v a, float32v b, mask32v m)
        {
            return F.Sub(a, F.Mask_f32(b, m));
        }

        public float32v MaskedMul_f32(float32v a, float32v b, mask32v m)
        {
            return F.Mul(a, F.Mask_f32(b, m));
        }

        // Masked int32

        public int32v MaskedAdd_i32(int32v a, int32v b, mask32v m)
        {
            return F.Add(a, F.Mask_i32(b, m));
        }

        public int32v MaskedSub_i32(int32v a, int32v b, mask32v m)
        {
            return F.Sub(a, F.Mask_i32(b, m));
        }

        public int32v MaskedMul_i32(int32v a, int32v b, mask32v m)
        {
            return F.Mul(a, F.Mask_i32(b, m));
        }

        // NMasked float

        public float32v NMaskedAdd_f32(float32v a, float32v b, mask32v m)
        {
            return F.Add(a, F.NMask_f32(b, m));
        }

        public float32v NMaskedSub_f32(float32v a, float32v b, mask32v m)
        {
            return F.Sub(a, F.NMask_f32(b, m));
        }

        public float32v NMaskedMul_f32(float32v a, float32v b, mask32v m)
        {
            return F.Mul(a, F.NMask_f32(b, m));
        }

        // NMasked int32

        public int32v NMaskedAdd_i32(int32v a, int32v b, mask32v m)
        {
            return F.Add(a, F.NMask_i32(b, m));
        }

        public int32v NMaskedSub_i32(int32v a, int32v b, mask32v m)
        {
            return F.Sub(a, F.NMask_i32(b, m));
        }

        public int32v NMaskedMul_i32(int32v a, int32v b, mask32v m)
        {
            return F.Mul(a, F.NMask_i32(b, m));
        }

        public int32v MaskedIncrement_i32(int32v a, mask32v m)
        {
            if (typeof(int32v) == typeof(mask32v))
                return F.Sub(a, Unsafe.As<mask32v, int32v>(ref m));
            else
                return MaskedSub_i32(a, F.Broad_i32(-1), m);
        }

        public int32v MaskedDecrement_i32(int32v a, mask32v m)
        {
            if (typeof(int32v) == typeof(mask32v))
                return F.Add(a, Unsafe.As<mask32v, int32v>(ref m));
            else
                return MaskedAdd_i32(a, F.Broad_i32(-1), m);
        }

        // Trig

        public float32v Cos_f32(float32v value)
        {
            unchecked
            {
                value = F.Abs_f32(value);
                value = F.Sub(value, F.Mul(F.Floor_f32(F.Mul(value, F.Broad_f32(0.1591549f))), F.Broad_f32(6.283185f)));

                mask32v geHalfPi = F.GreaterThanOrEqual(value, F.Broad_f32(1.570796f));
                mask32v geHalfPi2 = F.GreaterThanOrEqual(value, F.Broad_f32(3.141593f));
                mask32v geHalfPi3 = F.GreaterThanOrEqual(value, F.Broad_f32(4.7123889f));

                float32v cosAngle = F.Xor(
                    value, F.Mask_f32(F.Xor(value, F.Sub(F.Broad_f32(3.141593f), value)), geHalfPi));

                cosAngle = F.Xor(
                    cosAngle, F.Mask_f32(F.Casti32_f32(F.Broad_i32((int)0x80000000)), geHalfPi2));

                cosAngle = F.Xor(
                    cosAngle, F.Mask_f32(F.Xor(cosAngle, F.Sub(F.Broad_f32(6.283185f), value)), geHalfPi3));

                cosAngle = F.Mul(cosAngle, cosAngle);

                cosAngle = F.FMulAdd_f32(
                    cosAngle, 
                    F.FMulAdd_f32(cosAngle, F.Broad_f32(0.03679168f), F.Broad_f32(-0.49558072f)),
                    F.Broad_f32(0.99940307f));

                return F.Xor(cosAngle, F.Mask_f32(
                    F.Casti32_f32(F.Broad_i32((int)0x80000000)), 
                    F.BitwiseAndNot_m32(geHalfPi, geHalfPi3)));
            }
        }

        public float32v Sin_f32(float32v value)
        {
            return Cos_f32(F.Sub(F.Broad_f32(1.570796f), value));
        }

        public float32v Exp_f32(float32v x)
        {
            x = F.Min_f32(x, F.Broad_f32(88.3762626647949f));
            x = F.Max_f32(x, F.Broad_f32(-88.3762626647949f));

            // express exp(x) as exp(g + n*log(2))
            float32v fx = F.Mul(x, F.Broad_f32(1.44269504088896341f));
            fx = F.Add(fx, F.Broad_f32(0.5f));

            float32v flr = F.Floor_f32(fx);
            fx = MaskedSub_f32(flr, F.Broad_f32(1), F.GreaterThan(flr, fx));

            x = F.Sub(x, F.Mul(fx, F.Broad_f32(0.693359375f)));
            x = F.Sub(x, F.Mul(fx, F.Broad_f32(-2.12194440e-4f)));

            float32v y = F.Broad_f32(1.9875691500E-4f);
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(1.3981999507E-3f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(8.3334519073E-3f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(4.1665795894E-2f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(1.6666665459E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(5.0000001201E-1f));
            y = F.Mul(y, F.Mul(x, x));
            y = F.Add(y, F.Add(x, F.Broad_f32(1)));

            // build 2^n
            int32v i = F.Convertf32_i32(fx);
            // another two AVX2 instructions
            i = F.Add(i, F.Broad_i32(0x7f));
            i = F.LeftShift(i, 23);
            float32v pow2n = F.Casti32_f32(i);

            return F.Mul(y, pow2n);
        }

        public float32v Log_f32(float32v x)
        {
            mask32v validMask = F.GreaterThan(x, F.Broad_f32(0));

            x = F.Max_f32(x, F.Casti32_f32(F.Broad_i32(0x00800000)));  // cut off denormalized stuff

            // can be done with AVX2
            int32v i = F.BitwiseShiftRightZX_i32(F.Castf32_i32(x), 23);

            // keep only the fractional part 
            x = F.And(x, F.Casti32_f32(F.Broad_i32(~0x7f800000)));
            x = F.Or(x, F.Broad_f32(0.5f));

            // this is again another AVX2 instruction
            i = F.Sub(i, F.Broad_i32(0x7f));
            float32v e = F.Converti32_f32(i);

            e = F.Add(e, F.Broad_f32(1));

            mask32v mask = F.LessThan(x, F.Broad_f32(0.707106781186547524f));
            x = MaskedAdd_f32(x, x, mask);
            x = F.Sub(x, F.Broad_f32(1));
            e = MaskedSub_f32(e, F.Broad_f32(1), mask);

            float32v y = F.Broad_f32(7.0376836292E-2f);
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(-1.1514610310E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(1.1676998740E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(-1.2420140846E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(1.4249322787E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(-1.6668057665E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(2.0000714765E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(-2.4999993993E-1f));
            y = F.Mul(y, x);
            y = F.Add(y, F.Broad_f32(3.3333331174E-1f));
            y = F.Mul(y, x);

            float32v xx = F.Mul(x, x);
            y = F.Mul(y, xx);
            y = F.Mul(y, F.Mul(e, F.Broad_f32(-2.12194440e-4f)));
            y = F.Sub(y, F.Mul(xx, F.Broad_f32(0.5f)));

            x = F.Add(x, y);
            x = F.Add(x, F.Mul(e, F.Broad_f32(0.693359375f)));

            return F.Mask_f32(x, validMask);
        }

        public float32v Pow_f32(float32v value, float32v pow)
        {
            return Exp_f32(F.Mul(pow, Log_f32(value)));
        }
    }
}
