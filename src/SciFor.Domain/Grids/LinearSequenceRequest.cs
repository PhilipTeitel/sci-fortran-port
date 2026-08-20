namespace SciFor.Domain.Grids;

/// <summary>
/// A request for an inclusive linear sequence: <paramref name="Start"/> through
/// <paramref name="Stop"/> in <paramref name="Length"/> evenly spaced samples.
/// </summary>
/// <remarks>
/// Mirrors the recovered <c>TOOLS.linspace</c> arguments that BEH-001 documents, narrowed
/// to the default inclusive path. The optional legacy parameters <c>istart</c>,
/// <c>iend</c>, and <c>mesh</c> are deliberately absent: their branches were never
/// executed by the probe, so REQ-001 Q5 defers them rather than letting an unaccepted
/// spacing rule enter the managed contract.
/// <para>
/// <c>Start</c> and <c>Stop</c> are unconstrained here because the recovered code applies
/// no range check to them (DOMAIN §4a). Only <c>Length</c> has rules, and those are
/// enforced at the use case rather than in this constructor, so that a rejected request
/// is still reportable as the value that was rejected.
/// </para>
/// </remarks>
public sealed record LinearSequenceRequest(double Start, double Stop, int Length);
