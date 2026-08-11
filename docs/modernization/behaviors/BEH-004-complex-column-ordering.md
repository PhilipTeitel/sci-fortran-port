# BEH-004: Complex-column ordering `(Re,Im)` vs `(Im,Re)` contract

**Status:** Draft
**Evidence grade:** `E3 code-derived` (overall; help text `E2 documented`; contradictions unresolved; no `E1` complex-column capture)
**Legacy surfaces:** `fftgf` CLI; `ffcmplx` CLI; `SLREAD`/`SLPLOT` complex overloads; `COMMON_VARS` `txtfy` complex strings
**Date:** `2026-08-10`

---

## 1. Summary

Complex values appear externally as pairs of real columns or as `(re,im)` diagnostic strings. Column order is **surface-specific** and sometimes **internally inconsistent** between help text, readers, writers, and the `ex` swap flag. There is no single repository-wide positional convention that can be treated as product intent without an owner decision. This behavior records the observed contracts and tensions only.

## 2. Actors and triggers

| Actor / system | Trigger | Preconditions | Evidence |
|----------------|---------|---------------|----------|
| `fftgf` user | Stream/file of two numeric columns; optional `ex=` | Program built | `E2`/`E3` — `numutils/src/fftgf.f90:30-57,69-113` |
| `ffcmplx` user | File of `X` + two columns; optional `ex=` | Program built (not in default `all`) | `E2`/`E3` — `numutils/src/ffcmplx.f90:23-50`; `numutils/src/Makefile:8,24-26` |
| Library file I/O caller | `sread`/`splot` complex overloads | Path + arrays | `E3` — `src/slread_sread_V.f90:95-137,233-275`; `src/slplot_splot_V.f90:130-146,285-301` |
| Diagnostic formatting | `txtfy` on `complex(8)` | n/a | `E3` — `src/COMVARS.f90:275-283` |

## 3. Inputs

| Input | Type / format | Units | Range / constraints | Required? | Evidence |
|-------|---------------|-------|----------------------|-----------|----------|
| `fftgf` column pair | two reals per line | n/a | Default: read as `rey,imy` → `cmplx(rey,imy,8)`; if `ex`: swapped | yes | `E3` — `numutils/src/fftgf.f90:70-71` |
| `ffcmplx` help claim | `X,imG,reG` default; `X,reG,imG` if `ex=T` | n/a | Help only | documented | `E2` — `numutils/src/ffcmplx.f90:23-31` |
| `ffcmplx` actual call | `sread(fin,Gread,wm)` with parsed but unused `ex` | n/a | Argument order vs `pade` differs | yes | `E3` — `numutils/src/ffcmplx.f90:39-50`; `numutils/src/pade.f90:59` |
| `sreadV_RC` / `sreadM_RC` file rows | `X, im, re` then `cmplx(re,im)` | n/a | Default real-X complex path is Im then Re columns | yes for those overloads | `E3` — `src/slread_sread_V.f90:246-272`; `src/slread_sread_M.f90:204-207` |
| `sreadV_IC` integer-X rows | `X, re, im` then `cmplx(re,im)` | n/a | Re then Im | yes for those overloads | `E3` — `src/slread_sread_V.f90:108-134` |

## 4. Outputs and side effects

| Output / side effect | Type / format | Precision / ordering | Destination | Evidence |
|----------------------|---------------|----------------------|-------------|----------|
| `fftgf` default complex write (`ex=false`) | `dimag, real` per line | **(Im, Re)** | stdout/file | `E3` — `numutils/src/fftgf.f90:97-100,109-112` |
| `fftgf` exchanged write (`ex=true`) | `real, dimag` | **(Re, Im)** | stdout/file | `E3` — `numutils/src/fftgf.f90:93-96,105-108` |
| `splot` real-X complex | `X, dimag, dreal` | **(Im, Re)** | file | `E3` — `src/slplot_splot_V.f90:285-301` |
| `splot` integer-X complex | `X, dreal, dimag` | **(Re, Im)** | file | `E3` — `src/slplot_splot_V.f90:130-146` |
| `txtfy` complex | `"(re,im)"` string | Re then Im | diagnostic text | `E3` — `src/COMVARS.f90:275-283` |
| `ffcmplx` sibling outputs | abs/phase via `splot` | depends on what `sread` actually loaded | file `fout` (prior file removed) | `E3` — `numutils/src/ffcmplx.f90:52-55` |

