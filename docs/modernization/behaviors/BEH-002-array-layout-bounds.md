# BEH-002: Fortran array layout and bounds contract

**Status:** Draft
**Evidence grade:** `E3 code-derived` (overall; runtime stride/section/ownership edge cases remain `E5 unknown`)
**Legacy surfaces:** Library array APIs (`MATRIX`, `TOOLS` grids, FFT helpers); CLI stream-to-array utilities (`deriv`, `fftgf`); allocatable dumps from linked lists
**Date:** `2026-08-10`

---

## 1. Summary

SciFortran exposes Fortran arrays that are one-based by default when declared as `array(n)` / `array(n,m)`, column-major in memory for rank ≥ 2, and frequently use explicit non-default lower bounds (for example `0:L`, `-N:N`). Shape is carried by the Fortran array descriptor; many routines take assumed-shape `dimension(:,:)` arguments and derive `size`/`lda` from the actual argument. Observable indexing and layout are those Fortran semantics, not C-row-major defaults.

## 2. Actors and triggers

| Actor / system | Trigger | Preconditions | Evidence |
|----------------|---------|---------------|----------|
| Library consumer | Calls matrix/grid/FFT routines with arrays | Allocated/associated actual arguments of expected rank/shape | `E3` — `src/MATRIX.f90:87-128`; `src/tools_grids.f90:1-28` |
| CLI user | Streams columns that utilities pack into allocatable 1-D arrays | Successful list-directed reads until EOF | `E3` — `numutils/src/deriv.f90:37-69`; `numutils/src/fftgf.f90:67-79` |
| Fidelity driver | Allocates `x(n)` and fills via `linspace`/`deriv` | Built core library | `E3`/`E1` — `fidelity/driver.f90:11-16,55-65`; oracle §4 |

## 3. Inputs

| Input | Type / format | Units | Range / constraints | Required? | Evidence |
|-------|---------------|-------|----------------------|-----------|----------|
| Rank-1 real/complex vectors | `real(8)`/`complex(8)` assumed-shape or explicit `array(num)` | n/a | Default lower bound 1 unless declared otherwise | yes | `E3` — `src/tools_grids.f90:1-2`; `src/LIST_D_UNORDERED.f90:7` |
| Rank-2 matrices | `dimension(:,:)` kind-8 | n/a | Column-major; `size(M,1)` used as LDA | yes for matrix APIs | `E3` — `src/MATRIX.f90:87-98,110-122` |
| Explicitly bounded arrays | e.g. `in(-N:N)`, `gout(0:L)` | n/a | Lower/upper bounds are part of the callable contract | surface-specific | `E3` — `numutils/src/fftgf.f90:120-140,158` |
| Stream length `L` | Integer count of successful reads | count | Must match list size where checked | derived | `E3` — `numutils/src/fftgf.f90:75-77` |

## 4. Outputs and side effects

| Output / side effect | Type / format | Precision / ordering | Destination | Evidence |
|----------------------|---------------|----------------------|-------------|----------|
| Allocatable result arrays | Kind-8; usually `allocate(a(L))` ⇒ indices `1..L` | Element order follows Fortran storage/sequence association | Caller | `E3` — `numutils/src/deriv.f90:51-68`; `src/tools_grids.f90:1-14` |
| In-place matrix mutation | Same array argument | Column-major LAPACK layout assumptions | Caller buffer | `E3` — `src/MATRIX.f90:87-103` |
| Indexed FFT windows | Negative/zero lower bounds preserved in writes | Loop `i=-N,N` or `i=0,L` defines output order | stdout/file | `E3` — `numutils/src/fftgf.f90:144-162` |

## 5. Rules and invariants

| Rule | Evidence | Open question? |
|------|----------|----------------|
| Default-declared arrays `array(n)` are 1-indexed (`forall(i=1:num)`). | `E3` — `src/tools_grids.f90:11-14`; `fidelity/driver.f90:14-15` | no |
| Rank-2 matrices use Fortran column-major storage; LAPACK LDA taken from `size(M,1)`. | `E3` — `src/MATRIX.f90:97-98,121-122`; GAP-005 | no |
| Non-default bounds (`0:`, `-N:N`) are intentional on some FFT/time-domain surfaces. | `E3` — `numutils/src/fftgf.f90:120-140,158`; `src/tools_shifts.f90:8-14` | no |
| Stream utilities grow unknown-length input via linked lists then `dump_list` into `allocate(...(L))`. | `E3` — `numutils/src/deriv.f90:39-68`; `src/LIST_D_UNORDERED.f90:1-33` | no |
| Assumed-shape dummies observe the actual argument’s bounds/size; they do not invent C# zero-based semantics. | `E3` — `src/MATRIX.f90:87-88`; GAP-005 | yes — slice/view vs copy at target boundary |
| `fftgf` option `stride` is registered; effect on observable layout needs surface-specific tracing. | `E3`/`E5` — `numutils/src/fftgf.f90:46,56` | yes |

