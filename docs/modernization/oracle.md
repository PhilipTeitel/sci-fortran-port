<!-- Oracle contract:
- Classify the legacy oracle tier before planning parity work.
- T3 documented-only cannot claim independent parity.
- If a section has no content yet, write `None yet.`.
-->

# Legacy Oracle

**Legacy repo:** `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy` (strictly read-only; explicit user override of configured `../scifortran-legacy`)
**Oracle tier:** `T3 documented-only`
**Date:** `2026-08-09`

---

## 1. Oracle tier decision

| Tier | Selected? | Evidence | Consequence |
|------|-----------|----------|-------------|
| T1 executable | no | `E1 verified` — no legacy executable, object, module, or archive is present; no legacy binary was built or run in this probe; repeated execution was not demonstrated. Probe inventory on 2026-08-09; `docs/modernization/legacy-map.md:18,20,152`. | Repeated legacy-vs-new comparison is not currently possible. |
| T2 recorded | no | `E3 code-derived / E4 inferred` — `numutils/test/` has no runner, assertions, generation provenance, accepted baseline, or tolerance. Branch-only `initial-build` regenerates four “goldens” from Python formulas and copies the fifth from `numutils/test/xy2.deriv`; its driver does not call legacy `arange`. `docs/modernization/legacy-map.md:146-155`; `docs/modernization/intent-ledger.md:27,60,86-87`; `initial-build:scripts/fidelity.sh:95-154`; `initial-build:fidelity/driver.f90:26-31`. | Existing data cannot be treated as a frozen observed-output corpus. |
| T3 documented-only | yes | `E1 verified / E3 code-derived / E5 unknown` — the current baseline cannot perform a clean-shell build dry run because `src/options.inc` is absent; an include-path-assisted dry run only prints a proposed build graph and does not compile, link, or execute it. The authoritative branch, compiler, native-library versions, FFT backend, and missing `ZEROS` disposition remain unresolved. Probe commands below; `docs/modernization/legacy-map.md:149,161-165`; `docs/modernization/translation-gaps.md:33,49-50`. | Independent parity is unprovable; future behavior work requires accepted documentation/data or a newly established T1/T2 oracle. |

**Decisive evidence:** no repeated actual execution exists for T1, and no credibly observed and provenanced frozen output exists for T2. The profile-declared `T3 documented-only` classification is therefore retained.

**Probe record (all commands were non-executing with respect to legacy binaries):**

