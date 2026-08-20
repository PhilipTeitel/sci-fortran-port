<!-- Port story contract:
- Based on the standard user-story template, with modernization-specific provenance and parity sections.
- Every covered BEH-NNN must have evidence grade, citation, oracle fixture or acceptance-data source, and Phase P coverage.
- Phase P is mandatory before Phase Y. Z8 is mandatory in Phase Z.
-->

# VS1-1: Inclusive linspace at the managed port

**Story**: As a managed-API caller, I want to request an inclusive linear sequence by start, stop, and length so that I get the same evenly spaced binary64 samples the legacy `TOOLS.linspace` produced, without a Fortran toolchain, a CLI, or a process `STOP`.
**Epic**: 1 — VS-1 `BEH-001` inclusive linear sequence
**Size**: Medium
**Status**: Open
**Modernization slice:** `strangler`
**Structure fidelity:** `preserve-then-refactor`

> **This story is not Ready.** One owner gate in §3 remains open: ADR-009 and ADR-010 are `Proposed`. See §12 Tensions / conflicts. Do not start implementation until it closes — the type names, namespaces, project paths, and failure codes in §4b, §5, and §7 all come from those two ADRs. `REQ-001` was marked `Ready for Design` on 2026-08-20, closing the other gate.

---

## 1. Summary

Port the recovered `BEH-001` inclusive-linspace behavior to the first host-neutral C# port, and prove it against the one settled oracle in this repository (`FIX-001`). This is VS-1, the walking skeleton: it creates `SciFor.sln` and the three-project hexagonal layout that VS-2 (`fermi`) and VS-3 (`MATRIX`) grow into, plus the typed-domain-failure pattern those slices reuse.

The arithmetic is four divisions. The point of the slice is the trail: `BEH-001` → `REQ-001` S1–S6 → ADR-009/010 → this story → a parity test that cites `FIX-001` rather than a regenerated golden file. Its design cost is deliberately higher than its arithmetic suggests (`docs/modernization/migration-plan.md` §5, VS-1).

Scope is start, stop, and length with inclusive endpoints. Optional legacy `istart` / `iend` / `mesh` are **not** parameters on this port (`REQ-001` Q5).

### 1a. Domain model touchpoints

| Purpose / domain section | Terms / entities / fields / invariants touched | Evidence |
|--------------------------|-----------------------------------------------|----------|
| `docs/PURPOSE.md` — managed-API product boundary | Product is the host-neutral managed C# API; fidelity at that API over host-idiomatic defaults; domain has no compile-time I/O, CLI, timing, or hosting dependency | `E2 documented` — PURPOSE; ADR-002 §2 |
| `docs/DOMAIN.md` §4a | `LinearSequenceRequest.start`, `.stop` (binary64, unconstrained in recovered code); `.length` (inclusive default requires `>= 2`, `< 0` is a domain failure); `LinearSequence.samples` (ordered binary64) | `E3 code-derived` / `E1 verified` — BEH-001; FIX-001; ADR-003 |
| `docs/DOMAIN.md` §5a | `LinearSequenceRequest` / `LinearSequence` are value objects, no persistence, inclusive formula, typed domain failure instead of `STOP` | `E2 documented` — DOMAIN §5a; BEH-001 |
| `docs/DOMAIN.md` §7a | Consistency boundary "Linear sequence evaluation (VS-1)" protects the inclusive formula and the length rules; external interaction is the managed API only | `E2 documented` — DOMAIN §7a |
| `docs/DOMAIN.md` §9a | Emits `LinearSequenceProduced` on success and `LinearSequenceRejected` on a length/endpoint rule failure | `E2 documented` — DOMAIN §9a; `REQ-001` S5/S6 |
| `docs/DOMAIN.md` §10a | Closes the canonical-naming question **for VS-1 only**: domain types use ubiquitous language, the adapter method keeps the Fortran job name `Linspace` | `E2 documented` — ADR-009 §2 |

`LinearSequenceProduced` and `LinearSequenceRejected` are domain events in the modeling sense. This story does **not** add an event bus, publisher, or handler; no requirement asks for one.

### 1b. Legacy source touchpoints

| Legacy artifact | Role in story | Evidence grade | Citation | Notes |
|-----------------|---------------|----------------|----------|-------|
| `src/tools_grids.f90` | behavior — inclusive formula, endpoint defaults, abort conditions, `mesh` | `E3 code-derived` | `src/tools_grids.f90:1-30` | Formula at `:11-14`; `num<0` at `:7`; inclusive `num<2` at `:12` |
| `fidelity/driver.f90` | oracle — probe invocation that produced `FIX-001` | `E1 verified` / `E3 code-derived` | `fidelity/driver.f90:12-19` | `x = linspace(0.d0, 1.d0, n)` with `n = 5` |
| `docs/modernization/oracle.md` | oracle — capture, hash, cross-build parsed diff `0` | `E1 verified` | `oracle.md:83, 96, 109` | `CAP-20260810-LINSPACE` |
| `docs/modernization/fixtures/FIX-001-linspace-5.md` | fixture — accepted parsed values | `E1 verified` | entire file | The **only** parity authority in this story |
| `src/COMVARS.f90` | behavior — `error` writes ANSI stdout then `STOP` | `E3 code-derived` | `src/COMVARS.f90:189-199` | Not executed for linspace; replaced, not reproduced (ADR-007) |
| `src/TOOLS.f90`, `src/SCIFOR.f90` | source — public export path | `E3 code-derived` | `TOOLS.f90:18,161`; `SCIFOR.f90:14` | Downstream consumers `E5 unknown`; out of scope (ADR-005) |
| `fidelity/golden/linspace-5.txt` | defect — Python-regenerated, not a retained capture | `E3` / `E4 inferred` | `oracle.md:20,101` | `DEF-001`: **must not** be the test oracle |
| `numutils/src/linspace.f90` | out of scope — CLI program | `E3 code-derived` | `numutils/src/linspace.f90:1-51` | Retired from build scope (ADR-006) |

