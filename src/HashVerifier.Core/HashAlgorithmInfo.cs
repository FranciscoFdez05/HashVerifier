using System.Security.Cryptography;

namespace HashVerifier.Core;

/// <summary>Describes a hash algorithm and how to create it.</summary>
/// <param name="Kind">The algorithm identifier.</param>
/// <param name="Id">Short, lowercase identifier used on the command line (for example <c>sha256</c>).</param>
/// <param name="DisplayName">Human-readable name (for example <c>SHA-256</c>).</param>
/// <param name="Name">The .NET algorithm name used to create the hash.</param>
/// <param name="HashSizeBytes">Size of the digest in bytes.</param>
/// <param name="IsCryptographicallyBroken">True for algorithms that must not be relied on against tampering (MD5, SHA-1).</param>
public sealed record HashAlgorithmInfo(
    HashAlgorithmKind Kind,
    string Id,
    string DisplayName,
    HashAlgorithmName Name,
    int HashSizeBytes,
    bool IsCryptographicallyBroken)
{
    /// <summary>Length of the digest when written as hexadecimal text.</summary>
    public int HexLength => HashSizeBytes * 2;

    /// <summary>Whether the current platform can compute this algorithm.</summary>
    public bool IsSupported => Kind switch
    {
        HashAlgorithmKind.Sha3_256 => SHA3_256.IsSupported,
        HashAlgorithmKind.Sha3_384 => SHA3_384.IsSupported,
        HashAlgorithmKind.Sha3_512 => SHA3_512.IsSupported,
        _ => true,
    };

    /// <inheritdoc />
    public override string ToString() => DisplayName;
}
