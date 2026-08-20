using System.Globalization;

namespace SciFor.Tests.Parity;

/// <summary>
/// Reads the transcribed FIX-001 probe values. Parsing happens here so the parity test
/// asserts on binary64 values rather than on formatted text, which ADR-003 puts out of
/// scope.
/// </summary>
internal static class Fix001Fixture
{
    private const string RelativePath = "Parity/Fixtures/FIX-001-linspace-5.expected.txt";

    internal const double Start = 0.0;
    internal const double Stop = 1.0;
    internal const int Length = 5;

    internal static IReadOnlyList<double> ExpectedSamples()
    {
        var path = Path.Combine(AppContext.BaseDirectory, RelativePath);

        // A missing fixture must fail loudly. Silently falling back to inlined literals
        // would detach the assertion from its provenance header and re-open DEF-001.
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"FIX-001 fixture not found at '{path}'.", path);
        }

        return File.ReadLines(path)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0 && !line.StartsWith('#'))
            .Select(line => double.Parse(line, NumberStyles.Float, CultureInfo.InvariantCulture))
            .ToArray();
    }
}
