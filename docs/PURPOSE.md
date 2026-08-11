<!--
Purpose artifact contract:
- This file is produced or updated by /define-purpose (modeler in Purpose mode) or /recover-domain (modeler in Legacy recovery mode).
- Save it to the configured purpose path (default `docs/PURPOSE.md`).
- Purpose is canonical for product intent. Requirements, design, domain modeling, stories, reviews, and QA must not contradict it silently.
- Keep this artifact short enough for every later agent and human gate to read. If it becomes a requirements document, it has stopped doing its job.
- Unresolved purpose-level questions are listed under `Open purpose questions` and block downstream work for the affected scope.
-->

# Purpose: SciFortran numeric-contract modernization (framework exercise)

**Source material:**
- `docs/modernization/ASSESSMENT.md` §§1, 3, 8–9 (exercise-scoped `go-with-conditions`; not production redistribution approval)
- `docs/modernization/intent-ledger.md` (INT-001 experiment identity; INT-006/007 fidelity comparison provenance)
- `docs/modernization/legacy-map.md` §§1–2, 5–6 (library + CLI numeric surfaces; text interchange)
- `docs/modernization/behaviors/BEH-001-numeric-kind-representation.md`
- `docs/modernization/behaviors/BEH-002-array-layout-bounds.md`
- `docs/modernization/behaviors/BEH-003-numeric-text-formatting.md`
- `docs/modernization/behaviors/BEH-004-complex-column-ordering.md`
- `docs/modernization/flows/BEH-004-fftgf-complex-column-io.md`
- `docs/modernization/behaviors/BEH-005-stop-error-diagnostics.md`
- `.cursor/workflow.config.yml` (provisional parity knobs; target stack named for exercise context)
- Defect ledger: absent as of 2026-08-10

**Date:** 2026-08-10
**Status:** Draft

---

## Thesis

This effort exists to recover and modernize **observable SciFortran-derived numeric library and CLI contracts**—kinds, array layout/bounds, text codecs, complex-column order, and fatal/non-fatal diagnostics—under a controlled private framework-translation exercise, so agents can judge fidelity of those contracts without inventing a commercial SciFortran product thesis beyond evidence.

## The job it does

Primary actors are **library consumers and CLI/pipeline users** of SciFortran numerics (and, for this port project, the **exercise operators** who characterize and translate a narrow slice). They hire SciFortran-derived surfaces to compute and exchange kind-8 real/complex numeric values with Fortran-shaped arrays and text I/O, and to receive process-terminating or non-terminating diagnostics on failure/help. The job matters because those observable contracts are the only evidenced product of this experimental checkout; production scope, redistribution, and full scientific-product intent are not established.

## North-star outcome

For every retained surface in the authorized exercise slice, an observer can state the **numeric representation, layout/bounds, text/complex interchange, and diagnostic/termination contract** with evidence grades, and a modernized implementation can be judged against those contracts without silently “fixing” unresolved contradictions or overclaiming parity beyond the scoped oracle.

## Trade-off rule

When goals conflict, optimize for **faithful recovery of observable legacy contracts (and explicit tension recording)** over **convenience, global normalization, or host-idiomatic defaults**.

This ordering reflects the assessment and behavior catalog: complex-column order, text formats, tolerances, and `STOP` semantics are surface-specific and sometimes contradictory; choosing one global codec or host-friendly error model without an owner decision would alter some observables and betray the exercise’s fidelity purpose. `E2`/`E3` — BEH-003/004/005; ASSESSMENT §9.

## Anti-thesis

Tempting but wrong shapes for this effort:

- Treating the README “experiment” checkout as an approved commercial SciFortran product with undocumented feature ambition. `E2` — INT-001; ASSESSMENT §1.
- Approving redistribution or production/reusable-port readiness from the private framework-exercise authorization. `E2` — ASSESSMENT §1, Condition 2.
- Inventing a single repository-wide `(Re,Im)` or `(Im,Re)` external convention, or a single text codec, that would necessarily change some surfaces. `E2`/`E3` — BEH-004; GAP-013.
- Promoting provisional tolerances (`1e-6` / `1e-10`) or Python-generated “goldens” as accepted product parity policy. `E1`/`E2`/`E3`/`E4` — INT-006/007; BEH-001/003; oracle via ASSESSMENT §7.
- Silently fixing help-vs-code, unused-`ex`, or matrix-reader anomalies during translation without defect dispositions. `E2`/`E3` — BEH-004; ASSESSMENT RISK-011.
- Equating ASP.NET Core hosting semantics with legacy product intent (no legacy web/service topology). `E3`/`E5` — ASSESSMENT §9; legacy-map §1.

## Success signals

- Purpose and domain artifacts for the numeric/text/error slice cite evidence grades and do not invent unsupported product scope. `E2`/`E3` — this recovery; ASSESSMENT Condition 9.
- Retained surfaces name kind-8 numerics, array layout/bounds, text codecs, and complex-column order as contracts, not as assumed C# defaults. `E3` — BEH-001–004.
- Fatal vs non-fatal diagnostics and process termination are distinguished; portable exit codes are not assumed without source evidence. `E3`/`E5` — BEH-005.
- Documented vs coded contradictions appear as **Tensions / conflicts** or open questions, not as silently chosen winners. `E2`/`E3` — BEH-004 flow §5; ASSESSMENT §9.
- Parity claims stay inside scoped oracle / owner-approved comparison rules for the exercise slice. `E1`/`E5` — ASSESSMENT §§1, 7.

## Open purpose questions

These block design or story planning for the affected scope until resolved or explicitly risk-accepted.

- [ ] Is `e586903` (and its probe environment) the authoritative **production/parity** baseline for retained behaviors, or only the operational probe baseline for the exercise? `E1`/`E5` — ASSESSMENT Condition 1; INT open Qs.
- [ ] Which library modules and CLI utilities are **retained** for the exercise vs retired (including whether `ffcmplx` / non-default builds are in scope)? `E3`/`E5` — ASSESSMENT Condition 3; legacy-map §5.
- [ ] What **target public/process boundary** (managed library, compatibility CLI, HTTP, hybrid) defines success for the exercise first slice? `E3`/`E5` — ASSESSMENT Condition 4; GAP-019/020.
- [ ] Per retained surface: is comparison **exact-byte**, **parsed numeric**, or another owner-approved policy—and which absolute/relative/ULP/residual rules replace provisional `1e-6` / `1e-10`? `E1`/`E2`/`E3`/`E4`/`E5` — BEH-001/003; INT-006; GAP-009.
- [ ] For complex-column contradictions (especially `fftgf` help vs default writer): is the disposition `reproduce-faithfully`, `fix-now`, or `fix-later`? `E2`/`E3` — BEH-004; defect ledger absent.
- [ ] Must fatal diagnostics remain on **stdout** with bare `STOP` semantics for CLI compatibility, or may adapters remap to stderr/typed results/HTTP Problem Details without claiming legacy parity? `E3`/`E5` — BEH-005; GAP-026.
- [ ] Beyond this numeric/text/error slice, what (if any) broader scientific-product thesis is authorized? `E5` — INT-001; ASSESSMENT §1 scope limits.

## Links

- Related requirements: none yet (behavior catalog BEH-001–005 is the recovered contract source for this slice)
- Related domain model: `docs/DOMAIN.md`
- Related assessment: `docs/modernization/ASSESSMENT.md`
- Related behaviors: `docs/modernization/behaviors/BEH-001-*.md` … `BEH-005-*.md`
- Supersedes / superseded by: none

---

*Created: 2026-08-10 | Modeled by: modeler in Legacy recovery mode*
