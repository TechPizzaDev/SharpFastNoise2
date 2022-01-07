using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public struct FastSimd<m32, f32, i32, TFunc>
        where m32 : unmanaged
        where f32 : unmanaged
        where i32 : unmanaged
        where TFunc : unmanaged, IFunctionList<m32, f32, i32>
    {
        private static TFunc F = default;

        // Masked float

        public f32 MaskedAdd_f32(f32 a, f32 b, m32 m)
        {
            return F.Add(a, F.Mask_f32(b, m));
        }

        public f32 MaskedSub_f32(f32 a, f32 b, m32 m)
        {
            return F.Sub(a, F.Mask_f32(b, m));
        }

        public f32 MaskedMul_f32(f32 a, f32 b, m32 m)
        {
            return F.Mul(a, F.Mask_f32(b, m));
        }

        // Masked int32

        public i32 MaskedAdd_i32(i32 a, i32 b, m32 m)
        {
            return F.Add(a, F.Mask_i32(b, m));
        }

        public i32 MaskedSub_i32(i32 a, i32 b, m32 m)
        {
            return F.Sub(a, F.Mask_i32(b, m));
        }

        public i32 MaskedMul_i32(i32 a, i32 b, m32 m)
        {
            return F.Mul(a, F.Mask_i32(b, m));
        }

        // NMasked float

        public f32 NMaskedAdd_f32(f32 a, f32 b, m32 m)
        {
            return F.Add(a, F.NMask_f32(b, m));
        }

        public f32 NMaskedSub_f32(f32 a, f32 b, m32 m)
        {
            return F.Sub(a, F.NMask_f32(b, m));
        }

        public f32 NMaskedMul_f32(f32 a, f32 b, m32 m)
        {
            return F.Mul(a, F.NMask_f32(b, m));
        }

        // NMasked int32

        public i32 NMaskedAdd_i32(i32 a, i32 b, m32 m)
        {
            return F.Add(a, F.NMask_i32(b, m));
        }

        public i32 NMaskedSub_i32(i32 a, i32 b, m32 m)
        {
            return F.Sub(a, F.NMask_i32(b, m));
        }

        public i32 NMaskedMul_i32(i32 a, i32 b, m32 m)
        {
            return F.Mul(a, F.NMask_i32(b, m));
        }

        public i32 MaskedIncrement_i32(i32 a, m32 m)
        {
            if (typeof(i32) == typeof(m32))
                return F.Sub(a, Unsafe.As<m32, i32>(ref m));
            else
                return MaskedSub_i32(a, F.Broad_i32(-1), m);
        }

        public i32 MaskedDecrement_i32(i32 a, m32 m)
        {
            if (typeof(i32) == typeof(m32))
                return F.Add(a, Unsafe.As<m32, i32>(ref m));
            else
                return MaskedAdd_i32(a, F.Broad_i32(-1), m);
        }

        // Trig

        public f32 Cos_f32(f32 value)
        {
            unchecked
            {
                value = F.Abs_f32(value);
                value = F.Sub(value, F.Mul(F.Floor_f32(F.Mul(value, F.Broad_f32(0.1591549f))), F.Broad_f32(6.283185f)));

                m32 geHalfPi = F.GreaterThanOrEqual(value, F.Broad_f32(1.570796f));
                m32 geHalfPi2 = F.GreaterThanOrEqual(value, F.Broad_f32(3.141593f));
                m32 geHalfPi3 = F.GreaterThanOrEqual(value, F.Broad_f32(4.7123889f));

                f32 cosAngle = F.Xor(
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

        public f32 Sin_f32(f32 value)
        {
            return Cos_f32(F.Sub(F.Broad_f32(1.570796f), value));
        }

        public f32 Exp_f32(f32 x)
        {
            x = F.Min_f32(x, F.Broad_f32(88.3762626647949f));
            x = F.Max_f32(x, F.Broad_f32(-88.3762626647949f));

            // express exp(x) as exp(g + n*log(2))
            f32 fx = F.Mul(x, F.Broad_f32(1.44269504088896341f));
            fx = F.Add(fx, F.Broad_f32(0.5f));

            f32 flr = F.Floor_f32(fx);
            fx = MaskedSub_f32(flr, F.Broad_f32(1), F.GreaterThan(flr, fx));

            x = F.Sub(x, F.Mul(fx, F.Broad_f32(0.693359375f)));
            x = F.Sub(x, F.Mul(fx, F.Broad_f32(-2.12194440e-4f)));

            f32 y = F.Broad_f32(1.9875691500E-4f);
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
            i32 i = F.Convertf32_i32(fx);
            // another two AVX2 instructions
            i = F.Add(i, F.Broad_i32(0x7f));
            i = F.LeftShift(i, 23);
            f32 pow2n = F.Casti32_f32(i);

            return F.Mul(y, pow2n);
        }

        public f32 Log_f32(f32 x)
        {
            m32 validMask = F.GreaterThan(x, F.Broad_f32(0));

            x = F.Max_f32(x, F.Casti32_f32(F.Broad_i32(0x00800000)));  // cut off denormalized stuff

            // can be done with AVX2
            i32 i = F.BitwiseShiftRightZX_i32(F.Castf32_i32(x), 23);

            // keep only the fractional part 
            x = F.And(x, F.Casti32_f32(F.Broad_i32(~0x7f800000)));
            x = F.Or(x, F.Broad_f32(0.5f));

            // this is again another AVX2 instruction
            i = F.Sub(i, F.Broad_i32(0x7f));
            f32 e = F.Converti32_f32(i);

            e = F.Add(e, F.Broad_f32(1));

            m32 mask = F.LessThan(x, F.Broad_f32(0.707106781186547524f));
            x = MaskedAdd_f32(x, x, mask);
            x = F.Sub(x, F.Broad_f32(1));
            e = MaskedSub_f32(e, F.Broad_f32(1), mask);

            f32 y = F.Broad_f32(7.0376836292E-2f);
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

            f32 xx = F.Mul(x, x);
            y = F.Mul(y, xx);
            y = F.Mul(y, F.Mul(e, F.Broad_f32(-2.12194440e-4f)));
            y = F.Sub(y, F.Mul(xx, F.Broad_f32(0.5f)));

            x = F.Add(x, y);
            x = F.Add(x, F.Mul(e, F.Broad_f32(0.693359375f)));

            return F.Mask_f32(x, validMask);
        }

        public f32 Pow_f32(f32 value, f32 pow)
        {
            return Exp_f32(F.Mul(pow, Log_f32(value)));
        }
    }
}