## 6. Error handling and edge cases

| Case | Legacy behavior | Evidence | Defect decision |
|------|-----------------|----------|-----------------|
| `fftgf` length/list size mismatch | `abort("error in counting input")` → `STOP` | `E3` — `numutils/src/fftgf.f90:77`; `src/COMVARS.f90:192-208` | none |
| `rt2rw` even length / `rw2rt` odd length | `error("wrong dimension ...")` → `STOP` | `E3` — `numutils/src/fftgf.f90:118,138` | none |
| Empty stdin / L=0 | Not characterized | `E5` | TBD |
| Array section / non-contiguous actual | Not characterized | `E5` — GAP-005 | TBD |

## 7. Draft Gherkin

```gherkin
Given a kind-8 Fortran array argument with shape N or (M,N)
When a SciFortran routine indexes or mutates that array
Then default allocations are addressed starting at index 1
And rank-2 storage is column-major with LDA = size(array,1) where LAPACK is called
And routines that declare bounds such as -N:N or 0:L expose those bounds in their write loops
```

## 8. Legacy code and documentation citations

| Source | Lines / section | Claim supported | Evidence grade |
|--------|-----------------|-----------------|----------------|
| `src/tools_grids.f90` | 1-28 | 1-based `linspace` fill | E3 |
| `src/MATRIX.f90` | 87-128 | Assumed-shape matrices; LDA from `size(M,1)` | E3 |
| `src/tools_shifts.f90` | 8-50 | Explicit `0:` lower bounds in shifts | E3 |
| `numutils/src/fftgf.f90` | 78-79,118-162 | Allocatable data; `-N:N` / `0:L` windows | E3 |
| `numutils/src/deriv.f90` | 37-69 | Stream → list → `allocate(fi(L))` | E3 |
| `src/LIST_D_UNORDERED.f90` | 1-33 | Dynamic real list backbone | E3 |
| `fidelity/driver.f90` | 11-16,55-65 | `allocate(x(n))` driver usage | E3 |
| `docs/modernization/translation-gaps.md` | GAP-005 | Layout/bounds migration gap | E3 |

## 9. Oracle fixtures

| Fixture | Input | Expected output | Tolerance / normalization | Evidence |
|---------|-------|-----------------|---------------------------|----------|
| `CAP-20260810-DERIV` (ephemeral) | 1024×2 `xy2.data` rows → `deriv` | 1024×2 rows | Layout exercised as 1-based vectors; values exact across builds | `E1` — oracle §5 |
| Matrix/FFT layout fixtures | none retained | n/a | Required before matrix/FFT parity | `E5` — oracle §8 |

## 10. Open questions

- [ ] Which public APIs require preserving non-default lower bounds versus normalizing to 1-based copies?
- [ ] Are array sections passed to LAPACK/FFT paths ever non-contiguous in supported consumers?
- [ ] What is the observable effect of `fftgf` `stride` on input packing and output ordering?
- [ ] Do any consumers rely on sequence association / storage overlay beyond standard assumed-shape?

## 11. Links

- Intent ledger: `docs/modernization/intent-ledger.md`
- Legacy flow: `docs/modernization/flows/` (none yet; candidate for `/trace-flow` on `fftgf` bounds)
- Defect ledger: `docs/modernization/defect-ledger.md` — DEF-012 (`fftgf` unused `STRIDE`, open/TBD)
- Related gaps: GAP-005, GAP-014

### Tensions / conflicts

- Fortran one-based/column-major semantics conflict with default C# zero-based/row-major arrays; no accepted buffer ADR exists. `E3`/`E5` — GAP-005.
- Help text for `fftgf` length constraints and code checks agree for `rt2rw`/`rw2rt`, but stride/section behavior is undocumented beyond the option string. `E2`/`E3`/`E5` — `numutils/src/fftgf.f90:35-46,116-140`.

*Created: 2026-08-10*
