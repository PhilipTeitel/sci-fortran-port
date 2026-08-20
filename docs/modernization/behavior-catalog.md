# Behavior catalog (planning inventory)

**Legacy revision:** `e586903a26cc50ca8942f20ca3bccbd8814e6252`
**Date:** `2026-08-19`
**Authority:** ADR-004, ADR-005, ADR-006, ADR-007, ADR-008
**Oracle:** T1 for BEH-001–BEH-004 corpus only; T3 elsewhere until each slice is documented

This catalog is the recovered inventory of the legacy surface. **It is not a work list.** `docs/modernization/migration-plan.md` builds three vertical slices (`BEH-001`, `BEH-003`, `BEH-040`); everything else here is reserve or retired. Full BEH recovery files exist only where noted.

---

## 1. How to read status

| Status | Meaning |
|--------|---------|
| **Built** | In the demonstration slice set (ADR-008); planned work |
| Recovered | BEH file exists; next is `/refine-feature`, then slice design, then C# |
| T1 characterized | Probe executed the library call; recover fixtures/docs next |
| Reserve | Public surface retained by ADR-005 §3, but **not planned work**. `/document-legacy` only if the demonstration is widened |
| Evidence-only | Recovered documentation about legacy behavior; not built (ADR-006) |
| Retired | Explicitly out of product (ADR-005, ADR-006, ADR-007) |

Reserve and evidence-only rows carry no story and no schedule. Their presence here documents what the legacy system does; it does not imply commitment.

---

## 2. Recovered and T1 library behaviors

| ID | Job | Legacy surface | Status | Evidence | Next ADD command |
|----|-----|----------------|--------|----------|------------------|
| BEH-001 | Inclusive linear sequence | `TOOLS.linspace` | **Built (VS-1)** — recovered; ready for requirements | FIX-001 exact parsed `0,0.25,0.5,0.75,1` | `/refine-feature` |
| BEH-002 | Logarithmic sequence | `TOOLS.logspace` | Reserve; T1 characterized | `CAP-20260810-LOGSPACE` SHA-256 `c5b198af…`; parsed values not retained | none scheduled |
| BEH-003 | Fermi function | `FUNCTIONS.fermi` | **Built (VS-2)** — T1 characterized, fixture not yet recovered | `CAP-20260810-FERMI` SHA-256 `6f35eadc…` | `/document-legacy` |
| BEH-004 | Numerical derivative | `TOOLS.deriv` | Optional (VS-4); T1 characterized | `CAP-20260810-DERIV` SHA-256 `8a8879bc…`; input `numutils/test/xy2.data` | `/document-legacy` if added |
| BEH-005 | Integer/real range | `TOOLS.arange` | Reserve | Fidelity `arange-5` **did not call** `arange` | none scheduled |

---

## 3. Library families

One family is built (`BEH-040`, VS-3). The rest are reserve: retained in principle by ADR-005 §3, not planned work.

| ID | Family | Public names (non-exhaustive) | Module | Status | Notes |
|----|--------|-------------------------------|--------|--------|-------|
| BEH-010 | Remaining `TOOLS` grids/helpers | `powspace`, `upmspace`, `upminterval`, sort/uniq/shift; Bethe DOS; convergence checks | `TOOLS` | Reserve | |
| BEH-020 | Remaining `FUNCTIONS` | `heaviside`, `step`, `sgn`, `wfun`, `zerf` | `FUNCTIONS` | Reserve | Bundled special-function **internals** not public |
| BEH-030 | Quadrature | `trapz`, `simps`, `kronig`, `kramers_kronig`, `finter_*` | `INTEGRATE` | Reserve | QUADPACK behind facade |
| BEH-040 | Dense linear algebra | `matrix_inverse*`, `m_invert*`, `matrix_diagonalize`, `solve_linear_system` | `MATRIX` | **Built (VS-3)** | No T1 evidence. Needs fixture capture, a numeric-contract ADR, and a provider decision behind the numeric port |
| BEH-050 | FFT / GF transforms | `cfft_1d_*`, `fftgf_*`, `fftff_*` | `FFTGF` NR | Reserve | Permitted VS-3 substitute (ADR-008 §5); FFTPACK backend retired |
| BEH-060 | Nonlinear solve | `broydn`, `fzero`, `zbrent`, `fsolve`, `ffsolve` | `OPTIMIZE` | Reserve | Current facade; not historical `ZEROS` |
| BEH-070 | Interpolation | `poly_spline`, `cubic_spline`, `linear_spline`, `extract`, `interp_gtau` | `SPLINE` | Reserve | |
| BEH-080 | Random and stats | `rand`/`nrand`/…, histogram, moments, covariance | `RANDOM`, `STATISTICS` | Reserve | Sequence vs statistical parity undecided |
| BEH-090 | Many-body helpers | Green-function types, `pade_analytic_continuation`, square-lattice helpers | `GREENFUNX`, `PADE`, `SQUARE_LATTICE` | Reserve | |
| BEH-100 | File and plot data | `file_*`, `data_open`/`data_store`, `splot`/`sread` | `IOTOOLS` | **Reshaped** | Not a module translation. Under ADR-007 this is a **driven port with adapters**; these procedures are evidence of *what data* crossed the boundary, not a byte specification. No legacy-format fidelity required |
| BEH-110 | Diagnostics / timer / CLI parsing | `msg`/`warning`/`error`, `start_timer`, `PARSE_CMD` | `COMMON_VARS`, `TIMER`, `PARSE_CMD` | **Dissolved** | ADR-007: `PARSE_CMD` and `TIMER` dropped; failure *classification* is a typed domain failure, while message text, channel, styling, and exit status are adapter concerns |

