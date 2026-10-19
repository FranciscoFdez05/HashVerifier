namespace HashVerifier.App;

internal static class Program
{
    /// <summary>Application entry point. An optional file path argument is pre-loaded for hashing,
    /// so the app can be used from "Open with" or "Send to".</summary>
    [STAThread]
    private static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm(args.FirstOrDefault()));
    }
}
