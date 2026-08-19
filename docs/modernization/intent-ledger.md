# Intent Ledger

**Legacy repo:** `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy` (strictly read-only; explicit user override of configured `../scifortran-legacy`)
**Date:** `2026-08-10`
**Sources mined:** Build-related Git delta/history, current build and fidelity scripts, current configuration, facade/FFT source closure, and existing assessment citations only

---

## 1. Intent statements

| ID | Statement | Affected area | Evidence grade | Source | Confidence | Downstream artifact |
|----|-----------|---------------|----------------|--------|------------|---------------------|
| INT-001 | This checkout is an experiment cloned from another SciFortran repository; the README does not identify it as a production release. | Baseline identity | E2 documented | `README.md:1` | high for checkout description; low for production status | ASSESSMENT / TBD |
| INT-002 | The merged change set was intended to make the application successfully build, but that success claim is commit-message-only in this refresh. | Buildability | E4 inferred | Merge `e586903a26cc50ca8942f20ca3bccbd8814e6252`, subject: `Merge branch 'initial-build' containing changes required to get app to successfully build.` | low until Implementer execution | ASSESSMENT / oracle |
| INT-003 | The merged build entrypoint is intended to build the SciFor library and a fidelity driver for port-baseline testing, defaulting to gfortran, Homebrew-style OpenBLAS paths, and NR FFT. | Core build / port evidence | E3 code-derived | `scripts/build.sh:1-7,26-47,50-76,79-99` | medium for mechanism; no success claim | legacy map / oracle |
| INT-004 | The historical source change intended to rename `ZEROS` to `OPTIMIZE` “(scipy convention)”; current `master` now closes the facade reference by importing `OPTIMIZE`. | Public facade / source closure | E4 inferred for rationale; E3 code-derived for mechanism | Commit `722cde3083140aa8b36c927f0522279fcadb6350`; `src/SCIFOR.f90:14-16`; `src/Makefile:6,63-87`; `src/OPTIMIZE.f90:1-6` | medium for rename intent; low for consumer compatibility | dependency ledger / TBD |
| INT-005 | Current `master` and the merged build default select the Numerical Recipes FFT implementation while retaining FFTW3 and MKL as selectable variants. | FFT build variant | E3 code-derived | tracked symlink `src/FFTGF.f90 -> FFTGF_NR.f90`; `scripts/build.sh:7,50-62` | high for current selection; low for production acceptance | dependency ledger / oracle |
| INT-006 | The merged fidelity script is intended to compare five numeric sections with a default absolute tolerance of `1e-10`; this threshold is a script choice, not an accepted parity requirement. | Fidelity checks | E3 code-derived | `scripts/fidelity.sh:11,27-78,161-183` | high for script behavior; low for compatibility meaning | oracle / TBD |
| INT-007 | The five checked-in expected references are not a recorded legacy-output corpus: four are regenerated from Python formulas and one is copied from the historical `xy2.deriv` file. | Fidelity provenance | E3 code-derived / E4 inferred | `scripts/fidelity.sh:105-159`; `fidelity/golden/*.txt`; `numutils/test/xy2.deriv` | high for generation path; low for historical file provenance | oracle |
| INT-008 | Generated build/fidelity products are intended to stay out of version control, and prior tracked static archives were removed in the merged delta. | Artifact hygiene | E3 code-derived / E4 inferred | `.gitignore:4-13`; `git diff --name-status 5e4e6a3..e586903` | high for mechanism; medium for inferred rationale | legacy map |
| INT-009 | Owner selected library `linspace` as the first code slice, accepted the 2026-08-10 probe, required hexagonal architecture and a managed API, then corrected purpose to a whole-library C# port POC (licensing accepted; `/plan-migration` unblocked). | Product purpose / first slice | E2 documented | Owner decisions 2026-08-19; ADR-001–005 | high | PURPOSE / catalog / ASSESSMENT |
| INT-010 | `/plan-migration` sequences retained catalog IDs as SL-001–SL-025; first code-production command is `/refine-feature` on BEH-001; catalog-only rows stay document→refine→design→implement; ADR-005 TOOLS convergence checks travel with SL-014. | Migration sequencing | E2 documented | `docs/modernization/migration-plan.md`; catalog 2026-08-19 | high | migration plan / BEH-001 |

## 2. Release-note and support commitments

None yet. No release notes, changelog, tag, support record, or accepted build transcript was inspected in this tightly scoped refresh.

## 3. Commit-history signals

| Commit / tag | Signal | Evidence grade | Why it matters | Confirmation needed |
|--------------|--------|----------------|----------------|---------------------|
| `e586903` (2026-08-10) | Merges `initial-build` into `master` with a subject claiming changes required for a successful build. | E1 verified for merge/parents; E4 inferred for success intent | The previously branch-only build mechanisms are now part of authoritative repository `master`. | Yes — Implementer must compile/link/run from a clean disposable copy before success is factual. |
| `9b19827` (2026-08-09) | “initial builds but bug fixes were also done”; adds build/fidelity scripts and references, switches facade/backend, adjusts environment/linking, deletes tracked archives. | E3 code-derived for diff; E4 inferred for message | Build reconstruction and behavior-affecting changes are mixed in one commit. | Yes — separate build enablement from accepted behavior/defect decisions. |
| `722cde3` (2013-05-16) | Says `ZEROS` was renamed to `OPTIMIZE` for SciPy convention; diff renames/adds optimization source and Make target but leaves `SCIFOR` importing `ZEROS`. | E3 code-derived for diff; E4 inferred for rationale | Explains the source-closure inconsistency that current merge repairs. | Yes — confirm downstream API expectations; build closure does not prove compatibility. |
| `2580b6c` (2012-03-13) | Commit message claims FFT changes were extended across MKL/NR/FFTW3 and tested successfully in one Matsubara application. | E4 inferred | Historical cross-backend confidence is weak evidence relevant to the newly selected NR default. | Yes — recover executable evidence/environment or rerun accepted backend fixtures. |
| `298c79a` (2012-03-13) | Commit message says a structural update was preparing for portability and extended MKL FFT work to alternatives. | E4 inferred | Provides historical portability intent but does not select the present production backend. | Yes — owner confirmation and accepted baseline/backend. |

