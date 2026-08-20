namespace SciFor.Domain.Grids;

/// <summary>
/// A successfully evaluated inclusive linear sequence.
/// </summary>
/// <remarks>
/// Existence of an instance means the length rules passed: the type is only constructed
/// after validation (ADR-010 §2, DEF-002). There is no empty or sentinel
/// <see cref="LinearSequence"/> representing a rejected request.
/// <para>
/// <see cref="Samples"/> is zero-based. Legacy <c>array(num)</c> is 1-based, but the
/// Fortran ABI is not retained (ADR-005), and FIX-001 records the managed sequence
/// zero-based (ADR-009 §6). Managed index <c>i</c> corresponds to legacy index
/// <c>i + 1</c>.
/// </para>
/// </remarks>
public sealed class LinearSequence
{
    public LinearSequence(IReadOnlyList<double> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);
        Samples = samples;
    }

    /// <summary>
    /// The evaluated samples in start-toward-stop order, as IEEE-754 binary64 values
    /// (ADR-003). Count equals the requested length.
    /// </summary>
    public IReadOnlyList<double> Samples { get; }
}
