using HashVerifier.Core;

namespace HashVerifier.Core.Tests;

public sealed class HashAlgorithmsTests
{
    [Theory]
    [InlineData("sha256", HashAlgorithmKind.Sha256)]
    [InlineData("SHA-256", HashAlgorithmKind.Sha256)]
    [InlineData("Sha_512", HashAlgorithmKind.Sha512)]
    [InlineData("md5", HashAlgorithmKind.Md5)]
    [InlineData("sha3-256", HashAlgorithmKind.Sha3_256)]
    public void TryParse_AcceptsCommonSpellings(string name, HashAlgorithmKind expected)
    {
        Assert.True(HashAlgorithms.TryParse(name, out var algorithm));
        Assert.Equal(expected, algorithm.Kind);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("sha224")]
    [InlineData("crc32")]
    public void TryParse_RejectsUnknown(string? name)
    {
        Assert.False(HashAlgorithms.TryParse(name, out _));
    }

    [Theory]
    [InlineData(32, HashAlgorithmKind.Md5)]
    [InlineData(40, HashAlgorithmKind.Sha1)]
    [InlineData(64, HashAlgorithmKind.Sha256)]
    [InlineData(96, HashAlgorithmKind.Sha384)]
    [InlineData(128, HashAlgorithmKind.Sha512)]
    public void WithHexLength_PrefersMostCommonAlgorithm(int length, HashAlgorithmKind expected)
    {
        Assert.Equal(expected, HashAlgorithms.WithHexLength(length).First().Kind);
    }

    [Fact]
    public void All_HasUniqueIds() =>
        Assert.Equal(HashAlgorithms.All.Count, HashAlgorithms.All.Select(a => a.Id).Distinct().Count());
}
