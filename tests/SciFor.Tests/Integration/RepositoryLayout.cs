namespace SciFor.Tests.Integration;

/// <summary>
/// Locates repository files so the Phase Y boundary tests can inspect project files and
/// sources directly. Several binding constraints in story VS1-1 are about what the code
/// does <em>not</em> contain, which reflection over compiled output cannot fully show.
/// </summary>
internal static class RepositoryLayout
{
    private static readonly Lazy<DirectoryInfo> RootValue = new(FindRoot);

    internal static DirectoryInfo Root => RootValue.Value;

    internal static string ProjectFile(string projectName) =>
        Path.Combine(Root.FullName, "src", projectName, projectName + ".csproj");

    internal static IEnumerable<string> SourceFiles(params string[] projectNames) =>
        projectNames.SelectMany(name =>
            Directory.EnumerateFiles(Path.Combine(Root.FullName, "src", name), "*.cs", SearchOption.AllDirectories));

    internal static IEnumerable<string> TestSourceFiles() =>
        Directory.EnumerateFiles(Path.Combine(Root.FullName, "tests"), "*.cs", SearchOption.AllDirectories);

    private static DirectoryInfo FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "SciFor.sln")))
            {
                return directory;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not locate SciFor.sln above '{AppContext.BaseDirectory}'.");
    }
}
