using System.Reflection;
using HashVerifier.Core;

namespace HashVerifier.Cli;

/// <summary>
/// Command-line front end. Output follows the GNU coreutils format (<c>hash  file</c>)
/// so results can be checked with <c>sha256sum -c</c> and vice versa.
/// </summary>
internal static class Program
{
    private const int ExitOk = 0;
    private const int ExitMismatch = 1;
    private const int ExitError = 2;

    private const string Usage = """
        Usage:
          hashverifier-cli hash <file>... [-a <algorithm>]        Print the hash of each file
          hashverifier-cli verify <file> <hash> [-a <algorithm>]  Compare a file with an expected hash
          hashverifier-cli check <checksum-file> [-a <algorithm>] Verify every entry of a *SUMS file
          hashverifier-cli algorithms                             List supported algorithms

        Options:
          -a, --algorithm   md5, sha1, sha256 (default), sha384, sha512, sha3-256, sha3-384, sha3-512.
                            For 'verify' and 'check' it is inferred from the hash when omitted.
          -h, --help        Show this help
          -v, --version     Show version

        Exit codes: 0 = success, 1 = hash mismatch, 2 = error.
        """;

    private static async Task<int> Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        try
        {
            return await RunAsync(args, cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.Error.WriteLine("Cancelled.");
            return ExitError;
        }
        catch (UsageException ex)
        {
            Console.Error.WriteLine($"error: {ex.Message}");
            Console.Error.WriteLine("Run 'hashverifier-cli --help' for usage.");
            return ExitError;
        }
    }

    private static Task<int> RunAsync(string[] args, CancellationToken ct)
    {
        if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
        {
            Console.WriteLine(Usage);
            return Task.FromResult(ExitOk);
        }

        if (args[0] is "-v" or "--version")
        {
            var version = typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            Console.WriteLine($"hashverifier-cli {version}");
            return Task.FromResult(ExitOk);
        }

        var (positional, algorithm) = ParseOptions(args.AsSpan(1));
        return args[0] switch
        {
            "hash" => HashAsync(positional, algorithm ?? HashAlgorithms.Get(HashAlgorithms.Default), ct),
            "verify" => VerifyAsync(positional, algorithm, ct),
            "check" => CheckAsync(positional, algorithm, ct),
            "algorithms" => Task.FromResult(ListAlgorithms()),
            _ => throw new UsageException($"unknown command '{args[0]}'."),
        };
    }

    private static async Task<int> HashAsync(List<string> files, HashAlgorithmInfo algorithm, CancellationToken ct)
    {
        if (files.Count == 0)
        {
            throw new UsageException("'hash' needs at least one file.");
        }

        var exitCode = ExitOk;
        foreach (var file in files)
        {
            try
            {
                var hash = await FileHasher.ComputeAsync(file, algorithm.Kind, cancellationToken: ct);
                Console.WriteLine($"{HashText.ToHex(hash)}  {file}");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                Console.Error.WriteLine($"{file}: {ex.Message}");
                exitCode = ExitError;
            }
        }

        return exitCode;
    }