## 5. Rules and invariants

| Rule | Evidence | Open question? |
|------|----------|----------------|
| Complex **storage** in memory uses Fortran intrinsic `(re,im)` components. | `E3` — `src/COMVARS.f90:13-15,275-280` | no |
| External **column order is not universal**; it varies by surface and sometimes by X-type overload. | `E3` — citations in §§3-4; GAP-013 | no |
| `fftgf` help says input is Fortran complex `(re,im)` and documents `ex` to exchange real/imag in input and output. | `E2` — `numutils/src/fftgf.f90:32-45` | no |
| `fftgf` default output writes Imag then Real, which is not `(re,im)` column order. | `E3` — `numutils/src/fftgf.f90:97-100` | yes — defect vs intentional |
| `ffcmplx` parses `ex` but does not reference it after parsing. | `E3` — `numutils/src/ffcmplx.f90:39-50` | yes |
| `pade` calls `sread(fin,wm,gm)` while `ffcmplx` calls `sread(fin,Gread,wm)`. | `E3` — `numutils/src/pade.f90:59`; `numutils/src/ffcmplx.f90:50` | yes — compile/resolve behavior |
| `sreadM_RC`/`sreadM_IC` else-branches read into `imY` without allocating `imY` in the inspected source. | `E3` — `src/slread_sread_M.f90:92-99,200-207` | yes — latent defect? |
| Asymmetric complex fixtures are required before any codec can be accepted. | `E4`/`E5` — GAP-013; oracle §7 | yes |

## 6. Error handling and edge cases

| Case | Legacy behavior | Evidence | Defect decision |
|------|-----------------|----------|-----------------|
| Help vs default `fftgf` output order | Help implies `(re,im)`; default write is `(im,re)` | `E2`/`E3` — `numutils/src/fftgf.f90:32,97-100` | **DEF-001** open/TBD |
| Default `fftgf` input vs output asymmetry | Read `(Re,Im)`; write `(Im,Re)` | `E3` — `numutils/src/fftgf.f90:70-71,97-100` | **DEF-002** open/TBD |
| `ffcmplx` `ex` flag | Documented; unused in body | `E2`/`E3` — `numutils/src/ffcmplx.f90:23-50` | **DEF-003** open/TBD |
| `ffcmplx` `sread` argument order | `sread(fin,Gread,wm)` vs `pade`/`sreadM_RC` shape | `E3`/`E5` — `numutils/src/ffcmplx.f90:50`; `pade.f90:59` | **DEF-004** open/TBD |
| IC vs RC column split | Integer-X `(Re,Im)`; real-X `(Im,Re)` | `E3` — `slread_sread_V.f90` / `slplot_splot_V.f90` | **DEF-005** open/TBD |
| `txtfy` vs file/CLI writers | Diagnostic `(re,im)` vs several `(im,re)` writers | `E3` — `COMVARS.f90:275-283` | **DEF-006** open/TBD |
| `sreadM_*` unallocated `imY` path | Source appears to use unallocated `imY` | `E3` — `src/slread_sread_M.f90:200-207` | **DEF-007** open/TBD |
| Dual-Y formatted read duplicate `imY(2)` | Formatted branch writes `imY(2)` twice instead of `reY(2)` | `E3` — `src/slread_sread_M.f90:195,87` | **DEF-007** open/TBD |
| `fftgf` unused `STRIDE` | Parsed; not applied in body | `E3`/`E5` — `numutils/src/fftgf.f90:46,56` | **DEF-012** open/TBD |
| `tau2iw` help vs two-column read | Help claims real input; reader takes two columns | `E2`/`E3` — `fftgf.f90:32-33,70-71` | **DEF-013** open/TBD |
| Missing/`ffcmplx` not in default build | Source exists; omitted from `all` | `E3`/`E5` — `numutils/src/Makefile:8,24-26` | TBD (scope; see DEF-003/004) |

## 7. Draft Gherkin

```gherkin
Given an asymmetric complex value with Re ≠ Im
When it is exchanged through a named legacy surface (fftgf, sread/splot overload, txtfy)
Then the memory complex retains Fortran (Re,Im) components
And the external column order matches that surface’s reader/writer (not a global default)
And any help-text contradiction is treated as an unresolved tension until a defect decision exists
```

