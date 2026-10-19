using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace HashVerifier.Core;

/// <summary>Catalogue of the hash algorithms known to HashVerifier.</summary>
public static class HashAlgorithms
{
    /// <summary>The algorithm used when the user does not choose one.</summary>
    public const HashAlgorithmKind Default = HashAlgorithmKind.Sha256;

    /// <summary>Every known algorithm, whether or not the platform supports it.</summary>
    public static IReadOnlyList<HashAlgorithmInfo> All { get; } =
    [
        new(HashAlgorithmKind.Md5, "md5", "MD5", HashAlgorithmName.MD5, 16, true),
        new(HashAlgorithmKind.Sha1, "sha1", "SHA-1", HashAlgorithmName.SHA1, 20, true),
        new(HashAlgorithmKind.Sha256, "sha256", "SHA-256", HashAlgorithmName.SHA256, 32, false),
        new(HashAlgorithmKind.Sha384, "sha384", "SHA-384", HashAlgorithmName.SHA384, 48, false),
        new(HashAlgorithmKind.Sha512, "sha512", "SHA-512", HashAlgorithmName.SHA512, 64, false),
        new(HashAlgorithmKind.Sha3_256, "sha3-256", "SHA3-256", HashAlgorithmName.SHA3_256, 32, false),
        new(HashAlgorithmKind.Sha3_384, "sha3-384", "SHA3-384", HashAlgorithmName.SHA3_384, 48, false),
        new(HashAlgorithmKind.Sha3_512, "sha3-512", "SHA3-512", HashAlgorithmName.SHA3_512, 64, false),
    ];

    /// <summary>The algorithms the current platform can compute.</summary>
    public static IReadOnlyList<HashAlgorithmInfo> Supported { get; } = [.. All.Where(a => a.IsSupported)];

    /// <summary>Gets the description of <paramref name="kind"/>.</summary>
    public static HashAlgorithmInfo Get(HashAlgorithmKind kind) =>
        All.FirstOrDefault(a => a.Kind == kind)
        ?? throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown hash algorithm.");

    /// <summary>
    /// Parses an algorithm name, ignoring case and separators, so that
    /// <c>sha256</c>, <c>SHA-256</c> and <c>Sha_256</c> are all accepted.
    /// </summary>
    public static bool TryParse(string? name, [NotNullWhen(true)] out HashAlgorithmInfo? algorithm)
    {
        algorithm = null;
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var key = Normalize(name);
        algorithm = All.FirstOrDefault(a => Normalize(a.Id) == key);
        return algorithm is not null;
    }

    /// <summary>
    /// Supported algorithms whose hexadecimal digest has <paramref name="hexLength"/> characters,
    /// most common first (for example SHA-256 before SHA3-256).
    /// </summary>
    public static IEnumerable<HashAlgorithmInfo> WithHexLength(int hexLength) =>
        Supported.Where(a => a.HexLength == hexLength);

    /// <summary>Whether any known algorithm produces a digest with <paramref name="hexLength"/> characters.</summary>
    public static bool IsKnownHexLength(int hexLength) => All.Any(a => a.HexLength == hexLength);

    private static string Normalize(string name) =>
        string.Concat(name.Where(char.IsLetterOrDigit)).ToUpperInvariant();
}