## 2. Linked architecture decisions (ADRs)

- `docs/decisions/ADR-001-first-slice-oracle-baseline.md` — **Accepted**; probe `e586903a26cc50ca8942f20ca3bccbd8814e6252` and the 2026-08-10 environment are the `BEH-001` oracle
- `docs/decisions/ADR-002-hexagonal-managed-api.md` — **Accepted**; hexagonal core, managed API is the first driving adapter, `STOP` maps to a typed domain failure
- `docs/decisions/ADR-003-linspace-numeric-contract.md` — **Accepted**; binary64, `FIX-001` exact parsed equality, general inclusive formula
- `docs/decisions/ADR-005-planning-gate-scope.md` — **Accepted**; Fortran `.mod` / `libscifor.a` ABI is not retained
- `docs/decisions/ADR-006-retire-cli-surface.md` — **Accepted**; no CLI adapter
- `docs/decisions/ADR-007-io-and-host-concerns-are-adapters.md` — **Accepted**; message text, ANSI styling, output channel, and exit status are adapter concerns with no fidelity requirement
- `docs/decisions/ADR-008-demonstration-first-slice-scope.md` — **Accepted**; VS-1 is the settled-oracle slice
- `docs/decisions/ADR-009-vs1-managed-port-and-layout.md` — **Proposed** *(gate)*; `SciFor.sln`, three projects, `IGenerateLinearSequence`, `Grids.Linspace`, xUnit test folders, rank-1 zero-based results
- `docs/decisions/ADR-010-typed-domain-failure.md` — **Proposed** *(gate)*; `DomainFailureException`, `LinearSequenceRejection`, `LinearSequenceRejectedException`, stable `Code` values

## 3. Definition of Ready (DoR)

- [x] Purpose and domain touchpoints are current. — `docs/PURPOSE.md` and `docs/DOMAIN.md` §§4a, 5a, 7a, 9a, 10a; mapped in §1a.
- [x] Covered `BEH-NNN` artifacts have evidence grades and citations. — `BEH-001` carries `E1` for `FIX-001`, `E3` for the formula and abort conditions, `E5` for unexecuted flags; cited in §1b.
- [x] No covered behavior is `E4 inferred` or `E5 unknown` unless a user decision is recorded below. — See the E4/E5 decision record below.
- [x] Oracle tier and fixtures / acceptance data are documented. — `FIX-001` is scoped `T1` at probe `e586903` (ADR-001); the global profile `oracleTier` stays `T3 documented-only`.
- [x] Defect-ledger decisions are recorded for known mismatches. — `DEF-001` reproduce-faithfully (probe parsed values); `DEF-002` **fix-now**; `DEF-003` / `DEF-004` retired with evidence (ADR-006 / ADR-007); `DEF-308` open but does not bind S1.
- [ ] **BLOCKED — Linked ADRs are `Accepted` or this story is explicitly a spike.** ADR-009 and ADR-010 are `Proposed`. This story is **not** a spike: it claims parity. Every type name, namespace, project path, and failure code in §4b, §5, and §7 is provisional until the owner accepts both.
- [x] **`REQ-001` is `Ready for Design`.** Marked by the owner on 2026-08-20, closing Gate 2. The scenario set S1–S6 that Phase A and Phase P trace to is owner-accepted, and so are the `E4`/`E5` decisions recorded below.

**Recorded decisions for weak-evidence coverage.** The `/plan-port-story` contract forbids marking a story ready when covered behavior is `E4 inferred` or `E5 unknown` without a recorded user decision. `BEH-001` has both:

| Weak evidence | Grade | Recorded decision | Where recorded |
|---------------|-------|-------------------|----------------|
| `start == stop` and decreasing intervals, `num >= 2`, inclusive | `E4 inferred` from the formula; never executed | Specify from the accepted formula (S3, S4); **do not** claim parity — not `T1` until additional fixtures exist | `REQ-001` Q6; `BEH-001` §6; ADR-003 |
| Optional `istart` / `iend` / `mesh` branches | `E5 unknown`; unexecuted | Excluded from this contract; stay recovered in `BEH-001` and unaccepted for parity | `REQ-001` Q5 / non-goals; `BEH-001` §10 |
| Downstream Fortran `linspace` consumers | `E5 unknown` | Out of scope; Fortran ABI not retained | `REQ-001` Q9; ADR-005 |
| NaN / Infinity / signed-zero / subnormal / overflow / `ddp=16` | `E5 unknown` | Out of scope for VS-1 | `REQ-001` Q10; ADR-003 explicit non-decisions |