## 4. User-documentation signals

| Document | Section | Statement | Evidence grade | Behavior / domain implication |
|----------|---------|-----------|----------------|-------------------------------|
| `README.md` | Entire file | The repository is “part of an experiment” cloned from `https://github.com/liangjj/SciFortran.git`. | E2 documented | Does not establish a release, supported build, or production compatibility baseline. |

## 5. Tensions / conflicts

| Conflict | Sources | Impact | Required resolution |
|----------|---------|--------|---------------------|
| Merge and branch subjects claim build success, but this refresh did not run a build and found only ignored partial COMVARS outputs; final archives, fidelity executable, and full capture are absent. | E4 inferred / E1 verified — commits `e586903`, `9b19827`; ignored-status and path checks on 2026-08-10 | Buildability has stronger source evidence but no verified success. | Implementer runs and records a clean, contained build/link/run without modifying the authoritative checkout. |
| The new build script mutates the tracked FFT symlink and creates/rewrites in-tree build metadata and outputs; the fidelity script rewrites tracked expected-reference files. | E3 code-derived — `scripts/build.sh:61,65-76`; `scripts/fidelity.sh:105-175` | Neither script may be executed directly against the strict read-only legacy repository. | Execute only in a disposable writable copy and verify source/tree deltas. |
| Current source closure uses `OPTIMIZE`, consistent with the historical rename, but no consumer evidence says whether `ZEROS` names/behavior may be retired. | E3 code-derived / E4 inferred / E5 unknown — commit `722cde3`; `src/SCIFOR.f90:14-16` | A build fix could still change the supported public API. | Inventory consumers and record the accepted facade contract. |
| Current tree/build default selects NR, but FFTW3/MKL remain selectable, historical messages discuss all three, and no merged fidelity case exercises FFT. | E3 code-derived / E4 inferred / E5 unknown — `scripts/build.sh:7,50-62`; `fidelity/driver.f90:1-67`; commits `2580b6c`, `298c79a` | Backend-dependent signs, normalization, accepted lengths, precision, dependencies, and provenance remain unverified. | Select/accept the production backend and capture backend-specific evidence. |
| “Golden” naming suggests recorded truth, but four references are Python formulas and the fifth is a copied file with unknown generating environment. | E3 code-derived / E4 inferred — `scripts/fidelity.sh:105-159` | Treating these files as E1/T2 evidence would overstate parity confidence. | Preserve them only as candidate expectations until provenance or accepted execution establishes authority. |
| The configured path is `../scifortran-legacy`, while the explicit user path is `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy`. | E2 documented / E3 code-derived — binding request; `[target] .cursor/workflow.config.yml:2` | Automated phases can inspect the wrong checkout. | Correct the profile or continue requiring the explicit override. |

## 6. Open intent questions

- [x] Is `master` at `e586903` an accepted production/parity baseline, or only the current repository baseline? **POC (2026-08-19):** planning/oracle baseline for all retained surfaces (ADR-005). Not a public production release.
- [x] Which exact OS, architecture, compiler/version, flags, BLAS/LAPACK binaries, and environment define an accepted successful build? **POC:** the recorded 2026-08-10 probe environment (ADR-001/005).
- [x] Is the NR FFT selection intentional for production compatibility, and is its source/provenance approved? **POC:** NR is the probe backend to reproduce; do not copy NR source (ADR-005).
- [x] Does the supported facade replace `ZEROS` with `OPTIMIZE`, and what do downstream Fortran consumers compile against? **POC:** port current `OPTIMIZE`; Fortran ABI/`ZEROS` not retained (ADR-005).
- [ ] Are the build and fidelity scripts diagnostic reconstruction assets only, or intended supported entrypoints?
- [x] Which fidelity cases are accepted behaviors, and why does `arange-5` not invoke the legacy `arange` implementation? **`linspace` is BEH-001 (accepted). `logspace`/`fermi`/`deriv` are BEH-002–004 (T1 hash). `arange-5` remains a driver loop, not legacy `arange` (BEH-005).**
- [x] What evidence authorizes `1e-10`, versus the workflow profile's provisional `1e-6` relative/absolute values? **Neither is used for FIX-001; exact parsed equality (ADR-003). Other T1 exact once recovered; `1e-6` is a non-authoritative planning default (ADR-005).**
- [ ] Can `numutils/test/xy2.deriv` be tied to an exact legacy invocation and environment, or must it remain an unprovenanced candidate?

## 7. Links

- Purpose document: `docs/PURPOSE.md`
- Domain model: `docs/DOMAIN.md`
- Behavior catalog: `docs/modernization/behavior-catalog.md`
- Migration plan: `docs/modernization/migration-plan.md`
- Behavior: `docs/modernization/behaviors/BEH-001-linspace.md`

*Created: 2026-08-10 | First-slice intent: 2026-08-19 | Whole-library purpose: 2026-08-19 | `/plan-migration`: 2026-08-19*
