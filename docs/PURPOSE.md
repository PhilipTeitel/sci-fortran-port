<!--
Purpose artifact contract:
- This file is produced or updated by /define-purpose (modeler in Purpose mode) or /recover-domain (modeler in Legacy recovery mode).
- Save it to the configured purpose path (default `docs/PURPOSE.md`).
- Purpose is canonical for product intent. Requirements, design, domain modeling, stories, reviews, and QA must not contradict it silently.
- Keep this artifact short enough for every later agent and human gate to read. If it becomes a requirements document, it has stopped doing its job.
- Unresolved purpose-level questions are listed under `Open purpose questions` and block downstream work for the affected scope.
-->

# Purpose: SciFortran C# port (POC)

**Source material:**
- Owner correction dated 2026-08-19: port the whole Fortran library surface to usable C#; licensing is irrelevant for this private POC
- User purpose answers dated 2026-08-18: port of existing functionality; POC; success = swap migrated application for legacy given the same inputs and outputs
- `docs/modernization/ASSESSMENT.md` §§1, 3, 8–9 (exercise-scoped `go-with-conditions`; not production redistribution approval)
- Probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`
- ADRs 001–005
- Recovered BEH-001 / FIX-001 (first implementation slice, not the product boundary)
- Recovered cross-cutting contracts BEH-301–305 and flow `BEH-304-fftgf-complex-column-io`
- `docs/modernization/intent-ledger.md` (INT-001 experiment identity; INT-006/007 fidelity comparison provenance)
- `docs/modernization/legacy-map.md` §§1–2, 5–6 (library + CLI numeric surfaces; text interchange)
- `docs/modernization/defect-ledger.md` (DEF-001–004 first slice; DEF-301–313 cross-cutting)

**Date:** 2026-08-19
**Status:** Accepted on scope (whole-library POC); **contested on the success criterion** — see “Unresolved tension” below.

> **Merge note (2026-08-19).** Two purpose revisions were written in parallel and neither saw the other: the 2026-08-18 “drop-in I/O swap” answers recorded locally, and the 2026-08-19 whole-library correction recorded in the cloud branch (ADR-004). Whole-library **scope** is settled. What “success” means at the process boundary is **not**, because ADR-002/003 were decided without the 2026-08-18 answers in view. Both are preserved verbatim below; neither has been silently discarded.

---

## Thesis

This repository exists to **port SciFortran’s retained library functionality to C#** so callers get the same numerical results the legacy library produced at the accepted probe baseline. It is a private proof of concept. It is not a license to redistribute restricted Fortran, and it is not an ASP.NET rewrite of a system that had no web topology.

The 2026-08-18 answers sharpen what “the same results” must mean in practice: a migrated implementation should be **substitutable for the legacy application for given inputs and outputs**, preserving observable numeric, text, and error contracts rather than redesigning them. Whether that substitutability is required at the **CLI/text boundary** or only at the **managed API boundary** is the open tension recorded below.

## The job it does

A numerical-library maintainer needs a host-neutral C# library that reproduces the public `SCIFOR` surface (grids, functions, integration, matrix, FFT, optimization, splines, statistics, Green/Padé/lattice helpers, and I/O ports) plus CLI adapters over those same ports. The job matters because a linspace-only exercise cannot plan code production for the rest of the library.

The actors are **callers and pipelines that already use the legacy SciFortran surfaces**, plus the **POC operators** who prove substitution. They hire the system to accept the same inputs and produce the same outputs as the legacy application for retained surfaces, so the migrated build can replace the legacy one without changing those I/O contracts.

Reproducing those results is not only a matter of per-function arithmetic. The contracts recovered as BEH-301–305 — kind-8 numeric representation, Fortran array layout and bounds, text codecs, complex-column order, and fatal/non-fatal diagnostics — cut across every retained surface and must be treated as contracts rather than assumed C# defaults.

## North-star outcome

A C# caller of the managed port can exercise retained SciFortran behavior and obtain legacy-faithful results, starting with exact `linspace(0,1,5)` and expanding module by module under `/plan-migration`. Surfaces that cannot be built from this checkout (`vfplot`/`DLPLOT`, FFTPACK callees) are retired rather than faked.

For every retained surface, an observer can state its numeric representation, layout/bounds, text/complex interchange, and diagnostic/termination contract with evidence grades, and a caller can feed the **same inputs** and receive **outputs substitutable for the legacy outputs** — judged against the accepted baseline without silently “fixing” legacy quirks that callers would notice.

## Trade-off rule

When goals conflict, optimize for **usable C# with honest parity of retained behavior** over **Fortran ABI compatibility, ASP.NET hosting, or expanding into missing/unexported source**.

Within that, optimize for **fidelity to observable legacy behavior** over **convenience, global normalization, host-idiomatic defaults, or documentation-preferred corrections**. Help text, comments, and “cleaner” codecs lose when they conflict with what the legacy program actually emits or how it terminates for a given input. Host adapters (for example ASP.NET Problem Details) may exist beside the substitutable surface but must not silently redefine success. `E2` user 2026-08-18; `E2`/`E3` — BEH-303/304/305; ASSESSMENT §9.

Do not silently “fix” unexecuted branches. Where recovered contracts are surface-specific and mutually contradictory, record the tension and obtain an owner decision rather than choosing a global winner. Schedule `/document-legacy` and `/refine-feature` for each slice before claiming implementation-ready stories.

## Anti-thesis

- A documentation-only ADD exercise that never ships C#.
- A linspace-only product presented as a SciFortran port.
- A full SciFortran-on-ASP.NET rewrite treated as the first deliverable.
- Equating ASP.NET Core hosting semantics with legacy product intent or with the swap contract; the legacy checkout has no web/service topology. `E3`/`E5` — ASSESSMENT §9; legacy-map §1; user 2026-08-18.
- Copying Intel-confidential headers or Numerical Recipes source into the target tree.
- Treating the README “experiment” checkout as an approved commercial SciFortran product with undocumented feature ambition. `E2` — INT-001; ASSESSMENT §1.
- Approving redistribution or production/reusable-port readiness from the private POC authorization. `E2` — ASSESSMENT §1, Condition 2.
- “Improving” complex-column order, text formatting, or diagnostics so outputs no longer substitute for legacy on the same inputs. `E2` user 2026-08-18; `E2`/`E3` — BEH-304; GAP-013.
- Inventing a single repository-wide `(Re,Im)` or `(Im,Re)` convention, or a single text codec, that would change some retained surfaces’ I/O. `E2`/`E3` — BEH-304; GAP-013; DEF-301–306.
- Treating Python-generated golden files as legacy truth, or promoting provisional tolerances (`1e-6` / `1e-10`) to accepted parity policy. `E1`/`E2`/`E3`/`E4` — INT-006/007; BEH-001/303; DEF-001; DEF-308.
- Silently fixing help-vs-code, unused-`ex`, or matrix-reader anomalies during translation without defect dispositions. `E2`/`E3` — BEH-304; ASSESSMENT RISK-011; DEF-303/307/312.

## Success signals

- [x] `/plan-migration` produces an ordered slice plan covering the retained catalog (`docs/modernization/migration-plan.md`).
- Each slice yields C# behind hexagonal ports, with CLI adapters calling the same use cases.
- FIX-001 (and later recovered T1 fixtures) pass with their accepted comparison rules.
- Retired surfaces are listed as out of product, not half-translated.
- A retained-surface caller can swap migrated ↔ legacy for agreed input fixtures and observe substitutable outputs. `E2` — user 2026-08-18.
- Retained surfaces name kind-8 numerics, array layout/bounds, text codecs, and complex-column order as **I/O contracts to reproduce**, not as assumed C# defaults. `E3` — BEH-301–304.
- Fatal vs non-fatal diagnostics and process termination are distinguished; portable exit codes are not assumed without source evidence. `E3`/`E5` — BEH-305.
- Documented vs coded contradictions appear as **Tensions / conflicts** or open questions, not as silently chosen winners. `E2`/`E3` — flow `BEH-304` §5; ASSESSMENT §9.

## Resolved purpose decisions (2026-08-18)

Recorded from user answers dated **2026-08-18**, and annotated with how the 2026-08-19 correction and ADRs 001–005 interact with each. **The annotations are status, not dispositions**; items marked *in tension* still need an owner decision.

1. **Parity baseline (POC):** the verified operational probe revision **`e586903`** and its recorded environment are the **parity baseline** for retained behaviors. Broader “production authority” is out of scope. `E1`/`E2` — ASSESSMENT Condition 1; oracle; user 2026-08-18.
   *Status: narrowed, not contradicted.* ADR-001 accepts the probe as the parity oracle for BEH-001 only; ADR-005 §1 accepts it as the **planning** baseline for all retained surfaces, with every other surface T3 until its slice captures fixtures.
2. **Process-boundary success criterion:** POC success is **drop-in I/O substitution** (same inputs → substitutable outputs) on legacy-compatible surfaces (CLI streams/files and/or library call contracts as retained). New host shapes (e.g. HTTP) are not the definition of success unless they expose the same I/O contracts. `E2` — user 2026-08-18; GAP-019/020.
   *Status: **in tension** with ADR-002 §3 (first driving adapter is a managed C# API) and ADR-003 (Fortran `es24.17` / list-directed text “is not the product contract”). ADR-002/003 were decided on 2026-08-19 without these answers in view.*
3. **Defect disposition default for I/O contradictions:** where help/docs and executable I/O disagree, **reproduce the observable I/O** (`reproduce-faithfully` for swap-affecting behavior). Documentation mismatches are not a license to change outputs. `fix-now` / `fix-later` still requires a separate owner row when a change would break substitution. `E2` — user 2026-08-18; BEH-304; DEF-301–306, DEF-313.
   *Status: **recorded but unapplied**. Every affected ledger row (DEF-002–004, DEF-301–313) is still `TBD`/open. If this default stands, those rows should be dispositioned accordingly rather than left blank.*
4. **Diagnostics / termination on the swap surface:** for the substitutable CLI/library surface, **preserve observable diagnostic channel and termination behavior** (including stdout-mixed diagnostics and `STOP` semantics as characterized). Remapping to stderr / typed results / Problem Details is allowed only as a **non-parity host adapter**, not as the swap contract. `E2` — user 2026-08-18; BEH-305; GAP-026.
   *Status: **in tension** with ADR-002 §4, which maps `linspace` `STOP`/`error()` to a typed domain failure at the managed port and treats exit codes as an adapter concern.*
5. **Broader product thesis:** **none authorized** beyond this POC port of existing functionality for I/O swap. `E2` — user 2026-08-18; INT-001; ASSESSMENT §1.
   *Status: **superseded** by the 2026-08-19 owner correction and ADR-004, which authorize the whole retained library surface. The POC framing itself survives; only the scope limit was lifted.*

## Unresolved tension: what does “success” mean at the process boundary?

This blocks any story that fixes the shape of the first driving adapter or the treatment of Fortran text output.

- The 2026-08-18 answers define success as **drop-in I/O substitution**, which makes CLI streams, text codecs, stdout diagnostics, and `STOP` semantics part of the product contract.
- ADR-002/003 define the first product surface as a **host-neutral managed API**, explicitly putting Fortran text formatting and codecs outside the slice contract and mapping `STOP` to a typed failure.

These are not reconcilable by wording. An owner must either re-affirm drop-in I/O substitution (which would require revising ADR-002 §3, ADR-003, and ADR-002 §4), or supersede decisions 2 and 4 in favour of the managed-API boundary (which would narrow what “substitutable” is claimed to mean). Until then, treat ADR-002/003 as governing **BEH-001 only**, per their own stated scope.

## Open purpose questions

These block design or story planning for the affected scope until resolved or explicitly risk-accepted.

- [ ] **Process-boundary success criterion** — resolve the tension recorded above (decisions 2 and 4 vs ADR-002/003).
- [ ] **Comparison policy detail:** ADR-003 fixes exact parsed numeric equality for FIX-001. For each **other** retained surface, is substitutable output judged by exact bytes, normalized text, or parsed numeric equality—and which absolute/relative/ULP/residual thresholds replace provisional `1e-6` / `1e-10`? `E1`/`E2`/`E3`/`E4`/`E5` — BEH-303; INT-006; GAP-009; DEF-308.
- [ ] **Measured STOP exit status:** what exit status does bare `STOP` produce on the accepted POC runtime, and must that numeric code be part of the contract? `E3`/`E5` — BEH-305; DEF-309.
- [ ] For complex-column contradictions (especially `fftgf` help vs default writer): is the disposition `reproduce-faithfully`, `fix-now`, or `fix-later`? Decision 3 implies a default; no row records it. `E2`/`E3` — BEH-304; DEF-301/302.
- [ ] Which surfaces are promoted from the ADR-005 planning baseline (T3) to accepted parity evidence, and by what captures? `E1`/`E5` — ADR-001; ADR-005 §1.
- [ ] After the POC library exists, is a production thesis (legal clearance, packaging, support) ever required?
- [ ] Should unexported bundled special-function internals later join the public C# API?
- [ ] Are historical Fortran consumers of `libscifor.a` in scope after the managed API exists?

The **retained surface inventory** question raised by both passes is now closed by ADR-005 §5, which fixes the retained/retired list and retains `ffcmplx`.

## Links

- Related domain model: `docs/DOMAIN.md`
- Related catalog: `docs/modernization/behavior-catalog.md`
- Related plan: `docs/modernization/migration-plan.md`
- Related assessment: `docs/modernization/ASSESSMENT.md`
- Related ADRs: ADR-001–005
- Related behaviors: `docs/modernization/behaviors/BEH-001-linspace.md` (first slice); `BEH-301-*.md` … `BEH-305-*.md` (cross-cutting contracts)
- Related defect ledger: `docs/modernization/defect-ledger.md`
- Supersedes / superseded by: supersedes the purpose drafts dated 2026-08-10 and 2026-08-18 (same file), retaining the 2026-08-18 decisions above

---

*Created: 2026-08-10 | Updated: 2026-08-18 (POC / swap answers), 2026-08-19 (whole-library correction) | Parallel revisions merged: 2026-08-19 | Plan: `/plan-migration`*