Those decisions are recorded in `REQ-001`, which the owner marked `Ready for Design` on 2026-08-20. They are therefore owner-accepted, and this DoR item is satisfied. What they license is still narrow: S3 and S4 may be implemented and covered in Phase A, and they may **not** be described as parity or given a Phase P criterion until a fixture exists.

## 4. Binding constraints (non-negotiable)

- **B1 — Process boundary.** `SciFor.Domain` and `SciFor.Application` must have no compile-time dependency on ASP.NET, a CLI parser, Fortran formatted I/O, the filesystem, a shell, or a timer. `SciFor.Managed` takes no ASP.NET package on this slice. Verifiable by project reference inspection. `E2 documented` — PURPOSE; ADR-002 §2; ADR-007; ADR-009 §1.
- **B2 — Dependency direction.** `SciFor.Domain` references nothing in Application or adapters; `SciFor.Application` references Domain only; `SciFor.Managed` references Application. `E2 documented` — ADR-009 §1.
- **B3 — Numeric representation.** Legacy `real(8)` maps to IEEE-754 binary64 (`double`). Results are a zero-based `IReadOnlyList<double>`; Fortran 1-based `array(num)` indexing is not a managed contract. `E2 documented` — ADR-003; ADR-005; ADR-009 §6.
- **B4 — Comparison policy.** `FIX-001` is compared by **exact parsed numeric equality**. Do not use the profile `1e-6` relative/absolute tolerance or the fidelity script's `1e-10`. Do not compare formatted text. `E2 documented` — ADR-003; `REQ-001` §5; DEF-001.
- **B5 — Oracle authority.** The expected values come from `FIX-001` (probe parsed values). `fidelity/golden/linspace-5.txt` must not be read, copied, or regenerated by any test. `E2 documented` — DEF-001; ADR-001.
- **B6 — Validate before allocating.** Length rules are checked before any sample storage is allocated. The Fortran `array(num)`-declared-before-`num<0`-check ordering is **not** reproduced. `E2 documented` — DEF-002 **fix-now**; ADR-005 §2.
- **B7 — No process termination and no diagnostics from the core.** Domain and Application must not call `Environment.Exit` or `Environment.FailFast`, write to console/stderr, apply ANSI styling, or set an exit code. `E2 documented` — ADR-010 §3; ADR-007.
- **B8 — Fortran message text is not a contract.** No test may assert `linspace: N<0, abort.` or `linspace: N<2 with both start and end points`. Classification is the contract; `Message` is developer-facing English. `E2 documented` — ADR-007; ADR-010 §2; `REQ-001` S5/S6.
- **B9 — Parity is asserted at the product surface.** The Phase P test calls `Grids.Linspace`, not the use case directly. `E2 documented` — ADR-002; ADR-009 §5; README API Contract.
- **B10 — No endpoint flags on the signature.** `istart` / `iend` / `mesh` are not parameters, overloads, or optional arguments in this story. `E2 documented` — `REQ-001` Q5; ADR-009 §4.
- **B11 — No scope inflation.** No `logspace`, `arange`, `fermi`, or MATRIX types. No DI container. No HTTP host. No NuGet publication. `E2 documented` — ADR-006; ADR-008; ADR-009 §1 and explicit non-decisions.

## 4b. Ports & Adapters

| Port | Adapter | Boundary owned | Contract test | Integration test |
|------|---------|----------------|---------------|------------------|
| `SciFor.Application.Grids.IGenerateLinearSequence` (inbound / driving) | `SciFor.Grids` (managed library surface) | Linear sequence evaluation (DOMAIN §7a): inclusive formula + length rules | `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs` | `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs` |

There is **no driven (outbound) port on this slice.** Linspace is pure evaluation: no file, network, clock, RNG, or provider dependency exists in the recovered behavior (`BEH-001` §4, `E3` — `src/tools_grids.f90:1-30`). Do not invent a numeric-provider port here; VS-3 adds one under Application for `MATRIX` (ADR-009 Consequences; README High-Level Architecture).

Optional HTTP would be a second driving adapter over the same inbound port. It is out of this story (ADR-002; ADR-008).

## 5. API Endpoints + Schemas

Not HTTP. This is a managed library API (README API Contract).

