# BEH-101: `real(8)` / `complex(8)` numeric representation contract

**Status:** Draft
**Evidence grade:** `E3 code-derived` (overall; exact IEEE widths, rounding, and overflow remain `E5 unknown`)
**Legacy surfaces:** Library API (`COMMON_VARS`, `MATRIX`, `TOOLS`); fidelity driver numeric values; CLI utilities that declare `real(8)`/`complex(8)`
**Date:** `2026-08-10`

---

## 1. Summary

Public SciFortran numerics are declared as Fortran kind-8 reals and complexes (`real(8)`, `complex(8)`), with shared kind aliases `dbl`/`dp` = 8 and a declared but unused-in-declarations quad kind `ddp` = 16. Observable values are binary floating-point quantities of that kind under the compiling runtime. Exact storage width, IEEE edge behavior, fused operations, and overflow/underflow are not a portable documented contract until measured on an accepted compiler.

## 2. Actors and triggers

| Actor / system | Trigger | Preconditions | Evidence |
|----------------|---------|---------------|----------|
| Library consumer / CLI utility | Compile/link against SciFortran and call numeric APIs or run utilities | Kind-8 declarations in scope via `COMMON_VARS` or local `real(8)`/`complex(8)` | `E3` — `src/COMVARS.f90:13-39`; `src/PARSECMD.f90:130-149`; `numutils/src/deriv.f90:10-12` |
| Fidelity probe operator | Run bounded fidelity driver | Built core at probe revision `e586903` | `E1` — `docs/modernization/oracle.md:19-25,83-87` |

## 3. Inputs

| Input | Type / format | Units | Range / constraints | Required? | Evidence |
|-------|---------------|-------|----------------------|-----------|----------|
| Scalar/array real arguments | `real(8)` / `real(dbl)` | dimensionless or domain-specific | Kind-8 binary float; `max_real`/`epsilonr` constants published | yes for real APIs | `E3` — `src/COMVARS.f90:13-36` |
| Scalar/array complex arguments | `complex(8)` as `(re,im)` Fortran intrinsic pairing | same as components | Components are kind-8 reals; constructors use `cmplx(...,8)` | yes for complex APIs | `E3` — `src/COMVARS.f90:13-15`; `numutils/src/fftgf.f90:70-71` |
| Kind selectors | Integer parameters `dbl`/`dp`/`ddp`/`sp` | n/a | `dbl=dp=8`, `ddp=16`, `sp=kind(1.0)` | no (internal) | `E3` — `src/COMVARS.f90:32-34` |

## 4. Outputs and side effects

| Output / side effect | Type / format | Precision / ordering | Destination | Evidence |
|----------------------|---------------|----------------------|-------------|----------|
| Numeric return values / arrays | `real(8)` or `complex(8)` | Kind-8; operation order is source-defined | Caller memory / stdout after formatting | `E3` — `src/MATRIX.f90:87-128`; `fidelity/driver.f90:7-16` |
| Published numeric constants | `real(8)` / `complex(8)` parameters | Literal `d0` / `_dbl` forms | Module exports | `E3` — `src/COMVARS.f90:13-39` |
| Cross-build parsed equality (probe) | Parsed floats from driver text | Exact equality across two probe runs (max abs diff `0`) | Ephemeral captures only | `E1` — `docs/modernization/oracle.md:36,109` |

## 5. Rules and invariants

| Rule | Evidence | Open question? |
|------|----------|----------------|
| Public numeric API is dominated by `real(8)` and `complex(8)`. | `E3` — `src/COMVARS.f90:13-39`; `docs/modernization/legacy-map.md` §2 precision note | no |
| Kind aliases: `dbl=8`, `dp=8`; `ddp=16` is declared. | `E3` — `src/COMVARS.f90:32-34` | no |
| Whole-source scan found no explicit `real(16)`/`complex(16)` declarations despite `ddp=16`. | `E3` — `docs/modernization/legacy-map.md` §2; GAP-008 | yes — whether quad is ever intended |
| Kind values are implementation selections, not a portable precision guarantee across compilers. | `E3`/`E5` — GAP-008; `docs/modernization/legacy-map.md` §2 | yes |
| Complex intrinsic pairing is Fortran `(real_part, imag_part)` in constructors such as `cmplx(rey,imy,8)` and constants `zero`/`xi`/`one`. | `E3` — `src/COMVARS.f90:13-15`; `numutils/src/fftgf.f90:71` | no |
| Configured parity tolerances (`1e-6`) and fidelity script `TOL=1e-10` are provisional comparison knobs, not proven kind-width requirements. | `E2`/`E3`/`E4` — `.cursor/workflow.config.yml:44-47`; `scripts/fidelity.sh:11`; INT-006; **DEF-108** open/TBD | yes |