| Probe | Result | Evidence / safety note |
|-------|--------|------------------------|
| Source, build, history, dependency, and test-asset inspection | Completed before tool or build checks. | `E1 verified` — read-only inspection of current `master` plus `git show`/`git ls-tree` for `initial-build`; no branch checkout occurred. |
| Baseline identity and status | `master` at `5e4e6a3938a7ef6e1163a34b3236be7ea7eb22f9`; no tags; tracked tree unchanged. Pre-existing untracked `numutils/src/options.inc` and `numutils/src/sfmake.inc` remain. | `E1 verified` — `/usr/bin/git -C <legacy> rev-parse HEAD`, `branch --list`, `tag --list`, `diff --exit-code`, and status before/after probing. |
| Host/tool availability | Host is macOS 26.5.2 (`25F84`), Darwin 25.5.0, arm64. GNU Fortran 16.1.0 (`aarch64-apple-darwin25`), GNU Make 3.81, Bash 3.2.57, Apple Git 2.50.1, Xcode SDK 26.5, `ar`, `ranlib`, and `rsync` are present. Intel `ifort`/`ifx` were not found at standard Intel/local paths. | `E1 verified` — version/path commands only; no source compilation. |
| Native-library availability | Homebrew reported GCC 16.1.0, OpenBLAS 0.3.34, and LAPACK 3.12.1 locally. No installed FFTW, libmatheval, or Gnuplot path was established. | `E1 verified` for reported local versions; the Homebrew query also attempted a metadata refresh that was denied with HTTP 403, changed no packages, and was stopped. This does not establish ABI or legacy compatibility. |
| Shell syntax | `bash -n` passed for `bin/setup_sf.sh` and `bin/sciforvars.sh`. | `E1 verified` — syntax only; neither script was sourced or executed. |
| Clean core build dry run | `make -n -C <legacy>/src all` stopped at missing `options.inc`. | `E1 verified` — no recipe ran and no file was written. |
| Include-assisted core dry run | `make -n -I <legacy>/include -C <legacy>/src all` printed a complete-looking recipe list and exited 0. | `E1 verified` for Make graph expansion only. It would rewrite `scifor_version.inc`, compile in-tree objects/modules, and move archives if run; it did not test source acceptance, missing module resolution, linking, or runtime behavior. |
| CLI build dry run | `make -n -C <legacy>/numutils/src all` printed commands using the two untracked local include files, static linking, `-lscifor -llapack -lblas`, and `-lmatheval` for `func`. | `E1 verified` for graph expansion only; checkout-local configuration prevents a clean-baseline claim. |
| Containment tooling | Docker client 29.6.2 is installed, but daemon access was denied at the sandboxed Unix socket. QEMU exists but its version probe aborted on a sandboxed host `sysctl` assertion. | `E1 verified` — neither provides a documented, usable containment environment in this probe. |
| Actual setup, build, link, or legacy execution | Not attempted. | The supplied setup writes into the legacy tree, compiles local dependencies, creates links, and has known path/name defects. No accepted baseline or operational containment was available. |
| Harness or fixture creation | Not attempted. | Probe mode forbids harness code and fabricated fixtures. |

## 2. Legacy execution environment

| Requirement | Value | Evidence | Notes |
|-------------|-------|----------|-------|
| OS / image | Authoritative OS/image `TBD`; probe host is macOS 26.5.2 / Darwin 25.5.0 / arm64 | `E1 verified` — host probes; `E5 unknown` — production environment | Checked-in configuration names Darwin and Linux paths but selects old Intel tooling. The probe host is not accepted as the legacy runtime. |
| Runtime / compiler | Production compiler/version `TBD`; source names `ifort` or `gfortran`; probe host has GNU Fortran 16.1.0 and no detected Intel compiler | `E1 verified / E3 code-derived / E5 unknown` — version probes; `include/options.inc:1-16`; `etc/library.conf:1-9` | Compiler-dependent kind widths, module ABI, formatting, flags, and floating-point behavior must be pinned before execution evidence is trusted. |
| Build tooling | GNU Make plus Bash, Git, `ar`, `ranlib`, `rsync`, and Unix commands; clean build recipe unresolved | `E1 verified / E3 code-derived` — probe versions; `src/Makefile:1-315`; `bin/setup_sf.sh:1-205` | The Make graph writes into source/output directories and embeds Git state. Build only in a disposable writable snapshot, never the legacy checkout. |
| Native numeric libraries | BLAS/LAPACK required; current source selects MKL DFT; FFTW3/NR/FFTPACK alternatives exist; exact providers/versions `TBD` | `E3 code-derived / E5 unknown` — `include/sfmake.inc:7-29`; `src/FFTGF.f90:1-8`; `docs/modernization/dependency-ledger.md:54-60` | Probe-host OpenBLAS/LAPACK presence does not select a production backend or prove compatible results. MKL/FFTW/libmatheval/DISLIN requirements remain unresolved by surface. |
| Network | Deny during build and execution; no runtime network interface found in source | `E3 code-derived` — `docs/modernization/legacy-map.md:140`; `E4 inferred` — containment rule | Dependency acquisition, if later approved, must occur separately with pinned checksums. The oracle itself should not have network access. |
| Data set | Sanitized synthetic numeric inputs only; current `numutils/test/` assets are candidates, not accepted fixtures | `E3 code-derived / E4 inferred` — `docs/modernization/legacy-map.md:113-123,146-155` | No PII is evident in inspected numeric files, but provenance, generator, input/output roles, and acceptance authority are missing. |
| Secrets | None evidenced in source; production requirements `TBD` | `E3 code-derived / E5 unknown` — repository inventory and dependency ledger | Do not mount host credentials, SSH agents, cloud config, or the user's home directory into containment. |

