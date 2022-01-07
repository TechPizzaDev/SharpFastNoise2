using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace SharpFastNoise2
{
    using f32 = Vector128<float>;
    using i32 = Vector128<int>;
    using m32 = Vector128<int>;

    public struct Sse2Functions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => Sse2.IsSupported;

        public static int Count => 4;

        // Broadcast

        public f32 Broad_f32(float value)
        {
            return Vector128.Create(value);
        }

        public i32 Broad_i32(int value)
        {
            return Vector128.Create(value);
        }

        // Load

        public f32 Load_f32(ref byte p)
        {
            return Unsafe.ReadUnaligned<f32>(ref p);
        }

        public i32 Load_i32(ref byte p)
        {
            return Unsafe.ReadUnaligned<i32>(ref p);
        }

        // Incremented

        public f32 Incremented_f32()
        {
            return Vector128.Create(0f, 1, 2, 3);
        }

        public i32 Incremented_i32()
        {
            return Vector128.Create(0, 1, 2, 3);
        }

        // Store

        public void Store_f32(ref byte p, f32 a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_i32(ref byte p, i32 a)
        {
            Unsafe.WriteUnaligned(ref p, a);
        }

        public void Store_f32(ref float p, f32 a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<float, byte>(ref p), a);
        }

        public void Store_i32(ref int p, i32 a)
        {
            Unsafe.WriteUnaligned(ref Unsafe.As<int, byte>(ref p), a);
        }

        // Extract

        public float Extract0_f32(f32 a)
        {
            return a.ToScalar();
        }

        public int Extract0_i32(i32 a)
        {
            return a.ToScalar();
        }

        public float Extract_f32(f32 a, int idx)
        {
            return a.GetElement(idx);
        }

        public int Extract_i32(i32 a, int idx)
        {
            return a.GetElement(idx);
        }

        // Cast

        public f32 Casti32_f32(i32 a)
        {
            return Unsafe.As<i32, f32>(ref a);
        }

        public i32 Castf32_i32(f32 a)
        {
            return Unsafe.As<f32, i32>(ref a);
        }

        // Convert

        public f32 Converti32_f32(i32 a)
        {
            return Sse2.ConvertToVector128Single(a);
        }

        public i32 Convertf32_i32(f32 a)
        {
            return Sse2.ConvertToVector128Int32(a);
        }

        // Select

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 Select_f32(m32 m, f32 a, f32 b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.BlendVariable(b, a, m.AsSingle());
            }
            else
            {
                return Xor(b, And(m.AsSingle(), Xor(a, b)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 Select_i32(m32 m, i32 a, i32 b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.BlendVariable(b, a, m);
            }
            else
            {
                return Xor(b, And(m, Xor(a, b)));
            }
        }

        // Min, Max

        public f32 Min_f32(f32 a, f32 b)
        {
            return Sse.Min(a, b);
        }

        public f32 Max_f32(f32 a, f32 b)
        {
            return Sse.Max(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 Min_i32(i32 a, i32 b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.Min(a, b);
            }
            else
            {
                return Select_i32(LessThan(a, b), a, b);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 Max_i32(i32 a, i32 b)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.Max(a, b);
            }
            else
            {
                return Select_i32(GreaterThan(a, b), a, b);
            }
        }

        // Bitwise       

        public f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Sse.AndNot(b, a);
        }

        public i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return Sse2.AndNot(b, a);
        }

        public m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return Sse2.AndNot(b, a);
        }

        public f32 BitwiseShiftRightZX_f32(f32 a, byte b)
        {
            return Casti32_f32(Sse2.ShiftRightLogical(Castf32_i32(a), b));
        }

        public i32 BitwiseShiftRightZX_i32(i32 a, byte b)
        {
            return Sse2.ShiftRightLogical(a, b);
        }

        // Abs

        public f32 Abs_f32(f32 a)
        {
            i32 intMax = Sse2.ShiftRightLogical(m32.AllBitsSet, 1);
            return And(a, intMax.AsSingle());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 Abs_i32(i32 a)
        {
            if (Ssse3.IsSupported)
            {
                return Ssse3.Abs(a).AsInt32();
            }
            else
            {
                i32 signMask = Sse2.ShiftRightArithmetic(a, 31);
                return Sub(Xor(a, signMask), signMask);
            }
        }

        // Float math

        public f32 Sqrt_f32(f32 a)
        {
            return Sse.Sqrt(a);
        }

        public f32 InvSqrt_f32(f32 a)
        {
            return Sse.ReciprocalSqrt(a);
        }

        public f32 Reciprocal_f32(f32 a)
        {
            return Sse.Reciprocal(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 Floor_f32(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNegativeInfinity(a);
            }
            else
            {
                f32 f1 = Vector128.Create(1f);
                f32 fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                f32 cmp = Sse.CompareLessThan(a, fval);
                return Sse.Subtract(fval, Sse.And(cmp, f1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 Ceil_f32(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToPositiveInfinity(a);
            }
            else
            {
                f32 f1 = Vector128.Create(1f);
                f32 fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                f32 cmp = Sse.CompareLessThan(fval, a);
                return Sse.Add(fval, Sse.And(cmp, f1));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 Round_f32(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNearestInteger(a);
            }
            else
            {
                unchecked
                {
                    f32 aSign = And(a, Broad_i32((int)0x80000000).AsSingle());
                    f32 v = Add(a, Or(aSign, Broad_f32(0.5f)));
                    return Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(v));
                }
            }
        }

        // Mask

        public i32 Mask_i32(i32 a, m32 m)
        {
            return And(a, m);
        }

        public f32 Mask_f32(f32 a, m32 m)
        {
            return And(a, m.AsSingle());
        }

        public i32 NMask_i32(i32 a, m32 m)
        {
            return Sse2.AndNot(m, a);
        }

        public f32 NMask_f32(f32 a, m32 m)
        {
            return Sse.AndNot(m.AsSingle(), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AnyMask_bool(m32 m)
        {
            if (Sse41.IsSupported)
            {
                return !Sse41.TestZ(m, m);
            }
            else
            {
                return Sse.MoveMask(m.AsSingle()) != 0;
            }
        }

        // FMA

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 FMulAdd_f32(f32 a, f32 b, f32 c)
        {
            if (Fma.IsSupported)
            {
                return Fma.MultiplyAdd(a, b, c);
            }
            else
            {
                return Add(Mul(a, b), c);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
        {
            if (Fma.IsSupported)
            {
                return Fma.MultiplyAddNegated(a, b, c);
            }
            else
            {
                return Add(Negate(Mul(a, b)), c);
            }
        }

        // Generic math

        public f32 Add(f32 lhs, f32 rhs) => Sse.Add(lhs, rhs);
        public f32 And(f32 lhs, f32 rhs) => Sse.And(lhs, rhs);
        public i32 AsInt32(f32 lhs) => lhs.AsInt32();
        public f32 Complement(f32 lhs) => Sse.Xor(lhs, f32.AllBitsSet);
        public f32 Div(f32 lhs, f32 rhs) => Sse.Divide(lhs, rhs);
        public m32 Equal(f32 lhs, f32 rhs) => Sse.CompareEqual(lhs, rhs).AsInt32();
        public m32 GreaterThan(f32 lhs, f32 rhs) => Sse.CompareGreaterThan(lhs, rhs).AsInt32();
        public m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Sse.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public f32 LeftShift(f32 lhs, byte rhs) => throw new NotSupportedException();
        public m32 LessThan(f32 lhs, f32 rhs) => Sse.CompareLessThan(lhs, rhs).AsInt32();
        public m32 LessThanOrEqual(f32 lhs, f32 rhs) => Sse.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public f32 Mul(f32 lhs, f32 rhs) => Sse.Multiply(lhs, rhs);
        public f32 Negate(f32 lhs) => Sse.Xor(lhs, Vector128.Create(0x80000000).AsSingle());
        public m32 NotEqual(f32 lhs, f32 rhs) => Sse.CompareNotEqual(lhs, rhs).AsInt32();
        public f32 Or(f32 lhs, f32 rhs) => Sse.Or(lhs, rhs);
        public f32 RightShift(f32 lhs, byte rhs) => throw new NotSupportedException();
        public f32 Sub(f32 lhs, f32 rhs) => Sse.Subtract(lhs, rhs);
        public f32 Xor(f32 lhs, f32 rhs) => Sse.Xor(lhs, rhs);

        public i32 Add(i32 lhs, i32 rhs) => Sse2.Add(lhs, rhs);
        public i32 And(i32 lhs, i32 rhs) => Sse2.And(lhs, rhs);
        public f32 AsSingle(i32 lhs) => lhs.AsSingle();
        public i32 Complement(i32 lhs) => Sse2.Xor(lhs, i32.AllBitsSet);
        public i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public m32 Equal(i32 lhs, i32 rhs) => Sse2.CompareEqual(lhs, rhs);
        public m32 GreaterThan(i32 lhs, i32 rhs) => Sse2.CompareGreaterThan(lhs, rhs);
        public m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public i32 LeftShift(i32 lhs, byte rhs) => Sse2.ShiftLeftLogical(lhs, rhs);
        public m32 LessThan(i32 lhs, i32 rhs) => Sse2.CompareLessThan(lhs, rhs);
        public m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public i32 Mul(i32 lhs, i32 rhs)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.MultiplyLow(lhs, rhs);
            }
            else
            {
                var tmp1 = Sse2.Multiply(lhs.AsUInt32(), rhs.AsUInt32()); // mul 2,0
                var tmp2 = Sse2.Multiply(
                    Sse2.ShiftRightLogical128BitLane(lhs, 4).AsUInt32(),
                    Sse2.ShiftRightLogical128BitLane(rhs, 4).AsUInt32()); // mul 3,1

                const byte control = 8; // _MM_SHUFFLE(0,0,2,0)
                return Sse2.UnpackLow(
                    Sse2.Shuffle(tmp1.AsInt32(), control),
                    Sse2.Shuffle(tmp2.AsInt32(), control)); // shuffle results to [63..0] and pack
            }
        }

        public i32 Negate(i32 lhs) => Sse2.Subtract(i32.Zero, lhs);
        public m32 NotEqual(i32 lhs, i32 rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public i32 Or(i32 lhs, i32 rhs) => Sse2.Or(lhs, rhs);
        public i32 RightShift(i32 lhs, byte rhs) => Sse2.ShiftRightArithmetic(lhs, rhs);
        public i32 Sub(i32 lhs, i32 rhs) => Sse2.Subtract(lhs, rhs);
        public i32 Xor(i32 lhs, i32 rhs) => Sse2.Xor(lhs, rhs);
    }
}