## 6. Error handling and edge cases

| Case | Legacy behavior | Evidence | Defect decision |
|------|-----------------|----------|-----------------|
| Overflow/underflow / NaN / signed zero / subnormals | Not characterized for public surfaces | `E5` — GAP-008; oracle §7 | TBD |
| Matrix LAPACK `info /= 0` | Calls `error(...)` then `STOP` | `E3` — `src/MATRIX.f90:100-103,125-128` | none |
| `ddp` / quad path | Declared constants only; no `real(16)` use found | `E3`/`E5` — `src/COMVARS.f90:27-34` | TBD |

## 7. Draft Gherkin

```gherkin
Given a SciFortran consumer compiled with the accepted kind-8 environment
When a public API returns or accepts a real or complex scalar/array
Then the value is represented as Fortran kind-8 (`real(8)` / `complex(8)`)
And complex values use intrinsic real/imaginary components (not a swapped storage ABI)
And no portable claim is made about exact byte width or IEEE edge results until measured
```

## 8. Legacy code and documentation citations

| Source | Lines / section | Claim supported | Evidence grade |
|--------|-----------------|-----------------|----------------|
| `src/COMVARS.f90` | 13-39 | Kind-8 constants, `dbl`/`dp`/`ddp`, epsilon/huge helpers | E3 |
| `src/PARSECMD.f90` | 130-149 | CLI numeric options parsed into `real(8)` | E3 |
| `src/MATRIX.f90` | 87-128 | Kind-8 matrix/eigen interfaces | E3 |
| `numutils/src/deriv.f90` | 10-12 | CLI utility kind-8 arrays | E3 |
| `numutils/src/fftgf.f90` | 10-12,70-71 | Kind-8 complex construction | E3 |
| `fidelity/driver.f90` | 7-16 | Kind-8 driver locals and `linspace` call | E3 |
| `docs/modernization/translation-gaps.md` | GAP-008, GAP-009 | Kind/tolerance blockers | E3/E5 |
| `docs/modernization/oracle.md` | §§1,6 | Probe repeatability ≠ accepted tolerance | E1/E2/E5 |

## 9. Oracle fixtures

| Fixture | Input | Expected output | Tolerance / normalization | Evidence |
|---------|-------|-----------------|---------------------------|----------|
| `CAP-20260810-LINSPACE` (ephemeral) | `linspace(0,1,5)` | Observed section hash in oracle §5 | Probe: exact parsed equality across builds; not an accepted parity rule | `E1` — `docs/modernization/oracle.md:96,109` |
| `CAP-20260810-FERMI` (ephemeral) | five X; beta 100 | Observed section hash | same | `E1` — oracle §5 |
| Configured defaults | n/a | relative/absolute `1e-6` | Provisional only | `E2` — workflow config |
| Script `TOL` | n/a | absolute `1e-10` | Script self-check only | `E3` — `scripts/fidelity.sh:11` |

## 10. Open questions

- [ ] What exact storage width, endianness, and IEEE mode does the authoritative production compiler use for kind 8?
- [ ] Is `ddp=16` retained for future use, dead scaffolding, or required by unexamined bundled code paths?
- [ ] Which behavior-specific absolute/relative/ULP/residual rules replace provisional `1e-6` / `1e-10`?
- [ ] Must signed zero, subnormals, NaN payloads, and Infinity spelling match the probe gfortran environment?

## 11. Links

- Intent ledger: `docs/modernization/intent-ledger.md` (INT-006 tolerance)
- Legacy flow: `docs/modernization/flows/` (none yet)
- Defect ledger: `docs/modernization/defect-ledger.md` — DEF-108 (open/TBD)
- Related gaps: GAP-008, GAP-009
- Assessment Condition 7: `docs/modernization/ASSESSMENT.md` §8

### Tensions / conflicts

- Workflow relative/absolute `1e-6` conflicts with fidelity absolute-only `1e-10`; neither is an accepted parity requirement (**DEF-108** open/TBD). `E2`/`E3`/`E4` — `.cursor/workflow.config.yml:44-47`; `scripts/fidelity.sh:11`; INT-006; oracle §6.
- Kind-8 declarations are abundant, but portable precision equivalence to C# `double`/`Complex` is unproven. `E3`/`E5` — GAP-008.
- Probe established exact cross-build repeatability for exercised values, not a product tolerance policy. `E1`/`E5` — oracle §§1,6.

*Created: 2026-08-10*
