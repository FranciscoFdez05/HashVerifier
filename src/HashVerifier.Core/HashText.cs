using System.Security.Cryptography;

namespace HashVerifier.Core;

/// <summary>Conversions between digests and their text representation.</summary>
public static class HashText
{
    /// <summary>Formats a digest as lowercase hexadecimal.</summary>
    public static string ToHex(ReadOnlySpan<byte> hash) => Convert.ToHexStringLower(hash);

    /// <summary>Whether <paramref name="text"/> is a non-empty string of hexadecimal digits with even length.</summary>
    public static bool IsHex(ReadOnlySpan<char> text) =>
        !text.IsEmpty && text.Length % 2 == 0 && !text.ContainsAnyExcept("0123456789abcdefABCDEF");

    /// <summary>Compares two digests in constant time.</summary>
    public static bool AreEqual(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) =>
        left.Length == right.Length && CryptographicOperations.FixedTimeEquals(left, right);
}
