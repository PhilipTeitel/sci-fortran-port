<!--
Purpose artifact contract:
- This file is produced or updated by /define-purpose (modeler in Purpose mode) or /recover-domain (modeler in Legacy recovery mode).
- Save it to the configured purpose path (default `docs/PURPOSE.md`).
- Purpose is canonical for product intent. Requirements, design, domain modeling, stories, reviews, and QA must not contradict it silently.
- Keep this artifact short enough for every later agent and human gate to read. If it becomes a requirements document, it has stopped doing its job.
- Unresolved purpose-level questions are listed under `Open purpose questions` and block downstream work for the affected scope.
-->

# Purpose: SciFortran drop-in I/O port (POC)

**Source material:**
- User purpose answers dated 2026-08-18: port of existing functionality; POC; success = swap migrated application for legacy given the same inputs and outputs
- `docs/modernization/ASSESSMENT.md` §§1, 3, 8–9 (exercise-scoped `go-with-conditions`; not production redistribution approval)
- `docs/modernization/intent-ledger.md` (INT-001 experiment identity; INT-006/007 fidelity comparison provenance)
- `docs/modernization/legacy-map.md` §§1–2, 5–6 (library + CLI numeric surfaces; text interchange)
- `docs/modernization/behaviors/BEH-001-numeric-kind-representation.md`
- `docs/modernization/behaviors/BEH-002-array-layout-bounds.md`
- `docs/modernization/behaviors/BEH-003-numeric-text-formatting.md`
- `docs/modernization/behaviors/BEH-004-complex-column-ordering.md`
- `docs/modernization/flows/BEH-004-fftgf-complex-column-io.md`
- `docs/modernization/behaviors/BEH-005-stop-error-diagnostics.md`
- `docs/modernization/defect-ledger.md` (DEF-001–013 open dispositions)
- `.cursor/workflow.config.yml` (provisional parity knobs; target stack named for exercise context)

**Date:** 2026-08-18
**Status:** Draft

---

## Thesis

This POC exists to **port existing SciFortran-derived functionality** so a migrated implementation can be **swapped for the legacy application for given inputs and outputs**—preserving observable numeric, text, and error contracts rather than redesigning the product.

## The job it does

Primary actors are **callers and pipelines that already use the legacy SciFortran surfaces** (and the **POC operators** who prove substitution). They hire the system to accept the same inputs and produce the same outputs as the legacy application for retained surfaces, so the migrated build can replace the legacy one without changing those I/O contracts. The job matters because the POC’s value is interchangeability, not a new scientific product thesis.

## North-star outcome

For every retained surface in the POC, a caller can feed the **same inputs** to the migrated application and receive **outputs that are substitutable for the legacy outputs** (including numeric representation/layout effects visible at the boundary, text/complex interchange, and observable failure/help termination), judged against the accepted legacy baseline without silently “fixing” legacy quirks that callers would notice.

## Trade-off rule

When goals conflict, optimize for **drop-in I/O substitution against observable legacy behavior** over **convenience, global normalization, host-idiomatic defaults, or documentation-preferred corrections**.

Help text, comments, and “cleaner” codecs lose when they conflict with what the legacy program actually emits or how it terminates for a given input. Host adapters (for example ASP.NET Problem Details) may exist beside the substitutable surface but must not redefine POC success. `E2` user 2026-08-18; `E2`/`E3` — BEH-003/004/005; ASSESSMENT §9.

## Anti-thesis

Tempting but wrong shapes for this effort:

- Treating the README “experiment” checkout as an approved commercial SciFortran product with undocumented feature ambition. `E2` — INT-001; ASSESSMENT §1.
- Approving redistribution or production/reusable-port readiness from the private POC authorization. `E2` — ASSESSMENT §1, Condition 2; user POC framing 2026-08-18.
- “Improving” complex-column order, text formatting, or diagnostics so outputs no longer substitute for legacy for the same inputs. `E2` user 2026-08-18; `E2`/`E3` — BEH-004; GAP-013.
- Inventing a single repository-wide `(Re,Im)` or `(Im,Re)` convention, or a single text codec, that would change some retained surfaces’ I/O. `E2`/`E3` — BEH-004; GAP-013.
- Promoting provisional tolerances (`1e-6` / `1e-10`) or Python-generated “goldens” as accepted product parity policy without tying them to substitutable I/O. `E1`/`E2`/`E3`/`E4` — INT-006/007; BEH-001/003; oracle via ASSESSMENT §7.
- Equating ASP.NET Core hosting semantics with the swap contract (no legacy web/service topology; POC success is I/O substitution). `E3`/`E5` — ASSESSMENT §9; legacy-map §1; user 2026-08-18.