| Endpoint / schema | Purpose | Request / input | Response / output | Covers |
|-------------------|---------|-----------------|-------------------|--------|
| `SciFor.Grids.Linspace` | Inclusive linear sequence at the product surface | `double start`, `double stop`, `int length` | `LinearSequence`, or throws `LinearSequenceRejectedException` | `S1`–`S6`, `BEH-001` |
| `SciFor.Application.Grids.IGenerateLinearSequence.Generate` | Inbound port | `LinearSequenceRequest` | `LinearSequence`, or throws `LinearSequenceRejectedException` | `S1`–`S6` |
| `SciFor.Domain.Grids.LinearSequenceRequest` | Request value object | `record (double Start, double Stop, int Length)` | n/a | `S1`–`S6` |
| `SciFor.Domain.Grids.LinearSequence` | Result value object | n/a | `IReadOnlyList<double> Samples`, zero-based, `Count == request.Length` | `S1`–`S4` |
| `SciFor.Domain.Grids.LinearSequenceRejection` | Caller-visible failure classification | n/a | `NegativeLength` \| `InclusiveLengthBelowTwo` | `S5`, `S6` |
| `SciFor.Domain.Grids.LinearSequenceRejectedException` | Typed domain failure | n/a | `Reason`, `Request`, inherited `Code` | `S5`, `S6` |
| `SciFor.Domain.DomainFailureException` | Reusable base for VS-2 / VS-3 | n/a | `string Code` | `S5`, `S6` |

**Evaluation rule (managed indexing).** For `Length >= 2` with inclusive endpoints, `Samples[i] = Start + i * (Stop - Start) / (Length - 1)` for `i` in `0 .. Length-1`. Managed index `i` corresponds to legacy Fortran index `i+1` (ADR-009 §3; ADR-003; `REQ-001` §5).

**Stable failure codes** (ADR-010 §2), which adapters may map and which must not require Fortran text:

| Reason | `Code` | Condition |
|--------|--------|-----------|
| `NegativeLength` | `linear-sequence.negative-length` | any `Length < 0`, including `int.MinValue` |
| `InclusiveLengthBelowTwo` | `linear-sequence.inclusive-length-below-two` | `Length` is `0` or `1` with inclusive defaults |

`Generate` never returns `null` and never returns a sentinel or empty sequence for an invalid length (ADR-010 §1; alternatives rejected).

## 6. Frontend Flow

### 6a. User path

None. VS-1 is a class library with no UI (ADR-006, ADR-008). The caller path is:

```csharp
using SciFor;

var sequence = new Grids().Linspace(0.0, 1.0, 5);
// sequence.Samples == 0, 0.25, 0.5, 0.75, 1   (FIX-001)
```

### 6b. State and error handling

No UI state. The library is stateless: `Grids` holds only the inbound port, and `LinearSequenceRequest` / `LinearSequence` are immutable value objects. Invalid length surfaces as `LinearSequenceRejectedException`; the caller distinguishes cases on `Reason` (or the stable `Code`) without parsing a message.

### 6c. Legacy parity notes

- Parity is the **parsed** sequence, not formatted text. The driver's `es24.17` and the CLI's list-directed output are both out of scope (ADR-003; ADR-007; `DEF-004` retired with evidence).
- No locale or culture concern arises, because no text is produced. If a later adapter formats samples, `BEH-303` and `DEF-308` apply there, not here.
- `STOP` is deliberately **not** reproduced. A caller that ignores the exception sees the process continue; that difference from legacy is intended (ADR-010 Consequences; ADR-002).

## 7. File Touchpoints

Paths follow README "Project Structure" and ADR-009 §1/§5.

### Files to CREATE

- `SciFor.sln` — solution at the repository root
- `src/SciFor.Domain/SciFor.Domain.csproj` — `net8.0`, nullable enabled, `TreatWarningsAsErrors`, no package references
- `src/SciFor.Domain/DomainFailureException.cs` — abstract base with `string Code`
- `src/SciFor.Domain/Grids/LinearSequenceRequest.cs` — `sealed record (double Start, double Stop, int Length)`
- `src/SciFor.Domain/Grids/LinearSequence.cs` — `sealed class` exposing `IReadOnlyList<double> Samples`
- `src/SciFor.Domain/Grids/LinearSequenceRejection.cs` — `enum { NegativeLength, InclusiveLengthBelowTwo }`
- `src/SciFor.Domain/Grids/LinearSequenceRejectedException.cs` — `sealed`, derives `DomainFailureException`, carries `Reason` and `Request`
- `src/SciFor.Application/SciFor.Application.csproj` — references `SciFor.Domain` only
- `src/SciFor.Application/Grids/IGenerateLinearSequence.cs` — inbound port
- `src/SciFor.Application/Grids/GenerateLinearSequence.cs` — use case: validate, then evaluate the inclusive formula
- `src/SciFor.Managed/SciFor.Managed.csproj` — references `SciFor.Application`; `PackageId` `SciFor`; `RootNamespace` `SciFor`
- `src/SciFor.Managed/Grids.cs` — driving adapter, namespace `SciFor`; default constructor wires the use case, second constructor takes the port
- `tests/SciFor.Tests/SciFor.Tests.csproj` — `net8.0`, xUnit
- `tests/SciFor.Tests/Unit/LinearSequenceTests.cs` — domain value objects and formula helpers
- `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs` — the port via the real use case
- `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs` — `Grids` with the real use case, port not mocked
- `tests/SciFor.Tests/Parity/Fix001LinspaceParityTests.cs` — `FIX-001` exact parsed equality through `Grids.Linspace`
- `tests/SciFor.Tests/Parity/Fixtures/FIX-001-linspace-5.expected.txt` — expected parsed values transcribed from `FIX-001` with a provenance header comment

### Files to MODIFY

