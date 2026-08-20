<!--
Refined-requirements contract:
- This file is produced by /refine-feature (architect in Discovery / Refinement Mode).
- Save it under the configured requirements directory using the configured requirement naming pattern (default `docs/requirements/REQ-NNN-short-slug.md`). Use sequential three-digit `NNN` unless the profile overrides the pattern.
- Do not delete or renumber existing REQ files; append the next number.
- Every Gherkin scenario must have an ID matching the configured scenario ID pattern (default `Sn`). The architect references those IDs from story Test Plans so each scenario traces to a concrete test.
- Unresolved questions are listed under `Open questions` — they are blocking. Do not write design or stories until they are resolved (architect should stop and re-ask).
-->

# REQ-001: Inclusive linear sequence (linspace)

**Source material:**
- `docs/PURPOSE.md` — demonstration-first POC; product is the host-neutral managed C# API; fidelity at that API over host-idiomatic defaults
- `docs/DOMAIN.md` — `LinearSequenceRequest` / `LinearSequence`; first-slice events `LinearSequenceProduced` / `LinearSequenceRejected`
- `docs/modernization/behaviors/BEH-001-linspace.md` — recovered library `TOOLS.linspace`
- `docs/modernization/flows/BEH-001-linspace.md` — probe call path and abort mapping
- `docs/modernization/fixtures/FIX-001-linspace-5.md` — accepted parsed values
- `docs/modernization/defect-ledger.md` — DEF-001 (decided), DEF-002 (error-path allocation)
- `docs/modernization/migration-plan.md` — VS-1; open-at-refine questions
- ADR-001 (probe baseline for BEH-001), ADR-002 (hexagonal managed API; typed domain failure), ADR-003 (binary64; exact parsed equality; general inclusive formula), ADR-005 (Fortran ABI not retained), ADR-006 (CLI retired), ADR-007 (message text / channel / exit are adapters), ADR-008 (VS-1 is the settled-oracle slice)

**Date:** `2026-08-20`
**Status:** Ready for Design

---

## 1. Goals

- Give a managed-API caller a host-neutral way to request an inclusive linear sequence by start, stop, and length, matching legacy `TOOLS.linspace` with default inclusive endpoints.
- Prove the ADD modernization trail on a settled oracle: `FIX-001` (`linspace(0,1,5)` → `0, 0.25, 0.5, 0.75, 1`) under exact parsed numeric equality.
- Classify invalid length as a typed domain failure at the port, not as process `STOP` or Fortran stdout diagnostics.
- Keep unexecuted optional-flag branches recovered but out of this requirement, so later stories cannot treat them as accepted parity.

## 2. Non-goals

- The `linspace` CLI program, its `RANGE` parser, help text, or list-directed stdout (`BEH-201`; ADR-006).
- Optional legacy parameters `istart`, `iend`, and `mesh` as part of this managed contract. They remain recovered in `BEH-001` and unaccepted for parity (ADR-003).
- Fortran formatted text, locale, or complex-column codecs as product behavior (ADR-003, ADR-007).
- The Fortran `.mod` / `libscifor.a` ABI, including processor-dependent evaluation of `array(num)` before the `num<0` check (ADR-005; DEF-002).
- HTTP/ASP.NET as a driving adapter in this requirement (ADR-002; optional later).
- Other `TOOLS` grids (`logspace`, `arange`, …) and later vertical slices (VS-2, VS-3).
- NaN, Infinity, signed-zero, subnormal, overflow, or `ddp=16` behavior (ADR-003 explicit non-decisions).
- Treating `fidelity/golden/linspace-5.txt` or profile `1e-6` / script `1e-10` as the pass/fail rule (DEF-001; ADR-003).
- Silently “fixing” or inventing endpoint-flag semantics that were never executed.

## 3. Personas / actors

- **Managed-API caller** — A C# consumer of the host-neutral linspace port. Supplies start, stop, and length; receives an ordered binary64 sequence or a typed domain failure. Does not see Fortran stdout, CLI flags, or process exit codes. `E2` — ADR-002; `docs/PURPOSE.md` secondary actor.

- **ADD practitioner** — The primary purpose actor. Uses this requirement as the inspectable specification that later design, port stories, tests, and `FIX-001` parity must trace to. `E2` — `docs/PURPOSE.md`.

The legacy fidelity driver and Fortran `use TOOLS` callers are evidence of how the behavior was exercised, not product personas for this requirement.

## 4. User scenarios (Gherkin)