## Success signals

- A retained-surface caller can swap migrated ↔ legacy for agreed input fixtures and observe substitutable outputs. `E2` — user 2026-08-18.
- Purpose and domain artifacts cite evidence grades and do not invent unsupported commercial product scope. `E2`/`E3` — this recovery; ASSESSMENT Condition 9.
- Retained surfaces name kind-8 numerics, array layout/bounds, text codecs, and complex-column order as **I/O contracts to reproduce**, not as assumed C# defaults. `E3` — BEH-001–004; user 2026-08-18.
- Help-vs-code and similar contradictions are disposed toward **observable I/O fidelity** (typically `reproduce-faithfully`) unless an owner explicitly chooses a fix that still preserves agreed swap fixtures. `E2` user 2026-08-18; DEF-001–013.
- Parity claims stay inside the accepted POC baseline and comparison rules for retained surfaces. `E1`/`E5` — ASSESSMENT §§1, 7; resolved Q1 below.

## Resolved purpose decisions

Recorded from user answers dated **2026-08-18** (POC; port existing functionality; swap on inputs/outputs), applied to the assessment baseline:

1. **Parity baseline (POC):** For this POC, the verified operational probe revision **`e586903`** and its recorded probe environment are the **parity baseline** for retained behaviors. Broader “production authority” beyond the POC is **out of scope** and is not claimed. `E1`/`E2` — ASSESSMENT Condition 1; oracle; user 2026-08-18.
2. **Process-boundary success criterion:** POC success is **drop-in I/O substitution** (same inputs → substitutable outputs) on legacy-compatible surfaces (CLI streams/files and/or library call contracts as retained). New host shapes (e.g. HTTP) are not the definition of success unless they expose the same I/O contracts. `E2` — user 2026-08-18; GAP-019/020.
3. **Defect disposition default for I/O contradictions:** Where help/docs and executable I/O disagree, **reproduce the observable I/O** (`reproduce-faithfully` for swap-affecting behavior). Documentation mismatches are not a license to change outputs. Explicit `fix-now` / `fix-later` still requires a separate owner row when a change would break substitution. `E2` — user 2026-08-18; BEH-004; DEF-001–006, DEF-013.
4. **Diagnostics / termination on the swap surface:** For the substitutable CLI/library surface, **preserve observable diagnostic channel and termination behavior** (including stdout-mixed diagnostics and `STOP` semantics as characterized). Remapping to stderr / typed results / Problem Details is allowed only as a **non-parity host adapter**, not as the swap contract. `E2` — user 2026-08-18; BEH-005; GAP-026.
5. **Broader product thesis:** **None authorized** beyond this POC port of existing functionality for I/O swap. `E2` — user 2026-08-18; INT-001; ASSESSMENT §1.

## Open purpose questions

These still block design or story planning for the affected scope until resolved or explicitly risk-accepted.

- [ ] **Retained surface inventory:** Which library modules and CLI utilities are in the POC swap set vs retired (e.g. is `ffcmplx` / non-default `all` in scope)? “Port existing functionality” implies retain what callers need for substitution, but the concrete list is not yet named. `E3`/`E5` — ASSESSMENT Condition 3; legacy-map §5.
- [ ] **Comparison policy detail:** For each retained surface, is substitutable output judged by **exact bytes**, **normalized text**, and/or **parsed numeric** equality—and which absolute/relative/ULP/residual thresholds replace provisional `1e-6` / `1e-10` where exact match is impossible? Direction: whatever preserves drop-in substitution for real consumers. Numbers/rules still TBD. `E1`/`E2`/`E3`/`E4`/`E5` — BEH-001/003; INT-006; DEF-008; user 2026-08-18.
- [ ] **Measured STOP exit status:** What exit status does bare `STOP` produce on the accepted POC runtime, and must that numeric code be part of the swap contract? Channel/termination *shape* is resolved above; host exit-code bytes remain `E5` until captured. `E3`/`E5` — BEH-005; DEF-009.

## Links

- Related requirements: none yet (behavior catalog BEH-001–005 is the recovered contract source for this slice)
- Related domain model: `docs/DOMAIN.md`
- Related assessment: `docs/modernization/ASSESSMENT.md`
- Related defect ledger: `docs/modernization/defect-ledger.md`
- Related behaviors: `docs/modernization/behaviors/BEH-001-*.md` … `BEH-005-*.md`
- Supersedes / superseded by: purpose draft dated 2026-08-10 (same file; updated 2026-08-18)

---

*Created: 2026-08-10 | Updated: 2026-08-18 | Modeled by: modeler in Purpose mode (user POC / swap answers)*
