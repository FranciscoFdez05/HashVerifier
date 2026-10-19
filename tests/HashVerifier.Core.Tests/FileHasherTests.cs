using System.Text;
using HashVerifier.Core;

namespace HashVerifier.Core.Tests;

public sealed class FileHasherTests : IDisposable
{
    private readonly string _tempFile = Path.GetTempFileName();

    public void Dispose() => File.Delete(_tempFile);

    // Standard test vectors for the message "abc".
    [Theory]
    [InlineData(HashAlgorithmKind.Md5, "900150983cd24fb0d6963f7d28e17f72")]
    [InlineData(HashAlgorithmKind.Sha1, "a9993e364706816aba3e25717850c26c9cd0d89d")]
    [InlineData(HashAlgorithmKind.Sha256, "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData(HashAlgorithmKind.Sha384, "cb00753f45a35e8bb5a03d699ac65007272c32ab0eded1631a8b605a43ff5bed8086072ba1e7cc2358baeca134c825a7")]
    [InlineData(HashAlgorithmKind.Sha512, "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f")]
    public async Task ComputeAsync_MatchesKnownVectors(HashAlgorithmKind algorithm, string expected)
    {
        await File.WriteAllTextAsync(_tempFile, "abc", Encoding.ASCII);

        var hash = await FileHasher.ComputeAsync(_tempFile, algorithm);

        Assert.Equal(expected, HashText.ToHex(hash));
    }

    [Fact]
    public async Task ComputeAsync_Sha3_MatchesKnownVector_WhenSupported()
    {
        if (!HashAlgorithms.Get(HashAlgorithmKind.Sha3_256).IsSupported)
        {
            return;
        }

        await File.WriteAllTextAsync(_tempFile, "abc", Encoding.ASCII);

        var hash = await FileHasher.ComputeAsync(_tempFile, HashAlgorithmKind.Sha3_256);

        Assert.Equal("3a985da74fe225b2045c172d6bd390bd855f086e3e9d525b46bfe24511431532", HashText.ToHex(hash));
    }

    [Fact]
    public async Task ComputeAsync_LargerThanBuffer_MatchesOneShotHash()
    {
        var data = new byte[FileHasher.BufferSize * 3 + 123];
        new Random(42).NextBytes(data);
        await File.WriteAllBytesAsync(_tempFile, data);

        var hash = await FileHasher.ComputeAsync(_tempFile, HashAlgorithmKind.Sha256);

        Assert.Equal(System.Security.Cryptography.SHA256.HashData(data), hash);
    }

    [Fact]
    public async Task ComputeAsync_ReportsProgressUpToOne()
    {
        await File.WriteAllBytesAsync(_tempFile, new byte[FileHasher.BufferSize * 2]);
        var reports = new List<double>();

        await FileHasher.ComputeAsync(_tempFile, HashAlgorithmKind.Md5, new SyncProgress(reports.Add));

        Assert.Equal(0, reports[0]);
        Assert.Equal(1, reports[^1]);
        Assert.Equal(reports.Order(), reports);
    }

    [Fact]
    public async Task ComputeAsync_HonoursCancellation()
    {
        await File.WriteAllBytesAsync(_tempFile, new byte[1024]);
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => FileHasher.ComputeAsync(_tempFile, HashAlgorithmKind.Sha256, cancellationToken: cts.Token));
    }

    [Fact]
    public async Task VerifyAsync_DetectsMatchAndMismatch()
    {
        await File.WriteAllTextAsync(_tempFile, "abc", Encoding.ASCII);
        var good = Convert.FromHexString("900150983cd24fb0d6963f7d28e17f72");
        var bad = Convert.FromHexString("000150983cd24fb0d6963f7d28e17f72");

        Assert.True((await FileHasher.VerifyAsync(_tempFile, HashAlgorithmKind.Md5, good)).IsMatch);
        Assert.False((await FileHasher.VerifyAsync(_tempFile, HashAlgorithmKind.Md5, bad)).IsMatch);
    }

    /// <summary>Progress&lt;T&gt; posts asynchronously; this one reports inline so the test is deterministic.</summary>
    private sealed class SyncProgress(Action<double> report) : IProgress<double>
    {
        public void Report(double value) => report(value);
    }
}
