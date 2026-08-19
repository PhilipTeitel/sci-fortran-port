# Behavior catalog (planning inventory)

**Legacy revision:** `e586903a26cc50ca8942f20ca3bccbd8814e6252`
**Date:** `2026-08-19`
**Authority:** ADR-004, ADR-005
**Oracle:** T1 for BEH-001–BEH-004 corpus only; T3 elsewhere until each slice is documented

This catalog is the retained-surface list sequenced by `docs/modernization/migration-plan.md`. Full BEH recovery files exist only where noted. Catalog-only rows are **document → refine → design → implement** slices, not implementation-ready stories.

---

## 1. How to read status

| Status | Meaning |
|--------|---------|
| Recovered | BEH file exists; next is `/refine-feature`, then slice design, then C# |
| T1 characterized | Probe executed the library call; recover fixtures/docs next |
| Catalog-only | Public surface retained; `/document-legacy` when the slice is scheduled |
| Adapter | CLI or I/O host over a library port |
| Retired | Explicitly out of product (ADR-005) |

---

## 2. Recovered and T1 library behaviors

| ID | Job | Legacy surface | Status | Evidence | Next ADD command |
|----|-----|----------------|--------|----------|------------------|
| BEH-001 | Inclusive linear sequence | `TOOLS.linspace` | Recovered; ready for requirements | FIX-001 exact parsed `0,0.25,0.5,0.75,1` | `/refine-feature` (SL-001) |
| BEH-002 | Logarithmic sequence | `TOOLS.logspace` | T1 characterized | `CAP-20260810-LOGSPACE` SHA-256 `c5b198af…`; parsed values not retained | `/document-legacy` |
| BEH-003 | Fermi function | `FUNCTIONS.fermi` | T1 characterized | `CAP-20260810-FERMI` SHA-256 `6f35eadc…` | `/document-legacy` |
| BEH-004 | Numerical derivative | `TOOLS.deriv` | T1 characterized | `CAP-20260810-DERIV` SHA-256 `8a8879bc…`; input `numutils/test/xy2.data` | `/document-legacy` |
| BEH-005 | Integer/real range | `TOOLS.arange` | Catalog-only | Fidelity `arange-5` **did not call** `arange` | `/document-legacy` with a real invocation |

---

## 3. Catalog-only library families

Slice IDs are planning handles. `docs/modernization/migration-plan.md` splits BEH-010 (grids vs Bethe) and schedules CLI adapters after library ports.

| ID | Family | Public names (non-exhaustive) | Module | Notes |
|----|--------|-------------------------------|--------|-------|
| BEH-010 | Remaining `TOOLS` grids/helpers | `powspace`, `upmspace`, `upminterval`, sort/uniq/shift; Bethe deferred to SL-014 | `TOOLS` | Grids in SL-004; Bethe with BEH-090 |
| BEH-020 | Remaining `FUNCTIONS` | `heaviside`, `step`, `sgn`, `wfun`, `zerf` | `FUNCTIONS` | Bundled special-function **internals** not public |
| BEH-030 | Quadrature | `trapz`, `simps`, `kronig`, `kramers_kronig`, `finter_*` | `INTEGRATE` | QUADPACK behind facade |
| BEH-040 | Dense linear algebra | `matrix_inverse*`, `m_invert*`, `matrix_diagonalize`, `solve_linear_system` | `MATRIX` | Needs numeric-port provider (ADR-005) |
| BEH-050 | FFT / GF transforms | `cfft_1d_*`, `fftgf_*`, `fftff_*` | `FFTGF` NR | FFTPACK backend retired |
| BEH-060 | Nonlinear solve | `broydn`, `fzero`, `zbrent`, `fsolve`, `ffsolve` | `OPTIMIZE` | Current facade; not historical `ZEROS` |
| BEH-070 | Interpolation | `poly_spline`, `cubic_spline`, `linear_spline`, `extract`, `interp_gtau` | `SPLINE` | |
| BEH-080 | Random and stats | `rand`/`nrand`/…, histogram, moments, covariance | `RANDOM`, `STATISTICS` | Sequence vs statistical parity TBD at refine |
| BEH-090 | Many-body helpers | Green-function types, `pade_analytic_continuation`, square-lattice helpers | `GREENFUNX`, `PADE`, `SQUARE_LATTICE` | |
| BEH-100 | File and plot data | `file_*`, `data_open`/`data_store`, `splot`/`sread` | `IOTOOLS` | Shell gzip → `System.IO` |
| BEH-110 | Diagnostics / timer | `msg`/`warning`/`error`, `start_timer` | `COMMON_VARS`, `TIMER`, `PARSE_CMD` | Mapped at adapters; `STOP` → domain failure |

---

## 4. CLI adapters

Each row is a driving adapter over the library ports above. Do not reimplement arithmetic in the CLI project.

| ID | CLI | Default `all`? | Depends on | Status |
|----|-----|----------------|------------|--------|
| BEH-201 | `linspace` | yes | BEH-001 | Adapter; out of first recovered BEH file by design |
| BEH-202 | `logspace` | yes | BEH-002 | Adapter |
| BEH-203 | `arange` | yes | BEH-005 | Adapter |
| BEH-204 | `fermi` | yes | BEH-003 | Adapter |
| BEH-205 | `deriv` | yes | BEH-004 | Adapter |
| BEH-206 | `spline` | yes | BEH-070 | Adapter |
| BEH-207 | `fftgf` | yes | BEH-050 | Adapter; complex-column order is a refine decision |
| BEH-208 | `wmatsubara` | yes | BEH-010 / many-body helpers | Adapter |
| BEH-209 | `pade` | yes | BEH-090 | Adapter |
| BEH-210 | `random` | yes | BEH-080 | Adapter |
| BEH-211 | `histogram` | yes | BEH-080 | Adapter |
| BEH-212 | `kdensity` | yes | BEH-080 | Adapter |
| BEH-213 | `numstat` | yes | BEH-080 | Adapter |
| BEH-214 | `func` | yes | managed expression port | Adapter; substitute evaluator (ADR-005) |
| BEH-215 | `splot` | yes | BEH-100 | Adapter; Gnuplot wrap |
| BEH-216 | `ffcmplx` | no (Make target exists) | BEH-100 | Adapter retained |
| — | `vfplot` | no | `DLPLOT` | **Retired** |

---

## 5. Suggested strangler order

Superseded by `docs/modernization/migration-plan.md` (SL-001–SL-025). Summary: BEH-001 first; remaining grids; functions and `deriv`; spline/integrate/stats; matrix/optimize/FFT; many-body and I/O; then BEH-110 and CLI adapters. BEH-010 Bethe helpers travel with BEH-090.

---

## 6. Retired (do not plan C# stories)

- `vfplot` / DISLIN / `DLPLOT`
- FFTPACK FFT backend
- `CHRPACK`
- `bin/setup_sf.sh` as a product entrypoint
- Fortran module ABI as a supported consumer contract
- Unexported special-function internals

---

## 7. Links

- PURPOSE / DOMAIN: `docs/PURPOSE.md`, `docs/DOMAIN.md`
- Oracle: `docs/modernization/oracle.md`
- ADRs: ADR-004, ADR-005
- Recovered behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Migration plan: `docs/modernization/migration-plan.md`

*Created: 2026-08-19 | Sequenced: 2026-08-19*