The environment must also record source revision, branch, architecture, compiler and runtime versions, all flags, numeric-library binaries and hashes, FFT backend, locale, timezone, environment variables, and exact invocation before any output can be graded `E1 verified`.

## 3. Containment and security

| Risk | Isolation control | Evidence | Owner |
|------|-------------------|----------|-------|
| Unknown and obsolete native toolchain/dependencies (`ifort`, MPICH2-era paths, unpinned BLAS/LAPACK/FFT) | Build and run in a pinned, disposable container or VM with no network, non-root UID, dropped privileges, read-only root filesystem, bounded CPU/memory/PIDs/time, and a dedicated writable scratch volume | `E3 code-derived / E5 unknown` — `docs/modernization/dependency-ledger.md:113-132` | Legacy/oracle operator |
| Legacy source must remain immutable | Mount an accepted source snapshot read-only; perform all generated links, revision stamping, objects, modules, archives, and executables in a separate writable copy or overlay; verify source hash and diff before/after | `E1 verified / E3 code-derived` — Make dry runs; `src/Makefile:8-29`; `initial-build:scripts/build.sh` mutates backend links and local includes | Legacy/oracle operator |
| Setup script mutates source and local dependency trees and contains path/name defects | Do not run `bin/setup_sf.sh`; replace it only after documenting a non-interactive, reviewed containment recipe outside the legacy repo | `E3 code-derived` — `bin/setup_sf.sh:80-205`; `docs/modernization/legacy-map.md:162,199-201` | Legacy maintainer |
| Proprietary or unclear source/dependency licensing | Exclude checked-in Intel-confidential interfaces and unresolved Numerical Recipes/mixed-provenance material from redistribution; obtain legal approval before image creation or artifact export | `E3 code-derived / E5 unknown` — `docs/modernization/dependency-ledger.md:121-129`; `docs/modernization/translation-gaps.md:54` | Legal/product owner |
| Untrusted expressions, filenames, and shell-backed file operations | Use synthetic allowlisted inputs in an isolated scratch directory; prohibit path traversal/symlinks; disable `func`, plotting, gzip, and file-mutating surfaces until individually reviewed | `E3 code-derived` — `docs/modernization/legacy-map.md:131-138,168`; `docs/modernization/translation-gaps.md:53,55-57` | Security/oracle operator |
| Host data or secret exposure | Mount no home directory, production data, credentials, sockets, SSH agent, or cloud metadata; export only reviewed numeric/text captures and an environment manifest | `E4 inferred` from the absence of an established data/secrets contract | Security/oracle operator |
| Current containment tools are not operationally verified | Establish and test the pinned container/VM separately before any legacy build; current Docker daemon and QEMU checks are insufficient | `E1 verified` — sandboxed Docker socket denial and QEMU assertion during probe | Oracle operator |

## 4. Invocation contract

No `BEH-NNN` catalog exists yet. The rows below are candidate invocation surfaces recovered from source, not accepted behavior contracts.

