using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using SciFor.Application.Grids;
using SciFor.Domain.Grids;

namespace SciFor.Tests.Integration;

/// <summary>
/// Phase Y for story VS1-1: the binding constraints that make this a hexagonal port
/// rather than a class named after one.
/// </summary>
public sealed class ArchitectureBoundaryTests
{
    private static readonly string[] HostConcernMarkers =
    [
        "Microsoft.AspNetCore", "Microsoft.Extensions.Hosting", "Microsoft.Extensions.Logging",
        "Microsoft.Extensions.DependencyInjection", "System.CommandLine", "CommandLineParser",
        "Newtonsoft.Json", "Serilog", "NLog"
    ];

    /// <summary>
    /// Y1 - Domain and Application carry no I/O, CLI, hosting, or timing dependency, and
    /// the reference direction runs one way (B1, B2; ADR-009 §1).
    /// </summary>
    [Fact]
    public void domain_has_no_host_dependencies_Y1()
    {
        var domain = XDocument.Load(RepositoryLayout.ProjectFile("SciFor.Domain"));
        Assert.Empty(domain.Descendants("PackageReference"));
        Assert.Empty(domain.Descendants("ProjectReference"));

        var application = XDocument.Load(RepositoryLayout.ProjectFile("SciFor.Application"));
        Assert.Empty(application.Descendants("PackageReference"));
        Assert.Equal(
            ["..\\SciFor.Domain\\SciFor.Domain.csproj"],
            application.Descendants("ProjectReference").Select(r => r.Attribute("Include")!.Value));

        var managed = XDocument.Load(RepositoryLayout.ProjectFile("SciFor.Managed"));
        Assert.Empty(managed.Descendants("PackageReference"));
        Assert.Equal(
            ["..\\SciFor.Application\\SciFor.Application.csproj"],
            managed.Descendants("ProjectReference").Select(r => r.Attribute("Include")!.Value));

        // Compiled references corroborate the project files: nothing hosting-shaped is
        // reachable from the core assemblies.
        foreach (var assembly in new[] { typeof(LinearSequenceRequest).Assembly, typeof(IGenerateLinearSequence).Assembly })
        {
            var referenced = assembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty).ToArray();
            foreach (var marker in HostConcernMarkers)
            {
                Assert.DoesNotContain(referenced, name => name.StartsWith(marker, StringComparison.Ordinal));
            }
        }

