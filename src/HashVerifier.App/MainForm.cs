using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using HashVerifier.App.Resources;
using HashVerifier.Core;

namespace HashVerifier.App;

public partial class MainForm : Form
{
    private static readonly Color MatchColor = Color.FromArgb(0, 128, 0);
    private static readonly Color MismatchColor = Color.FromArgb(192, 0, 0);

    /// <summary>Product version from Directory.Build.props, e.g. "2.0.0".</summary>
    private static readonly string AppVersion = typeof(MainForm).Assembly.GetName().Version?.ToString(3) ?? "";

    private CancellationTokenSource? _cts;
    private int _generatedHashExtraLines;

    public MainForm(string? initialFile = null)
    {
        InitializeComponent();
        Icon = Icon.ExtractAssociatedIcon(Environment.ProcessPath!);

        comboBox_algorithm.Items.AddRange([.. HashAlgorithms.Supported]);
        comboBox_algorithm.SelectedItem = HashAlgorithms.Get(HashAlgorithms.Default);

        comboBox_language.Items.AddRange([.. LanguageOption.All]);
        comboBox_language.SelectedItem = LanguageOption.ForCulture(CultureInfo.CurrentUICulture);

        EnableFileDrop(textBox_fileToHash, groupBox_generate);
        EnableFileDrop(textBox_fileToVerify, groupBox_verify);

        if (!string.IsNullOrWhiteSpace(initialFile))
        {
            textBox_fileToHash.Text = initialFile;
        }
    }

    private HashAlgorithmInfo SelectedAlgorithm =>
        comboBox_algorithm.SelectedItem as HashAlgorithmInfo ?? HashAlgorithms.Get(HashAlgorithms.Default);

    // ---------------------------------------------------------------- Localization

    private void ApplyLocalization()
    {
        Text = $"{Strings.AppTitle} v{AppVersion}";
        label_algorithm.Text = Strings.Algorithm;
        label_language.Text = Strings.Language;
        groupBox_generate.Text = Strings.GenerateGroup;
        groupBox_verify.Text = Strings.VerifyGroup;
        label_fileToHash.Text = Strings.FileToHash;
        label_fileToVerify.Text = Strings.FileToVerify;
        label_generatedHash.Text = Strings.GeneratedHash;
        label_expectedHash.Text = Strings.ExpectedHash;
        button_browseHash.Text = button_browseVerify.Text = Strings.Browse;
        button_calculate.Text = Strings.Calculate;
        button_copy.Text = Strings.Copy;
        button_paste.Text = Strings.Paste;
        button_verify.Text = Strings.Verify;
        button_clear.Text = Strings.Clear;
        button_cancel.Text = Strings.Cancel;
        label_result.Text = string.Empty;
        SetStatus(Strings.StatusReady);
    }

