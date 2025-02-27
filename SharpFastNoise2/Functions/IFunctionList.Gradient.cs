﻿using System.Runtime.CompilerServices;

namespace SharpFastNoise2.Functions
{
    public partial interface IFunctionList<f32, i32, F>
        where F : IFunctionList<f32, i32, F>
    {
        const MethodImplOptions Options = 
            MethodImplOptions.AggressiveOptimization | 
            MethodImplOptions.AggressiveInlining; 

        // Gradient dot fancy
        
        [MethodImpl(Options)]
        static virtual f32 GetGradientDotFancy(i32 hash, f32 fX, f32 fY)
        {
            i32 index = F.Convert_i32(F.Mul(
                F.Convert_f32(F.And(hash, F.Broad(0x3FFFFF))),
                F.Broad(1.3333333333333333f)));

            // Bit-4 = Choose X Y ordering
            f32 xy = F.Cast_f32(F.NotEqual(F.And(index, F.Broad(1 << 2)), F.Broad(0)));
            f32 a = F.Select(xy, fY, fX);
            f32 b = F.Select(xy, fX, fY);

            // Bit-1 = b flip sign
            b = F.Xor(b, F.Cast_f32(F.LeftShift(index, 31)));

            // Bit-2 = Mul a by 2 or Root3
            f32 aMul2 = F.Cast_f32(F.NotEqual(F.And(index, F.Broad(1 << 1)), F.Broad(0)));

            a = F.Mul(a, F.Select(aMul2, F.Broad(2f), F.Broad(Gradient.ROOT3)));
            // b zero value if a mul 2
            b = F.NMask(b, aMul2);

            // Bit-8 = Flip sign of a + b
            return F.Xor(F.Add(a, b), F.Cast_f32(F.LeftShift(F.RightShift(index, 3), 31)));
        }

        // Gradient dot
        
        [MethodImpl(Options)]
        static virtual f32 GetGradientDot(i32 hash, f32 fX, f32 fY)
        {
            // ( 1+R2, 1 ) ( -1-R2, 1 ) ( 1+R2, -1 ) ( -1-R2, -1 )
            // ( 1, 1+R2 ) ( 1, -1-R2 ) ( -1, 1+R2 ) ( -1, -1-R2 )

            i32 bit1 = F.LeftShift(hash, 31);
            i32 bit2 = F.LeftShift(F.RightShift(hash, 1), 31);
            f32 mbit4 = F.Cast_f32(F.NotEqual(F.And(hash, F.Broad(1 << 2)), F.Broad(0)));

            fX = F.Xor(fX, F.Cast_f32(bit1));
            fY = F.Xor(fY, F.Cast_f32(bit2));

            f32 a = F.Select(mbit4, fY, fX);
            f32 b = F.Select(mbit4, fX, fY);

            return F.FMulAdd(F.Broad(1.0f + Gradient.ROOT2), a, b);
        }
        
        [MethodImpl(Options)]
        static virtual f32 GetGradientDot(i32 hash, f32 fX, f32 fY, f32 fZ)
        {
            i32 hasha13 = F.And(hash, F.Broad(13));

            //if h < 8 then x, else y
            f32 u = F.Select(F.Cast_f32(F.LessThan(hasha13, F.Broad(8))), fX, fY);

            //if h < 4 then y else if h is 12 or 14 then x else z
            f32 v = F.Select(F.Cast_f32(F.Equal(hasha13, F.Broad(12))), fX, fZ);
            v = F.Select(F.Cast_f32(F.LessThan(hasha13, F.Broad(2))), fY, v);

            //if h1 then -u else u
            //if h2 then -v else v
            f32 h1 = F.Cast_f32(F.LeftShift(hash, 31));
            f32 h2 = F.Cast_f32(F.LeftShift(F.And(hash, F.Broad(2)), 30));
            //then add them
            return F.Add(F.Xor(u, h1), F.Xor(v, h2));
        }
        
        [MethodImpl(Options)]
        static virtual f32 GetGradientDot(i32 hash, f32 fX, f32 fY, f32 fZ, f32 fW)
        {
            i32 p = F.And(hash, F.Broad(3 << 3));

            f32 a = F.Select(F.Cast_f32(F.GreaterThan(p, F.Broad(0))), fX, fY);
            f32 b = F.Select(F.Cast_f32(F.GreaterThan(p, F.Broad(1 << 3))), fY, fZ);
            f32 c = F.Select(F.Cast_f32(F.GreaterThan(p, F.Broad(2 << 3))), fZ, fW);

            f32 aSign = F.Cast_f32(F.LeftShift(hash, 31));
            f32 bSign = F.And(F.Cast_f32(F.LeftShift(hash, 30)), F.Broad(-0f));
            f32 cSign = F.And(F.Cast_f32(F.LeftShift(hash, 29)), F.Broad(-0f));
            return F.Add(F.Add(F.Xor(a, aSign), F.Xor(b, bSign)), F.Xor(c, cSign));
        }
    }
}
