using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    internal static class BoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AsInt32(this bool value)
        {
            return value ? -1 : 0;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AsFloat(this bool value)
        {
            return Unsafe.BitCast<int, float>(value ? -1 : 0);
        }
    }
}