- `README.md` — Backlog Items: link Epic 1 / `VS1-1` to this story; keep every other design section unchanged
- `docs/modernization/behavior-catalog.md` — `BEH-001` next command → this story (after the DoR gates close)
- `docs/modernization/migration-plan.md` — VS-1 row: next command and implementation-ready state (after the DoR gates close)
- `.cursor/workflow.config.yml` — `retainedScope.nextCommand` (after the DoR gates close)

### Files to leave UNCHANGED

- `docs/requirements/REQ-001-linspace.md` — `Ready for Design` as of 2026-08-20; a later change of scope is a new REQ or a `Superseded` link, not an edit here
- `docs/decisions/ADR-009-*.md`, `docs/decisions/ADR-010-*.md` — only the owner moves `Proposed` → `Accepted`
- `docs/modernization/fixtures/FIX-001-linspace-5.md` — the fixture is evidence; transcribe it, never edit it
- `docs/modernization/oracle.md`, `docs/modernization/behaviors/`, `docs/modernization/flows/` — recovered evidence, not product
- `docs/modernization/defect-ledger.md` — `DEF-308` stays open; no row is retired by this story
- Any legacy Fortran checkout — read-only; this story runs no legacy build or probe

## 8. Acceptance Criteria Checklist

### Phase A: Ported behavior

- [ ] **A1** — `Grids.Linspace(0, 1, 5)` returns five samples `0, 0.25, 0.5, 0.75, 1`
  - Calling the product surface with the `FIX-001` inputs yields `Samples.Count == 5` and those five values, compared exactly.
  - Evidence: `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs::Linspace_UnitInterval_FivePoints_A1(dotnet test)`
  - Covers: `BEH-001`, `S1`

- [ ] **A2** — Inclusive evaluation follows the accepted formula for any `Length >= 2`
  - For representative `(start, stop, length)` triples, `Samples[i] == Start + i * (Stop - Start) / (Length - 1)`, `Samples[0] == Start`, `Samples[^1] == Stop`, and `Samples.Count == Length`.
  - Evidence: `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_InclusiveFormula_A2(dotnet test)`
  - Covers: `BEH-001`, `S2`
  - Not a parity claim: `E3` formula, `E1` only at `FIX-001` (ADR-003).

- [ ] **A3** — A decreasing interval uses a negative step and still includes both endpoints
  - With `Start > Stop` and `Length >= 2`, the step is negative, `Samples[0] == Start`, `Samples[^1] == Stop`, and interior samples follow A2.
  - Evidence: `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_DecreasingInterval_A3(dotnet test)`
  - Covers: `BEH-001`, `S3`
  - `E4 inferred`; decision recorded in `REQ-001` Q6. Not `T1`.

- [ ] **A4** — Equal endpoints produce a constant sequence of the requested length
  - With `Start == Stop` and `Length >= 2`, every sample equals `Start` and `Samples.Count == Length`.
  - Evidence: `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_EqualEndpoints_A4(dotnet test)`
  - Covers: `BEH-001`, `S4`
  - `E4 inferred`; decision recorded in `REQ-001` Q6. Not `T1`.

- [ ] **A5** — Negative length throws a typed domain failure classified `NegativeLength`
  - `Length < 0` (including `-1` and `int.MinValue`) throws `LinearSequenceRejectedException` with `Reason == NegativeLength` and `Code == "linear-sequence.negative-length"`; no sequence is returned; the process keeps running.
  - Evidence: `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_NegativeLength_Rejected_A5(dotnet test)`
  - Covers: `BEH-001`, `S5`

- [ ] **A6** — Inclusive length below 2 throws a typed domain failure classified `InclusiveLengthBelowTwo`
  - `Length` of `0` or `1` throws `LinearSequenceRejectedException` with `Reason == InclusiveLengthBelowTwo` and `Code == "linear-sequence.inclusive-length-below-two"`; the two rejection reasons are distinguishable without inspecting `Message`.
  - Evidence: `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_InclusiveLengthBelowTwo_Rejected_A6(dotnet test)`
  - Covers: `BEH-001`, `S6`

- [ ] **A7** — Rejection carries the offending request and allocates no samples
  - The thrown exception exposes the `LinearSequenceRequest` that was rejected, and no `LinearSequence` instance is constructed on the failure path.
  - Evidence: `tests/SciFor.Tests/Unit/LinearSequenceTests.cs::Rejection_CarriesRequest_NoSamples_A7(dotnet test)`
  - Covers: `BEH-001`, `S5`, `S6`
  - Encodes `DEF-002` **fix-now** at the type level (B6).

### Phase P: Parity

`BEH-001` is the only covered behavior, and `FIX-001` is the only accepted fixture, so there is exactly one parity criterion. S2–S6 deliberately have **no** Phase P criterion: no fixture exists for them, and inventing one would let Phase P evidence exceed the oracle tier.

