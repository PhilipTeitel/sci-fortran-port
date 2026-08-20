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
- Owner decision dated 2026-08-19: the process boundary is the **managed C# API** per ADR-002/003; this supersedes the 2026-08-18 drop-in I/O swap criterion
- User purpose answers dated 2026-08-18: port of existing functionality; POC (scope and success criterion since revised — see below)
- `docs/modernization/ASSESSMENT.md` §§1, 3, 8–9 (exercise-scoped `go-with-conditions`; not production redistribution approval)
- Probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`
- ADRs 001–005
- Recovered BEH-001 / FIX-001 (first implementation slice, not the product boundary)
- Recovered cross-cutting contracts BEH-301–305 and flow `BEH-304-fftgf-complex-column-io`
- `docs/modernization/intent-ledger.md` (INT-001 experiment identity; INT-006/007 fidelity comparison provenance)
- `docs/modernization/legacy-map.md` §§1–2, 5–6 (library + CLI numeric surfaces; text interchange)
- `docs/modernization/defect-ledger.md` (DEF-001–004 first slice; DEF-301–313 cross-cutting)

**Date:** 2026-08-19
**Status:** Accepted (whole-library POC; managed-API process boundary)

---

## Thesis

This repository exists to **port SciFortran’s retained library functionality to C#** so callers get the same numerical results the legacy library produced at the accepted probe baseline. It is a private proof of concept. It is not a license to redistribute restricted Fortran, and it is not an ASP.NET rewrite of a system that had no web topology.

The product is the **host-neutral managed C# API**. Parity is claimed and judged at that boundary. CLI utilities are driving adapters over the same ports; Fortran command parsing, stdout formatting, ANSI styling, and process exit codes are adapter work and are not the product contract. `E2` — ADR-002 §3; ADR-003; ADR-005 §2; owner decision 2026-08-19.

## The job it does

A numerical-library maintainer needs a host-neutral C# library that reproduces the public `SCIFOR` surface (grids, functions, integration, matrix, FFT, optimization, splines, statistics, Green/Padé/lattice helpers, and the retained IOTOOLS file helpers) plus CLI adapters over those same ports. The job matters because a linspace-only exercise cannot plan code production for the rest of the library.

The actors are **C# callers of the managed port** and the **POC operators** who verify fidelity against the accepted baseline. They hire the library to compute what the legacy library computed, for the same arguments, without inheriting Fortran hosting or text-stream conventions they did not ask for.

Reproducing those results is not only a matter of per-function arithmetic. The contracts recovered as BEH-301–305 — kind-8 numeric representation, Fortran array layout and bounds, text codecs, complex-column order, and fatal/non-fatal diagnostics — cut across every retained surface. The managed-API boundary decides **which of them bind the product**; see “Process boundary” below.

## North-star outcome

A C# caller of the managed port can exercise retained SciFortran behavior and obtain legacy-faithful results, starting with exact `linspace(0,1,5)` and expanding module by module under `/plan-migration`. Surfaces that cannot be built from this checkout (`vfplot`/`DLPLOT`, FFTPACK callees) are retired rather than faked.

For every retained surface, an observer can state its numeric representation, layout/bounds, and failure classification with evidence grades, and can compare managed results against the accepted baseline under an approved comparison rule — without silently “fixing” legacy behavior that callers would notice.

## Trade-off rule

When goals conflict, optimize for **usable C# with honest parity of retained behavior at the managed API** over **Fortran ABI compatibility, CLI stream compatibility, ASP.NET hosting, or expanding into missing/unexported source**.

Within the product boundary, optimize for **fidelity to observable legacy behavior** over **convenience, global normalization, host-idiomatic defaults, or documentation-preferred corrections**. Help text and comments lose when they conflict with what the legacy code actually computes. Outside the product boundary, adapters may present results idiomatically, but must not **claim** legacy parity that the managed boundary has not established. `E2`/`E3` — BEH-303/304/305; ASSESSMENT §9.

Do not silently “fix” unexecuted branches or recorded contradictions. Where recovered contracts are surface-specific and mutually contradictory, record the tension and obtain an owner decision rather than choosing a global winner. Schedule `/document-legacy` and `/refine-feature` for each slice before claiming implementation-ready stories.

## Process boundary: what the managed-API decision binds

Recorded from the owner decision of **2026-08-19** in favour of ADR-002/003. This is the operative reading of the recovered cross-cutting contracts.

| Contract | Status under the managed-API boundary |
|----------|---------------------------------------|
| BEH-301 numeric kind / representation | **Product contract.** `real(8)`/`complex(8)` map to binary64 at the API (ADR-003). |
| BEH-302 array layout and bounds | **Product contract.** Index base, shape, and non-default lower bounds are part of the call contract. |
| BEH-305 failure classification | **Product contract** for *which* conditions fail: `error`/`STOP` becomes a typed domain failure at the port (ADR-002 §4). |
| BEH-305 diagnostic channel, ANSI styling, exit codes | **Adapter concern.** Not parity-bearing; stdout/stderr and exit status are host decisions. |
| BEH-303 text codecs, BEH-304 complex-column order — *CLI stdout* | **Adapter concern.** Fortran list-directed and `es24.17` output is not the product contract. |
| BEH-303 / BEH-304 — *retained IOTOOLS `splot`/`sread` helpers* | **Still product contract.** ADR-005 §3 retains IOTOOLS as library modules, and those procedures exist to read and write text files, so their formats remain observable library behavior. See the open question below. |

The last row is the one place where “text is not the product contract” does **not** apply. ADR-003 scoped that statement to the BEH-001 managed-API slice; it should not be read library-wide without a decision at the IOTOOLS slice.

## Anti-thesis

- A documentation-only ADD exercise that never ships C#.
- A linspace-only product presented as a SciFortran port.
- A full SciFortran-on-ASP.NET rewrite treated as the first deliverable.
- Equating ASP.NET Core hosting semantics with legacy product intent; the legacy checkout has no web/service topology. `E3`/`E5` — ASSESSMENT §9; legacy-map §1.
- Copying Intel-confidential headers or Numerical Recipes source into the target tree.
- Treating the README “experiment” checkout as an approved commercial SciFortran product with undocumented feature ambition. `E2` — INT-001; ASSESSMENT §1.
- Approving redistribution or production/reusable-port readiness from the private POC authorization. `E2` — ASSESSMENT §1, Condition 2.
- **Advertising CLI drop-in compatibility.** The managed-API boundary does not establish it, and no fixture demonstrates it. A CLI adapter may be built, but must not claim byte-compatible legacy streams without its own captures and dispositions. `E2` — owner decision 2026-08-19; GAP-019/020.
- Inventing a single repository-wide `(Re,Im)` or `(Im,Re)` convention, or a single text codec, that would change some retained IOTOOLS surfaces’ file formats. `E2`/`E3` — BEH-304; GAP-013; DEF-301–306.
- Treating Python-generated golden files as legacy truth, or promoting provisional tolerances (`1e-6` / `1e-10`) to accepted parity policy. `E1`/`E2`/`E3`/`E4` — INT-006/007; BEH-001/303; DEF-001; DEF-308.
- Silently fixing help-vs-code, unused-`ex`, or matrix-reader anomalies during translation without defect dispositions. Moving a contradiction out of the product boundary defers it; it does not resolve it. `E2`/`E3` — BEH-304; ASSESSMENT RISK-011; DEF-303/307/312.

## Success signals

- [x] `/plan-migration` produces an ordered slice plan covering the retained catalog (`docs/modernization/migration-plan.md`).
- Each slice yields C# behind hexagonal ports, with CLI adapters calling the same use cases.
- FIX-001 (and later recovered T1 fixtures) pass with their accepted comparison rules.
- Retired surfaces are listed as out of product, not half-translated.
- A C# caller obtains legacy-faithful values from the managed port for agreed fixtures, judged by the approved per-surface comparison rule. `E2` — ADR-003; owner decision 2026-08-19.
- Retained surfaces name kind-8 numerics and array layout/bounds as **API contracts to reproduce**, not as assumed C# defaults. `E3` — BEH-301/302.
- Fatal vs non-fatal conditions are distinguished as typed domain failures; diagnostic channel and exit codes are stated as adapter choices, not parity claims. `E3`/`E5` — BEH-305; ADR-002 §4.
- Contradictions deferred outside the product boundary stay recorded as open DEF rows rather than disappearing. `E2`/`E3` — flow `BEH-304` §5; ASSESSMENT §9.

## Purpose decisions of record

Decisions 1–5 were recorded from user answers dated **2026-08-18**. Decisions 2, 4, and 5 have since been superseded by the 2026-08-19 owner corrections. All are retained here so the reasoning trail survives.

1. **Parity baseline (POC):** the verified operational probe revision **`e586903`** and its recorded environment are the **parity baseline** for retained behaviors. Broader “production authority” is out of scope. `E1`/`E2` — ASSESSMENT Condition 1; oracle; user 2026-08-18.
   *Status: **in force**, narrowed by ADR-001 (parity oracle for BEH-001 only) and ADR-005 §1 (planning baseline for all retained surfaces; every other surface T3 until its slice captures fixtures).*
2. ~~**Process-boundary success criterion:** POC success is drop-in I/O substitution (same inputs → substitutable outputs) on legacy-compatible surfaces (CLI streams/files and/or library call contracts as retained).~~ `E2` — user 2026-08-18; GAP-019/020.
   *Status: **superseded 2026-08-19** by owner decision in favour of ADR-002 §3 and ADR-003. Success is measured at the managed C# API. CLI stream compatibility is neither required nor claimed.*
3. **Defect disposition default for contradictions:** where help/docs and executable behavior disagree, **reproduce the observable behavior** (`reproduce-faithfully`). Documentation mismatches are not a license to change results. `fix-now` / `fix-later` still requires a separate owner row. `E2` — user 2026-08-18; BEH-304; DEF-301–306, DEF-313.
   *Status: **in force, re-scoped**. The default now governs behavior observable at the managed API. Contradictions that fall outside the product boundary (CLI stdout codecs, diagnostic channel, exit codes) are **deferred, not dispositioned**: their DEF rows stay open and must be settled before any adapter claims legacy compatibility. No ledger row has yet been dispositioned under this default.*
4. ~~**Diagnostics / termination on the swap surface:** preserve observable diagnostic channel and termination behavior, including stdout-mixed diagnostics and `STOP` semantics; remapping allowed only as a non-parity host adapter.~~ `E2` — user 2026-08-18; BEH-305; GAP-026.
   *Status: **superseded 2026-08-19** by owner decision in favour of ADR-002 §4. `error`/`STOP` maps to a typed domain failure at the managed port; channel and exit status are adapter concerns. The recovered BEH-305 characterization remains the evidence base if a compatibility adapter is later specified.*
5. ~~**Broader product thesis:** none authorized beyond a POC port of existing functionality for I/O swap.~~ `E2` — user 2026-08-18; INT-001; ASSESSMENT §1.
   *Status: **superseded 2026-08-19** by the owner correction and ADR-004, which authorize the whole retained library surface. The POC framing itself survives; only the scope limit was lifted.*

## Open purpose questions

These block design or story planning for the affected scope until resolved or explicitly risk-accepted.

- [ ] **IOTOOLS file-format contract:** for the retained `splot`/`sread` helpers, must the managed port reproduce legacy file bytes (delimiters, `es24.17`/list-directed forms, complex-column order per overload), or only round-trip consistently within the port? ADR-003’s “codecs out of scope” was scoped to BEH-001 and must not be extended library-wide by default. `E2`/`E3`/`E5` — BEH-303/304; DEF-301–306, DEF-313; ADR-005 §3.
- [ ] **Comparison policy detail:** ADR-003 fixes exact parsed numeric equality for FIX-001. For each **other** retained surface, which absolute/relative/ULP/residual rule replaces provisional `1e-6` / `1e-10`? Byte and text comparison are off the table for computational surfaces under the managed-API boundary. `E1`/`E2`/`E3`/`E4`/`E5` — BEH-303; INT-006; GAP-009; DEF-308.
- [ ] Which surfaces are promoted from the ADR-005 planning baseline (T3) to accepted parity evidence, and by what captures? `E1`/`E5` — ADR-001; ADR-005 §1.
- [ ] Do the deferred CLI-scope contradictions (DEF-301–306, DEF-309, DEF-310, DEF-312, DEF-313) need dispositions before a CLI adapter is built, or is the adapter simply declared non-parity? `E2`/`E3` — owner decision 2026-08-19.
- [ ] After the POC library exists, is a production thesis (legal clearance, packaging, support) ever required?
- [ ] Should unexported bundled special-function internals later join the public C# API?
- [ ] Are historical Fortran consumers of `libscifor.a` in scope after the managed API exists? (ADR-005 §2 marks the Fortran ABI **not retained**; this asks whether that ever needs revisiting.)

Closed: the **retained surface inventory** question is settled by ADR-005 §5. The **process-boundary success criterion** is settled by the 2026-08-19 owner decision recorded above. The **measured `STOP` exit status** question is no longer purpose-blocking, since exit codes are adapter concerns; it survives as DEF-309 for any future compatibility adapter.

## Links

- Related domain model: `docs/DOMAIN.md`
- Related catalog: `docs/modernization/behavior-catalog.md`
- Related plan: `docs/modernization/migration-plan.md`
- Related assessment: `docs/modernization/ASSESSMENT.md`
- Related ADRs: ADR-001–005 (ADR-002 §3/§4 and ADR-003 are the operative boundary decisions)
- Related behaviors: `docs/modernization/behaviors/BEH-001-linspace.md` (first slice); `BEH-301-*.md` … `BEH-305-*.md` (cross-cutting contracts)
- Related defect ledger: `docs/modernization/defect-ledger.md`
- Supersedes / superseded by: supersedes the purpose drafts dated 2026-08-10 and 2026-08-18 (same file); decisions 2, 4, and 5 of 2026-08-18 are superseded above

---

*Created: 2026-08-10 | Updated: 2026-08-18 (POC / swap answers), 2026-08-19 (whole-library correction; managed-API boundary accepted) | Plan: `/plan-migration`*
