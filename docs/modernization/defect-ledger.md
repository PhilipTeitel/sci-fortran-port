<!-- Defect ledger contract:
- Every known or suspected legacy defect needs a decision before affected parity criteria can pass.
- Valid decisions: reproduce-faithfully, fix-now, fix-later.
- If a section has no content yet, write `None yet.`.
-->

# Defect Ledger

**Legacy repo:** `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy` (strictly read-only; explicit user override of configured `../scifortran-legacy`)
**Date:** `2026-08-10`
**Defect policy (project):** `reproduce-then-refactor` — policy preference only; **no row below is decided** until an owner records `reproduce-faithfully`, `fix-now`, or `fix-later`.
**Slice scope:** Numeric / text / error contracts (`BEH-101`–`BEH-105`) surfaced during `/document-legacy`. Out-of-slice items (e.g. `logspace` documentation mismatch, `ZEROS`/`OPTIMIZE`, FFT backend math) are noted only as deferred/out-of-scope unless they intersect this slice.

---

## 1. Defect decisions

| ID | Defect / mismatch | Affected behavior | Evidence | Decision | UAT impact | Backlog / story |
|----|-------------------|-------------------|----------|----------|------------|-----------------|
| DEF-101 | Suspected help/code contradiction: `fftgf` help describes Fortran complex `(re,im)` while the default writer emits `dimag, real` ⇒ external **(Im, Re)** columns. | BEH-104; flow `BEH-104-fftgf-complex-column-io` | `E2 documented` help `numutils/src/fftgf.f90:32-45`; `E3 code-derived` default write `numutils/src/fftgf.f90:97-100,109-112`; no `E1` asymmetric capture | **open/TBD** | Parity cannot treat help text and default stdout as one contract; Phase P blocked for this surface until disposition | TBD |
| DEF-102 | Suspected input/output asymmetry on default `fftgf` path: default read builds `cmplx(rey,imy)` **(Re, Im)** while default write prints **(Im, Re)** — round-trip without `ex` swaps columns. | BEH-104; flow `BEH-104-fftgf-complex-column-io` | `E3 code-derived` `numutils/src/fftgf.f90:70-71,97-100`; help claims bidirectional `ex` (`E2` `:45`); `E5` until asymmetric fixture run | **open/TBD** | Round-trip and codec acceptance for CLI complex streams blocked | TBD |
| DEF-103 | Suspected help/code contradiction: `ffcmplx` documents `ex` to swap Im/Re vs Re/Im column order, but `ex` is never referenced after parse. | BEH-104 | `E2 documented` `numutils/src/ffcmplx.f90:23-31`; `E3 code-derived` parse-only `numutils/src/ffcmplx.f90:39-50` | **open/TBD** | Cannot claim documented `ex` behavior; utility may be dead help, broken feature, or unsupported | TBD |
| DEF-104 | Suspected call-site mismatch: `ffcmplx` uses `sread(fin,Gread,wm)` while sibling `pade` uses `sread(fin,wm,gm)`; no inspected `SLREAD` generic matches `(char, complex(:,:), real(:))`. | BEH-104 | `E3 code-derived` `numutils/src/ffcmplx.f90:50`; `numutils/src/pade.f90:59`; `src/SLREAD.f90:13-21`; resolve/build outcome `E5 unknown` (utility omitted from default `all`) | **open/TBD** | Unknown whether utility builds or what columns it would load; GAP-019/020 adjacency | TBD |
| DEF-105 | Suspected contract split (not necessarily a bug): `SLREAD`/`SLPLOT` integer-X complex paths use external **(Re, Im)** while real-X complex paths use **(Im, Re)**. | BEH-104 (codec adjacency BEH-103) | `E3 code-derived` IC read/write `src/slread_sread_V.f90:108-134`, `src/slplot_splot_V.f90:130-146`; RC read/write `src/slread_sread_V.f90:246-272`, `src/slplot_splot_V.f90:285-301`; GAP-013 | **open/TBD** | A single global `(Re,Im)`/`(Im,Re)` codec would alter some observable overload | TBD |
| DEF-106 | Suspected surface inconsistency: `txtfy`/`c_to_ch` always formats complex as `"(re,im)"` while several file/CLI writers use **(Im, Re)** columns. | BEH-104; BEH-103 | `E3 code-derived` `src/COMVARS.f90:275-283` vs `src/slplot_splot_V.f90:285-301` / `numutils/src/fftgf.f90:97-100` | **open/TBD** | Diagnostic strings must not be assumed equal to file/CLI column order | TBD |
| DEF-107 | Suspected latent defects in matrix complex readers: `sreadM_IC`/`sreadM_RC` else-branches appear to read `imY` without allocating it; formatted `Y2` branches write `imY(2)` twice instead of `reY(2)`. | BEH-104 | `E3 code-derived` `src/slread_sread_M.f90:82-99,87,190-207,195`; reachability/runtime outcome `E5 unknown` | **open/TBD** | Shared IOTOOLS matrix complex codec may be unsafe or unreachable; parity for matrix complex I/O blocked | TBD |
| DEF-108 | Unaccepted comparison-policy tension (not automatically a legacy bug): workflow relative/absolute `1e-6` vs fidelity script absolute `1e-10` vs probe cross-build exact parsed equality. | BEH-101; BEH-103 (oracle comparison; INT-006) | `E2 documented` `.cursor/workflow.config.yml:44-47`; `E3 code-derived` `scripts/fidelity.sh:11`; `E1 verified` probe exact equality / oracle §6; INT-006 | **open/TBD** | Pass/fail criteria for Phase P can change materially; no accepted parity tolerance yet | TBD |
| DEF-109 | Suspected exit-status inconsistency: most fatals use bare `STOP` (exit code unspecified in source) while fidelity driver uses `stop 1` on one I/O failure. | BEH-105 | `E3 code-derived` `src/COMVARS.f90:208`; `fidelity/driver.f90:45-46`; bare-`STOP` host mapping `E5 unknown` (GAP-026) | **open/TBD** | Host/CLI exit-code parity and ASP.NET error mapping blocked until disposition | TBD |
| DEF-110 | Suspected diagnostics/data channel mixing: `error`/`warning`/`msg`/help write to Fortran unit `*` (stdout), same channel as numeric CLI data. | BEH-105 (CLI adjacency BEH-103) | `E3 code-derived` `src/COMVARS.f90:201-247`; `src/PARSECMD.f90:50-57`; common Unix stderr convention conflict is interpretive (`E4`/`E5`); GAP-020/026 | **open/TBD** | Cannot assume stderr separation or Problem Details mapping without decision | TBD |
| DEF-111 | Suspected comment/code mismatch in diagnostic formatting: `r8_to_s_left` comment mentions G14.6 while the write uses `g16.9`. | BEH-103 | `E3 code-derived` `src/COMVARS.f90:495-521` | **open/TBD** | Diagnostic string width/precision contract unclear for `txtfy` consumers | TBD |
| DEF-112 | Suspected inert CLI control: `fftgf` parses `STRIDE` but never references it in the inspected program body. | BEH-104 (layout adjacency BEH-102) | `E3 code-derived` `numutils/src/fftgf.f90:46,56` vs absence of later uses in same file; effect `E5 unknown` | **open/TBD** | Documented option may be dead help, missing feature, or out of retained scope | TBD |
| DEF-113 | Suspected help/reader tension: `fftgf` help says `tau2iw` needs real input while the read path still consumes two columns into complex. | BEH-104 | `E2 documented` `numutils/src/fftgf.f90:32-33`; `E3 code-derived` read `numutils/src/fftgf.f90:70-71,165-177` | **open/TBD** | Accepted file shape / arity for `tau2iw` unknown | TBD |

