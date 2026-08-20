# ADR-009: VS-1 solution layout and managed linspace port

**Status:** Proposed
**Date:** 2026-08-20

---

## Context

ADR-002 chose hexagonal architecture and a managed-API driving adapter, and explicitly deferred type names, namespaces, project layout, DI, and package IDs. `REQ-001` now specifies the linspace contract (S1–S6) and lists those deferred items as design work.

VS-1 must also establish a layout later slices can grow into (VS-2 `fermi`, VS-3 `MATRIX` behind a numeric port) without putting ASP.NET, CLI, or file I/O into the domain. Purpose requires that domain code have no compile-time dependency on I/O, CLI, timing, or hosting, verifiable by inspection.

DOMAIN still had an open naming question: keep Fortran identifiers as aliases, or use ubiquitous-language types only.

---

## Decision

### 1. Solution and packages

- Solution file: `SciFor.sln` at the repository root.
- Target framework: `net8.0` class libraries. Nullable reference types on. Treat warnings as errors.
- Packable product: `SciFor.Managed`, NuGet ID `SciFor`. This is a private POC package; it is not a SciFortran redistribution.
- Root namespace: `SciFor` (ubiquitous name for the former `SCIFOR` facade).

Three projects enforce dependency direction:

| Project | Responsibility | May reference |
|---------|----------------|---------------|
| `SciFor.Domain` | Value objects, invariants, domain failures | nothing in Application or adapters |
| `SciFor.Application` | Inbound ports and use cases | Domain only |
| `SciFor.Managed` | Driving adapter (public library surface) | Application (and thus Domain) |

No DI container in VS-1. The managed adapter constructs the use case in its default constructor and accepts the inbound port in a second constructor for tests. Microsoft.Extensions.DependencyInjection stays out until a host adapter needs it.

No ASP.NET, CLI, or filesystem package may be referenced by Domain or Application. `SciFor.Managed` also takes no ASP.NET package in this slice.

### 2. Naming

- **Domain types** use ubiquitous language: `LinearSequenceRequest`, `LinearSequence`, `LinearSequenceRejectedException`, `LinearSequenceRejection`.
- **Managed adapter method** keeps the Fortran job name `Linspace` so the product surface is recognizable without implying a complete SciFortran port.
- Other Fortran names (`fermi`, `fftgf`, matrix routines) are **not** decided here.

### 3. Types and inbound port

```csharp
namespace SciFor.Domain.Grids;

public sealed record LinearSequenceRequest(double Start, double Stop, int Length);

public sealed class LinearSequence
{
    public IReadOnlyList<double> Samples { get; }
    // constructed only after length rules pass; Samples is zero-based, length == request.Length
}

namespace SciFor.Application.Grids;

public interface IGenerateLinearSequence
{
    LinearSequence Generate(LinearSequenceRequest request);
}
```

`Generate` either returns a `LinearSequence` or throws `LinearSequenceRejectedException` (ADR-010). It does not return `null`, terminate the process, or allocate a result sequence before the length rules pass (DEF-002).

Inclusive-endpoint evaluation uses the accepted formula, with managed index `i` in `0 .. Length-1` corresponding to legacy Fortran index `i+1`:

`samples[i] = start + i * (stop - start) / (length - 1)` for `length >= 2`.

### 4. Driving adapter

```csharp
namespace SciFor;

public sealed class Grids
{
    public Grids(); // wires GenerateLinearSequence
    public Grids(IGenerateLinearSequence generator);

    public LinearSequence Linspace(double start, double stop, int length);
}
```

`Linspace` builds a `LinearSequenceRequest` and calls the port. It does not reimplement the formula. Callers write `new Grids().Linspace(0.0, 1.0, 5)`.

Optional legacy flags `istart` / `iend` / `mesh` are not parameters on this method (`REQ-001` Q5).

### 5. Tests

One test project `tests/SciFor.Tests` (`net8.0`, xUnit):

| Folder | Level | Against |
|--------|-------|---------|
| `Unit/` | unit | Domain types and formula helpers, no I/O |
| `Contract/` | contract | `IGenerateLinearSequence` via `GenerateLinearSequence` |
| `Integration/` | integration | `Grids` with the real use case (no mock of the port) |
| `Parity/` | parity | `FIX-001` exact parsed equality through `Grids.Linspace` |

### 6. Layout (rank-1 only)

Rank-1 results are a zero-based `IReadOnlyList<double>`. Fortran 1-based `array(num)` indexing is not a managed-API contract (ADR-005). Matrix column-major / LDA conventions remain a VS-3 question (BEH-302).

---

## Consequences

**Positive**

- Project references make the host-neutral rule mechanically visible.
- VS-2 can add `SciFor.Domain.Functions` without renaming VS-1 types.
- VS-3 can add a driven numeric-provider port under Application without moving linspace.
- Parity is asserted at the product surface (`Grids.Linspace`), matching ADR-002.

**Negative / costs**

- Three projects for one function is ceremony; that ceremony is the demonstration.
- `Linspace` as a method name on `Grids` is not a Fortran module ABI and will not compile Fortran callers.
- Static `Grids.Linspace` was rejected; callers must construct `Grids`.

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Single `SciFor` project | Would not make domain-vs-adapter references inspectable |
| Put the formula in `SciFor.Managed` | Driving adapter would own arithmetic; later HTTP would duplicate or skip the port |
| Return `double[]` only, no `LinearSequence` | Drops the ubiquitous-language type DOMAIN already named |
| Microsoft.Extensions.DependencyInjection in VS-1 | No driven adapters to swap; pulls hosting types into a library |
| ASP.NET controller as first adapter | Forbidden by ADR-002; no legacy web topology |
| Keep 1-based indexing at the C# port | Conflicts with FIX-001’s zero-based managed sequence and with C# defaults |

---

## Explicit non-decisions

- NuGet versioning, signing, and public feed publication.
- DI container choice for a future HTTP host.
- VS-2/VS-3 type names, MATRIX provider port, and rank-2 layout.
- IEEE specials (NaN, Infinity, signed-zero, subnormals) — ADR-003 non-decisions.
- Endpoint flags / `mesh` — `REQ-001` deferred.
- Whether `LinearSequence` later implements `IReadOnlyList<double>` itself vs exposing `Samples`.

---

## Links

- Requirements: `docs/requirements/REQ-001-linspace.md` (S1–S6)
- Related design doc section: High-Level Architecture; Project Structure; API Contract
- Related: ADR-002, ADR-003, ADR-005, ADR-007, ADR-008, ADR-010
- Domain: `LinearSequenceRequest`, `LinearSequence`
- Fixture: `docs/modernization/fixtures/FIX-001-linspace-5.md`