    private static async Task<int> VerifyAsync(List<string> args, HashAlgorithmInfo? algorithm, CancellationToken ct)
    {
        if (args.Count != 2)
        {
            throw new UsageException("'verify' needs a file and an expected hash.");
        }

        var (file, expectedText) = (args[0], args[1]);
        if (!ChecksumEntry.TryParse(expectedText, out var expected))
        {
            throw new UsageException("the expected hash is not valid hexadecimal.");
        }

        algorithm = ResolveAlgorithm(algorithm, expected);
        try
        {
            var (actual, isMatch) = await FileHasher.VerifyAsync(file, algorithm.Kind, expected.Hash, cancellationToken: ct);
            Console.WriteLine(isMatch
                ? $"OK        {file} ({algorithm.DisplayName})"
                : $"MISMATCH  {file} ({algorithm.DisplayName})\n  expected: {expected.Hex}\n  actual:   {HashText.ToHex(actual)}");
            return isMatch ? ExitOk : ExitMismatch;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"{file}: {ex.Message}");
            return ExitError;
        }
    }

    private static async Task<int> CheckAsync(List<string> args, HashAlgorithmInfo? algorithm, CancellationToken ct)
    {
        if (args.Count != 1)
        {
            throw new UsageException("'check' needs exactly one checksum file.");
        }

        var checksumFile = Path.GetFullPath(args[0]);
        var baseDirectory = Path.GetDirectoryName(checksumFile)!;
        int ok = 0, failed = 0, errors = 0;

        foreach (var (line, lineNumber) in (await File.ReadAllLinesAsync(checksumFile, ct)).Select((l, i) => (l.Trim(), i + 1)))
        {
            if (line.Length == 0 || line.StartsWith('#'))
            {
                continue;
            }

            if (!ChecksumEntry.TryParse(line, out var entry) || entry.FileName is null)
            {
                Console.Error.WriteLine($"{args[0]}:{lineNumber}: improperly formatted line");
                errors++;
                continue;
            }

            var path = Path.Combine(baseDirectory, entry.FileName);
            try
            {
                var lineAlgorithm = ResolveAlgorithm(algorithm, entry);
                var (_, isMatch) = await FileHasher.VerifyAsync(path, lineAlgorithm.Kind, entry.Hash, cancellationToken: ct);
                Console.WriteLine($"{entry.FileName}: {(isMatch ? "OK" : "FAILED")}");
                if (isMatch) { ok++; } else { failed++; }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or UsageException)
            {
                Console.WriteLine($"{entry.FileName}: FAILED open or read ({ex.Message})");
                errors++;
            }
        }

        Console.Error.WriteLine($"{ok} OK, {failed} mismatched, {errors} errors");
        return errors > 0 ? ExitError : failed > 0 ? ExitMismatch : ExitOk;
    }

    private static int ListAlgorithms()
    {
        foreach (var a in HashAlgorithms.All)
        {
            var notes = a.IsSupported ? (a.IsCryptographicallyBroken ? "legacy, not collision-resistant" : "") : "not supported on this system";
            Console.WriteLine($"{a.Id,-10} {a.HexLength,4} hex chars  {notes}");
        }

        return ExitOk;
    }

    /// <summary>Uses the explicit algorithm, else the one named in the input, else infers it from the digest length.</summary>
    private static HashAlgorithmInfo ResolveAlgorithm(HashAlgorithmInfo? explicitAlgorithm, ChecksumEntry entry)
    {
        var algorithm = explicitAlgorithm
            ?? entry.Algorithm
            ?? HashAlgorithms.WithHexLength(entry.Hex.Length).FirstOrDefault()
            ?? throw new UsageException($"cannot infer the algorithm for a {entry.Hex.Length}-character hash; use -a.");

        if (algorithm.HexLength != entry.Hex.Length)
        {
            throw new UsageException($"{algorithm.DisplayName} hashes have {algorithm.HexLength} hex characters, got {entry.Hex.Length}.");
        }

        return algorithm;
    }

    private static (List<string> Positional, HashAlgorithmInfo? Algorithm) ParseOptions(ReadOnlySpan<string> args)
    {
        var positional = new List<string>();
        HashAlgorithmInfo? algorithm = null;

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] is "-a" or "--algorithm")
            {
                if (++i >= args.Length)
                {
                    throw new UsageException("missing value for --algorithm.");
                }

                if (!HashAlgorithms.TryParse(args[i], out algorithm))
                {
                    throw new UsageException($"unknown algorithm '{args[i]}'. Run 'hashverifier-cli algorithms'.");
                }

                if (!algorithm.IsSupported)
                {
                    throw new UsageException($"{algorithm.DisplayName} is not supported on this system.");
                }
            }
            else
            {
                positional.Add(args[i]);
            }
        }

        return (positional, algorithm);
    }

    private sealed class UsageException(string message) : Exception(message);
}
