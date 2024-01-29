using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    internal static class BoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AsUInt32(this bool value)
        {
            return value ? uint.MaxValue : 0;
        }
    }
}