- [ ] **P1** — Legacy `linspace(0, 1, 5)` parsed output matches the managed implementation under exact parsed numeric equality
  - Oracle: `FIX-001` (`docs/modernization/fixtures/FIX-001-linspace-5.md`), capture `CAP-20260810-LINSPACE`, probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`, scoped `T1`
  - Tolerance / normalization: **exact** parsed binary64 equality on a zero-based ordered sequence. No relative or absolute tolerance. No text comparison, no formatting, no whitespace normalization. Profile `1e-6` and script `1e-10` are explicitly not used.
  - Defect decision: `DEF-001` reproduce-faithfully — expected values are the **probe parsed values** transcribed from `FIX-001`; `fidelity/golden/linspace-5.txt` is not the authority and must not be read. `DEF-002` fix-now applies to the error path, not to P1.
  - Evidence: `tests/SciFor.Tests/Parity/Fix001LinspaceParityTests.cs::parity_BEH_001_P1(dotnet test)`
  - Asserted through `Grids.Linspace` (B9), not through the use case.

### Phase Y: Binding & stack compliance

- [ ] **Y1** — **(binding)** `SciFor.Domain` and `SciFor.Application` have no I/O, CLI, hosting, or timing dependency (B1, B2)
  - Verified by inspecting resolved project and package references: `SciFor.Domain` has no project reference and no package reference; `SciFor.Application` references `SciFor.Domain` only; neither references ASP.NET, a CLI parser, or a filesystem package.
  - Evidence: `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::domain_has_no_host_dependencies_Y1(dotnet test)`

- [ ] **Y2** — **(binding)** The inbound port is exercised through the real adapter and the real use case (§4b, B9)
  - `Grids`'s default constructor wires `GenerateLinearSequence`; the integration and parity tests use it without substituting a fake port.
  - Evidence: `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs::real_use_case_through_adapter_Y2(dotnet test)`

- [ ] **Y3** — **(binding)** The core neither terminates the process nor writes diagnostics (B7)
  - No `Environment.Exit`, `Environment.FailFast`, `Console`, or ANSI escape usage appears in `SciFor.Domain`, `SciFor.Application`, or `SciFor.Managed`; the rejection tests observe a returning exception, not a terminated process.
  - Evidence: `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::core_does_not_terminate_or_print_Y3(dotnet test)`

- [ ] **Y4** — **(binding)** No test asserts legacy Fortran diagnostic text, and no test reads the Python-generated golden file (B5, B8)
  - The parity fixture's provenance header cites `FIX-001`; no assertion references `linspace: N<0, abort.`, `linspace: N<2 with both start and end points`, or `fidelity/golden/linspace-5.txt`.
  - Evidence: `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::no_legacy_text_or_golden_oracle_Y4(dotnet test)`

- [ ] **Y5** — **(binding)** The managed surface exposes no endpoint flags and no scope beyond linspace (B10, B11)
  - `Grids` has exactly one public evaluation method, `Linspace(double, double, int)`, with no `istart` / `iend` / `mesh` parameter or overload; the public surface contains no `logspace`, `arange`, `fermi`, or MATRIX type.
  - Evidence: `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::public_surface_is_vs1_only_Y5(dotnet test)`

### Phase Z: Quality Gates

- [ ] **Z1** — `dotnet build SciFor.sln` passes with zero build/type errors
- [ ] **Z2** — `dotnet format --verify-no-changes` passes (no lint command was chosen in the design; README "Available Scripts" nominates this as the Z2 stand-in)
- [ ] **Z3** — Configured type policy passes — C# equivalent of the default no-`any` rule: `Nullable` enabled and `TreatWarningsAsErrors` set in every new project, and no `dynamic` in new or modified files (ADR-009 §1)
- [ ] **Z4** — Configured shared type import policy — **not applicable**: there is no `@shared/types` alias in a C# solution. The equivalent is enforced structurally by Y1/Y2 (project references define the dependency direction).
- [ ] **Z5** — New or modified code includes appropriate logging for errors and significant operations per the implementer's logging guidelines — **satisfied by exception**: VS-1 has no logger by design; `DomainFailureException.Code` is the machine-readable signal a later adapter logs (README "Logging and Observability"; ADR-010 §3). Adding a logger to Domain or Application would violate B1/B7.
- [ ] **Z6** — `/review-story VS1-1` satisfies the configured review gate, including zero high or critical `TEST-#`, `SEC-#`, `REL-#`, `API-#`, `MODEL-#`, `PAR-#`, or `PROV-#` findings
- [ ] **Z7** — `/review-story VS1-1` satisfies the configured model-fidelity gate
- [ ] **Z8** — `/verify-parity VS1-1` satisfies the configured parity gate and writes the configured parity report

## 8a. Test Plan

