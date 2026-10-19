namespace HashVerifier.Core;

/// <summary>Hash algorithms supported by HashVerifier.</summary>
public enum HashAlgorithmKind
{
    /// <summary>MD5 (128-bit). Broken; use only to compare against published legacy checksums.</summary>
    Md5,

    /// <summary>SHA-1 (160-bit). Broken; use only to compare against published legacy checksums.</summary>
    Sha1,

    /// <summary>SHA-256 (SHA-2 family).</summary>
    Sha256,

    /// <summary>SHA-384 (SHA-2 family).</summary>
    Sha384,

    /// <summary>SHA-512 (SHA-2 family).</summary>
    Sha512,

    /// <summary>SHA3-256. Requires OS support.</summary>
    Sha3_256,

    /// <summary>SHA3-384. Requires OS support.</summary>
    Sha3_384,

    /// <summary>SHA3-512. Requires OS support.</summary>
    Sha3_512,
}