---

## 4. CLI programs — recovered evidence, not planned work

**ADR-006 retired every program below from build scope.** This section is retained because these programs are often the clearest surviving evidence of how a library procedure is called and what its arguments mean, and because reading them is what surfaced several recorded contradictions.

Use these rows as **input to `/document-legacy`** for the corresponding library behavior. Do not schedule a story against them.

| ID | CLI | Library behavior it exercises | Evidentiary value |
|----|-----|-------------------------------|-------------------|
| BEH-201 | `linspace` | BEH-001 | Argument aliases and defaults; help-vs-code contradictions |
| BEH-202 | `logspace` | BEH-002 | Endpoint and base handling |
| BEH-203 | `arange` | BEH-005 | The only observed invocation shape; fidelity `arange-5` did not call the library procedure |
| BEH-204 | `fermi` | BEH-003 | Sweep range and beta parameterization — **relevant to VS-2** |
| BEH-205 | `deriv` | BEH-004 | Tabulated input format and spacing assumptions |
| BEH-206 | `spline` | BEH-070 | Interpolation mode selection |
| BEH-207 | `fftgf` | BEH-050 | Complex-column order (DEF-305 evidence); relevant if VS-3 is swapped to FFT |
| BEH-208 | `wmatsubara` | BEH-010 / many-body | Matsubara grid construction |
| BEH-209 | `pade` | BEH-090 | Continuation parameters |
| BEH-210–213 | `random`, `histogram`, `kdensity`, `numstat` | BEH-080 | Seeding and binning conventions |
| BEH-214 | `func` | — | `libmatheval` grammar. **No library behavior behind it**; the evaluator is retired (ADR-006) |
| BEH-215 | `splot` | BEH-100 | What data crosses the I/O boundary — useful for the ADR-007 driven port, though the format is not reproduced |
| BEH-216 | `ffcmplx` | BEH-100 | DEF-304 `sread` call-site anomaly |
| — | `vfplot` | — | **Retired** (ADR-005); callee source absent |

---

## 5. Build order

See `docs/modernization/migration-plan.md`. Summary: VS-1 `BEH-001` → VS-2 `BEH-003` → VS-3 `BEH-040`, with `BEH-004` optional and `BEH-050` a permitted substitute for VS-3. Reserve families have no order because they have no schedule.

---

## 6. Retired (do not plan C# stories)

| Surface | Authority |
|---------|-----------|
| All sixteen `numutils/src/` CLI programs (BEH-200–BEH-216) | ADR-006 |
| Managed expression evaluator (`func` / `libmatheval`) | ADR-006 |
| Gnuplot wrapping (`splot`) | ADR-006 |
| `PARSE_CMD` as library surface; `TIMER` as product surface | ADR-007 |
| Legacy file-format fidelity for `splot`/`sread` | ADR-007 |
| `vfplot` / DISLIN / `DLPLOT` | ADR-005 |
| FFTPACK FFT backend | ADR-005 |
| `CHRPACK` | ADR-005 |
| `bin/setup_sf.sh` as a product entrypoint | ADR-005 |
| Fortran module ABI as a supported consumer contract | ADR-005 |
| Unexported special-function internals | ADR-005 |

---

## 7. Links

- PURPOSE / DOMAIN: `docs/PURPOSE.md`, `docs/DOMAIN.md`
- Oracle: `docs/modernization/oracle.md`
- ADRs: ADR-004, ADR-005, ADR-006, ADR-007, ADR-008
- Recovered behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Migration plan: `docs/modernization/migration-plan.md`

*Created: 2026-08-19 | Sequenced: 2026-08-19 | ADR-005 name coverage: 2026-08-19 | Rescoped per ADR-006/007/008: 2026-08-19*