## 8. Legacy code and documentation citations

| Source | Lines / section | Claim supported | Evidence grade |
|--------|-----------------|-----------------|----------------|
| `numutils/src/fftgf.f90` | 30-48,70-71,93-113,124-175 | Help, read, default/exchanged writes | E2/E3 |
| `numutils/src/ffcmplx.f90` | 23-55 | Help Im/Re; unused `ex`; `sread` call order | E2/E3 |
| `numutils/src/pade.f90` | 59 | Contrasting `sread(fin,wm,gm)` order | E3 |
| `src/slread_sread_V.f90` | 108-134,246-272 | IC `(re,im)` vs RC `(im,re)` columns | E3 |
| `src/slread_sread_M.f90` | 73-100,181-210 | Matrix complex read; allocation/format issues | E3 |
| `src/slplot_splot_V.f90` | 130-146,285-301 | Integer-X vs real-X write order split | E3 |
| `src/COMVARS.f90` | 275-283 | Diagnostic `(re,im)` string | E3 |
| `docs/modernization/translation-gaps.md` | GAP-013 | Per-surface codec requirement | E2/E3 |
| `docs/modernization/ASSESSMENT.md` | §9 complex-column conflict | Planning stopped pending decisions | E2/E3 |

## 9. Oracle fixtures

| Fixture | Input | Expected output | Tolerance / normalization | Evidence |
|---------|-------|-----------------|---------------------------|----------|
| None | Complex columns not in fidelity driver | n/a | Need asymmetric Re≠Im round-trips per surface | `E5` — oracle §§4,7 |
| Candidate approach | Crafted `(1,2)` vs `(2,1)` rows | Detect swaps | Exact column semantic compare | `E4` inferred test design — not executed |

## 10. Open questions

- [ ] For each retained surface, is canonical external order `(Re,Im)`, `(Im,Re)`, or help-text order?
- [ ] Is default `fftgf` `(Im,Re)` output a defect (`fix-*`) or compatibility requirement (`reproduce-faithfully`)?
- [ ] Should unused `ffcmplx` `ex` be treated as dead help, broken feature, or unsupported utility?
- [ ] Does `ffcmplx`’s `sread(fin,Gread,wm)` compile/resolve on the accepted compiler, and what does it read?
- [ ] Are `sreadM_*` allocation/format anomalies latent bugs or unreachable?

## 11. Links

- Intent ledger: `docs/modernization/intent-ledger.md`
- Legacy flow: `docs/modernization/flows/BEH-004-fftgf-complex-column-io.md`
- Defect ledger: `docs/modernization/defect-ledger.md` — DEF-001–007, DEF-012–013 (all open/TBD)
- Related gaps: GAP-013, GAP-007
- Related behavior: BEH-003

### Tensions / conflicts

- **`fftgf` help vs default writer:** help describes Fortran `(re,im)`; default output is `dimag,real` ⇒ `(Im,Re)`. `E2`/`E3` — `numutils/src/fftgf.f90:32,97-100`.
- **`fftgf` input vs output asymmetry:** default input builds `cmplx(rey,imy)` `(Re,Im)` while default output prints `(Im,Re)`. `E3` — `numutils/src/fftgf.f90:70-71,97-100`.
- **`ffcmplx` help vs code:** help documents `ex` swapping Im/Re vs Re/Im, but `ex` is never used after parse. `E2`/`E3` — `numutils/src/ffcmplx.f90:23-50`.
- **`ffcmplx` vs `pade` `sread` argument order:** `sread(fin,Gread,wm)` vs `sread(fin,wm,gm)`. `E3` — both call sites.
- **`SLREAD`/`SLPLOT` overload split:** integer-X complex paths use `(Re,Im)` columns; real-X complex paths use `(Im,Re)`. `E3` — `src/slread_sread_V.f90`; `src/slplot_splot_V.f90`.
- **`txtfy` always `(re,im)`** while several file codecs write `(im,re)`. `E3` — `src/COMVARS.f90:275-283` vs splot RC path.
- No global codec may be chosen without altering some observable surface. `E3`/`E5` — GAP-013; assessment §9.

*Created: 2026-08-10*
