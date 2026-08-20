namespace SciFor.Tests.Parity;

/// <summary>
/// Phase P for story VS1-1. BEH-001 is the only covered behavior and FIX-001 is the only
/// accepted fixture, so this is the only parity criterion in the slice. S2-S6 are covered
/// in Phase A instead: giving them a parity criterion would let Phase P evidence exceed
/// the oracle tier, which no captured data supports.
/// </summary>
public sealed class Fix001LinspaceParityTests
{
    /// <summary>
    /// P1 - legacy linspace(0, 1, 5) parsed output matches the managed implementation
    /// under exact parsed numeric equality.
    ///
    /// Oracle: FIX-001 / CAP-20260810-LINSPACE at revision e586903, scoped T1 (ADR-001).
    /// Tolerance: none. Exact binary64 equality (ADR-003, REQ-001 S1).
    /// Defect decision: DEF-001 reproduce-faithfully - probe parsed values, never the
    /// Python-generated golden file.
    ///
    /// Asserted through the product surface Grids.Linspace rather than the use case,
    /// because the managed API is what parity is claimed about (ADR-002, ADR-009 §5).
    /// </summary>
    [Fact]
    public void parity_BEH_001_P1()
    {
        var expected = Fix001Fixture.ExpectedSamples();

        var actual = new Grids().Linspace(Fix001Fixture.Start, Fix001Fixture.Stop, Fix001Fixture.Length);

        Assert.Equal(expected.Count, actual.Samples.Count);

        // Assert.Equal on doubles without a precision argument is exact bitwise-value
        // equality, which is the ADR-003 rule. Do not add a precision or tolerance here.
        for (var i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], actual.Samples[i]);
        }
    }
}
