using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using SharpFastNoise2;

namespace SharpFastNoise2.Functions
{
    using f32 = Vector256<float>;
    using i32 = Vector256<int>;
    using m32 = Vector256<int>;

    public struct Avx2Functions : IFunctionList<m32, f32, i32>
    {
        public static bool IsSupported => Avx2.IsSupported;

        public static int Count => f32.Count;

        // Broadcast

        public static f32 Broad_f32(float value)
        {
            return Vector256.Create(value);
        }

        public static i32 Broad_i32(int value)
        {
            return Vector256.Create(value);
        }

        // Load

        public static f32 Load_f32(ref readonly float p)
        {
            return Vector256.LoadUnsafe(in p);
        }

        public static i32 Load_i32(ref readonly int p)
        {
            return Vector256.LoadUnsafe(in p);
        }

        // Incremented

        public static f32 Incremented_f32()
        {
            return Vector256.Create(0f, 1, 2, 3, 4, 5, 6, 7);
        }

        public static i32 Incremented_i32()
        {
            return Vector256.Create(0, 1, 2, 3, 4, 5, 6, 7);
        }

        // Store

        public static void Store_f32(ref float p, f32 a)
        {
            Vector256.StoreUnsafe(a, ref p);
        }

        public static void Store_i32(ref int p, i32 a)
        {
            Vector256.StoreUnsafe(a, ref p);
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
            return Avx.ConvertToVector256Single(a);
        }

        public static i32 Convertf32_i32(f32 a)
        {
            return Avx.ConvertToVector256Int32(a);
        }

        // Select

        public static f32 Select_f32(m32 m, f32 a, f32 b)
        {
            return Avx.BlendVariable(b, a, m.AsSingle());
        }

        public static i32 Select_i32(m32 m, i32 a, i32 b)
        {
            return Avx2.BlendVariable(b, a, m);
        }

        // Min, Max

        public static f32 Min_f32(f32 a, f32 b)
        {
            return Avx.Min(a, b);
        }

        public static f32 Max_f32(f32 a, f32 b)
        {
            return Avx.Max(a, b);
        }

        public static i32 Min_i32(i32 a, i32 b)
        {
            return Avx2.Min(a, b);
        }

        public static i32 Max_i32(i32 a, i32 b)
        {
            return Avx2.Max(a, b);
        }

        // Bitwise       

        public static f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Avx.AndNot(b, a);
        }

        public static i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return Avx2.AndNot(b, a);
        }

        public static m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return Avx2.AndNot(b, a);
        }

        public static f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b)
        {
            return Casti32_f32(Avx2.ShiftRightLogical(Castf32_i32(a), b));
        }

        public static i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b)
        {
            return Avx2.ShiftRightLogical(a, b);
        }

        // Abs

        public static f32 Abs_f32(f32 a)
        {
            return And(a, Avx2.ShiftRightLogical(m32.AllBitsSet, 1).AsSingle());
        }

        public static i32 Abs_i32(i32 a)
        {
            return Avx2.Abs(a).AsInt32();
        }

        // Float math

        public static f32 Sqrt_f32(f32 a)
        {
            return Avx.Sqrt(a);
        }

        public static f32 InvSqrt_f32(f32 a)
        {
            return Avx.ReciprocalSqrt(a);
        }

        public static f32 Reciprocal_f32(f32 a)
        {
            return Avx.Reciprocal(a);
        }

        // Floor, Ceil, Round: http://dss.stephanierct.com/DevBlog/?p=8

        public static f32 Floor_f32(f32 a)
        {
            return Avx.RoundToNegativeInfinity(a);
        }

        public static f32 Ceil_f32(f32 a)
        {
            return Avx.RoundToPositiveInfinity(a);
        }

        public static f32 Round_f32(f32 a)
        {
            return Avx.RoundToNearestInteger(a);
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
            return Avx2.AndNot(m, a);
        }

        public static f32 NMask_f32(f32 a, m32 m)
        {
            return Avx.AndNot(m.AsSingle(), a);
        }

        public static bool AnyMask_bool(m32 m)
        {
            return !Avx.TestZ(m, m);
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

        public static f32 Add(f32 lhs, f32 rhs) => Avx.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Avx.And(lhs, rhs);
        public static i32 AsInt32(f32 lhs) => lhs.AsInt32();
        public static f32 Complement(f32 lhs) => Vector256.OnesComplement(lhs);
        public static f32 Div(f32 lhs, f32 rhs) => Avx.Divide(lhs, rhs);
        public static m32 Equal(f32 lhs, f32 rhs) => Avx.CompareEqual(lhs, rhs).AsInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => Avx.CompareGreaterThan(lhs, rhs).AsInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareGreaterThanOrEqual(lhs, rhs).AsInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static m32 LessThan(f32 lhs, f32 rhs) => Avx.CompareLessThan(lhs, rhs).AsInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => Avx.CompareLessThanOrEqual(lhs, rhs).AsInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => Avx.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => Vector256.Negate(lhs);
        public static m32 NotEqual(f32 lhs, f32 rhs) => Avx.CompareNotEqual(lhs, rhs).AsInt32();
        public static f32 Or(f32 lhs, f32 rhs) => Avx.Or(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Avx.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Avx.Xor(lhs, rhs);

        public static i32 Add(i32 lhs, i32 rhs) => Avx2.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Avx2.And(lhs, rhs);
        public static f32 AsSingle(i32 lhs) => lhs.AsSingle();
        public static i32 Complement(i32 lhs) => Vector256.OnesComplement(lhs);
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static m32 Equal(i32 lhs, i32 rhs) => Avx2.CompareEqual(lhs, rhs);
        public static m32 GreaterThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(lhs, rhs);
        public static m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => Avx2.ShiftLeftLogical(lhs, rhs);
        public static m32 LessThan(i32 lhs, i32 rhs) => Avx2.CompareGreaterThan(rhs, lhs);
        public static m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => Avx2.MultiplyLow(lhs, rhs);
        public static i32 Negate(i32 lhs) => Vector256.Negate(lhs);
        public static m32 NotEqual(i32 lhs, i32 rhs) => NotEqual(lhs.AsSingle(), rhs.AsSingle());
        public static i32 Or(i32 lhs, i32 rhs) => Avx2.Or(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => Avx2.ShiftRightArithmetic(lhs, rhs);
        public static i32 Sub(i32 lhs, i32 rhs) => Avx2.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Avx2.Xor(lhs, rhs);
    }
}