## 2. Reproduce faithfully

| DEF ID | Expected port behavior | Parity fixture | Rationale |
|--------|------------------------|----------------|-----------|
| None yet. | — | — | No `reproduce-faithfully` decision recorded. For any future `reproduce-faithfully` disposition, expected port behavior and fixture IDs remain **TBD** until that decision. |

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

- [ ] **DEF-101** — `fftgf` help `(re,im)` vs default writer `(Im,Re)`
- [ ] **DEF-102** — `fftgf` default input `(Re,Im)` vs default output `(Im,Re)` asymmetry
- [ ] **DEF-103** — `ffcmplx` unused `ex` despite help
- [ ] **DEF-104** — `ffcmplx` `sread(fin,Gread,wm)` resolve/argument-order anomaly
- [ ] **DEF-105** — IOTOOLS IC `(Re,Im)` vs RC `(Im,Re)` overload split (accept as per-surface contract vs unify)
- [ ] **DEF-106** — `txtfy` `(re,im)` vs file/CLI `(im,re)` writers
- [ ] **DEF-107** — `sreadM_*` unallocated/`imY` / duplicate `imY(2)` anomalies
- [ ] **DEF-108** — Accepted numeric comparison policy: `1e-6` vs `1e-10` vs exact (per retained surface)
- [ ] **DEF-109** — Bare `STOP` vs `stop 1` exit-status contract
- [ ] **DEF-110** — Fatal/help diagnostics on stdout mixed with data
- [ ] **DEF-111** — `r8_to_s_left` comment `G14.6` vs code `g16.9`
- [ ] **DEF-112** — `fftgf` unused `STRIDE`
- [ ] **DEF-113** — `fftgf` `tau2iw` help (real input) vs two-column read