    private void ComboBox_language_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (comboBox_language.SelectedItem is LanguageOption language)
        {
            CultureInfo.CurrentUICulture = language.Culture;
            Strings.Culture = language.Culture;
            ApplyLocalization();
        }
    }

    // ---------------------------------------------------------------- Generate

    private void Button_browseHash_Click(object? sender, EventArgs e) => BrowseInto(textBox_fileToHash);

    private async void Button_calculate_Click(object? sender, EventArgs e)
    {
        if (!TryGetExistingFile(textBox_fileToHash.Text, out var path))
        {
            return;
        }

        var algorithm = SelectedAlgorithm;
        textBox_generatedHash.Clear();
        var stopwatch = Stopwatch.StartNew();

        var hash = await RunHashingAsync(path, algorithm);
        if (hash is null)
        {
            return;
        }

        var hex = HashText.ToHex(hash);
        textBox_generatedHash.Text = hex;
        if (TrySetClipboard(hex))
        {
            SetStatus(Format(Strings.StatusHashed, algorithm.DisplayName, stopwatch.Elapsed.TotalSeconds));
        }
    }

    private void Button_copy_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(textBox_generatedHash.Text))
        {
            ShowWarning(Strings.ErrorNothingToCopy);
        }
        else if (TrySetClipboard(textBox_generatedHash.Text))
        {
            SetStatus(Strings.StatusCopied);
        }
    }

    private void TextBox_generatedHash_LayoutChanged(object? sender, EventArgs e) => AdjustGeneratedHashHeight();

    /// <summary>
    /// Grows the generated-hash box downwards, one line at a time, when the hash wraps
    /// (long digests or a narrow window), pushing the controls below it. It never shrinks
    /// below its single-line design height.
    /// </summary>
    private void AdjustGeneratedHashHeight()
    {
        var box = textBox_generatedHash;
        if (!box.IsHandleCreated)
        {
            return;
        }

        var lines = box.TextLength == 0 ? 1 : box.GetLineFromCharIndex(box.TextLength - 1) + 1;
        var extraLines = lines - 1;
        if (extraLines == _generatedHashExtraLines)
        {
            return;
        }

        var delta = (extraLines - _generatedHashExtraLines) * box.Font.Height;
        _generatedHashExtraLines = extraLines;

        SuspendLayout();
        box.Height += delta;
        button_calculate.Top += delta;
        groupBox_generate.Height += delta;
        groupBox_verify.Top += delta;

        // Raise the minimum before growing and lower it after shrinking, so the height is never clamped.
        // Raising the minimum may already enlarge the window, so the target height is computed up front.
        var targetHeight = Height + delta;
        if (delta > 0)
        {
            MinimumSize = MinimumSize with { Height = MinimumSize.Height + delta };
        }

        if (WindowState == FormWindowState.Normal)
        {
            Height = targetHeight;
        }

        if (delta < 0)
        {
            MinimumSize = MinimumSize with { Height = MinimumSize.Height + delta };
        }

        ResumeLayout();
    }

    // ---------------------------------------------------------------- Verify

    private void Button_browseVerify_Click(object? sender, EventArgs e) => BrowseInto(textBox_fileToVerify);

    private void Button_paste_Click(object? sender, EventArgs e)
    {
        if (Clipboard.ContainsText())
        {
            textBox_expectedHash.Text = Clipboard.GetText().Trim();
        }
        else
        {
            ShowWarning(Strings.ErrorClipboardEmpty);
        }
    }

    private void TextBox_expectedHash_TextChanged(object? sender, EventArgs e) => label_result.Text = string.Empty;

    private async void Button_verify_Click(object? sender, EventArgs e)
    {
        if (!TryGetExistingFile(textBox_fileToVerify.Text, out var path))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(textBox_expectedHash.Text))
        {
            ShowWarning(Strings.ErrorEnterHash);
            return;
        }

        if (!ChecksumEntry.TryParse(textBox_expectedHash.Text, out var expected))
        {
            ShowWarning(Strings.ErrorInvalidHash);
            return;
        }

        var algorithm = ResolveAlgorithm(expected);
        if (algorithm is null)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var actual = await RunHashingAsync(path, algorithm);
        if (actual is null)
        {
            return;
        }

        var isMatch = HashText.AreEqual(actual, expected.Hash);
        label_result.Text = isMatch ? Strings.ResultMatch : Strings.ResultMismatch;
        label_result.ForeColor = isMatch ? MatchColor : MismatchColor;
        SetStatus(Format(Strings.StatusVerified, algorithm.DisplayName, stopwatch.Elapsed.TotalSeconds));
    }

    /// <summary>
    /// Returns the selected algorithm if it fits the expected hash. Otherwise switches to the algorithm
    /// named in the input or implied by the hash length, or warns when none fits.
    /// </summary>
    private HashAlgorithmInfo? ResolveAlgorithm(ChecksumEntry expected)
    {
        var selected = SelectedAlgorithm;
        var hexLength = expected.Hex.Length;
        if (selected.HexLength == hexLength && (expected.Algorithm is null || expected.Algorithm == selected))
        {
            return selected;
        }

        var detected = expected.Algorithm is { IsSupported: true } named && named.HexLength == hexLength
            ? named
            : HashAlgorithms.WithHexLength(hexLength).FirstOrDefault();

        if (detected is null)
        {
            ShowWarning(Format(Strings.ErrorLengthMismatch, hexLength, selected.DisplayName, selected.HexLength));
            return null;
        }

        comboBox_algorithm.SelectedItem = detected;
        SetStatus(Format(Strings.StatusAlgorithmDetected, detected.DisplayName));
        return detected;
    }

    // ---------------------------------------------------------------- Shared

    private void ComboBox_algorithm_SelectedIndexChanged(object? sender, EventArgs e)
    {
        label_result.Text = string.Empty;
        var algorithm = SelectedAlgorithm;
        SetStatus(algorithm.IsCryptographicallyBroken
            ? Format(Strings.LegacyAlgorithmHint, algorithm.DisplayName)
            : Strings.StatusReady);
    }

    private void Button_cancel_Click(object? sender, EventArgs e) => _cts?.Cancel();

    private void Button_clear_Click(object? sender, EventArgs e)
    {
        textBox_fileToHash.Clear();
        textBox_generatedHash.Clear();
        textBox_fileToVerify.Clear();
        textBox_expectedHash.Clear();
        label_result.Text = string.Empty;
        progressBar.Value = 0;
        SetStatus(Strings.StatusReady);
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e) => _cts?.Cancel();

    /// <summary>Hashes a file in the background while keeping the UI responsive.</summary>
    /// <returns>The digest, or <see langword="null"/> if the operation was cancelled or failed.</returns>
    private async Task<byte[]?> RunHashingAsync(string path, HashAlgorithmInfo algorithm)
    {
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var progress = new Progress<double>(p => progressBar.Value = (int)(p * progressBar.Maximum));

        SetBusy(true);
        SetStatus(Format(Strings.StatusHashing, algorithm.DisplayName, Path.GetFileName(path)));
        try
        {
            return await FileHasher.ComputeAsync(path, algorithm.Kind, progress, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            progressBar.Value = 0;
            SetStatus(Strings.StatusCancelled);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
        {
            progressBar.Value = 0;
            SetStatus(Strings.StatusReady);
            ShowError(Format(Strings.ErrorGeneric, ex.Message));
        }
        finally
        {
            SetBusy(false);
        }

        return null;
    }

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        groupBox_generate.Enabled = !busy;
        groupBox_verify.Enabled = !busy;
        comboBox_algorithm.Enabled = !busy;
        comboBox_language.Enabled = !busy;
        button_clear.Enabled = !busy;
        button_cancel.Enabled = busy;
    }

    private void SetStatus(string message) => statusLabel.Text = message;

    private bool TryGetExistingFile(string input, out string path)
    {
        // Paths copied from Explorer ("Copy as path") come wrapped in quotes.
        path = input.Trim().Trim('"');
        if (path.Length == 0)
        {
            ShowWarning(Strings.ErrorSelectFile);
            return false;
        }

        if (!File.Exists(path))
        {
            ShowWarning(Format(Strings.ErrorFileNotFound, path));
            return false;
        }

        return true;
    }

    private void BrowseInto(TextBox target)
    {
        using var dialog = new OpenFileDialog
        {
            Title = Strings.SelectFileTitle,
            Filter = Strings.AllFilesFilter,
            Multiselect = false,
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            target.Text = dialog.FileName;
        }
    }

    /// <summary>Lets the user drop a file on <paramref name="target"/> or on any of <paramref name="surfaces"/>.</summary>
    private static void EnableFileDrop(TextBox target, params Control[] surfaces)
    {
        foreach (var control in surfaces.Prepend(target))
        {
            control.AllowDrop = true;
            control.DragEnter += (_, e) =>
                e.Effect = e.Data?.GetDataPresent(DataFormats.FileDrop) == true ? DragDropEffects.Copy : DragDropEffects.None;
            control.DragDrop += (_, e) =>
            {
                if (e.Data?.GetData(DataFormats.FileDrop) is string[] { Length: > 0 } files)
                {
                    target.Text = files[0];
                }
            };
        }
    }

    private bool TrySetClipboard(string text)
    {
        try
        {
            Clipboard.SetText(text);
            return true;
        }
        catch (ExternalException ex)
        {
            // Another process may be holding the clipboard open.
            ShowError(Format(Strings.ErrorGeneric, ex.Message));
            return false;
        }
    }

    private void ShowWarning(string message) =>
        MessageBox.Show(this, message, Strings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private void ShowError(string message) =>
        MessageBox.Show(this, message, Strings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

    private static string Format(string format, params object[] args) =>
        string.Format(CultureInfo.CurrentCulture, format, args);

    /// <summary>An entry of the language selector.</summary>
    private sealed record LanguageOption(CultureInfo Culture)
    {
        public static IReadOnlyList<LanguageOption> All { get; } = [new(new CultureInfo("en")), new(new CultureInfo("es"))];

        public static LanguageOption ForCulture(CultureInfo culture) =>
            All.FirstOrDefault(l => l.Culture.TwoLetterISOLanguageName == culture.TwoLetterISOLanguageName) ?? All[0];

        public override string ToString() => Culture.TextInfo.ToTitleCase(Culture.NativeName);
    }
}
