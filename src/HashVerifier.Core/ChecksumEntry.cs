using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace HashVerifier.Core;

/// <summary>A digest parsed from user input or from a line of a checksum file.</summary>
/// <param name="Hash">The digest bytes.</param>
/// <param name="FileName">File name the digest refers to, when the input included one.</param>
/// <param name="Algorithm">Algorithm named by the input (for example <c>SHA256 (file) = …</c>), if any.</param>
public sealed partial record ChecksumEntry(byte[] Hash, string? FileName, HashAlgorithmInfo? Algorithm)
{
    /// <summary>The digest as lowercase hexadecimal.</summary>
    public string Hex => HashText.ToHex(Hash);

    /// <summary>
    /// Parses a digest in any of the common formats:
    /// <list type="bullet">
    /// <item>bare hex, optionally split by whitespace: <c>e3b0c442 98fc1c14 …</c></item>
    /// <item>GNU coreutils: <c>e3b0c442…  file.iso</c> or <c>e3b0c442… *file.iso</c></item>
    /// <item>BSD / <c>--tag</c>: <c>SHA256 (file.iso) = e3b0c442…</c></item>
    /// <item>prefixed: <c>sha256:e3b0c442…</c></item>
    /// </list>
    /// Only the first non-empty line is considered.
    /// </summary>
    public static bool TryParse(string? text, [NotNullWhen(true)] out ChecksumEntry? entry)
    {
        entry = null;
        var line = text?.Split('\n').Select(l => l.Trim()).FirstOrDefault(l => l.Length > 0);
        if (line is null)
        {
            return false;
        }

        var bsd = BsdFormat().Match(line);
        if (bsd.Success)
        {
            return TryCreate(bsd.Groups["hash"].Value, bsd.Groups["file"].Value, bsd.Groups["algo"].Value, out entry);
        }

        var prefixed = PrefixedFormat().Match(line);
        if (prefixed.Success && HashAlgorithms.TryParse(prefixed.Groups["algo"].Value, out _))
        {
            return TryCreate(RemoveWhitespace(prefixed.Groups["hash"].Value), null, prefixed.Groups["algo"].Value, out entry);
        }

        // A bare hash, possibly displayed in groups ("abcd 1234 ...").
        var compact = RemoveWhitespace(line);
        if (HashText.IsHex(compact) && HashAlgorithms.IsKnownHexLength(compact.Length))
        {
            return TryCreate(compact, null, null, out entry);
        }

        var gnu = GnuFormat().Match(line);
        if (gnu.Success && HashAlgorithms.IsKnownHexLength(gnu.Groups["hash"].Length))
        {
            return TryCreate(gnu.Groups["hash"].Value, gnu.Groups["file"].Value, null, out entry);
        }

        // Fall back to accepting any even-length hex string; length is validated against the algorithm later.
        return TryCreate(compact, null, null, out entry);
    }

    private static bool TryCreate(string hex, string? fileName, string? algorithmName, [NotNullWhen(true)] out ChecksumEntry? entry)
    {
        entry = null;
        if (!HashText.IsHex(hex))
        {
            return false;
        }

        HashAlgorithms.TryParse(algorithmName, out var algorithm);
        entry = new ChecksumEntry(Convert.FromHexString(hex), string.IsNullOrEmpty(fileName) ? null : fileName, algorithm);
        return true;
    }

    private static string RemoveWhitespace(string value) => string.Concat(value.Where(c => !char.IsWhiteSpace(c)));

    [GeneratedRegex(@"^(?<algo>[A-Za-z0-9_-]+)\s*\((?<file>.*)\)\s*=\s*(?<hash>[0-9A-Fa-f]+)$")]
    private static partial Regex BsdFormat();

    [GeneratedRegex(@"^(?<algo>[A-Za-z0-9_-]+):\s*(?<hash>[0-9A-Fa-f\s]+)$")]
    private static partial Regex PrefixedFormat();

    [GeneratedRegex(@"^(?<hash>[0-9A-Fa-f]+)\s+\*?(?<file>.+)$")]
    private static partial Regex GnuFormat();
}