| # | Level | File::test name | Covers AC | Covers Sn | Covers BEH | Notes |
|---|-------|------------------|-----------|-----------|------------|-------|
| 1 | parity | `tests/SciFor.Tests/Parity/Fix001LinspaceParityTests.cs::parity_BEH_001_P1` | P1 | S1 | BEH-001 | `FIX-001` exact parsed equality through `Grids.Linspace`; write first and watch it fail |
| 2 | integration | `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs::Linspace_UnitInterval_FivePoints_A1` | A1 | S1 | BEH-001 | product surface, real use case |
| 3 | integration | `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs::real_use_case_through_adapter_Y2` | Y2 | S1 | BEH-001 | port not mocked |
| 4 | contract | `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_InclusiveFormula_A2` | A2 | S2 | BEH-001 | formula for `Length >= 2`; not a parity claim |
| 5 | contract | `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_DecreasingInterval_A3` | A3 | S3 | BEH-001 | `E4`; no fixture, so no Phase P row |
| 6 | contract | `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_EqualEndpoints_A4` | A4 | S4 | BEH-001 | `E4`; no fixture, so no Phase P row |
| 7 | contract | `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_NegativeLength_Rejected_A5` | A5 | S5 | BEH-001 | `Reason`/`Code`; includes `int.MinValue` |
| 8 | contract | `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs::Generate_InclusiveLengthBelowTwo_Rejected_A6` | A6 | S6 | BEH-001 | `0` and `1`; distinguishable from A5 |
| 9 | unit | `tests/SciFor.Tests/Unit/LinearSequenceTests.cs::Rejection_CarriesRequest_NoSamples_A7` | A7 | S5, S6 | BEH-001 | `DEF-002` fix-now: no allocation before validation |
| 10 | integration | `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::domain_has_no_host_dependencies_Y1` | Y1 | — | BEH-001 | reference inspection; B1/B2 |
| 11 | integration | `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::core_does_not_terminate_or_print_Y3` | Y3 | S5, S6 | BEH-001 | B7 |
| 12 | integration | `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::no_legacy_text_or_golden_oracle_Y4` | Y4 | S5, S6 | BEH-001 | B5/B8 guard against false parity |
| 13 | integration | `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs::public_surface_is_vs1_only_Y5` | Y5 | — | BEH-001 | B10/B11 |

## 8b. Parity Plan

| BEH ID | Evidence grade | Oracle / fixture | Tolerance | Defect decision | Acceptance data gap |
|--------|----------------|------------------|-----------|-----------------|---------------------|
| BEH-001 (S1, default inclusive `linspace(0,1,5)`) | `E1 verified` (parsed values; capture bytes not retained) | `FIX-001` / `CAP-20260810-LINSPACE` at `e586903`, scoped `T1` | exact parsed binary64 equality; no text compare | `DEF-001` reproduce-faithfully (probe values, not the Python golden) | none |
| BEH-001 (S2, general inclusive formula) | `E3 code-derived` — `src/tools_grids.f90:11-14` | none — Phase A only | n/a | none | **Yes** — no captured fixture beyond `FIX-001`; a parity claim needs new captures and an ADR-003 amendment |
| BEH-001 (S3/S4, decreasing and equal endpoints) | `E4 inferred` from the formula; unexecuted | none — Phase A only | n/a | none; decision recorded in `REQ-001` Q6 | **Yes** — must not be described as parity |
| BEH-001 (S5/S6, invalid length) | `E3 code-derived` — `src/tools_grids.f90:7,12`; unexecuted | none — classification only, Phase A | n/a | `DEF-002` **fix-now** (validate before allocating) | **Yes** — legacy abort was never executed; only classification is claimed, never message text or exit status |
| BEH-001 optional `istart`/`iend`/`mesh` | `E5 unknown`; unexecuted | out of scope | n/a | none | Excluded by `REQ-001` Q5; a later REQ needs fixtures first |

Global profile `oracleTier` remains `T3 documented-only`. Only `FIX-001` is scoped `T1` for this story (ADR-001), which is why exactly one Phase P criterion exists.

## 9. Risks & Tradeoffs

| Risk | Impact | Mitigation | Evidence |
|------|--------|------------|----------|
| ADR-009 / ADR-010 are `Proposed`; owner changes a type name, namespace, or the exception-vs-`Result` choice | Rework across §4b, §5, §7, and most test names | Do not start until the DoR gate closes; the arithmetic and the `Sn` mapping survive a rename, so churn is confined to names and paths | ADR-009/010 Status; ADR-010 Alternatives (a `Result` type was a live option) |
| A later REQ restates or renumbers `S1`–`S6` | Test Plan `Covers Sn` column and Phase A/P mapping drift | Largely retired: Gate 2 closed on 2026-08-20, so the numbering is now owner-accepted. Residual risk is a future REQ superseding this one; `Sn` stays in one Test Plan column so a renumber is a mechanical edit. The parallel-refine incident already produced two numbering schemes for this behavior | `REQ-001` Status `Ready for Design`; PR #5 conflict resolution |
| Four exact dyadic values make S1 look trivially passable | A weak implementation passes P1 while the formula is wrong for non-dyadic spacings | Keep A2's formula coverage in Phase A with non-dyadic triples; do not let P1 stand alone as proof of the formula | ADR-003 authorizes exact equality only for `FIX-001` |
| Someone points the parity test at `fidelity/golden/linspace-5.txt` because it is a convenient file that numerically matches | Silent regression to a non-authoritative oracle; `DEF-001` reopened | B5 plus Y4 test; fixture file carries a provenance header naming `FIX-001` | `DEF-001`; ADR-001; `oracle.md:20,101` |
| Three projects and a test taxonomy for one function reads as over-engineering | Reviewer or client dismisses the demonstration | Accept it: the ceremony is the demonstration, and VS-2/VS-3 amortize the layout | ADR-009 Consequences; `migration-plan.md` §5 |
| Exceptions used for precondition failures | A future HTTP adapter may prefer a `Result`; call sites written now assume `catch` | `Code` on `DomainFailureException` keeps adapter mapping stable; a `Result` overload can be added later without changing the formula | ADR-010 Consequences and Alternatives |
| Legacy `int.MinValue`-style extremes were never executed | Overclaiming behavior for inputs the probe never saw | A5 asserts classification only, never legacy text or exit status; `E5` inputs stay out of Phase P | `REQ-001` S5; ADR-007 |

