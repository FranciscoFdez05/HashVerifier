using HashVerifier.Core;

namespace HashVerifier.Core.Tests;

public sealed class ChecksumEntryTests
{
    private const string Sha256 = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad";

    [Theory]
    [InlineData(Sha256)]
    [InlineData("  BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD  ")]
    [InlineData("ba7816bf 8f01cfea 414140de 5dae2223 b00361a3 96177a9c b410ff61 f20015ad")]
    [InlineData(Sha256 + "\r\n")]
    public void TryParse_BareHash(string input)
    {
        Assert.True(ChecksumEntry.TryParse(input, out var entry));
        Assert.Equal(Sha256, entry.Hex);
        Assert.Null(entry.FileName);
        Assert.Null(entry.Algorithm);
    }

    [Theory]
    [InlineData(Sha256 + "  ubuntu.iso", "ubuntu.iso")]
    [InlineData(Sha256 + " *ubuntu.iso", "ubuntu.iso")]
    [InlineData(Sha256 + "  my file.iso", "my file.iso")]
    public void TryParse_GnuFormat(string input, string fileName)
    {
        Assert.True(ChecksumEntry.TryParse(input, out var entry));
        Assert.Equal(Sha256, entry.Hex);
        Assert.Equal(fileName, entry.FileName);
    }

    [Fact]
    public void TryParse_BsdFormat_CapturesAlgorithm()
    {
        Assert.True(ChecksumEntry.TryParse($"SHA256 (ubuntu.iso) = {Sha256}", out var entry));
        Assert.Equal(Sha256, entry.Hex);
        Assert.Equal("ubuntu.iso", entry.FileName);
        Assert.Equal(HashAlgorithmKind.Sha256, entry.Algorithm?.Kind);
    }

    [Theory]
    [InlineData("sha256:" + Sha256)]
    [InlineData("SHA-256: " + Sha256)]
    public void TryParse_PrefixedFormat_DoesNotTreatPrefixAsHex(string input)
    {
        // The previous implementation stripped non-hex characters, turning "sha256:" into "a256".
        Assert.True(ChecksumEntry.TryParse(input, out var entry));
        Assert.Equal(Sha256, entry.Hex);
        Assert.Equal(HashAlgorithmKind.Sha256, entry.Algorithm?.Kind);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not a hash")]
    [InlineData("abc")] // odd length
    [InlineData("zz7816bf8f01cfea414140de5dae2223")]
    public void TryParse_RejectsInvalidInput(string? input)
    {
        Assert.False(ChecksumEntry.TryParse(input, out _));
    }
}
