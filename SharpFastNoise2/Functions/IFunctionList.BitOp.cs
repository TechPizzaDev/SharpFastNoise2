
namespace SharpFastNoise2.Functions;

public partial interface IFunctionList<f32, i32, F>
    where F : IFunctionList<f32, i32, F>
{
    static abstract int Log2(i32 a);
    static abstract int PopCount(i32 a);

    static abstract int LeadingZeroCount(i32 a);
    static abstract int TrailingZeroCount(i32 a);
}
