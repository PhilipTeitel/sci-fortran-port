<!-- Defect ledger contract:
- Every known or suspected legacy defect needs a decision before affected parity criteria can pass.
- Valid decisions: reproduce-faithfully, fix-now, fix-later.
- If a section has no content yet, write `None yet.`.
-->

# Defect Ledger

**Legacy repo:** `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy` (strictly read-only; explicit user override of configured `../scifortran-legacy`); GitHub `PhilipTeitel/scifortran-legacy` at `e586903a26cc50ca8942f20ca3bccbd8814e6252`
**Date:** `2026-08-19` (first-slice rows `2026-08-19`; cross-cutting rows recovered `2026-08-10`)
**Defect policy (project):** `reproduce-then-refactor` — policy preference only; **no row below is decided** until an owner records `reproduce-faithfully`, `fix-now`, or `fix-later`.

**Scope:** Two `/document-legacy` passes are recorded here and use disjoint identifier blocks.

- `DEF-001`–`DEF-004` — first implementation slice (`BEH-001` linspace and adjacent CLI/text surfaces).
- `DEF-301`–`DEF-313` — cross-cutting numeric / text / error contracts (`BEH-301`–`BEH-305`).

Library-wide contradictions remain listed as open when they must not be silently “fixed” later.

---

## 1. Defect decisions

### 1a. First-slice defects (`DEF-001`–`DEF-004`)

| ID | Defect / mismatch | Affected behavior | Evidence | Decision | UAT impact | Backlog / story |
|----|-------------------|-------------------|----------|----------|------------|-----------------|
| DEF-001 | Checked-in `fidelity/golden/linspace-5.txt` is regenerated from a Python formula, not a retained legacy capture, even though it numerically matched the probe | BEH-001 / FIX-001 | `E1 verified / E3 code-derived / E4 inferred` — `docs/modernization/oracle.md:20,101`; ADR-001 | reproduce-faithfully the **probe parsed values**; do not treat the golden file as authority | Parity must use FIX-001, not the golden path | TBD port story |
| DEF-002 | `linspace` declares `array(num)` before checking `num<0`, so negative length may be processor-dependent prior to `error`/`STOP` | BEH-001 error path | `E3 code-derived` — `src/tools_grids.f90:1-7`; unexecuted | TBD | Blocks error-path parity until decided | TBD |
| DEF-003 | CLI program unit is `linsp` while help/NAME is `linspace` | CLI surface (not first slice) | `E3 code-derived` — `numutils/src/linspace.f90:1-13` | TBD | None for managed-API slice | TBD |
| DEF-004 | Fidelity driver prints `es24.17`; CLI prints list-directed `write(*,*)` | Text surfaces (not first slice) | `E3 code-derived` — `fidelity/driver.f90:17`; `numutils/src/linspace.f90:47-49` | TBD | None while parity is parsed managed-API values (ADR-003) | TBD |

Known assessment-era mismatches **outside** both passes (FFT backends, `ZEROS`/`OPTIMIZE`, `logspace` docs, 310 warnings) are **not** given `DEF-NNN` rows here. They must still not be silently corrected if those surfaces are later selected. `E1/E3/E4` — `docs/modernization/ASSESSMENT.md:31`; `docs/modernization/intent-ledger.md:42-50`. Complex-column order was previously listed as an unrecorded mismatch and is now carried explicitly as `DEF-301`–`DEF-306` and `DEF-313`.

### 1b. Cross-cutting numeric / text / error defects (`DEF-301`–`DEF-313`)