Parity is claimed only for **S1**. S2–S4 are specified by the accepted default-inclusive formula (ADR-003) and need additional fixtures before a parity claim. S5–S6 are specified by recovered abort classification (E3) mapped through ADR-002/007; they are not T1.

### S1 — Inclusive five-point unit interval

```gherkin
Given a linspace request with start 0, stop 1, and length 5
And   default inclusive endpoints
When  the host-neutral linspace port is invoked
Then  the result is a sequence of length 5
And   the values equal 0, 0.25, 0.5, 0.75, and 1 by exact parsed numeric equality
```

### S2 — Default-inclusive evaluation follows the accepted formula

```gherkin
Given a linspace request with start S, stop T, and length N
And   N is an integer greater than or equal to 2
And   default inclusive endpoints
When  the host-neutral linspace port is invoked
Then  the result is a sequence of length N
And   sample i (1-based in the legacy formula) equals S + (i-1) * (T-S)/(N-1)
And   the first sample is S and the last sample is T
```

### S3 — Decreasing interval uses the same inclusive formula

```gherkin
Given a linspace request whose start is greater than its stop
And   length is an integer greater than or equal to 2
And   default inclusive endpoints
When  the host-neutral linspace port is invoked
Then  the result uses a negative step
And   both endpoints are included
And   interior samples follow the S2 formula
```

### S4 — Equal endpoints produce a constant sequence

```gherkin
Given a linspace request whose start equals its stop
And   length is an integer greater than or equal to 2
And   default inclusive endpoints
When  the host-neutral linspace port is invoked
Then  every sample equals start
And   the result length equals the requested length
```

### S5 — Negative length is a typed domain failure

```gherkin
Given a linspace request whose length is less than 0
When  the host-neutral linspace port is invoked
Then  the call does not return a sequence
And   the failure is a typed domain failure that callers can distinguish from success
And   the process is not terminated
And   leftover Fortran text "linspace: N<0, abort." is not the managed-API contract
```

### S6 — Inclusive length less than 2 is a typed domain failure

```gherkin
Given a linspace request with default inclusive endpoints
And   length is 0 or 1
When  the host-neutral linspace port is invoked
Then  the call does not return a sequence
And   the failure is a typed domain failure that callers can distinguish from success
And   the process is not terminated
And   leftover Fortran text "linspace: N<2 with both start and end points" is not the managed-API contract
```

## 5. Constraints

- **Process boundary.** The product contract is the host-neutral managed API. Domain code must not depend on ASP.NET, a CLI parser, Fortran formatted I/O, or filesystem/shell adapters (ADR-002 §2, ADR-007).
- **Numeric representation.** Legacy `real(8)` sequence values map to IEEE-754 binary64. `FIX-001` records the managed sequence as a zero-based ordered list of those values (ADR-003; FIX-001).
- **Comparison policy for this requirement.** S1 uses exact parsed numeric equality. Do not use workflow `1e-6` or fidelity-script `1e-10` (ADR-003). DEF-308 remains open for other slices, not for S1.
- **Inclusive formula.** With both endpoints included and `N >= 2`, `step = (stop-start)/(N-1)` and `array(i) = start + (i-1)*step` for `i = 1..N` (ADR-003; `src/tools_grids.f90` via BEH-001).
- **Failure classification.** `length < 0`, and inclusive endpoints with `length < 2`, are domain failures at the port (BEH-001 rules; ADR-002 §4). Message text, ANSI styling, output channel, and process exit status are adapter concerns with no fidelity requirement (ADR-007).
- **DEF-001.** Reproduce the probe parsed values. The Python-generated golden file is not authority.
- **DEF-002.** Do not reproduce Fortran declaration of `array(num)` before the negative-length check. Reject invalid length as a typed domain failure without attempting to allocate a sequence (Fortran ABI not retained, ADR-005 §2).
- **Oracle baseline.** Source revision `e586903a26cc50ca8942f20ca3bccbd8814e6252` and the recorded 2026-08-10 probe environment (ADR-001).
- **Optional flags.** This requirement’s managed contract is start, stop, and length with inclusive defaults. `istart` / `iend` / `mesh` are deferred, not dropped from the catalog: they stay in `BEH-001` as unexecuted branches and must not be implemented as accepted parity in VS-1.
- **Downstream Fortran callers** besides the fidelity driver are out of scope (ADR-005; BEH-001 E5).

## 6. Resolved questions

