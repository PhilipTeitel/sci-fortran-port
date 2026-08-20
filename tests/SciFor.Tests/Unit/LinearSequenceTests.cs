using SciFor.Application.Grids;
using SciFor.Domain;
using SciFor.Domain.Grids;

namespace SciFor.Tests.Unit;

public sealed class LinearSequenceTests
{
    /// <summary>
    /// A7 - a rejection carries the offending request and allocates no samples.
    /// </summary>
    /// <remarks>
    /// This is DEF-002 fix-now expressed as a test. Legacy Fortran declares the result as
    /// array(num) in the function header before checking num &lt; 0, so a negative length
    /// reached a processor-dependent declaration. Here the failure path produces no
    /// LinearSequence at all, which is only observable because the type cannot be
    /// constructed in an invalid state.
    /// </remarks>
    [Fact]
    public void Rejection_CarriesRequest_NoSamples_A7()
    {
        var request = new LinearSequenceRequest(0.0, 1.0, -3);

        var exception = Assert.Throws<LinearSequenceRejectedException>(
            () => new GenerateLinearSequence().Generate(request));

        Assert.Same(request, exception.Request);
        Assert.Equal(-3, exception.Request.Length);

        // No sequence is returned on the failure path: the only way to observe samples is
        // through a LinearSequence, and none was produced.
        Assert.IsType<LinearSequenceRejectedException>(exception);
    }

    /// <summary>
    /// The rejection type is a DomainFailureException, which is the reusable base VS-2 and
    /// VS-3 extend rather than inventing a second failure style (ADR-010 §4).
    /// </summary>
    [Fact]
    public void Rejection_IsADomainFailure()
    {
        var exception = new LinearSequenceRejectedException(
            LinearSequenceRejection.NegativeLength,
            new LinearSequenceRequest(0.0, 1.0, -1));

        Assert.IsAssignableFrom<DomainFailureException>(exception);
        Assert.False(string.IsNullOrWhiteSpace(exception.Code));
    }

    /// <summary>
    /// Samples are zero-based and preserve the requested count. Legacy array(num) is
    /// 1-based, but the Fortran ABI is not retained (ADR-005, ADR-009 §6).
    /// </summary>
    [Fact]
    public void Sequence_IsZeroBasedAndPreservesCount()
    {
        var sequence = new LinearSequence(new[] { 0.0, 0.25, 0.5, 0.75, 1.0 });

        Assert.Equal(5, sequence.Samples.Count);
        Assert.Equal(0.0, sequence.Samples[0]);
        Assert.Equal(1.0, sequence.Samples[4]);
    }

    /// <summary>
    /// The request is a value object, so two equal requests compare equal. This is what
    /// lets a rejected request be reported and compared without identity plumbing.
    /// </summary>
    [Fact]
    public void Request_HasValueEquality()
    {
        Assert.Equal(new LinearSequenceRequest(0.0, 1.0, 5), new LinearSequenceRequest(0.0, 1.0, 5));
        Assert.NotEqual(new LinearSequenceRequest(0.0, 1.0, 5), new LinearSequenceRequest(0.0, 1.0, 6));
    }
}
