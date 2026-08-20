# ADR-010: Typed domain failure for rejected linear sequences

**Status:** Accepted
**Date:** 2026-08-20 (accepted 2026-08-20)

---

## Context

Legacy `linspace` calls `error(...)` then `STOP` when `num < 0` or when both endpoints are included and `num < 2`. ADR-002 §4 maps that to a typed domain failure at the managed port. ADR-007 splits the contract: **which conditions fail** is domain; message text, ANSI styling, stdout vs stderr, and process exit status are adapter concerns with no fidelity requirement.

`REQ-001` S5 and S6 require that those length rules do not return a sequence, that callers can distinguish failure from success, and that the process is not terminated. Leftover Fortran strings (`linspace: N<0, abort.` and `linspace: N<2 with both start and end points`) are not the managed contract.

GAP-026 asked for an error taxonomy at ports. VS-2 and VS-3 will need the same pattern. This ADR names the VS-1 types and the reusable base.

DEF-002 is **fix-now**: reject negative length before allocating a result sequence. Fortran `array(num)` declaration-before-check is not retained.

---

## Decision

### 1. Failure is an exception, not a `Result` and not process termination

The inbound port `IGenerateLinearSequence.Generate` throws on S5/S6. It does not return a sentinel sequence, `null`, or a `Result<T>` type in VS-1.

A dedicated exception is the C# form of “typed domain failure” that a later HTTP adapter can catch and map to Problem Details without the domain knowing HTTP.

### 2. Types

```csharp
namespace SciFor.Domain;

public abstract class DomainFailureException : Exception
{
    public string Code { get; }
}

namespace SciFor.Domain.Grids;

public enum LinearSequenceRejection
{
    NegativeLength,
    InclusiveLengthBelowTwo
}

public sealed class LinearSequenceRejectedException : DomainFailureException
{
    public LinearSequenceRejection Reason { get; }
    public LinearSequenceRequest Request { get; }
}
```

- **`Reason`** is the caller-visible classification (S5 vs S6).
- **`Code`** is a stable, host-neutral identifier (`linear-sequence.negative-length`, `linear-sequence.inclusive-length-below-two`). Adapters may map `Code`; they must not require Fortran text.
- **`Message`** is developer-facing English. It is **not** a parity surface and must not be required to match legacy `error()` strings (ADR-007).
- The exception is thrown **after** length validation and **before** allocating `LinearSequence.Samples` (DEF-002).

`length == 0` and `length == 1` with inclusive defaults use `InclusiveLengthBelowTwo`. Any `length < 0` uses `NegativeLength`, including `int.MinValue`.

### 3. What stays out of the domain

Domain and Application must not:

- call `Environment.Exit`, `Environment.FailFast`, or otherwise stop the process
- write to console, stderr, or a diagnostic channel
- apply ANSI styling
- set a Unix exit code

A future HTTP or logging adapter may observe `DomainFailureException` and emit host-specific output. That adapter is not part of VS-1.

### 4. Reuse

Later slices add sibling types (`FermiRejectedException`, …) under `DomainFailureException`. They do not invent a second failure style (HRESULT, `bool`+`out`, `STOP` emulation) without a new ADR.

---

## Consequences

**Positive**

- S5/S6 are testable without spawning a process.
- HTTP can map `Code` later without changing the use case.
- Fortran diagnostic text cannot leak into parity assertions.

**Negative / costs**

- Exceptions for precondition failures are a control-flow choice; a `Result` type would also have satisfied “distinguishable from success.” Exceptions were chosen for idiomatic C# library call sites and for adapter `catch` mapping.
- Callers that ignore exceptions still see process continuation, unlike legacy `STOP`. That difference is intended (ADR-002).

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| `Result<LinearSequence, LinearSequenceRejection>` | Valid hexagonal style; less idiomatic at a C# library call site; still easy to add later if a host prefers it |
| Throw `ArgumentOutOfRangeException` | Loses a domain type and the `LinearSequenceRejected` event; mixes BCL precondition vocabulary with recovered length rules |
| Preserve Fortran message strings as `Exception.Message` | ADR-007: message text is not a fidelity requirement; would invite false parity |
| Return an empty sequence on invalid length | Changes recovered classification; would be a silent fix |
| `Environment.FailFast` / `Environment.Exit` | Re-implements `STOP`; forbidden at the managed port |

---

## Explicit non-decisions

- HTTP Problem Details titles, status codes, and serialization.
- Logging library and whether adapters log `Code` at `warn`.
- Whether VS-3 LAPACK `info` values become `DomainFailureException` subclasses or a different numeric-status type — decide with the VS-3 contract ADR.
- Localization of `Message`.

---

## Links

- Requirements: `docs/requirements/REQ-001-linspace.md` S5, S6
- Related design doc section: Key Design Decisions — domain failure; API Contract
- Related: ADR-002 §4, ADR-007, ADR-009, GAP-026, DEF-002
- Domain event: `LinearSequenceRejected`
