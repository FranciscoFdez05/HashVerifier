<#
.SYNOPSIS
    Builds, tests and optionally publishes HashVerifier.

.DESCRIPTION
    Without -Publish, restores, builds and runs the unit tests.
    With -Publish, also produces self-contained single-file executables for the
    GUI and the CLI, and copies them to ./artifacts with the version in the name
    (HashVerifier-<version>-<runtime>.exe), plus a SHA256SUMS file, ready to attach
    to a GitHub release.

.EXAMPLE
    ./scripts/build.ps1
.EXAMPLE
    ./scripts/build.ps1 -Publish -Runtime win-arm64
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [switch] $Publish,

    [ValidateSet('win-x64', 'win-x86', 'win-arm64')]
    [string] $Runtime = 'win-x64',

    [switch] $SkipTests
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot
$solution = Join-Path $root 'HashVerifier.sln'

function Invoke-Step([string] $Name, [scriptblock] $Action) {
    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0) { throw "$Name failed with exit code $LASTEXITCODE." }
}

Invoke-Step 'Restore' { dotnet restore $solution }
Invoke-Step 'Build' { dotnet build $solution --configuration $Configuration --no-restore }

if (-not $SkipTests) {
    Invoke-Step 'Test' { dotnet test $solution --configuration $Configuration --no-build }
}

if ($Publish) {
    $output = Join-Path $root "artifacts/$Runtime"
    if (Test-Path $output) { Remove-Item $output -Recurse -Force }

    $publishArgs = @(
        '--configuration', $Configuration,
        '--runtime', $Runtime,
        '--self-contained', 'true',
        '--output', $output,
        '-p:PublishSingleFile=true',
        '-p:IncludeNativeLibrariesForSelfExtract=true',
        '-p:EnableCompressionInSingleFile=true',
        '-p:DebugType=none',
        '-p:PublishReferencesDocumentationFiles=false'
    )

    Invoke-Step 'Publish GUI' { dotnet publish (Join-Path $root 'src/HashVerifier.App/HashVerifier.App.csproj') @publishArgs }
    Invoke-Step 'Publish CLI' { dotnet publish (Join-Path $root 'src/HashVerifier.Cli/HashVerifier.Cli.csproj') @publishArgs }

    # Release assets carry the version and runtime in their name, e.g. HashVerifier-2.0.0-win-x64.exe.
    $version = (Select-Xml -Path (Join-Path $root 'Directory.Build.props') -XPath '//Version').Node.InnerText
    $artifacts = Join-Path $root 'artifacts'
    $assets = @(
        @{ Source = 'HashVerifier.exe'; Name = "HashVerifier-$version-$Runtime.exe" },
        @{ Source = 'hashverifier-cli.exe'; Name = "hashverifier-cli-$version-$Runtime.exe" }
    )
    foreach ($asset in $assets) {
        Copy-Item (Join-Path $output $asset.Source) (Join-Path $artifacts $asset.Name) -Force
    }

    # Publish checksums next to the release, in the same format HashVerifier understands.
    $sums = Join-Path $artifacts 'SHA256SUMS'
    Get-ChildItem $artifacts -Filter '*.exe' |
        ForEach-Object { '{0}  {1}' -f (Get-FileHash $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant(), $_.Name } |
        Set-Content $sums -Encoding utf8NoBOM

    Write-Host "Release assets in ${artifacts}:" -ForegroundColor Green
    $assets | ForEach-Object { Write-Host "  $($_.Name)" -ForegroundColor Green }
}
