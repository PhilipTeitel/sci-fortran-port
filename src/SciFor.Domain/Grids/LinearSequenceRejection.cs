namespace SciFor.Domain.Grids;

/// <summary>
/// Caller-visible classification of a rejected linear-sequence request (ADR-010 §2).
/// </summary>
/// <remarks>
/// The two members correspond to the two abort conditions BEH-001 recovered from
/// <c>src/tools_grids.f90</c>. Callers distinguish them without inspecting any message,
/// which is what REQ-001 S5 and S6 require and what keeps the unexecuted Fortran strings
/// out of the contract.
/// </remarks>
public enum LinearSequenceRejection
{
    /// <summary>
    /// Length below zero. Legacy: <c>error("linspace: N&lt;0, abort.")</c> then
    /// <c>STOP</c> at <c>src/tools_grids.f90:7</c>. REQ-001 S5.
    /// </summary>
    NegativeLength,

    /// <summary>
    /// Length of 0 or 1 with both endpoints included, where the inclusive step
    /// <c>(stop - start) / (length - 1)</c> has no meaning. Legacy:
    /// <c>error("linspace: N&lt;2 with both start and end points")</c> at
    /// <c>src/tools_grids.f90:12</c>. REQ-001 S6.
    /// </summary>
    InclusiveLengthBelowTwo
}
