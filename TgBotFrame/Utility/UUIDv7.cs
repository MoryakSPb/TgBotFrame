#if !NET9_0_OR_GREATER
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace TgBotFrame.Utility;

public static class UUIDv7
{
    public static readonly Guid Max = new([
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
        255,
    ]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid NewUUIDv7Fast()
    {
        Span<byte> result = stackalloc byte[16];

        BinaryPrimitives.WriteInt64BigEndian(result, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() << 16);

        Span<byte> randomSpan = result[6..];
        Random.Shared.NextBytes(randomSpan);

        result[6] &= 0b0000_1111;
        result[6] |= 0b0111_0000;
        result[8] &= 0b0011_1111;
        result[8] |= 0b1000_0000;

        return new(result, true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid NewUUIDv7()
    {
        Span<byte> result = stackalloc byte[16];

        BinaryPrimitives.WriteInt64BigEndian(result, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() << 16);

        Span<byte> randomSpan = result[6..];
        RandomNumberGenerator.Fill(randomSpan);

        result[6] &= 0b0000_1111;
        result[6] |= 0b0111_0000;
        result[8] &= 0b0011_1111;
        result[8] |= 0b1000_0000;

        return new(result, true);
    }
}
#endif