| ID | Defect / mismatch | Affected behavior | Evidence | Decision | UAT impact | Backlog / story |
|----|-------------------|-------------------|----------|----------|------------|-----------------|
| DEF-301 | Suspected help/code contradiction: `fftgf` help describes Fortran complex `(re,im)` while the default writer emits `dimag, real` ⇒ external **(Im, Re)** columns. | BEH-304; flow `BEH-304-fftgf-complex-column-io` | `E2 documented` help `numutils/src/fftgf.f90:32-45`; `E3 code-derived` default write `numutils/src/fftgf.f90:97-100,109-112`; no `E1` asymmetric capture | **open/TBD** | Parity cannot treat help text and default stdout as one contract; Phase P blocked for this surface until disposition | TBD |
| DEF-302 | Suspected input/output asymmetry on default `fftgf` path: default read builds `cmplx(rey,imy)` **(Re, Im)** while default write prints **(Im, Re)** — round-trip without `ex` swaps columns. | BEH-304; flow `BEH-304-fftgf-complex-column-io` | `E3 code-derived` `numutils/src/fftgf.f90:70-71,97-100`; help claims bidirectional `ex` (`E2` `:45`); `E5` until asymmetric fixture run | **open/TBD** | Round-trip and codec acceptance for CLI complex streams blocked | TBD |
| DEF-303 | Suspected help/code contradiction: `ffcmplx` documents `ex` to swap Im/Re vs Re/Im column order, but `ex` is never referenced after parse. | BEH-304 | `E2 documented` `numutils/src/ffcmplx.f90:23-31`; `E3 code-derived` parse-only `numutils/src/ffcmplx.f90:39-50` | **open/TBD** | Cannot claim documented `ex` behavior; utility may be dead help, broken feature, or unsupported | TBD |
| DEF-304 | Suspected call-site mismatch: `ffcmplx` uses `sread(fin,Gread,wm)` while sibling `pade` uses `sread(fin,wm,gm)`; no inspected `SLREAD` generic matches `(char, complex(:,:), real(:))`. | BEH-304 | `E3 code-derived` `numutils/src/ffcmplx.f90:50`; `numutils/src/pade.f90:59`; `src/SLREAD.f90:13-21`; resolve/build outcome `E5 unknown` (utility omitted from default `all`) | **open/TBD** | Unknown whether utility builds or what columns it would load; GAP-019/020 adjacency | TBD |
| DEF-305 | Suspected contract split (not necessarily a bug): `SLREAD`/`SLPLOT` integer-X complex paths use external **(Re, Im)** while real-X complex paths use **(Im, Re)**. | BEH-304 (codec adjacency BEH-303) | `E3 code-derived` IC read/write `src/slread_sread_V.f90:108-134`, `src/slplot_splot_V.f90:130-146`; RC read/write `src/slread_sread_V.f90:246-272`, `src/slplot_splot_V.f90:285-301`; GAP-013 | **open/TBD** | A single global `(Re,Im)`/`(Im,Re)` codec would alter some observable overload | TBD |
| DEF-306 | Suspected surface inconsistency: `txtfy`/`c_to_ch` always formats complex as `"(re,im)"` while several file/CLI writers use **(Im, Re)** columns. | BEH-304; BEH-303 | `E3 code-derived` `src/COMVARS.f90:275-283` vs `src/slplot_splot_V.f90:285-301` / `numutils/src/fftgf.f90:97-100` | **open/TBD** | Diagnostic strings must not be assumed equal to file/CLI column order | TBD |
| DEF-307 | Suspected latent defects in matrix complex readers: `sreadM_IC`/`sreadM_RC` else-branches appear to read `imY` without allocating it; formatted `Y2` branches write `imY(2)` twice instead of `reY(2)`. | BEH-304 | `E3 code-derived` `src/slread_sread_M.f90:82-99,87,190-207,195`; reachability/runtime outcome `E5 unknown` | **open/TBD** | Shared IOTOOLS matrix complex codec may be unsafe or unreachable; parity for matrix complex I/O blocked | TBD |
| DEF-308 | Unaccepted comparison-policy tension (not automatically a legacy bug): workflow relative/absolute `1e-6` vs fidelity script absolute `1e-10` vs probe cross-build exact parsed equality. | BEH-301; BEH-303 (oracle comparison; INT-006) | `E2 documented` `.cursor/workflow.config.yml:44-47`; `E3 code-derived` `scripts/fidelity.sh:11`; `E1 verified` probe exact equality / oracle §6; INT-006 | **open/TBD** | Pass/fail criteria for Phase P can change materially; no accepted parity tolerance yet | TBD |
| DEF-309 | Suspected exit-status inconsistency: most fatals use bare `STOP` (exit code unspecified in source) while fidelity driver uses `stop 1` on one I/O failure. | BEH-305 | `E3 code-derived` `src/COMVARS.f90:208`; `fidelity/driver.f90:45-46`; bare-`STOP` host mapping `E5 unknown` (GAP-026) | **open/TBD** | Host/CLI exit-code parity and ASP.NET error mapping blocked until disposition | TBD |
| DEF-310 | Suspected diagnostics/data channel mixing: `error`/`warning`/`msg`/help write to Fortran unit `*` (stdout), same channel as numeric CLI data. | BEH-305 (CLI adjacency BEH-303) | `E3 code-derived` `src/COMVARS.f90:201-247`; `src/PARSECMD.f90:50-57`; common Unix stderr convention conflict is interpretive (`E4`/`E5`); GAP-020/026 | **open/TBD** | Cannot assume stderr separation or Problem Details mapping without decision | TBD |
| DEF-311 | Suspected comment/code mismatch in diagnostic formatting: `r8_to_s_left` comment mentions G14.6 while the write uses `g16.9`. | BEH-303 | `E3 code-derived` `src/COMVARS.f90:495-521` | **open/TBD** | Diagnostic string width/precision contract unclear for `txtfy` consumers | TBD |
| DEF-312 | Suspected inert CLI control: `fftgf` parses `STRIDE` but never references it in the inspected program body. | BEH-304 (layout adjacency BEH-302) | `E3 code-derived` `numutils/src/fftgf.f90:46,56` vs absence of later uses in same file; effect `E5 unknown` | **open/TBD** | Documented option may be dead help, missing feature, or out of retained scope | TBD |
| DEF-313 | Suspected help/reader tension: `fftgf` help says `tau2iw` needs real input while the read path still consumes two columns into complex. | BEH-304 | `E2 documented` `numutils/src/fftgf.f90:32-33`; `E3 code-derived` read `numutils/src/fftgf.f90:70-71,165-177` | **open/TBD** | Accepted file shape / arity for `tau2iw` unknown | TBD |