| Behavior | Legacy command / interaction | Inputs | Outputs | Exit / error behavior |
|----------|------------------------------|--------|---------|-----------------------|
| `BEH-TBD` — sequence generators | `arange`, `linspace`, `logspace`, or `wmatsubara` with per-command `NAME=value` arguments | Numeric range/count/base/type options | Whitespace-formatted numeric values on stdout | Help aliases call `STOP`; invalid/missing argument behavior and exit status are `E5 unknown`. `E2/E3` — `docs/modernization/legacy-map.md:89-90,129`. |
| `BEH-TBD` — stream transforms | Pipe numeric rows to `deriv`, `fermi`, `spline`, or `fftgf` plus `NAME=value` options | stdin numeric columns; transform/interpolation options | Numeric rows on stdout | EOF ends input; malformed values, diagnostics stream, partial output, and exit mapping are unverified. `E2/E3/E5` — `docs/modernization/legacy-map.md:92-95,130`. |
| `BEH-TBD` — statistics | Pipe samples to `histogram`, `kdensity`, or `numstat` plus options | One or more numeric columns, ranges/bins/bandwidth/statistic selection | Density, histogram, moment, covariance, or correlation rows on stdout | Empty/malformed input and ordering/tie behavior are unverified. `E2/E3/E5` — `docs/modernization/legacy-map.md:97-99`. |
| `BEH-TBD` — random generation | `random` with count/distribution/parameter options | Count, distribution, moments/range; no explicit stable seed contract recovered | Random values on stdout | Clock-based seeding makes repeated output nondeterministic; invalid distribution behavior is unverified. `E3/E5` — `src/RANDOM.f90:197-208`; `docs/modernization/legacy-map.md:91`. |
| `BEH-TBD` — Padé continuation | `pade` with input/output/range/rank/broadening options | Complex Matsubara data via file or stream depending on options | Complex real-frequency data to file/stdout | Complex-column convention, convergence/failure, overwrite, and exit behavior are unresolved. `E2/E3/E5` — `docs/modernization/intent-ledger.md:19,71`; `docs/modernization/legacy-map.md:100`. |
| `BEH-TBD` — file expansion | `ffcmplx <input-file>` | Complex-valued text file | `.abs`, `.arg`, `.real`, and `.imag` siblings; prior outputs may be removed | Filesystem and malformed-input failure behavior are unverified. `E3` — `docs/modernization/legacy-map.md:101,131-132`. |
| `BEH-TBD` — plotting | Pipe grid data to `splot`, or invoke `vfplot` with a file | Numeric grid/vector data and plot options | Plot data/scripts or DISLIN output/window | Gnuplot/DLPLOT availability, process status, display behavior, and supported status are unresolved. `E2/E3/E5` — `docs/modernization/legacy-map.md:102-103,137-138`. |
| `BEH-TBD` — expression evaluation | Pipe X values to `func` with an expression option | Untrusted expression text plus numeric stdin | X/result rows on stdout | Parser errors, grammar, resource limits, locale, and null-handle behavior are unresolved; do not execute before threat review. `E3/E5` — `docs/modernization/legacy-map.md:96,136`; `docs/modernization/dependency-ledger.md:129`. |
| `BEH-TBD` — library API | Compile a Fortran consumer against compiler-specific modules and `libscifor.a` | Typed scalars, arrays, callbacks, files, and module state | Return values, mutated/allocated arrays, files, and console diagnostics | Consumer set, ABI, missing `ZEROS`/`OPTIMIZE` expectation, and `STOP` behavior are unresolved. `E3/E5` — `docs/modernization/legacy-map.md:104-105,193`; `docs/modernization/intent-ledger.md:84`. |

## 5. Fixture corpus

No fixture is promoted to an oracle corpus in probe mode. These are candidates requiring provenance and accepted input/output roles.

