# Defect Ledger

**Legacy repo:** `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy` (read-only); GitHub `PhilipTeitel/scifortran-legacy` at `e586903a26cc50ca8942f20ca3bccbd8814e6252`
**Date:** `2026-08-19`
**Scope:** Opened during `/document-legacy` for BEH-001. Library-wide contradictions remain listed as open when they must not be silently “fixed” later.

---

## 1. Defect decisions

| ID | Defect / mismatch | Affected behavior | Evidence | Decision | UAT impact | Backlog / story |
|----|-------------------|-------------------|----------|----------|------------|-----------------|
| DEF-001 | Checked-in `fidelity/golden/linspace-5.txt` is regenerated from a Python formula, not a retained legacy capture, even though it numerically matched the probe | BEH-001 / FIX-001 | `E1 verified / E3 code-derived / E4 inferred` — `docs/modernization/oracle.md:20,101`; ADR-001 | reproduce-faithfully the **probe parsed values**; do not treat the golden file as authority | Parity must use FIX-001, not the golden path | TBD port story |
| DEF-002 | `linspace` declares `array(num)` before checking `num<0`, so negative length may be processor-dependent prior to `error`/`STOP` | BEH-001 error path | `E3 code-derived` — `src/tools_grids.f90:1-7`; unexecuted | TBD | Blocks error-path parity until decided | TBD |
| DEF-003 | CLI program unit is `linsp` while help/NAME is `linspace` | CLI surface (not first slice) | `E3 code-derived` — `numutils/src/linspace.f90:1-13` | TBD | None for managed-API slice | TBD |
| DEF-004 | Fidelity driver prints `es24.17`; CLI prints list-directed `write(*,*)` | Text surfaces (not first slice) | `E3 code-derived` — `fidelity/driver.f90:17`; `numutils/src/linspace.f90:47-49` | TBD | None while parity is parsed managed-API values (ADR-003) | TBD |

Known assessment-era mismatches **outside** this slice (FFT backends, `ZEROS`/`OPTIMIZE`, complex-column order, `logspace` docs, 310 warnings) are **not** given `DEF-NNN` rows here. They must still not be silently corrected if those surfaces are later selected. `E1/E3/E4` — `docs/modernization/ASSESSMENT.md:31`; `docs/modernization/intent-ledger.md:42-50`.

## 2. Reproduce faithfully

| DEF ID | Expected port behavior | Parity fixture | Rationale |
|--------|------------------------|----------------|-----------|
| DEF-001 | Return exact FIX-001 samples | `FIX-001` | Owner accepted probe values, not the golden-file provenance |

## 3. Fix now

None yet.

## 4. Fix later

None yet.

## 5. Open defect decisions

- [ ] DEF-002 — `reproduce-faithfully`, `fix-now`, or `fix-later` for negative `num` sizing/`STOP`
- [ ] DEF-003 — only if the CLI becomes in scope
- [ ] DEF-004 — only if Fortran text compatibility becomes in scope

## 6. Links

- Behavior catalog: `docs/modernization/behaviors/BEH-NNN-*.md`
- Oracle: `docs/modernization/oracle.md`
- Migration plan: `docs/modernization/migration-plan.md` (not created; `/plan-migration` remains out of scope)

*Created: 2026-08-19*