        // Domain must not reach Application, which is the direction a layering mistake
        // would break first.
        Assert.DoesNotContain(
            typeof(LinearSequenceRequest).Assembly.GetReferencedAssemblies().Select(a => a.Name),
            name => name == "SciFor.Application");
    }

    /// <summary>
    /// Y3 - the core neither terminates the process nor writes diagnostics (B7;
    /// ADR-010 §3, ADR-007). Legacy fatal paths printed ANSI-styled text to stdout and
    /// called STOP; none of that may reappear here.
    /// </summary>
    [Fact]
    public void core_does_not_terminate_or_print_Y3()
    {
        string[] forbidden =
        [
            "Environment.Exit", "Environment.FailFast", "Console.", "\\u001b[", "\\x1b["
        ];

        foreach (var file in RepositoryLayout.SourceFiles("SciFor.Domain", "SciFor.Application", "SciFor.Managed"))
        {
            var source = File.ReadAllText(file);
            foreach (var marker in forbidden)
            {
                Assert.False(
                    source.Contains(marker, StringComparison.Ordinal),
                    $"{Path.GetFileName(file)} contains '{marker}', which B7 forbids in the core.");
            }
        }

        // A rejection returns control to the caller rather than ending the process. If
        // this test runs to completion, the process survived an invalid request.
        Assert.Throws<LinearSequenceRejectedException>(() => new Grids().Linspace(0.0, 1.0, -1));
    }

    /// <summary>
    /// Y4 - no test asserts legacy Fortran diagnostic text, and none reads the
    /// Python-generated golden file (B5, B8; DEF-001, ADR-007).
    /// </summary>
    [Fact]
    public void no_legacy_text_or_golden_oracle_Y4()
    {
        string[] forbidden =
        [
            "linspace: N<0", "N<2 with both start and end points", "fidelity/golden", "linspace-5.txt"
        ];

        // Scans tests rather than all sources because the rejection enum's doc comments
        // quote the legacy strings as provenance, which is legitimate; the rule is that no
        // *assertion* may depend on that text. This file is skipped because the markers
        // above are the rule's definition, not a use of it.
        var guardFile = Path.GetFullPath(GuardSourcePath());

        foreach (var file in RepositoryLayout.TestSourceFiles())
        {
            if (Path.GetFullPath(file) == guardFile)
            {
                continue;
            }

            var source = File.ReadAllText(file);
            foreach (var marker in forbidden)
            {
                Assert.False(
                    source.Contains(marker, StringComparison.Ordinal),
                    $"{Path.GetFileName(file)} references '{marker}'; parity must rest on FIX-001 parsed values.");
            }
        }

        // The parity fixture must name its provenance, so a future reader cannot mistake
        // it for a regenerated file.
        var fixture = File.ReadAllText(Path.Combine(
            RepositoryLayout.Root.FullName,
            "tests/SciFor.Tests/Parity/Fixtures/FIX-001-linspace-5.expected.txt"));
        Assert.Contains("CAP-20260810-LINSPACE", fixture, StringComparison.Ordinal);
        Assert.Contains("e586903a26cc50ca8942f20ca3bccbd8814e6252", fixture, StringComparison.Ordinal);
    }

    private static string GuardSourcePath([CallerFilePath] string path = "") => path;

    /// <summary>
    /// Y5 - the managed surface exposes no endpoint flags and nothing beyond linspace
    /// (B10, B11; REQ-001 Q5, ADR-008).
    /// </summary>
    [Fact]
    public void public_surface_is_vs1_only_Y5()
    {
        var evaluationMethods = typeof(Grids)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .ToArray();

        var linspace = Assert.Single(evaluationMethods);
        Assert.Equal("Linspace", linspace.Name);

        // Exactly start, stop, length. An added optional parameter would be how istart /
        // iend / mesh leak in as accepted parity.
        Assert.Equal(
            [typeof(double), typeof(double), typeof(int)],
            linspace.GetParameters().Select(p => p.ParameterType));
        Assert.DoesNotContain(linspace.GetParameters(), p => p.IsOptional);

        // No unbuilt behavior may appear as a public type in any shipped assembly.
        string[] unbuilt = ["logspace", "arange", "fermi", "matrix", "fftgf", "deriv"];
        var publicTypeNames = new[]
            {
                typeof(Grids).Assembly, typeof(IGenerateLinearSequence).Assembly, typeof(LinearSequenceRequest).Assembly
            }
            .SelectMany(a => a.GetExportedTypes())
            .Select(t => t.Name)
            .ToArray();

        foreach (var name in unbuilt)
        {
            Assert.DoesNotContain(publicTypeNames, t => t.Contains(name, StringComparison.OrdinalIgnoreCase));
        }
    }

    /// <summary>
    /// Z3 - the C# reading of the configured type policy: nullable on and warnings as
    /// errors in every project file, and no <c>dynamic</c> in the sources.
    /// </summary>
    [Fact]
    public void projects_enable_nullable_and_treat_warnings_as_errors_Z3()
    {
        foreach (var project in new[] { "SciFor.Domain", "SciFor.Application", "SciFor.Managed" })
        {
            var document = XDocument.Load(RepositoryLayout.ProjectFile(project));
            Assert.Equal("enable", document.Descendants("Nullable").Single().Value);
            Assert.Equal("true", document.Descendants("TreatWarningsAsErrors").Single().Value);
        }

        var dynamicDeclaration = new Regex(@"\bdynamic\s+\w+", RegexOptions.Compiled);
        foreach (var file in RepositoryLayout.SourceFiles("SciFor.Domain", "SciFor.Application", "SciFor.Managed"))
        {
            Assert.DoesNotMatch(dynamicDeclaration, File.ReadAllText(file));
        }
    }
}
