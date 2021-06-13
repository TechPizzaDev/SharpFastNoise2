using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    internal static class BoolExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte AsByte(this bool value)
        {
            return Unsafe.As<bool, byte>(ref value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AsInt32(this bool value)
        {
            return value.AsByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AsSingle(this bool value)
        {
            int raw = value.AsInt32();
            return Unsafe.As<int, float>(ref raw);
        }
    }
}
