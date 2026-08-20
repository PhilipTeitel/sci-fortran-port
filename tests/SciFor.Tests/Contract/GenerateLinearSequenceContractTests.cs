using SciFor.Application.Grids;
using SciFor.Domain.Grids;

namespace SciFor.Tests.Contract;

/// <summary>
/// Phase A coverage of the inbound port, exercised through the real use case.
/// </summary>
/// <remarks>
/// None of these are parity claims. S2 is E3 code-derived from the recovered formula, and
/// S3/S4 are E4 inferred from it because the probe never executed those inputs. Only
/// FIX-001 carries a Phase P criterion.
/// </remarks>
public sealed class GenerateLinearSequenceContractTests
{
    private static LinearSequence Generate(double start, double stop, int length) =>
        new GenerateLinearSequence().Generate(new LinearSequenceRequest(start, stop, length));

    /// <summary>A2 - inclusive evaluation follows the accepted formula for length >= 2.</summary>
    [Theory]
    [InlineData(0.0, 1.0, 5)]      // FIX-001 inputs
    [InlineData(0.0, 1.0, 2)]      // minimum inclusive length
    [InlineData(-5.0, 5.0, 1024)]  // the legacy CLI's documented defaults
    [InlineData(0.0, 1.0, 3)]      // non-dyadic interior spacing
    [InlineData(2.5, 7.5, 11)]     // offset start, non-unit width
    [InlineData(0.0, 1.0, 50)]     // endpoint diverges from stop by 1 ULP (DEF-005)
    public void Generate_InclusiveFormula_A2(double start, double stop, int length)
    {
        var sequence = Generate(start, stop, length);

        Assert.Equal(length, sequence.Samples.Count);

        var step = (stop - start) / (length - 1);
        for (var i = 0; i < length; i++)
        {
            Assert.Equal(start + i * step, sequence.Samples[i]);
        }

        // The first sample is start exactly, since i = 0 zeroes the step term.
        Assert.Equal(start, sequence.Samples[0]);
    }

    /// <summary>
    /// A2a - characterizes the endpoint. The recovered formula reaches stop for most
    /// lengths but not all, and the legacy code has no fixup, so this asserts the formula
    /// rather than REQ-001 S2's stronger "the last sample is T" claim. See DEF-005.
    /// </summary>
    [Theory]
    [InlineData(0.0, 1.0, 5, true)]    // dyadic: exact
    [InlineData(0.0, 1.0, 3, true)]    // exact
    [InlineData(0.0, 1.0, 50, false)]  // 0.9999999999999999, one ULP below 1
    public void Generate_EndpointFollowsFormulaNotFixup_A2a(double start, double stop, int length, bool reachesStopExactly)
    {
        var sequence = Generate(start, stop, length);

        var step = (stop - start) / (length - 1);
        Assert.Equal(start + (length - 1) * step, sequence.Samples[^1]);
        Assert.Equal(reachesStopExactly, sequence.Samples[^1] == stop);
    }

    /// <summary>A3 - a decreasing interval uses a negative step and includes both ends.</summary>
    [Theory]
    [InlineData(1.0, 0.0, 5)]
    [InlineData(5.0, -5.0, 3)]
    public void Generate_DecreasingInterval_A3(double start, double stop, int length)
    {
        var sequence = Generate(start, stop, length);

        var step = (stop - start) / (length - 1);
        Assert.True(step < 0.0, "A decreasing interval must produce a negative step.");

        Assert.Equal(start, sequence.Samples[0]);
        Assert.Equal(stop, sequence.Samples[^1]);

        for (var i = 0; i < length; i++)
        {
            Assert.Equal(start + i * step, sequence.Samples[i]);
        }
    }

    /// <summary>A4 - equal endpoints produce a constant sequence of the requested length.</summary>
    [Theory]
    [InlineData(0.0, 2)]
    [InlineData(3.25, 7)]
    [InlineData(-1.5, 4)]
    public void Generate_EqualEndpoints_A4(double value, int length)
    {
        var sequence = Generate(value, value, length);

        Assert.Equal(length, sequence.Samples.Count);
        Assert.All(sequence.Samples, sample => Assert.Equal(value, sample));
    }

    /// <summary>A5 - negative length is rejected as NegativeLength.</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(int.MinValue)]
    public void Generate_NegativeLength_Rejected_A5(int length)
    {
        var exception = Assert.Throws<LinearSequenceRejectedException>(() => Generate(0.0, 1.0, length));

        Assert.Equal(LinearSequenceRejection.NegativeLength, exception.Reason);
        Assert.Equal("linear-sequence.negative-length", exception.Code);
        Assert.Equal(length, exception.Request.Length);
    }

    /// <summary>A6 - inclusive length below 2 is rejected as InclusiveLengthBelowTwo.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Generate_InclusiveLengthBelowTwo_Rejected_A6(int length)
    {
        var exception = Assert.Throws<LinearSequenceRejectedException>(() => Generate(0.0, 1.0, length));

        Assert.Equal(LinearSequenceRejection.InclusiveLengthBelowTwo, exception.Reason);
        Assert.Equal("linear-sequence.inclusive-length-below-two", exception.Code);
    }

    /// <summary>
    /// A5/A6 - the two rejection classes are distinguishable without reading Message,
    /// which is what lets adapters map failures without depending on Fortran text.
    /// </summary>
    [Fact]
    public void Generate_RejectionReasonsAreDistinguishable_A5_A6()
    {
        var negative = Assert.Throws<LinearSequenceRejectedException>(() => Generate(0.0, 1.0, -1));
        var belowTwo = Assert.Throws<LinearSequenceRejectedException>(() => Generate(0.0, 1.0, 1));

        Assert.NotEqual(negative.Reason, belowTwo.Reason);
        Assert.NotEqual(negative.Code, belowTwo.Code);
    }
}
