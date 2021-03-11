using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    public static class UnsafeR
    {
        public static ref readonly TTo As<TFrom, TTo>(in TFrom value)
        {
            return ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(value));
        }
    }
}