## Implementation Order

1. **Do not start** until the remaining DoR gate in §3 closes: ADR-009 and ADR-010 must be `Accepted`. If any name changed on acceptance, update §4b, §5, §7, and the Test Plan before writing code. (`REQ-001` reached `Ready for Design` on 2026-08-20.)
2. Create `SciFor.sln` and the four projects with nullable enabled and warnings as errors, wiring only the references B2 allows. Confirm `dotnet build` succeeds on empty projects (Z1).
3. Transcribe `FIX-001` into `tests/SciFor.Tests/Parity/Fixtures/` with a provenance header, then write `parity_BEH_001_P1` and **observe it fail** because `Grids.Linspace` does not exist yet. That failure is the required starting evidence.
4. Add the Domain value objects and failure types (`LinearSequenceRequest`, `LinearSequence`, `LinearSequenceRejection`, `LinearSequenceRejectedException`, `DomainFailureException`), with A7 covering validate-before-allocate.
5. Add `IGenerateLinearSequence` and `GenerateLinearSequence`: validate length first (A5, A6), then evaluate the inclusive formula (A2–A4).
6. Add the `SciFor.Grids` adapter and its two constructors; make P1, A1, and Y2 pass.
7. Add the boundary tests (Y1, Y3, Y4, Y5).
8. Run `dotnet test`, `dotnet build`, and `dotnet format --verify-no-changes`; then `/review-story VS1-1` (Z6, Z7) and `/verify-parity VS1-1` (Z8).
9. Only after Z8 passes, update the catalog, migration plan, and `nextCommand` rows listed in §7 "Files to MODIFY".

## 10. Completion Metadata

| Field | Value |
|-------|-------|
| Completed by | `TBD` |
| Completion ref | `TBD if not committed` |
| Review ref | `TBD` |
| QA ref | `TBD` |
| Parity ref | `TBD` |

## 11. Post-complete Follow-up Ledger

| ID | Date | Change class | Intent | Files touched | Verification | Change ref | Review ref | Docs impact | AC impact |
|----|------|--------------|--------|---------------|--------------|------------|------------|-------------|-----------|
| — | — | — | None yet. | — | — | — | — | — | — |

## 12. Tensions / conflicts

These are for the owner to resolve. This story does not resolve them, and it must not be marked Ready while item 1 stands.

**Closed 2026-08-20 — `REQ-001` Gate 2.** The owner marked `REQ-001` `Ready for Design`, accepting S1–S6 and the `E4`/`E5` decisions in §3. Recorded here because the DoR and §9 previously treated it as blocking. Nothing in the Test Plan moved as a result: the story was already written against that scenario set, and the acceptance narrows nothing and widens nothing.

1. **ADR-009 and ADR-010 are `Proposed` (Gate 4).** The architect contract requires linked ADRs to be `Accepted` for a non-spike story, and this story claims parity, so it is not a spike. Every type name, namespace, project path, and `Code` value here is provisional. The design doc states the same rule: "Implementation stories wait until those are accepted" (`README.md` Requirements section). **This is now the only gate holding the story.**

2. **`/plan-project` has not run, so Epic 1 and the ID `VS1-1` are provisional.** ADR-009's design note says story IDs come from `/plan-project` ("This design does not invent story IDs"), and `.cursor/workflow.config.yml` sets `nextCommand: plan-project`. This story was written directly against the design at the user's request. If a later `/plan-project` pass numbers epics differently, reconcile the ID and the README backlog row rather than renumbering silently.

3. **The design doc reached `main` only by recovery.** `README.md`, ADR-009, and ADR-010 were authored on the branch for pull request #6 but were pushed after that PR was squash-merged, so they never landed. They were recovered onto this branch by cherry-pick. Confirm they are the design the owner intends before accepting them.

4. **`DEF-308` (comparison policy) stays open** and does not bind this story: S1 is exact parsed equality under ADR-003. It must be settled before VS-2 and VS-3 acceptance criteria. `docs/modernization/defect-ledger.md`.

5. **Z4 and Z5 have no C# meaning as written.** The default Phase Z gates assume a TypeScript project (`@shared/types` alias, a logger). §8 records the C# equivalents — project references for Z4, and no-logger-by-design for Z5. If the profile is meant to carry C# defaults, that is a `.cursor/workflow.config.yml` change, not a per-story exception.

---

*Created: 2026-08-20 | Command: `/plan-port-story` | Port story template | Slice: VS-1 | Requirements: `REQ-001` S1–S6 | Behavior: `BEH-001` | Fixture: `FIX-001`*
