<!--
Purpose artifact contract:
- This file is produced or updated by /define-purpose (modeler in Purpose mode) or /recover-domain (modeler in Legacy recovery mode).
- Save it to the configured purpose path (default `docs/PURPOSE.md`).
- Purpose is canonical for product intent. Requirements, design, domain modeling, stories, reviews, and QA must not contradict it silently.
- Keep this artifact short enough for every later agent and human gate to read. If it becomes a requirements document, it has stopped doing its job.
- Unresolved purpose-level questions are listed under `Open purpose questions` and block downstream work for the affected scope.
-->

# Purpose: Artifact-Driven Development for legacy migration (POC)

**Source material:**
- Owner statement dated 2026-08-19: the objective is a POC that Artifact-Driven Development extends to migrating applications, prepared as evidence for a prospective client converting thick applications to thin UIs backed by services
- Owner decision dated 2026-08-19: the process boundary is the **managed C# API** (ADR-002/003); the CLI is retired (ADR-006); I/O and host concerns are adapters (ADR-007); build scope is representative vertical slices (ADR-008)
- Owner correction dated 2026-08-19: whole-library port authorized in principle (ADR-004), narrowed to planned slices by ADR-008
- User purpose answers dated 2026-08-18: port of existing functionality; POC (success criterion since revised — see decisions of record)
- `docs/modernization/ASSESSMENT.md` §§1, 3, 8–9 (exercise-scoped `go-with-conditions`; not production redistribution approval)
- Probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`
- ADRs 001–008
- Recovered BEH-001 / FIX-001; cross-cutting contracts BEH-301–305
- `docs/modernization/defect-ledger.md` (DEF-001–004; DEF-301–313, most retired by ADR-006/007)

**Date:** 2026-08-19
**Status:** Accepted

---

## Thesis

This repository exists to **demonstrate that Artifact-Driven Development extends from greenfield work to migrating legacy applications**. The SciFortran-to-C# port is the worked example. A managed C# numeric API is the artifact that proves the method reaches working, verified code rather than stopping at documentation.

It is a private proof of concept. It is not a SciFortran replacement, not a license to redistribute restricted Fortran, and not an ASP.NET rewrite of a system that had no web topology.

## The job it does

The primary actor is the **practitioner preparing evidence for prospective clients** who have thick applications and want thin UIs backed by services. They hire this repository to produce an **inspectable artifact trail**: legacy characterization, recovered behavior with evidence grades, decisions with rationale, requirements, stories, implementation, and verification — unbroken, on a real legacy codebase that nobody wrote for this purpose.

The secondary actor is a **C# caller of the managed port** for the slices actually built. That caller is real: the code must work and be verified against the legacy baseline. But the library is a fragment by design, and its breadth is not the point.

The job matters because the claim under test is not "C# can compute a grid." It is that a disciplined artifact chain can carry a migration from an undocumented legacy codebase to verified replacement code, and that the ports it produces are shaped to become services later.

## North-star outcome

A reader can follow an unbroken trail from legacy characterization to passing C# for each named vertical slice, including at least one surface hard enough to be convincing — an external numeric dependency substituted behind a port, with parity evidence, without copying vendored source. The domain code carries no dependency on file I/O, CLI parsing, timing, or hosting, so the same use cases could be exposed as services without touching the domain.

## Trade-off rule

When goals conflict, optimize for **completeness and honesty of the artifact trail** over **breadth of library coverage**.

A shortcut that produces working code but leaves a gap in the trail defeats the purpose; take the longer path. Equally, porting another module because it is there adds cost without adding evidence; leave it in reserve.

Within a slice, optimize for **fidelity to observable legacy behavior at the managed API** over convenience, global normalization, host-idiomatic defaults, or documentation-preferred corrections. Help text and comments lose when they conflict with what the legacy code computes. Do not silently "fix" unexecuted branches or recorded contradictions — a silently fixed quirk is a hole in the trail, which is worse here than a faithfully reproduced bug.

## Product boundary

The product is the **host-neutral managed C# API**. Parity is claimed and judged there. `ADR-007` settles what that includes:

- **Domain:** numeric representation (`BEH-301`), array layout and bounds (`BEH-302`), and the classification of which conditions fail, as typed domain failures (`BEH-305`).
- **Adapters, with no legacy-fidelity requirement:** file I/O and serialization (a driven port), text codecs (`BEH-303`), external complex-column order (`BEH-304`), diagnostic channel, ANSI styling, and process exit codes.
- **Dropped:** CLI parsing and wall-clock timing.

Fixtures are still captured from legacy text output, so the **verification harness** parses Fortran-formatted text. That parsing never enters the product.

## Anti-thesis

- A documentation-only ADD exercise that never ships C#.
- **Presenting the fragment as a SciFortran port.** ADR-008 builds three slices; no artifact may imply library coverage the repository does not have.
- **A demonstration that only covers easy arithmetic.** If no slice exercises dependency substitution behind a port, the demonstration does not answer the question a skeptical client will ask.
- Building CLI, plotting, or ASP.NET surfaces that serve neither the API nor the demonstration. `E2` — ADR-006.
- A full SciFortran-on-ASP.NET rewrite treated as the first deliverable.
- Copying Intel-confidential headers or Numerical Recipes source into the target tree.
- Approving redistribution or production readiness from the private POC authorization. `E2` — ASSESSMENT §1, Condition 2.
- Treating the README "experiment" checkout as an approved commercial SciFortran product. `E2` — INT-001; ASSESSMENT §1.
- Treating Python-generated golden files as legacy truth, or promoting provisional tolerances (`1e-6` / `1e-10`) to accepted parity policy. `E1`/`E2`/`E3`/`E4` — INT-006/007; DEF-001; DEF-308.
- Silently fixing help-vs-code or reader anomalies during translation without a recorded disposition. Retiring a contradiction out of scope is a disposition; quietly correcting it is not. `E2`/`E3` — ASSESSMENT RISK-011.
- Claiming parity for any surface beyond the fixtures actually captured. Every module outside the built slices is T3.

## Success signals

- [x] `/plan-migration` produced an ordered slice plan; ADR-008 narrowed it to the built set.
- Each named vertical slice (VS-1 `linspace`, VS-2 `fermi`, VS-3 `MATRIX`) passes its ADD gates with recorded evidence.
- The artifact trail for each built slice is complete and inspectable end to end, with no step taken on assertion alone.
- VS-3 demonstrates an external numeric dependency substituted behind a port, with parity evidence and no vendored source copied.
- Domain code has no compile-time dependency on I/O, CLI, timing, or hosting — verifiable by inspection, which is the claim that the ports could become services.
- `FIX-001` and each later recovered fixture pass under their accepted comparison rules.
- Contradictions are visibly disposed — reproduced, fixed with a row, or retired with evidence — never silently resolved.
- Reserve modules stay marked as reserve, so no artifact implies committed work.

## Decisions of record

**From user answers dated 2026-08-18.** Retained so the reasoning trail survives.

1. **Parity baseline (POC):** probe revision `e586903` and its recorded environment are the parity baseline for retained behaviors. *Status: **in force**, narrowed by ADR-001 (oracle for BEH-001 only) and ADR-005 §1 (planning baseline elsewhere; every other surface T3 until its slice captures fixtures).*
2. ~~**Success is drop-in I/O substitution** on legacy-compatible surfaces.~~ *Status: **superseded 2026-08-19** in favour of ADR-002 §3 / ADR-003. Success is measured at the managed C# API; CLI stream compatibility is neither required nor claimed.*
3. **Defect disposition default:** where help/docs and executable behavior disagree, reproduce the observable behavior; `fix-now`/`fix-later` needs its own owner row. *Status: **in force**, scoped to behavior observable at the managed API. Contradictions outside the product boundary are retired with evidence (ADR-006/007), not silently dropped.*
4. ~~**Preserve stdout diagnostics and `STOP` semantics** as the swap contract.~~ *Status: **superseded 2026-08-19** by ADR-002 §4. `error`/`STOP` becomes a typed domain failure; channel and exit status are adapter concerns.*
5. ~~**No broader product thesis** beyond a POC port for I/O swap.~~ *Status: **superseded 2026-08-19** by ADR-004. Scope limit lifted; the POC framing survives.*

**From owner decisions dated 2026-08-19.**

6. **Process boundary is the managed C# API.** ADR-002 §3/§4, ADR-003.
7. **The CLI surface is retired from build scope**, its catalog kept as recovered evidence. ADR-006.
8. **File I/O, CLI parsing, timing, and console diagnostics are adapters, not domain.** Resolves the ADR-002 §2 versus ADR-005 §3 contradiction. ADR-007.
9. **Build scope is representative vertical slices, not a complete port.** The measure of done is the artifact trail, not library coverage. ADR-008.

## Open purpose questions

These block design or story planning for the affected scope until resolved or explicitly risk-accepted.

- [ ] **VS-3 fixture capture:** `MATRIX` has no T1 evidence. What captures promote it from T3 to an accepted oracle, and what numeric contract ADR (in the shape of ADR-003) governs it? This is the likeliest thing to slip. `E1`/`E5` — ADR-005 §1; ADR-008.
- [ ] **VS-3 dependency route:** which BLAS/LAPACK provider or managed implementation sits behind the numeric port, and how is its behavior shown to match the probe-linked OpenBLAS? `E3`/`E5` — ADR-005 §6; dependency ledger.
- [ ] **Comparison policy per slice:** ADR-003 fixes exact parsed numeric equality for FIX-001. What rule replaces provisional `1e-6`/`1e-10` for VS-2 and VS-3? `E1`/`E2`/`E3`/`E4` — INT-006; GAP-009; DEF-308.
- [ ] **Is an HTTP adapter built?** The services claim currently rests on inspection of the domain's independence. A thin HTTP adapter over one existing port would demonstrate it directly, and is the closest artifact to the client's actual ask. Cost versus evidentiary value is undecided. `E5` — ADR-005 §2; ADR-008.
- [ ] After the POC, is a production thesis (legal clearance, packaging, support) ever required?

Closed: retained surface inventory (ADR-005 §5); process-boundary success criterion (decision 6); the IOTOOLS file-format carve-out, withdrawn by ADR-007; `STOP` exit status, now an adapter concern retired with DEF-309.

## Links

- Related domain model: `docs/DOMAIN.md`
- Related catalog: `docs/modernization/behavior-catalog.md`
- Related plan: `docs/modernization/migration-plan.md`
- Related assessment: `docs/modernization/ASSESSMENT.md`
- Related ADRs: ADR-001–010 (ADR-006/007/008 are the operative scope decisions; ADR-009/010 are Proposed VS-1 design)
- Related design: `README.md`
- Related behaviors: `docs/modernization/behaviors/BEH-001-linspace.md`; `BEH-301-*.md` … `BEH-305-*.md`
- Related requirements: `docs/requirements/REQ-001-linspace.md`
- Related defect ledger: `docs/modernization/defect-ledger.md`
- Supersedes: purpose drafts dated 2026-08-10 and 2026-08-18 (same file); 2026-08-18 decisions 2, 4, and 5

---

*Created: 2026-08-10 | Updated: 2026-08-18, 2026-08-19 (managed-API boundary; CLI retired; adapter classification; demonstration-first scope), 2026-08-20 (`REQ-001`, ADR-009/010) | Plan: `docs/modernization/migration-plan.md`*
