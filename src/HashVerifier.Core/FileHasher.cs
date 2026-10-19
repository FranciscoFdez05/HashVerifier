using System.Buffers;
using System.Security.Cryptography;

namespace HashVerifier.Core;

/// <summary>Computes file digests by streaming, with progress reporting and cancellation.</summary>
public static class FileHasher
{
    /// <summary>Read buffer size. 1 MiB keeps large files fast without holding much memory.</summary>
    public const int BufferSize = 1024 * 1024;

    /// <summary>Computes the digest of the file at <paramref name="path"/>.</summary>
    /// <param name="path">File to hash.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="progress">Receives the fraction processed, from 0 to 1.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    public static async Task<byte[]> ComputeAsync(
        string path,
        HashAlgorithmKind algorithm,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await using var stream = new FileStream(path, new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.Read,
            BufferSize = 0, // we already read in large blocks
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
        });

        return await ComputeAsync(stream, algorithm, progress, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Computes the digest of <paramref name="stream"/> from its current position to the end.</summary>
    public static async Task<byte[]> ComputeAsync(
        Stream stream,
        HashAlgorithmKind algorithm,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var info = HashAlgorithms.Get(algorithm);
        if (!info.IsSupported)
        {
            throw new PlatformNotSupportedException($"{info.DisplayName} is not supported on this platform.");
        }

        using var hash = IncrementalHash.CreateHash(info.Name);
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            var total = stream.CanSeek ? stream.Length - stream.Position : -1;
            long processed = 0;
            var lastPercent = -1;

            progress?.Report(0);
            int read;
            while ((read = await stream.ReadAsync(buffer.AsMemory(0, BufferSize), cancellationToken).ConfigureAwait(false)) > 0)
            {
                hash.AppendData(buffer, 0, read);
                processed += read;

                // Only report whole-percent changes so the UI is not flooded with updates.
                if (progress is not null && total > 0)
                {
                    var percent = (int)(processed * 100 / total);
                    if (percent != lastPercent)
                    {
                        lastPercent = percent;
                        progress.Report(Math.Min(1.0, (double)processed / total));
                    }
                }
            }

            progress?.Report(1);
            return hash.GetHashAndReset();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>Computes the digest of <paramref name="path"/> and compares it with <paramref name="expected"/>.</summary>
    /// <returns>The actual digest and whether it matches.</returns>
    public static async Task<(byte[] Actual, bool IsMatch)> VerifyAsync(
        string path,
        HashAlgorithmKind algorithm,
        ReadOnlyMemory<byte> expected,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var actual = await ComputeAsync(path, algorithm, progress, cancellationToken).ConfigureAwait(false);
        return (actual, HashText.AreEqual(actual, expected.Span));
    }
}
