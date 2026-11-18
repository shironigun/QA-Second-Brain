namespace QaSecondBrain.TestKit.Fakes;

/// <summary>Locates <c>tests/fixtures</c> by walking up from the test binaries to the repo root (the folder with global.json).</summary>
public static class FixturePaths
{
    private static readonly Lazy<string> Root = new(FindRoot);

    public static string Fixtures => Path.Combine(Root.Value, "tests", "fixtures");
    public static string Ado => Path.Combine(Fixtures, "ado");
    public static string Llm => Path.Combine(Fixtures, "llm");

    private static string FindRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "global.json")))
            {
                return dir.FullName;
            }
        }

        throw new DirectoryNotFoundException("Could not find the repo root (global.json) above " + AppContext.BaseDirectory);
    }
}