## 2. Reproduce faithfully

| DEF ID | Expected port behavior | Parity fixture | Rationale |
|--------|------------------------|----------------|-----------|
| DEF-001 | Return exact FIX-001 samples | `FIX-001` | Owner accepted probe values, not the golden-file provenance |

No `reproduce-faithfully` decision is recorded for any `DEF-1xx` row. For any future `reproduce-faithfully` disposition there, expected port behavior and fixture IDs remain **TBD** until that decision.

## 3. Fix now

| DEF ID | Corrected expectation | Acceptance criterion | Approval source |
|--------|-----------------------|----------------------|-----------------|
| None yet. | — | — | No `fix-now` decision recorded. |

## 4. Fix later

| DEF ID | Deferred backlog item | Why deferred | Guardrail |
|--------|-----------------------|--------------|-----------|
| None yet. | — | — | No `fix-later` decision recorded. When a `fix-later` disposition is chosen, create/link a backlog item here and keep parity guards on the current observable until the fix ships. |

## 5. Open defect decisions

Owner must choose `reproduce-faithfully`, `fix-now`, or `fix-later` for each:

- [ ] **DEF-002** — negative `num` sizing/`STOP` on the `linspace` error path
- [ ] **DEF-003** — only if the CLI becomes in scope
- [ ] **DEF-004** — only if Fortran text compatibility becomes in scope
- [ ] **DEF-301** — `fftgf` help `(re,im)` vs default writer `(Im,Re)`
- [ ] **DEF-302** — `fftgf` default input `(Re,Im)` vs default output `(Im,Re)` asymmetry
- [ ] **DEF-303** — `ffcmplx` unused `ex` despite help
- [ ] **DEF-304** — `ffcmplx` `sread(fin,Gread,wm)` resolve/argument-order anomaly
- [ ] **DEF-305** — IOTOOLS IC `(Re,Im)` vs RC `(Im,Re)` overload split (accept as per-surface contract vs unify)
- [ ] **DEF-306** — `txtfy` `(re,im)` vs file/CLI `(im,re)` writers
- [ ] **DEF-307** — `sreadM_*` unallocated/`imY` / duplicate `imY(2)` anomalies
- [ ] **DEF-308** — Accepted numeric comparison policy: `1e-6` vs `1e-10` vs exact (per retained surface)
- [ ] **DEF-309** — Bare `STOP` vs `stop 1` exit-status contract
- [ ] **DEF-310** — Fatal/help diagnostics on stdout mixed with data
- [ ] **DEF-311** — `r8_to_s_left` comment `G14.6` vs code `g16.9`
- [ ] **DEF-312** — `fftgf` unused `STRIDE`
- [ ] **DEF-313** — `fftgf` `tau2iw` help (real input) vs two-column read