| Fixture ID | Behavior | Inputs | Expected legacy output | Evidence grade | Determinism notes |
|------------|----------|--------|------------------------|----------------|-------------------|
| `CAND-001` | `BEH-TBD` — derivative | `numutils/test/xy2.data` | Candidate `numutils/test/xy2.deriv`; not accepted as expected output | `E3 code-derived / E4 inferred` | Generator, compiler, flags, derivative options, timestamp, and tolerance are unknown. |
| `CAND-002` | `BEH-TBD` — numeric stream/grid operations | `numutils/test/{x,x2,y,y2,xy}.data` | None established | `E3 code-derived / E4 inferred` | File roles and producing commands are unknown. |
| `CAND-003` | `BEH-TBD` — histogram/density | `numutils/test/hist.dat` and/or `pdf.dat` | None established; `pdf.dat$` is a Grace project, not an asserted numeric golden | `E3 code-derived / E4 inferred` | Bin rules, normalization, ordering, and provenance are unknown. |
| `CAND-004` | `BEH-TBD` — plotting | `numutils/test/{plot1,plot2,new.plot,prova.plot,plot_prova.plot}` | Manual data/script artifacts only; no accepted image or byte-level output | `E3 code-derived / E4 inferred` | Gnuplot/DISLIN version, terminal, fonts, locale, and manual workflow are unknown. |
| `CAND-005` | `BEH-TBD` — five branch fidelity cases | `initial-build:fidelity/golden/*` and driver inputs | Not accepted: four values are formula-generated, one is copied, and the driver does not invoke all named legacy surfaces | `E3 code-derived / E4 inferred` | Branch is not the accepted baseline; its absolute `1e-10` threshold has no acceptance authority. |

Promotion requires an accepted baseline and environment, exact invocation, sanitized inputs, captured stdout/stderr/files and exit status, cryptographic hashes, repeat-run evidence, behavior/defect IDs, and user-approved comparison rules.

## 6. Tolerances and normalization

| Output kind | Rule | Applies to | Rationale | ADR / decision |
|-------------|------|------------|-----------|----------------|
| Numeric | Provisional configured defaults: relative `1e-6` and absolute `1e-6`; not a proven behavior contract | None yet; no accepted `BEH` or `FIX` | `E2 documented` — `.cursor/workflow.config.yml:44-47`; branch-only absolute `1e-10` conflicts and is not authoritative. | `TBD` — behavior-specific numerical comparison decision |
| Text | Provisional configured defaults: trim surrounding whitespace, normalize line endings, preserve case | None yet; no accepted `BEH` or `FIX` | `E2 documented` — binding request and `.cursor/workflow.config.yml:48-52`; compiler-specific Fortran formatting and per-surface complex ordering remain unknown. | `TBD` — per-surface text/culture/complex-column decision |
| Time / random | No accepted seed, frozen clock, or comparison window | Random, timestamp/version, and time-reporting candidates | Clock-seeded RNG and runtime timestamp/version functions prevent assuming repeatability. `E3` — `src/RANDOM.f90:197-208`; `src/COMVARS.f90:112-124`. | `TBD` — RNG sequence vs statistical-equivalence and time controls |

Configured defaults may be used to explore candidate data, but they cannot establish parity while the oracle is T3.

## 7. Determinism hazards