**Policy note:** Project `defectPolicy: reproduce-then-refactor` suggests a default preference toward faithful reproduction before later cleanup, but **does not auto-fill** any Decision column. Mismatches without decisions block affected Phase P parity criteria.

### Out of scope for this slice (not assigned DEF IDs here)

- `logspace` documentation mismatch (named in assessment; does not intersect the numeric/text/error *codec* contracts of BEH-101–105 beyond ordinary grid CLI formatting covered by BEH-103).
- `ZEROS`/`OPTIMIZE` facade compatibility, FFT backend sign/normalization math, square-lattice denominator reversal, untriaged compiler-warning surfaces — record when those behaviors are cataloged.

## 6. Links

- Behavior catalog: `docs/modernization/behaviors/BEH-NNN-*.md`
- Flow (BEH-104): `docs/modernization/flows/BEH-104-fftgf-complex-column-io.md`
- Oracle: `docs/modernization/oracle.md`
- Intent ledger: `docs/modernization/intent-ledger.md` (INT-006 tolerance; open question on `1e-10` vs `1e-6`)
- Translation gaps: GAP-007, GAP-013, GAP-019, GAP-020, GAP-026
- Assessment: `docs/modernization/ASSESSMENT.md` §1/§9 (complex-column + tolerance stops)
- Migration plan: TBD (not created for this slice)
- Project defect policy: `.cursor/workflow.config.yml` → `defectPolicy: reproduce-then-refactor`

### Tensions / conflicts

- Help text, readers, writers, and diagnostics disagree on complex-column order across `fftgf`, `ffcmplx`, IOTOOLS overloads, and `txtfy` (DEF-101–106, DEF-113). `E2`/`E3`/`E5`.
- Matrix complex readers show source anomalies that may be latent defects or unreachable (DEF-107). `E3`/`E5`.
- Three comparison regimes coexist without an accepted parity rule (DEF-108). `E1`/`E2`/`E3`/`E4`.
- Termination and diagnostic channel contracts are non-uniform and largely unverified on fatal paths (DEF-109–110). `E3`/`E5`.
- No owner disposition yet; under hard rules, none of these may be silently “fixed” or treated as intentional bugs during the port.

*Created: 2026-08-10*
