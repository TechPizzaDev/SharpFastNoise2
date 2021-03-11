using System.Runtime.CompilerServices;

namespace SharpFastNoise2
{
    internal static class BoolExtensions
    {
        public static float AsSingle(this bool value)
        {
            int raw = value.AsInt32();
            return Unsafe.As<int, float>(ref raw);
        }

        public static int AsInt32(this bool value)
        {
            return Unsafe.As<bool, byte>(ref value);
        }
    }
}