| Hazard | Affected behavior | Control | Evidence |
|--------|-------------------|---------|----------|
| `SYSTEM_CLOCK`-seeded process-global RNG | `random` and any library consumer of `RANDOM` | Capture seed state if possible; otherwise require user decision between exact sequence and statistical equivalence; run isolated single-process trials | `E3 code-derived / E5 unknown` — `src/RANDOM.f90:197-208`; `docs/modernization/translation-gaps.md:47` |
| Compiler, optimization, architecture, numeric kind, and IEEE-mode differences | All numerical routines | Pin source, compiler/runtime, flags, architecture, and floating-point environment; compare Debug/Release and repeat builds before accepting captures | `E3/E5` — `include/options.inc:1-16`; `docs/modernization/translation-gaps.md:40-41` |
| BLAS/LAPACK provider, threading, pivoting, and eigenpair ordering/sign | Matrix/statistics and downstream algorithms | Pin library binaries and thread count; use exact ordering only where accepted, otherwise approved residual/canonical matching | `E3/E5` — `docs/modernization/translation-gaps.md:42,46` |
| FFT backend selection and sign/normalization/index conventions | `fftgf` and FFT library calls | Select one authoritative backend; capture impulse/tone/round-trip and domain-transform cases per backend | `E3/E4/E5` — `docs/modernization/translation-gaps.md:43`; `docs/modernization/intent-ledger.md:50,54,85` |
| List-directed/fixed-width Fortran I/O, locale, line endings, exponent/special-value spelling, and signed zero | CLI streams, files, diagnostics, fixtures | Pin compiler and locale (`LC_ALL`), capture bytes plus parsed values, and decide exact vs normalized comparison per surface | `E3/E5` — `docs/modernization/legacy-map.md:123`; `docs/modernization/translation-gaps.md:39` |
| Inconsistent `(Re,Im)` versus `(Im,Re)` complex columns | `fftgf`, Padé, plotting, and stored numeric data | Use asymmetric complex values and a separate codec/fixture decision for every surface | `E2/E3` — `docs/modernization/intent-ledger.md:25,89`; `docs/modernization/translation-gaps.md:45` |
| Shared mutable MPI/OpenMP-named module state and possible process initialization order | Diagnostics and any stateful library calls | Default to isolated serialized processes; capture initial state and repeated-call order; do not infer thread safety | `E3/E5` — `src/COMVARS.f90:59-67`; `docs/modernization/translation-gaps.md:34,48` |
| Mutable revision stamping and runtime timestamp/version output | Build products and diagnostics | Build from a writable immutable snapshot, inject/record source hash, freeze time where relevant, and exclude metadata only by explicit decision | `E1/E3` — core dry run; `src/Makefile:2,24-25`; `docs/modernization/legacy-map.md:173` |
| Filesystem state, overwrite/removal behavior, permissions, shell tools, and working directory | `ffcmplx`, plotting, gzip, lattice, and diagnostics | Fresh disposable scratch per run; fixed paths/permissions; inventory all files and hashes before/after | `E3` — `docs/modernization/legacy-map.md:115-119,131-132,168` |
| Potential uninitialized values or compiler-extension acceptance in large included/vendored routines | Numerical edge cases and error paths | Compile with diagnostics in containment, initialize scratch, repeat under accepted compiler and a diagnostic build; do not “fix” behavior without a defect decision | `E4 inferred / E5 unknown` — exact production compiler and accepted source closure are unresolved. |

## 8. Open oracle questions

- [ ] Which revision and branch is authoritative: `master`/`5e4e6a3`, `initial-build`/`9b19827`, `722cde3`, a pinned upstream revision, or another source/binary?
- [ ] Is a known-working installation or archived executable available, with hashes and legal permission to run it?
- [ ] Which OS/image, architecture, compiler/version, flags, runtime, BLAS/LAPACK provider, and FFT backend produced trusted results?
- [ ] Can a reviewed Docker/VM environment be made operational with no network and immutable source, and can it build twice and run the same corpus repeatedly?
- [ ] Is `OPTIMIZE` the accepted replacement for missing `ZEROS`, and which incomplete/optional surfaces are supported?
- [ ] Which library APIs and CLI utilities are actual product scope, and which downstream consumers can supply invocations and acceptance data?
- [ ] Can `numutils/test/` or `initial-build` artifacts be provenanced as observed accepted outputs rather than generated expectations?
- [ ] What per-behavior numeric tolerance, residual/identity rule, ordering rule, text normalization, locale, complex-column convention, and error/exit contract is accepted?
- [ ] Must random behavior preserve exact sequences, seeded replay, or only distribution-level properties?
- [ ] Which sanitized data sets represent normal, boundary, malformed, non-convergent, and failure cases without PII or secrets?
- [ ] What legal disposition permits building/running and exporting outputs from MKL, FFTW, Numerical Recipes-derived, Intel-confidential, and other mixed-provenance inputs?
- [ ] Are exact dependency versions available for vulnerability scanning? No CVE conclusion is supportable from unpinned names alone.

## 9. Links

- Behavior catalog: `docs/modernization/behaviors/BEH-NNN-*.md` (none created yet)
- Defect ledger: `docs/modernization/defect-ledger.md` (not created yet)
- Parity reports: `docs/modernization/parity/{STORY-ID}-parity.md`

*Created: 2026-08-09*
