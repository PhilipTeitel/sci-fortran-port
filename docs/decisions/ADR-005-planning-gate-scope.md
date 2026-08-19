# ADR-005: Planning-gate scope, retirements, and POC defaults

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

`/plan-migration` was blocked because supported scope, process boundary, dependency routes, and a library-wide behavior catalog were unresolved. The owner has now authorized a whole-library C# port (ADR-004). This ADR records the defaults required to plan code production without pretending unexecuted surfaces are T1 or copying missing source.

Inventory is taken from `src/SCIFOR.f90` and public exports of its imported modules, plus `numutils/src/` programs, at revision `e586903`.

---

## Decision

### 1. Oracle baseline (extends ADR-001)

For this POC, the accepted source and environment for **all retained surfaces** is the 2026-08-10 operational probe:

- Commit `e586903a26cc50ca8942f20ca3bccbd8814e6252`
- GNU Fortran 16.1.0, OpenBLAS 0.3.34, `FFT_BACKEND=NR`, `LC_ALL=C`, pinned single-thread BLAS

Executable T1 evidence still covers only the fidelity-driver corpus (linspace, logspace, fermi, deriv). Every other retained surface is T3 until its slice captures or records fixtures. The probe is the **planning baseline**, not a claim that those other surfaces already have parity evidence.

### 2. Public / process boundary (extends ADR-002)

| Surface | Target |
|---------|--------|
| Library (`SCIFOR` public API) | Host-neutral managed C# API (the product) |
| CLI utilities | Driving adapters over the same ports; Fortran `PARSE_CMD` / stdout formatting is adapter work, not domain arithmetic |
| HTTP / ASP.NET | Optional later adapter; not required to start code production |
| Fortran `.mod` / `libscifor.a` ABI | **Not retained** |

Fortran `error`/`STOP` maps to typed domain failures at the port (ADR-002). Host exit codes and HTTP Problem Details stay adapter concerns.

### 3. Retained library modules

Port every public procedure of:

| Module | Representative public surface |
|--------|-------------------------------|
| `TOOLS` | `linspace`, `logspace`, `arange`, `powspace`, `upmspace`, `upminterval`, `deriv`, sort/uniq/shift, Bethe helpers, convergence checks |
| `FUNCTIONS` | `heaviside`, `step`, `fermi`, `sgn`, `wfun`, `zerf` |
| `INTEGRATE` | `trapz`, `simps`, `kronig` / `kramers_kronig`, `finter_*` |
| `MATRIX` | `matrix_inverse*` , `m_invert*`, `matrix_diagonalize`, `solve_linear_system` |
| `FFTGF` (NR backend) | `cfft_1d_*`, `fftgf_*`, `fftff_*` |
| `OPTIMIZE` | `broydn`, `fzero` / `zbrent`, `fsolve` / `ffsolve` |
| `SPLINE` | `poly_spline`, `cubic_spline`, `linear_spline`, `extract`, `interp_gtau` |
| `RANDOM` | `nrand`, `irand`, `drand`, `crand`, `rand`, `init_random_number`, `random_order` |
| `STATISTICS` | histogram helpers, moments, covariance |
| `GREENFUNX` | allocate/identity/less/gtr helpers |
| `PADE` | `pade_analytic_continuation` |
| `SQUARE_LATTICE` | dispersion/velocity/grid helpers |
| `IOTOOLS` (`IOFILE`, `SLPLOT`, `SLREAD`) | file metadata, `splot`/`sread` data helpers |
| `PARSE_CMD` | CLI adapter support |
| `COMMON_VARS` | diagnostics mapped at the port; ANSI helpers as adapter concerns |
| `TIMER` | `start_timer` / `stop_timer` / `eta` / `print_bar` as adapter/progress ports |

`VECTORS` is retained if referenced by retained modules; it is not a `SCIFOR` import and is not a standalone product surface.

The large bundled special-function include (`functions_special_funcs.f90`) is **not** a public `FUNCTIONS` export. It is out of the managed API until a later owner decision adds specific routines.

### 4. Retained CLI adapters

Default `all` programs: `deriv`, `kdensity`, `numstat`, `splot`, `func`, `wmatsubara`, `fftgf`, `arange`, `pade`, `logspace`, `linspace`, `fermi`, `spline`, `random`, `histogram`.

Also retain `ffcmplx` (source and Make target exist; omitted from `all`).

Each CLI is an adapter over the library port, not a second arithmetic implementation.

### 5. Explicit retirements

| Surface | Why |
|---------|-----|
| `vfplot` / `DLPLOT` / DISLIN | Callee source and linkage are absent |
| `FFTGF_FFTPACK` backend | Required `zffti`/`zfftf`/`zfftb` definitions are absent |
| `CHRPACK` | Unused in the mapped build |
| Interactive `bin/setup_sf.sh` | Broken installer, not a product API |
| `ifort` / MKL / FFTW3 as required product providers | Probe used gfortran + OpenBLAS + NR |
| MPI/OpenMP as operational requirements | No MPI/OpenMP calls in scanned source |
| Historical `ZEROS` Fortran name | Current facade imports `OPTIMIZE`; no consumer inventory. Provide C# names for the current optimize surface |

### 6. POC dependency defaults

Blocked ledger routes are decided as follows for **planning and implementation**, not as production legal clearance:

| Concern | Default |
|---------|---------|
| BLAS/LAPACK (`MATRIX`) | Reproduce probe-linked OpenBLAS behavior behind a numeric port; implement with a managed provider or approved native wrapper. Do not copy `mkl_lapack.fi` into C#. |
| FFT | Reproduce NR-selected `FFTGF` contracts; reimplement behind a transform port. Do not copy Numerical Recipes source into the target tree. |
| QUADPACK / MINPACK / spline / NR helpers | Reimplement accepted facades in C# from characterization, not by pasting vendored Fortran. |
| `func` / `libmatheval` | Retain the CLI job; substitute a managed expression evaluator with an explicit grammar recovered from characterization. |
| Gnuplot / `splot` | Wrap script/data generation; do not run plot processes inside ASP.NET. |
| Gzip / filesystem helpers | Reimplement with `System.IO` / `GZipStream`. |

### 7. Numeric comparison until per-behavior ADRs exist

- FIX-001 remains exact parsed equality (ADR-003).
- Other T1 fidelity sections (logspace, fermi, deriv) use exact parsed equality of probe-repeatable values once fixtures are recovered; do not use profile `1e-6` to hide those cases.
- Unexecuted surfaces keep the profile `1e-6` only as a **non-authoritative planning default** until their slice ADR.

---

## Consequences

**Positive**

- `/plan-migration` can sequence strangler slices against a closed retained/retired list.
- Missing-source surfaces will not spawn blocked stories.

**Negative / costs**

- Special-function internals and FFTPACK/`vfplot` will not appear in C#.
- CLI text/locale/complex-column contracts still need per-adapter ADRs when those slices are refined.

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Retain Fortran ABI | Unknown consumers; wrong shape for a C# library POC |
| Require MKL/FFTW/DISLIN | Not what the probe ran; DISLIN/`DLPLOT` cannot be built |
| Include bundled special-function internals in the first catalog | Not exported by `FUNCTIONS`; would explode unverified scope |

---

## Links

- Catalog: `docs/modernization/behavior-catalog.md`
- Dependency ledger: `docs/modernization/dependency-ledger.md`
- Related: ADR-001–004
