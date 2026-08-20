using SciFor.Domain.Grids;

namespace SciFor.Application.Grids;

/// <summary>
/// Inbound (driving) port for inclusive linear-sequence evaluation (ADR-009 §3).
/// </summary>
/// <remarks>
/// The managed library is the first driving adapter over this port. An HTTP adapter could
/// be a second one later without the arithmetic moving (ADR-002); that is the property
/// this interface exists to preserve.
/// <para>
/// There is no driven (outbound) port on this slice. BEH-001 §4 records no file, network,
/// or RNG interaction in the recovered function, so introducing a provider abstraction
/// here would be invented structure. VS-3 adds a numeric-provider port for MATRIX.
/// </para>
/// </remarks>
public interface IGenerateLinearSequence
{
    /// <summary>
    /// Evaluates <paramref name="request"/> with both endpoints included.
    /// </summary>
    /// <returns>The evaluated sequence. Never <see langword="null"/>.</returns>
    /// <exception cref="LinearSequenceRejectedException">
    /// The length rules failed (REQ-001 S5, S6). No sequence is allocated, and the
    /// process is not terminated.
    /// </exception>
    LinearSequence Generate(LinearSequenceRequest request);
}
