using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2;

namespace SharpFastNoise2.Functions
{
    using f32 = Vector128<float>;
    using i32 = Vector128<int>;
    using m32 = Vector128<int>;

    public struct Sse2Functions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => Sse2.IsSupported;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad_f32(float value)
        {
            return Vector128.Create(value);
        }

        public static i32 Broad_i32(int value)
        {
            return Vector128.Create(value);
        }

        // Load

        public static f32 Load_f32(ref readonly float p)
        {
            return Vector128.LoadUnsafe(in p);
        }

        public static i32 Load_i32(ref readonly int p)
        {
            return Vector128.LoadUnsafe(in p);
        }

        // Incremented

        public static f32 Incremented_f32()
        {
            return Vector128.Create(0f, 1, 2, 3);
        }

        public static i32 Incremented_i32()
        {
            return Vector128.Create(0, 1, 2, 3);
        }

        // Store

        public static void Store_f32(ref float p, f32 a)
        {
            Vector128.StoreUnsafe(a, ref p);
        }

        public static void Store_i32(ref int p, i32 a)
        {
            Vector128.StoreUnsafe(a, ref p);
        }

        // Extract

        public static float Extract0_f32(f32 a)
        {
            return a.ToScalar();
        }

        public static int Extract0_i32(i32 a)
        {
            return a.ToScalar();
        }

        public static float Extract_f32(f32 a, int idx)
        {
            return a.GetElement(idx);
        }

        public static int Extract_i32(i32 a, int idx)
        {
            return a.GetElement(idx);
        }

        // Cast

        public static f32 Casti32_f32(i32 a)
        {
            return a.AsSingle();
        }

        public static i32 Castf32_i32(f32 a)
        {
            return a.AsInt32();
        }

        // Convert

        public static f32 Converti32_f32(i32 a)
        {
            return Sse2.ConvertToVector128Single(a);
        }

        public static i32 Convertf32_i32(f32 a)
        {
            return Sse2.ConvertToVector128Int32(a);
        }

        // Select

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Select_f32(m32 m, f32 a, f32 b)
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
        public static i32 Select_i32(m32 m, i32 a, i32 b)
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

        public static f32 Min_f32(f32 a, f32 b)
        {
            return Sse.Min(a, b);
        }

        public static f32 Max_f32(f32 a, f32 b)
        {
            return Sse.Max(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 Min_i32(i32 a, i32 b)
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
        public static i32 Max_i32(i32 a, i32 b)
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

        public static f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Sse.AndNot(b, a);
        }

        public static i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return Sse2.AndNot(b, a);
        }

        public static m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return Sse2.AndNot(b, a);
        }

        public static f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b)
        {
            return Casti32_f32(Sse2.ShiftRightLogical(Castf32_i32(a), b));
        }

        public static i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b)
        {
            return Sse2.ShiftRightLogical(a, b);
        }

        // Abs

        public static f32 Abs_f32(f32 a)
        {
            i32 intMax = Sse2.ShiftRightLogical(m32.AllBitsSet, 1);
            return And(a, intMax.AsSingle());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 Abs_i32(i32 a)
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

        public static f32 Sqrt_f32(f32 a)
        {
            return Sse.Sqrt(a);
        }

        public static f32 InvSqrt_f32(f32 a)
        {
            return Sse.ReciprocalSqrt(a);
        }

        public static f32 Reciprocal_f32(f32 a)
        {
            return Sse.Reciprocal(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Floor_f32(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNegativeInfinity(a);
            }
            else
            {
                f32 fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                f32 cmp = Sse.CompareLessThan(a, fval);
                return Sse.Subtract(fval, Sse.And(cmp, Vector128.Create(1f)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Ceil_f32(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToPositiveInfinity(a);
            }
            else
            {
                f32 fval = Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(a));
                f32 cmp = Sse.CompareLessThan(fval, a);
                return Sse.Add(fval, Sse.And(cmp, Vector128.Create(1f)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static f32 Round_f32(f32 a)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNearestInteger(a);
            }
            else
            {
                f32 aSign = And(a, Broad_i32(unchecked((int) 0x80000000)).AsSingle());
                f32 v = Add(a, Or(aSign, Broad_f32(0.5f)));
                return Sse2.ConvertToVector128Single(Sse2.ConvertToVector128Int32WithTruncation(v));
            }
        }

        // Mask

        public static i32 Mask_i32(i32 a, m32 m)
        {
            return And(a, m);
        }

        public static f32 Mask_f32(f32 a, m32 m)
        {
            return And(a, m.AsSingle());
        }

        public static i32 NMask_i32(i32 a, m32 m)
        {
            return Sse2.AndNot(m, a);
        }

        public static f32 NMask_f32(f32 a, m32 m)
        {
            return Sse.AndNot(m.AsSingle(), a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyMask_bool(m32 m)
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
        public static f32 FMulAdd_f32(f32 a, f32 b, f32 c)
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
        public static f32 FNMulAdd_f32(f32 a, f32 b, f32 c)
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

        public static f32 Add(f32 lhs, f32 rhs) => Sse.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Sse.And(lhs, rhs);
        public static i32 AsInt32(f32 lhs) => lhs.AsInt32();
        public static f32 Complement(f32 lhs) => Vector128.OnesComplement(lhs);
        public static f32 Div(f32 lhs, f32 rhs) => Sse.Divide(lhs, rhs);
        public static m32 Equal(f32 lhs, f32 rhs) => Sse.CompareEqual(lhs, rhs).AsInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => Sse.CompareGreaterThan(lhs, rhs).AsInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Sse.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static m32 LessThan(f32 lhs, f32 rhs) => Sse.CompareLessThan(lhs, rhs).AsInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => Sse.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => Sse.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => Vector128.Negate(lhs);
        public static m32 NotEqual(f32 lhs, f32 rhs) => Sse.CompareNotEqual(lhs, rhs).AsInt32();
        public static f32 Or(f32 lhs, f32 rhs) => Sse.Or(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Sse.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Sse.Xor(lhs, rhs);

        public static i32 Add(i32 lhs, i32 rhs) => Sse2.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Sse2.And(lhs, rhs);
        public static f32 AsSingle(i32 lhs) => lhs.AsSingle();
        public static i32 Complement(i32 lhs) => Vector128.OnesComplement(lhs);
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static m32 Equal(i32 lhs, i32 rhs) => Sse2.CompareEqual(lhs, rhs);
        public static m32 GreaterThan(i32 lhs, i32 rhs) => Sse2.CompareGreaterThan(lhs, rhs);
        public static m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => Sse2.ShiftLeftLogical(lhs, rhs);
        public static m32 LessThan(i32 lhs, i32 rhs) => Sse2.CompareLessThan(lhs, rhs);
        public static m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static i32 Mul(i32 lhs, i32 rhs)
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

        public static i32 Negate(i32 lhs) => Vector128.Negate(lhs);
        public static m32 NotEqual(i32 lhs, i32 rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public static i32 Or(i32 lhs, i32 rhs) => Sse2.Or(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => Sse2.ShiftRightArithmetic(lhs, rhs);
        public static i32 Sub(i32 lhs, i32 rhs) => Sse2.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Sse2.Xor(lhs, rhs);
    }
}
