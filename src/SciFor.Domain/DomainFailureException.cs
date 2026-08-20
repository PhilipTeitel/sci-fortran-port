namespace SciFor.Domain;

/// <summary>
/// Base for typed domain failures at a managed port (ADR-010 §2).
/// </summary>
/// <remarks>
/// Legacy SciFortran signalled fatal conditions by printing an ANSI-styled diagnostic to
/// stdout and calling <c>STOP</c>. ADR-007 splits that into two contracts: which
/// conditions fail is domain, while message text, output channel, and exit status are
/// adapter concerns with no fidelity requirement. This type carries only the domain half.
/// <para>
/// <see cref="Code"/> exists so an adapter can map a failure without depending on
/// <see cref="Exception.Message"/>. Message text is developer-facing English and is not a
/// parity surface; no test may require it to match a Fortran <c>error()</c> string.
/// </para>
/// <para>
/// Later slices add sibling types under this base. They must not introduce a second
/// failure style (HRESULT, bool + out parameter, STOP emulation) without a new ADR
/// (ADR-010 §4).
/// </para>
/// </remarks>
public abstract class DomainFailureException : Exception
{
    protected DomainFailureException(string code, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        Code = code;
    }

    /// <summary>
    /// Stable, host-neutral failure identifier. Adapters may map this; they must not
    /// require legacy Fortran diagnostic text.
    /// </summary>
    public string Code { get; }
}