**Policy note:** Project `defectPolicy: reproduce-then-refactor` suggests a default preference toward faithful reproduction before later cleanup, but **does not auto-fill** any Decision column. Mismatches without decisions block affected Phase P parity criteria.

### Out of scope for both passes (not assigned DEF IDs here)

- `logspace` documentation mismatch (named in assessment). It does not intersect the numeric/text/error *codec* contracts of BEH-301–305 beyond ordinary grid CLI formatting covered by BEH-303; the function itself is cataloged as BEH-002, so record a DEF row there if the mismatch is confirmed when that slice is scheduled.
- `ZEROS`/`OPTIMIZE` facade compatibility, FFT backend sign/normalization math, square-lattice denominator reversal, untriaged compiler-warning surfaces — record when those behaviors are cataloged.

## 6. Links

- Behavior catalog: `docs/modernization/behavior-catalog.md`
- Per-function behaviors: `docs/modernization/behaviors/BEH-001-linspace.md` … `BEH-004-deriv.md`
- Cross-cutting contracts: `docs/modernization/behaviors/BEH-301-numeric-kind-representation.md` … `BEH-305-stop-error-diagnostics.md`
- Flow (BEH-001): `docs/modernization/flows/BEH-001-linspace.md`
- Flow (BEH-304): `docs/modernization/flows/BEH-304-fftgf-complex-column-io.md`
- Fixture: `docs/modernization/fixtures/FIX-001-linspace-5.md`
- Oracle: `docs/modernization/oracle.md`
- Intent ledger: `docs/modernization/intent-ledger.md` (INT-006 tolerance; open question on `1e-10` vs `1e-6`)
- Translation gaps: GAP-007, GAP-013, GAP-019, GAP-020, GAP-026
- Assessment: `docs/modernization/ASSESSMENT.md` §1/§9 (complex-column + tolerance stops)
- Migration plan: `docs/modernization/migration-plan.md`
- Project defect policy: `.cursor/workflow.config.yml` → `defectPolicy: reproduce-then-refactor`

### Tensions / conflicts

- Help text, readers, writers, and diagnostics disagree on complex-column order across `fftgf`, `ffcmplx`, IOTOOLS overloads, and `txtfy` (DEF-301–306, DEF-313). `E2`/`E3`/`E5`.
- Matrix complex readers show source anomalies that may be latent defects or unreachable (DEF-307). `E3`/`E5`.
- Three comparison regimes coexist without an accepted parity rule (DEF-308); the first slice separately accepted probe parsed values over the checked-in golden file (DEF-001). `E1`/`E2`/`E3`/`E4`.
- Termination and diagnostic channel contracts are non-uniform and largely unverified on fatal paths (DEF-309–310). `E3`/`E5`.
- Apart from DEF-001, no owner disposition yet; under hard rules, none of these may be silently “fixed” or treated as intentional bugs during the port.

*Created: 2026-08-10 (cross-cutting pass) | 2026-08-19 (first-slice pass) | Ledgers merged: 2026-08-19*
