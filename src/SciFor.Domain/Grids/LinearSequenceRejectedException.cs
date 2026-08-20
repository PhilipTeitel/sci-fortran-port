namespace SciFor.Domain.Grids;

/// <summary>
/// Thrown when a linear-sequence request violates the recovered length rules
/// (REQ-001 S5, S6; ADR-010).
/// </summary>
/// <remarks>
/// This replaces the legacy <c>STOP</c>. A caller that ignores it sees the process
/// continue, which differs from legacy behavior and is intended (ADR-002; ADR-010
/// Consequences).
/// </remarks>
public sealed class LinearSequenceRejectedException : DomainFailureException
{
    private const string NegativeLengthCode = "linear-sequence.negative-length";
    private const string InclusiveLengthBelowTwoCode = "linear-sequence.inclusive-length-below-two";

    public LinearSequenceRejectedException(LinearSequenceRejection reason, LinearSequenceRequest request)
        : base(CodeFor(reason), MessageFor(reason, request))
    {
        ArgumentNullException.ThrowIfNull(request);
        Reason = reason;
        Request = request;
    }

    /// <summary>Which length rule failed. The caller-visible classification.</summary>
    public LinearSequenceRejection Reason { get; }

    /// <summary>The request that was rejected. No sequence was allocated for it.</summary>
    public LinearSequenceRequest Request { get; }

    private static string CodeFor(LinearSequenceRejection reason) => reason switch
    {
        LinearSequenceRejection.NegativeLength => NegativeLengthCode,
        LinearSequenceRejection.InclusiveLengthBelowTwo => InclusiveLengthBelowTwoCode,
        _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unknown rejection reason.")
    };

    // Developer-facing English, deliberately not the legacy text. ADR-007 makes message
    // wording an adapter concern with no fidelity requirement, and the Fortran strings
    // were never executed, so reproducing them would manufacture false parity.
    private static string MessageFor(LinearSequenceRejection reason, LinearSequenceRequest request) => reason switch
    {
        LinearSequenceRejection.NegativeLength =>
            $"Linear sequence length must not be negative, but was {request.Length}.",
        LinearSequenceRejection.InclusiveLengthBelowTwo =>
            $"Linear sequence length must be at least 2 when both endpoints are included, but was {request.Length}.",
        _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unknown rejection reason.")
    };
}
