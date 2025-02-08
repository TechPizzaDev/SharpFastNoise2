using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace Sandbox;

public struct L32 : IPixel<L32>, IPackedVector<uint>
{
    private uint _packedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="L32"/> struct.
    /// </summary>
    /// <param name="luminance">The luminance component</param>
    public L32(uint luminance) => _packedValue = luminance;

    public L32(int luminance)
    {

    }

    /// <inheritdoc />
    public uint PackedValue
    {
        readonly get => _packedValue;
        set => _packedValue = value;
    }

    /// <summary>
    /// Compares two <see cref="L32"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="L32"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="L32"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(L32 left, L32 right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="L32"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="L32"/> on the left side of the operand.</param>
    /// <param name="right">The <see cref="L32"/> on the right side of the operand.</param>
    /// <returns>
    /// True if the <paramref name="left"/> parameter is not equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(L32 left, L32 right) => !left.Equals(right);

    /// <inheritdoc />
    public readonly PixelOperations<L32> CreatePixelOperations() => new();

    /// <inheritdoc/>
    public void FromScaledVector4(Vector4 vector) => ConvertFromRgbaScaledVector4(vector);

    /// <inheritdoc/>
    public readonly Vector4 ToScaledVector4() => ToVector4();

    /// <inheritdoc />
    public void FromVector4(Vector4 vector) => ConvertFromRgbaScaledVector4(vector);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Vector4 ToVector4()
    {
        float scaled = _packedValue / (float) uint.MaxValue;
        return new Vector4(scaled, scaled, scaled, 1F);
    }

    /// <inheritdoc/>
    public void FromArgb32(Argb32 source) => _packedValue = GetBT709FromU8(source.R, source.G, source.B);

    /// <inheritdoc/>
    public void FromBgr24(Bgr24 source) => _packedValue = GetBT709FromU8(source.R, source.G, source.B);

    /// <inheritdoc/>
    public void FromBgra32(Bgra32 source) => _packedValue = GetBT709FromU8(source.R, source.G, source.B);

    /// <inheritdoc/>
    public void FromAbgr32(Abgr32 source) => _packedValue = GetBT709FromU8(source.R, source.G, source.B);

    /// <inheritdoc/>
    public void FromBgra5551(Bgra5551 source) => FromScaledVector4(source.ToScaledVector4());

    /// <inheritdoc />
    public void FromL8(L8 source) => _packedValue = UpscaleU8ToU32(source.PackedValue);

    /// <inheritdoc />
    public void FromL32(L32 source) => this = source;

    /// <inheritdoc/>
    public void FromLa16(La16 source) => _packedValue = UpscaleU8ToU32(source.L);

    /// <inheritdoc/>
    public void FromLa32(La32 source) => _packedValue = UpscaleU16ToU32(source.L);

    /// <inheritdoc />
    public void FromRgb24(Rgb24 source) => _packedValue = GetBT709FromU8(source.R, source.G, source.B);

    /// <inheritdoc />
    public void FromRgba32(Rgba32 source) => _packedValue = GetBT709FromU8(source.R, source.G, source.B);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void ToRgba32(ref Rgba32 dest)
    {
        byte rgb = DownscaleFromU32ToU8(_packedValue);
        dest.R = rgb;
        dest.G = rgb;
        dest.B = rgb;
        dest.A = byte.MaxValue;
    }

    /// <inheritdoc/>
    public void FromRgb48(Rgb48 source) => _packedValue = GetBT709FromU16(source.R, source.G, source.B);

    /// <inheritdoc/>
    public void FromRgba64(Rgba64 source) => _packedValue = GetBT709FromU16(source.R, source.G, source.B);

    /// <inheritdoc />
    public override readonly bool Equals(object? obj) => obj is L32 other && Equals(other);

    /// <inheritdoc />
    public readonly bool Equals(L32 other) => _packedValue == other._packedValue;

    /// <inheritdoc />
    public override readonly string ToString() => $"L32({_packedValue})";

    /// <inheritdoc />
    public override readonly int GetHashCode() => _packedValue.GetHashCode();

    /// <inheritdoc />
    public void FromL16(L16 source) => _packedValue = UpscaleU16ToU32(source.PackedValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ConvertFromRgbaScaledVector4(Vector4 vector)
    {
#if NET9_0_OR_GREATER
        Vector4 clamped = Vector4.ClampNative(vector, Vector4.Zero, Vector4.One);
#else
        Vector4 clamped = Vector4.Clamp(vector, Vector4.Zero, Vector4.One);
#endif
        _packedValue = UpscaleF32ToU32(GetBT709FromF32(clamped));
    }

    /// <summary>
    /// Gets the luminance from the rgb components using the formula as
    /// specified by ITU-R Recommendation BT.709.
    /// </summary>
    /// <param name="r">The red component.</param>
    /// <param name="g">The green component.</param>
    /// <param name="b">The blue component.</param>
    /// <returns>The <see cref="float"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float GetBT709FromF32(Vector4 value)
    {
        return Vector4.Dot(value, new Vector4(.2126F, .7152F, .0722F, 0));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint GetBT709FromU16(ushort r, ushort g, ushort b)
    {
        const float scale = 65537f;
        return UpscaleF32ToU32(r * (.2126F * scale) + g * (.7152F * scale) + b * (.0722F * scale));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint GetBT709FromU8(byte r, byte g, byte b)
    {
        const uint scale = 257u;
        return GetBT709FromU16((ushort) (r * scale), (ushort) (g * scale), (ushort) (b * scale));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte DownscaleFromU16ToU8(ushort value) => (byte) (((value * 255) + 32895) >> 16);

    internal static byte DownscaleFromU32ToU8(uint value) => (byte) (value / 16843009u);

    internal static uint UpscaleU8ToU32(byte value) => value * 16843009u;

    internal static uint UpscaleU16ToU32(ushort value) => value * 65537u;

    public static uint UpscaleF32ToU32(float value)
    {
#if NET9_0_OR_GREATER
        return float.ConvertToIntegerNative<uint>(value);
#else
        return (value >= uint.MaxValue) ? uint.MaxValue : (uint) value;
#endif
    }
}
