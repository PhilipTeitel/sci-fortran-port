# ADR-002: Hexagonal architecture with a managed-API first driving adapter

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

The target stack is C# / .NET 8 / ASP.NET Core, but the legacy checkout has no web or service topology. Condition 4 blocked first-slice work until a public/process boundary was chosen. The owner stated that the eventual architecture is hexagonal, so a specific host implementation is not the point of the first slice, and that the first driving surface should be a **managed API**.

Hexagonal (ports and adapters) architecture is also a required ADD methodology setting.

---

## Decision

1. The target application uses **hexagonal (ports and adapters)** architecture.
2. Domain/use-case code for retained numerical behavior is **host-neutral**: it must not depend on ASP.NET, a CLI parser, Fortran formatted I/O, or filesystem/shell adapters.
3. The **first driving adapter** for BEH-001 is a managed C# API (library surface). CLI and HTTP adapters, if added later, call the same use case and do not redefine `linspace` arithmetic.
4. Fortran `STOP` / `error()` on invalid `linspace` inputs maps, at the managed port, to a typed domain failure. Host-specific CLI exit codes or HTTP Problem Details are adapter concerns and are out of this slice.

Concrete type names, project layout, DI container choice, and package IDs are **not** part of this decision. Those belong to later design/story work.

---

## Consequences

**Positive**

- The first slice can prove recovered behavior without pretending ASP.NET is a Fortran translation.
- Later CLI or HTTP adapters can be added without changing the `linspace` contract.
- Parity can be asserted against numeric results at the port, not against Fortran stdout formatting.

**Negative / costs**

- The legacy CLI (`numutils/src/linspace.f90`) is not the first-slice surface; CLI argument aliases, defaults, and list-directed output remain unaccepted.
- Callers that today link `libscifor.a` are not inventoried; this slice does not preserve the Fortran module ABI.

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Compatibility CLI first | Owner chose managed API for now |
| ASP.NET HTTP endpoint first | Hosting is net-new; hexagonal core should not start there |
| Native Fortran ABI wrapper | Unknown consumers; wrong shape for a C# port exercise |

---

## Explicit non-decisions

- This ADR does not define port method signatures, namespaces, or NuGet packaging.
- This ADR does not choose serialization, auth, cancellation, or ASP.NET middleware.
- This ADR does not retain or retire the `linspace` CLI program.
- Out of scope: MPI/OpenMP-named globals, plotting, expression evaluation, and other library modules.

---

## Links

- Assessment: `docs/modernization/ASSESSMENT.md` Conditions 3–4 (narrowed to first slice)
- Translation gaps: GAP-019, GAP-020, GAP-026, GAP-028
- Behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Related: ADR-001, ADR-003