| # | Question | Resolution | Source |
|---|----------|------------|--------|
| 1 | What is the first retained behavior and driving adapter? | Library `linspace`; first driving adapter is the managed C# API, not CLI or HTTP. | ADR-001; ADR-002; owner 2026-08-19 |
| 2 | Which oracle and comparison rule apply? | Probe `e586903` environment; `FIX-001` exact parsed equality `0, 0.25, 0.5, 0.75, 1`. Not the Python golden file; not `1e-6`/`1e-10`. | ADR-001; ADR-003; DEF-001; FIX-001 |
| 3 | How does `real(8)` map at the port? | IEEE-754 binary64 (`double` at the managed API). | ADR-003 |
| 4 | Are CLI aliases, defaults (`wmin=-5`), and Fortran stdout in scope? | No. CLI retired; text codecs are adapters; parity is parsed values at the port. | ADR-006; ADR-007; ADR-003 |
| 5 | Should `istart` / `iend` / `mesh` be on the first managed port? | No for this requirement. Inclusive defaults only. Flags remain recovered and unaccepted for parity; a later REQ may add them with fixtures. | ADR-003; ADR-008; BEH-001 §10; flow §6 |
| 6 | Must decreasing intervals and `start == stop` be specified now? | Yes, as consequences of the accepted inclusive formula (S3, S4). They are not T1; additional fixtures are required before a parity claim. | ADR-003; BEH-001 §6 (E4 from formula) |
| 7 | What failure vocabulary replaces `STOP` and leftover `N<0` / `N<2` strings? | Typed domain failure at the port. Classification is domain; exact Fortran message text, channel, and exit status are not the managed contract. Concrete type names are design. | ADR-002 §4; ADR-007; DOMAIN `LinearSequenceRejected` |
| 8 | Must Fortran `array(num)`-before-check behavior be reproduced (DEF-002)? | No. Fortran ABI is not retained. Invalid length is rejected as a typed domain failure without sequence allocation. | ADR-005 §2; ADR-002 §4; DEF-002 disposition 2026-08-20 |
| 9 | Are downstream Fortran `linspace` callers besides the fidelity driver in scope? | No. Unknown consumers are not required. | ADR-005; BEH-001 §10 |
| 10 | Are NaN / Infinity / signed-zero / `ddp=16` in this requirement? | No. ADR-003 leaves those paths undecided; they stay out of VS-1. | ADR-003 explicit non-decisions |

## 7. Open questions

Remaining items are closed for VS-1 design (ADR-009/010, Proposed) or remain later-slice work:

- [x] Concrete managed type names, namespaces, and solution layout — `SciFor.*` projects, `IGenerateLinearSequence`, `Grids.Linspace` ([ADR-009](../decisions/ADR-009-vs1-managed-port-and-layout.md), Proposed).
- [x] Concrete typed-failure type name for `LinearSequenceRejected` — `LinearSequenceRejectedException` / `LinearSequenceRejection` ([ADR-010](../decisions/ADR-010-typed-domain-failure.md), Proposed).

DEF-308 (library-wide comparison-policy tension) remains open and **does not block this requirement**: S1 is exact parsed equality under ADR-003.

## 8. Suggested ADR triggers

| Trigger | Why it likely needs an ADR | Related Sn |
|---------|----------------------------|------------|
| Managed port signature and sequence type | ADR-002 deferred names, namespaces, and layout; first port signature is VS-1 design | S1–S6 |
| Typed domain-failure type for length rejection | Classification is decided; the failure type is the reusable VS-2/VS-3 pattern | S5, S6 |
| Endpoint-flag / `mesh` surface (future REQ) | Recovered but unexecuted; adding them is a new contract, not a silent extension of REQ-001 | (none in this REQ) |
| Additional `linspace` fixtures beyond FIX-001 | S2–S4 are specified from the formula; a later numeric-contract amendment would promote any new captures | S2, S3, S4 |

Do **not** create those ADRs in this command. `/design-application` or `/plan-port-story` owns them.

## 9. Links

- Source material: see header
- Related REQ files: none — REQ-001 is first
- Related ADRs: ADR-001, ADR-002, ADR-003, ADR-005, ADR-006, ADR-007, ADR-008, ADR-009 (Proposed), ADR-010 (Proposed)
- Design: `README.md`
- Behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Fixture: `docs/modernization/fixtures/FIX-001-linspace-5.md`
- Domain: `docs/DOMAIN.md` §§4a, 5a, 7a, 9a, 10a
- Plan: `docs/modernization/migration-plan.md` (VS-1)

---

*Created: 2026-08-20 | Refined by: architect in Discovery Mode | Design types: ADR-009/010 Proposed | Marked `Ready for Design` by owner: 2026-08-20*
