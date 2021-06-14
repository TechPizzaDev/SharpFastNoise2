using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct FastSimd<mask32v, float32v, int32v, TFunctions>
        where mask32v : unmanaged, IFMask<mask32v>
        where float32v : unmanaged, IFVector<float32v, mask32v>
        where int32v : unmanaged, IFVector<int32v, mask32v>
        where TFunctions : unmanaged, IFunctionList<mask32v, float32v, int32v>
    {
        private static TFunctions FS;

        // Masked float

        public float32v MaskedAdd_f32(float32v a, float32v b, mask32v m)
        {
            return a.Add(FS.Mask_f32(b, m));
        }

        public float32v MaskedSub_f32(float32v a, float32v b, mask32v m)
        {
            return a.Sub(FS.Mask_f32(b, m));
        }

        public float32v MaskedMul_f32(float32v a, float32v b, mask32v m)
        {
            return a.Mul(FS.Mask_f32(b, m));
        }

        // Masked int32

        public int32v MaskedAdd_i32(int32v a, int32v b, mask32v m)
        {
            return a.Add(FS.Mask_i32(b, m));
        }

        public int32v MaskedSub_i32(int32v a, int32v b, mask32v m)
        {
            return a.Sub(FS.Mask_i32(b, m));
        }

        public int32v MaskedMul_i32(int32v a, int32v b, mask32v m)
        {
            return a.Mul(FS.Mask_i32(b, m));
        }

        // NMasked float

        public float32v NMaskedAdd_f32(float32v a, float32v b, mask32v m)
        {
            return a.Add(FS.NMask_f32(b, m));
        }

        public float32v NMaskedSub_f32(float32v a, float32v b, mask32v m)
        {
            return a.Sub(FS.NMask_f32(b, m));
        }

        public float32v NMaskedMul_f32(float32v a, float32v b, mask32v m)
        {
            return a.Mul(FS.NMask_f32(b, m));
        }

        // NMasked int32

        public int32v NMaskedAdd_i32(int32v a, int32v b, mask32v m)
        {
            return a.Add(FS.NMask_i32(b, m));
        }

        public int32v NMaskedSub_i32(int32v a, int32v b, mask32v m)
        {
            return a.Sub(FS.NMask_i32(b, m));
        }

        public int32v NMaskedMul_i32(int32v a, int32v b, mask32v m)
        {
            return a.Mul(FS.NMask_i32(b, m));
        }

        public int32v MaskedIncrement_i32(int32v a, mask32v m)
        {
            if (typeof(int32v) == typeof(mask32v))
                return a.Sub(Unsafe.As<mask32v, int32v>(ref m));
            else
                return MaskedSub_i32(a, FS.Broad_i32(-1), m);
        }

        public int32v MaskedDecrement_i32(int32v a, mask32v m)
        {
            if (typeof(int32v) == typeof(mask32v))
                return a.Add(Unsafe.As<mask32v, int32v>(ref m));
            else
                return MaskedAdd_i32(a, FS.Broad_i32(-1), m);
        }

        // Bitwise

        public mask32v BitwiseAndNot_m32(mask32v a, mask32v b)
        {
            if (typeof(int32v) == typeof(mask32v))
            {
                int32v result = FS.BitwiseAndNot_i32(
                    Unsafe.As<mask32v, int32v>(ref a),
                    Unsafe.As<mask32v, int32v>(ref b));
                return Unsafe.As<int32v, mask32v>(ref result);
            }
            else
            {
                return a.And(b.Complement());
            }
        }

        // Trig

        public float32v Cos_f32(float32v value)
        {
            value = FS.Abs_f32(value);
            value = value.Sub(FS.Floor_f32(value.Mul(FS.Broad_f32(0.1591549f))).Mul(FS.Broad_f32(6.283185f)));

            mask32v geHalfPi = value.GreaterThanOrEqual(FS.Broad_f32(1.570796f));
            mask32v geHalfPi2 = value.GreaterThanOrEqual(FS.Broad_f32(3.141593f));
            mask32v geHalfPi3 = value.GreaterThanOrEqual(FS.Broad_f32(4.7123889f));

            unchecked
            {
                float32v cosAngle = value.Xor(FS.Mask_f32(value.Xor(FS.Broad_f32(3.141593f)).Sub(value), geHalfPi));
                cosAngle = cosAngle.Xor(FS.Mask_f32(FS.Casti32_f32(FS.Broad_i32((int)0x80000000)), geHalfPi2));
                cosAngle = cosAngle.Xor(FS.Mask_f32(cosAngle.Xor(FS.Broad_f32(6.283185f).Sub(value)), geHalfPi3));

                cosAngle = cosAngle.Mul(cosAngle);

                cosAngle = FS.FMulAdd_f32(
                    cosAngle,
                    FS.FMulAdd_f32(cosAngle, FS.Broad_f32(0.03679168f), FS.Broad_f32(-0.49558072f)),
                    FS.Broad_f32(0.99940307f));

                return cosAngle.Xor(FS.Mask_f32(FS.Casti32_f32(FS.Broad_i32((int)0x80000000)), BitwiseAndNot_m32(geHalfPi, geHalfPi3)));
            }
        }

        public float32v Sin_f32(float32v value)
        {
            return Cos_f32(FS.Broad_f32(1.570796f).Sub(value));
        }

        public float32v Exp_f32(float32v x)
        {
            x = FS.Min_f32(x, FS.Broad_f32(88.3762626647949f));
            x = FS.Max_f32(x, FS.Broad_f32(-88.3762626647949f));

            // express exp(x) as exp(g + n*log(2))
            float32v fx = x.Mul(FS.Broad_f32(1.44269504088896341f));
            fx = fx.Add(FS.Broad_f32(0.5f));

            float32v flr = FS.Floor_f32(fx);
            fx = MaskedSub_f32(flr, FS.Broad_f32(1), flr.GreaterThan(fx));

            x = x.Sub(fx.Mul(FS.Broad_f32(0.693359375f)));
            x = x.Sub(fx.Mul(FS.Broad_f32(-2.12194440e-4f)));

            float32v y = FS.Broad_f32(1.9875691500E-4f);
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(1.3981999507E-3f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(8.3334519073E-3f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(4.1665795894E-2f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(1.6666665459E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(5.0000001201E-1f));
            y = y.Mul(x.Mul(x));
            y = y.Add(x.Add(FS.Broad_f32(1)));

            // build 2^n
            int32v i = FS.Convertf32_i32(fx);
            // another two AVX2 instructions
            i = i.Add(FS.Broad_i32(0x7f));
            i = i.LeftShift(23);
            float32v pow2n = FS.Casti32_f32(i);

            return y.Mul(pow2n);
        }

        public float32v Log_f32(float32v x)
        {
            mask32v validMask = x.GreaterThan(FS.Broad_f32(0));

            x = FS.Max_f32(x, FS.Casti32_f32(FS.Broad_i32(0x00800000)));  // cut off denormalized stuff

            // can be done with AVX2
            int32v i = FS.BitwiseShiftRightZX_i32(FS.Castf32_i32(x), 23);

            // keep only the fractional part 
            x = x.And(FS.Casti32_f32(FS.Broad_i32(~0x7f800000)));
            x = x.Or(FS.Broad_f32(0.5f));

            // this is again another AVX2 instruction
            i = i.Sub(FS.Broad_i32(0x7f));
            float32v e = FS.Converti32_f32(i);

            e = e.Add(FS.Broad_f32(1));

            mask32v mask = x.LessThan(FS.Broad_f32(0.707106781186547524f));
            x = MaskedAdd_f32(x, x, mask);
            x = x.Sub(FS.Broad_f32(1));
            e = MaskedSub_f32(e, FS.Broad_f32(1), mask);

            float32v y = FS.Broad_f32(7.0376836292E-2f);
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(-1.1514610310E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(1.1676998740E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(-1.2420140846E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(1.4249322787E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(-1.6668057665E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(2.0000714765E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(-2.4999993993E-1f));
            y = y.Mul(x);
            y = y.Add(FS.Broad_f32(3.3333331174E-1f));
            y = y.Mul(x);

            float32v xx = x.Mul(x);
            y = y.Mul(xx);
            y = y.Mul(e.Mul(FS.Broad_f32(-2.12194440e-4f)));
            y = y.Sub(xx.Mul(FS.Broad_f32(0.5f)));

            x = x.Add(y);
            x = x.Add(e.Mul(FS.Broad_f32(0.693359375f)));

            return FS.Mask_f32(x, validMask);
        }

        public float32v Pow_f32(float32v value, float32v pow)
        {
            return Exp_f32(pow.Mul(Log_f32(value)));
        }
    }
}
