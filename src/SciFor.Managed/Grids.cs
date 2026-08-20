using SciFor.Application.Grids;
using SciFor.Domain.Grids;

namespace SciFor;

/// <summary>
/// Managed-API surface for the recovered <c>TOOLS</c> grid functions. This is the driving
/// adapter and the surface parity is claimed about (ADR-002, ADR-009 §4).
/// </summary>
/// <remarks>
/// VS-1 exposes <see cref="Linspace"/> only. <c>logspace</c>, <c>arange</c>, and the rest
/// of the recovered catalog are reserve, not built, and this type must not imply otherwise
/// (ADR-008).
/// <para>
/// The method keeps the Fortran job name so the recovered behavior stays recognizable,
/// while the domain types use ubiquitous language (ADR-009 §2).
/// </para>
/// </remarks>
public sealed class Grids
{
    private readonly IGenerateLinearSequence _generateLinearSequence;

    /// <summary>
    /// Creates the adapter with the default use case. No DI container is used in VS-1:
    /// there are no driven adapters to swap, and a container would pull hosting types into
    /// a class library (ADR-009 §1).
    /// </summary>
    public Grids()
        : this(new GenerateLinearSequence())
    {
    }

    /// <summary>
    /// Creates the adapter over a supplied port. Present for tests; production callers use
    /// the default constructor.
    /// </summary>
    public Grids(IGenerateLinearSequence generateLinearSequence)
    {
        ArgumentNullException.ThrowIfNull(generateLinearSequence);
        _generateLinearSequence = generateLinearSequence;
    }

    /// <summary>
    /// Returns <paramref name="length"/> evenly spaced values from
    /// <paramref name="start"/> through <paramref name="stop"/>, both endpoints included.
    /// </summary>
    /// <remarks>
    /// The legacy optional flags <c>istart</c>, <c>iend</c>, and <c>mesh</c> are not
    /// parameters here. Their branches were never executed, so REQ-001 Q5 defers them; a
    /// later requirement may add them once fixtures exist.
    /// </remarks>
    /// <exception cref="LinearSequenceRejectedException">
    /// <paramref name="length"/> is negative, or is below 2 with inclusive endpoints.
    /// </exception>
    public LinearSequence Linspace(double start, double stop, int length)
    {
        // Builds the request and delegates. The formula deliberately lives behind the
        // port: a later HTTP adapter must reuse it rather than restate it (ADR-002).
        return _generateLinearSequence.Generate(new LinearSequenceRequest(start, stop, length));
    }
}
