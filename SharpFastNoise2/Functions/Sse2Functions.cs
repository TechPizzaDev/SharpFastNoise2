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
        public static bool IsSupported => Sse2.IsSupported || Vector128.IsHardwareAccelerated;

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
            return Vector128.ConvertToSingle(a);
        }

        public static i32 Convertf32_i32(f32 a)
        {
            if (Sse2.IsSupported)
            {
                return Sse2.ConvertToVector128Int32(a);
            }
            else
            {
                f32 aSign = a & Vector128.Create(unchecked((int) 0x80000000)).AsSingle();
                f32 v = a & (aSign | Vector128.Create(0.5f));
                return Vector128.ConvertToInt32(v);
            }
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
                return Vector128.ConditionalSelect(m.AsSingle(), a, b);
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
                return Vector128.ConditionalSelect(m, a, b);
            }
        }

        // Min, Max

        public static f32 Min_f32(f32 a, f32 b)
        {
            return Vector128.Min(a, b);
        }

        public static f32 Max_f32(f32 a, f32 b)
        {
            return Vector128.Max(a, b);
        }

        public static i32 Min_i32(i32 a, i32 b)
        {
            return Vector128.Min(a, b);
        }

        public static i32 Max_i32(i32 a, i32 b)
        {
            return Vector128.Max(a, b);
        }

        // Bitwise       

        public static f32 BitwiseAndNot_f32(f32 a, f32 b)
        {
            return Vector128.AndNot(a, b);
        }

        public static i32 BitwiseAndNot_i32(i32 a, i32 b)
        {
            return Vector128.AndNot(a, b);
        }

        public static m32 BitwiseAndNot_m32(m32 a, m32 b)
        {
            return Vector128.AndNot(a, b);
        }

        public static f32 BitwiseShiftRightZX_f32(f32 a, [ConstantExpected] byte b)
        {
            return Vector128.ShiftRightLogical(a.AsInt32(), b).AsSingle();
        }

        public static i32 BitwiseShiftRightZX_i32(i32 a, [ConstantExpected] byte b)
        {
            return Vector128.ShiftRightLogical(a, b);
        }

        // Abs

        public static f32 Abs_f32(f32 a)
        {
            return Vector128.Abs(a);
        }

        public static i32 Abs_i32(i32 a)
        {
            return Vector128.Abs(a);
        }

        // Float math

        public static f32 Sqrt_f32(f32 a)
        {
            return Vector128.Sqrt(a);
        }

        public static f32 InvSqrt_f32(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.ReciprocalSqrt(a);
            }
            else
            {
                return Vector128.Create(1f) / Vector128.Sqrt(a);
            }
        }

        public static f32 Reciprocal_f32(f32 a)
        {
            if (Sse.IsSupported)
            {
                return Sse.Reciprocal(a);
            }
            else
            {
                return Vector128.Create(1f) / a;
            }
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
                f32 fval = Vector128.ConvertToSingle(Vector128.ConvertToInt32(a));
                f32 cmp = Vector128.LessThan(a, fval);
                return fval - (cmp & Vector128.Create(1f));
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
                f32 fval = Vector128.ConvertToSingle(Vector128.ConvertToInt32(a));
                f32 cmp = Vector128.LessThan(fval, a);
                return fval + (cmp & Vector128.Create(1f));
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
                f32 aSign = a & Vector128.Create(unchecked((int) 0x80000000)).AsSingle();
                f32 v = a & (aSign | Vector128.Create(0.5f));
                return Vector128.ConvertToSingle(Vector128.ConvertToInt32(v));
            }
        }

        // Mask

        public static i32 Mask_i32(i32 a, m32 m)
        {
            return a & m;
        }

        public static f32 Mask_f32(f32 a, m32 m)
        {
            return a & m.AsSingle();
        }

        public static i32 NMask_i32(i32 a, m32 m)
        {
            return Vector128.AndNot(a, m);
        }

        public static f32 NMask_f32(f32 a, m32 m)
        {
            return Vector128.AndNot(a, m.AsSingle());
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
                return m.ExtractMostSignificantBits() != 0;
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
                return (a * b) + c;
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
                return -(a * b) + c;
            }
        }

        // Generic math

        public static f32 Add(f32 lhs, f32 rhs) => Vector128.Add(lhs, rhs);
        public static f32 And(f32 lhs, f32 rhs) => Vector128.BitwiseAnd(lhs, rhs);
        public static f32 Complement(f32 lhs) => Vector128.OnesComplement(lhs);
        public static f32 Div(f32 lhs, f32 rhs) => Vector128.Divide(lhs, rhs);
        public static m32 Equal(f32 lhs, f32 rhs) => Vector128.Equals(lhs, rhs).AsInt32();
        public static m32 GreaterThan(f32 lhs, f32 rhs) => Vector128.GreaterThan(lhs, rhs).AsInt32();
        public static m32 GreaterThanOrEqual(f32 lhs, f32 rhs) => Vector128.GreaterThanOrEqual(lhs, rhs).AsInt32();
        public static f32 LeftShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static m32 LessThan(f32 lhs, f32 rhs) => Vector128.LessThan(lhs, rhs).AsInt32();
        public static m32 LessThanOrEqual(f32 lhs, f32 rhs) => Vector128.LessThanOrEqual(lhs, rhs).AsInt32();
        public static f32 Mul(f32 lhs, f32 rhs) => Vector128.Multiply(lhs, rhs);
        public static f32 Negate(f32 lhs) => Vector128.Negate(lhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static m32 NotEqual(f32 lhs, f32 rhs)
        {
            if (Sse.IsSupported)
            {
                return Sse.CompareNotEqual(lhs, rhs).AsInt32();
            }
            else
            {
                return ~Vector128.Equals(lhs, rhs).AsInt32();
            }
        }

        public static f32 Or(f32 lhs, f32 rhs) => Vector128.BitwiseOr(lhs, rhs);
        public static f32 RightShift(f32 lhs, [ConstantExpected] byte rhs) => throw new NotSupportedException();
        public static f32 Sub(f32 lhs, f32 rhs) => Vector128.Subtract(lhs, rhs);
        public static f32 Xor(f32 lhs, f32 rhs) => Vector128.Xor(lhs, rhs);

        public static i32 Add(i32 lhs, i32 rhs) => Vector128.Add(lhs, rhs);
        public static i32 And(i32 lhs, i32 rhs) => Vector128.BitwiseAnd(lhs, rhs);
        public static i32 Complement(i32 lhs) => Vector128.OnesComplement(lhs);
        public static i32 Div(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static m32 Equal(i32 lhs, i32 rhs) => Vector128.Equals(lhs, rhs);
        public static m32 GreaterThan(i32 lhs, i32 rhs) => Vector128.GreaterThan(lhs, rhs);
        public static m32 GreaterThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 LeftShift(i32 lhs, [ConstantExpected] byte rhs) => Vector128.ShiftLeft(lhs, rhs);
        public static m32 LessThan(i32 lhs, i32 rhs) => Vector128.LessThan(lhs, rhs);
        public static m32 LessThanOrEqual(i32 lhs, i32 rhs) => throw new NotSupportedException();
        public static i32 Mul(i32 lhs, i32 rhs) => lhs * rhs;
        public static i32 Negate(i32 lhs) => Vector128.Negate(lhs);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static m32 NotEqual(i32 lhs, i32 rhs)
        {
            if (Sse.IsSupported)
            {
                return Sse.CompareNotEqual(lhs.AsSingle(), rhs.AsSingle()).AsInt32();
            }
            else
            {
                return ~Vector128.Equals(lhs.AsSingle(), rhs.AsSingle()).AsInt32();
            }
        }

        public static i32 Or(i32 lhs, i32 rhs) => Vector128.BitwiseOr(lhs, rhs);
        public static i32 RightShift(i32 lhs, [ConstantExpected] byte rhs) => Vector128.ShiftRightArithmetic(lhs, rhs);
        public static i32 Sub(i32 lhs, i32 rhs) => Vector128.Subtract(lhs, rhs);
        public static i32 Xor(i32 lhs, i32 rhs) => Vector128.Xor(lhs, rhs);
    }
}
