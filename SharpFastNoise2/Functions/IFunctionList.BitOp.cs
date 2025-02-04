
namespace SharpFastNoise2.Functions;

public partial interface IFunctionList<m32, f32, i32, F>
    where F : IFunctionList<m32, f32, i32, F>
{
    static abstract int Log2(m32 a);
    static abstract int PopCount(m32 a);

    static abstract int LeadingZeroCount(m32 a);
    static abstract int TrailingZeroCount(m32 a);
}
