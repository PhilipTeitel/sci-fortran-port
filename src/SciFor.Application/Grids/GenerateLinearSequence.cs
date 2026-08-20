using SciFor.Domain.Grids;

namespace SciFor.Application.Grids;

/// <summary>
/// Evaluates an inclusive linear sequence using the formula recovered from
/// <c>src/tools_grids.f90:11-14</c> (BEH-001, ADR-003).
/// </summary>
public sealed class GenerateLinearSequence : IGenerateLinearSequence
{
    private const int MinimumInclusiveLength = 2;

    /// <inheritdoc />
    public LinearSequence Generate(LinearSequenceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validation strictly precedes allocation. Legacy Fortran declares the result as
        // array(num) in the function header before testing num < 0, so a negative length
        // reached a processor-dependent declaration first. DEF-002 is dispositioned
        // fix-now: that ordering is a Fortran ABI artifact, not observable managed
        // behavior, and the ABI is not retained (ADR-005 §2).
        if (request.Length < 0)
        {
            throw new LinearSequenceRejectedException(LinearSequenceRejection.NegativeLength, request);
        }

        if (request.Length < MinimumInclusiveLength)
        {
            throw new LinearSequenceRejectedException(LinearSequenceRejection.InclusiveLengthBelowTwo, request);
        }

        return new LinearSequence(EvaluateInclusive(request));
    }

    /// <summary>
    /// samples[i] = start + i * (stop - start) / (length - 1), for i in 0..length-1.
    /// </summary>
    /// <remarks>
    /// Managed index <c>i</c> corresponds to legacy 1-based index <c>i + 1</c>, so this is
    /// the recovered <c>array(i) = start + real(i-1,8) * step</c> reindexed (ADR-009 §3).
    /// <para>
    /// The step is computed once and multiplied, matching the legacy operation order.
    /// Preserve-then-refactor applies here: do not substitute a running accumulator or a
    /// fused alternative, because either changes the rounding of interior samples and
    /// this slice's job is to reproduce the recovered arithmetic (ADR-003).
    /// </para>
    /// <para>
    /// A decreasing interval yields a negative step (S3) and equal endpoints yield a zero
    /// step and a constant sequence (S4), with no special-casing needed. Those inputs were
    /// never executed by the probe, so they are E4 inferred and carry no parity claim.
    /// </para>
    /// <para>
    /// The final sample is <em>not</em> assigned <c>stop</c> directly. REQ-001 S2 also
    /// claims "the last sample is T", but that does not follow from the formula in
    /// binary64 for every length: over [0, 1] it fails for 2504 of the lengths in
    /// 2..20000, the smallest being length 50, where the last sample is
    /// 0.9999999999999999 rather than 1. The recovered code has no endpoint fixup
    /// (BEH-001 §5 states only the formula), so snapping the endpoint would be a silent
    /// numerical improvement over the legacy behavior this slice exists to reproduce.
    /// Recorded as DEF-005; see story VS1-1 §12. Needs an owner decision.
    /// </para>
    /// </remarks>
    private static double[] EvaluateInclusive(LinearSequenceRequest request)
    {
        var step = (request.Stop - request.Start) / (request.Length - 1);

        var samples = new double[request.Length];
        for (var i = 0; i < samples.Length; i++)
        {
            samples[i] = request.Start + i * step;
        }

        return samples;
    }
}
