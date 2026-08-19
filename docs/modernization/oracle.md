<!-- Oracle contract:
- Classify the legacy oracle tier before planning parity work.
- T3 documented-only cannot claim independent parity.
- If a section has no content yet, write `None yet.`.
-->

# Legacy Oracle

**Legacy repo:** `/Users/philipteitel/code/ADD-migrations/sci-fortran-legacy` (strictly read-only; explicit user override of configured `../scifortran-legacy`)
**Oracle tier:** `T1 executable` — bounded to the core library and merged fidelity driver/corpus at accepted probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`
**Date:** `2026-08-10`

---

## 1. Oracle tier decision

| Tier | Selected? | Evidence | Consequence |
|------|-----------|----------|-------------|
| T1 executable | yes, scoped | `E1 verified` — two independently materialized clean Git archives of accepted commit `e586903` (tree `691333e7709f4d3396fbe714726e0a010a71355b`; identical archive SHA-256 `e00dd2daeee84037674e99ef440a353a2fb3ac743d36d7179afa6758abf391f1`) each ran `scripts/build.sh` and `scripts/fidelity.sh` with exit `0` under the controlled environment below. Full and per-case execution captures were byte-identical. `scripts/build.sh:1-99`; `scripts/fidelity.sh:1-186`; 2026-08-10 probe record below. | Repeated comparison is possible only for this revision, environment, driver, and five-section corpus. It does not establish the broader library/CLI surface. |
| T2 recorded | no | `E3 code-derived / E4 inferred` — no fixture corpus is retained by this probe. Four checked-in “goldens” are regenerated from Python formulas and one is copied from unprovenanced `numutils/test/xy2.deriv`; passing those comparisons does not make them observed legacy expectations. `scripts/fidelity.sh:105-159`; `docs/modernization/legacy-map.md:154-165`; `docs/modernization/intent-ledger.md:18-19`. | Execution captures may be graded E1 for what ran, but fixture expectation provenance remains unpromoted. |
| T3 documented-only | no for bounded scope | `E1 verified` — actual compile, link, and repeated execution now supersede the prior dry-run-only evidence for the bounded core/fidelity path. `docs/modernization/translation-gaps.md:35,51-52`. | T3 still describes every unexecuted or unsupported surface outside the stated T1 boundary. |

**Decisive evidence:** the accepted source revision built and ran successfully in two independent clean snapshots, and both executions produced the same parsed values and byte-level captures. This satisfies `T1 executable` only for the bounded core library plus fidelity driver/corpus.

**Scope limitations:** the probe did not build the 17 CLI utilities; execute FFT, BLAS/LAPACK matrix routines, RNG, expression evaluation, plotting, shell-backed/file-mutating surfaces, or network paths; test malformed/error behavior; prove downstream `ZEROS` compatibility; or establish production acceptance. The `arange-5` section is a driver loop, not a call to legacy `arange`. `fidelity/driver.f90:27-30`; `docs/modernization/legacy-map.md:154-165,171-175`.

**Probe record:**

| Probe | Result | Evidence / safety note |
|-------|--------|------------------------|
| Legacy baseline before probe | `master` at `e586903a26cc50ca8942f20ca3bccbd8814e6252`; tracked/untracked porcelain output empty. Ignored pre-existing files were `numutils/src/{options.inc,sfmake.inc}`, `src/{debug_mods/common_vars.mod,debug_objs/COMVARS.o,objs/COMVARS.o,scifor_version.inc}`. | `E1 verified` — pre-probe `/usr/bin/git rev-parse`, `branch --show-current`, and status checks against the legacy checkout. No build command ran there. |
| Source materialization | Two separate `git archive` invocations produced identical tar SHA-256 `e00dd2...91f1`; each extracted snapshot had no `.git`. A tracked-path scan found no `.env`, private-key, credential, or secret-named path. | `E1 verified` — accepted commit/tree and archive hashes recorded above. Only tracked commit content was copied; ignored legacy residue was excluded. |
| Network control | macOS `sandbox-exec` policy `(version 1) (allow default) (deny network*)` denied a socket connection with `PermissionError: [Errno 1] Operation not permitted`; the same policy wrapped every build and fidelity process. | `E1 verified` — preflight denial plus exact invocation below. The policy denied network but was not a full filesystem/container isolation boundary. |
| Independent builds | Both fresh snapshots ran `scripts/build.sh`; both exit statuses were `0`; each produced optimized/debug archives and `.bin/scifor-fidelity`. Selected `FFT_BACKEND=NR`. | `E1 verified` — canonical clean build captures on 2026-08-10; `scripts/build.sh:50-76,79-99`. |
| Independent executions | Both snapshots ran `scripts/fidelity.sh`; both exit statuses were `0`; each reported `5 passed, 0 failed`. | `E1 verified` — full/per-case hashes and value comparison in Sections 4–6. |
| Output repeatability | Full capture and all five section files were byte-identical. Parsed cross-build maximum absolute difference was `0` for all 1,044 rows. | `E1 verified` — hashes in Section 5; row/column counts were 5×1, 5×1, 5×1, 5×2, and 1024×2. |
| Build warnings and artifact determinism | Each build emitted 310 compiler warning lines and one duplicate-`-lgfortran` linker warning. Static archives and driver binaries had different SHA-256 hashes. | `E1 verified` — warning categories and artifact hashes in Section 7. Runtime text remained identical despite binary non-reproducibility. |
| Tracked snapshot integrity | After build/fidelity, neither snapshot had a changed or missing archived file; generated products were confined to disposable paths. | `E1 verified` — SHA-256/symlink comparison against each source archive. |
| Harness or fixture creation | None. No output capture, binary, build log, generated expected file, or helper code was retained. | Probe mode and user containment requirement. Only this text record is retained. |

## 2. Legacy execution environment

| Requirement | Value | Evidence | Notes |
|-------------|-------|----------|-------|
| Source | Commit `e586903a26cc50ca8942f20ca3bccbd8814e6252`; tree `691333e7709f4d3396fbe714726e0a010a71355b`; Git-archive SHA-256 `e00dd2daeee84037674e99ef440a353a2fb3ac743d36d7179afa6758abf391f1` | `E1 verified` — Git identity plus two independent archive hashes | No `.git` or ignored checkout residue was copied. |
| OS / architecture | macOS 26.5.2 (`25F84`), Darwin 25.5.0, arm64 | `E1 verified` — `sw_vers`, `uname -a`, `uname -m` | This is the verified oracle host, not evidence of the historical production host. |
| Runtime / compiler | GNU Fortran 16.1.0 (`Homebrew GCC 16.1.0`); compiler binary SHA-256 `1f2580f9691ce4a9bfcf7ca42f0243aa3a740d450faf5499396a7902a3d88ca6` | `E1 verified` — version and binary hash | `FC=gfortran`; production compiler acceptance remains `E5 unknown`. |
| Build / comparison tooling | GNU Make 3.81; Bash 3.2.57; Python 3.9.6; Apple Git 2.50.1; openrsync protocol 29 / rsync-compatible 2.6.9 | `E1 verified` — local version probes | Python only regenerated/computed candidate expectations and comparisons; it did not produce E1 expectation provenance. |
| Compile/link flags | Optimized objects `-O2 -static`; debug objects `-O0 -p -g -Wall -static`; fidelity driver `-O2 -I<include> -L<lib> -L<openblas> -lscifor -lopenblas -lgfortran` | `E3 code-derived / E1 executed` — `include/options.inc:9-15`; `scripts/build.sh:72-76`; build transcripts summarized in Section 1 | Both optimized and debug archives compiled; the executed driver links the optimized archive. |
| Native numeric libraries | OpenBLAS 0.3.34; `libopenblas.dylib` SHA-256 `aeb5f40d3b5cc0fca84e05e90b8e7da6921cea2c33ca701062b0ccd7b4caf117`. LAPACK 3.12.1 was installed (`liblapack.dylib` SHA-256 `e7d673020838b52c0d93b8422d4c4aaca8a90dd49c3e22af268e8eb73ac1a95a`) but the Darwin driver linkage resolved OpenBLAS, not the separate LAPACK dylib. | `E1 verified` — versioned Homebrew symlinks, binary hashes, and `otool -L` | Driver linkage also resolved `libgfortran.5`, `libquadmath.0`, and `libSystem.B`. Matrix/LAPACK behavior was not exercised. |
| FFT backend | `NR`; tracked and selected `FFTGF_NR.f90` | `E1 verified / E3 code-derived` — environment plus `scripts/build.sh:7,50-62` | The core compiled with NR, but the fidelity driver did not call FFT behavior. Product and legal acceptance remain open. |
| Locale / timezone / threading | `LC_ALL=C`, `LANG=C`, `TZ=UTC`, `OPENBLAS_NUM_THREADS=1`, `OMP_NUM_THREADS=1`, `VECLIB_MAXIMUM_THREADS=1`, `PYTHONHASHSEED=0` | `E1 verified` — exact clean environment used for all four canonical commands | Controls comparison formatting and thread-count variability for this probe. |
| Network | None; denied by `sandbox-exec` for build and execution | `E1 verified` — policy preflight and wrapped commands | No dependency acquisition or network-capable application surface was invoked. |
| Data set | Tracked `numutils/test/xy2.data` plus driver literals; synthetic numeric content | `E1 verified / E3 code-derived` — executed `fidelity/driver.f90:10-67` | No PII was observed. Input/expectation acceptance provenance remains unresolved. |
| Secrets | No secret-named tracked path found; isolated `HOME` and `TMPDIR` were inside each snapshot | `E1 verified / E4 inferred` — archive path scan and invocation environment | The OS policy denied network but did not provide blanket host-filesystem read denial; reviewed scripts did not request credentials or user data. |

Exact environment prefix used for every canonical command (snapshot-specific paths substituted):

`env -i HOME=<snapshot>/.probe-home TMPDIR=<snapshot>/.probe-tmp PATH=/opt/homebrew/opt/gcc/bin:/usr/bin:/bin:/usr/sbin:/sbin LC_ALL=C LANG=C TZ=UTC FC=gfortran FFT_BACKEND=NR TOL=1e-10 OPENBLAS_NUM_THREADS=1 OMP_NUM_THREADS=1 VECLIB_MAXIMUM_THREADS=1 PYTHONHASHSEED=0 GIT_CEILING_DIRECTORIES=<snapshot> sandbox-exec -p '(version 1) (allow default) (deny network*)'`

`GIT_CEILING_DIRECTORIES` was the only environment correction: because archives intentionally contain no `.git` and reside beneath the target repository, it prevented revision stamping from accidentally discovering the target repository's Git metadata. Consequently each build emitted one expected “not a git repository” diagnostic and generated blank `sf_version`; the accepted source hash above is external provenance.

## 3. Containment and security

| Risk | Isolation control | Evidence | Owner |
|------|-------------------|----------|-------|
| Legacy checkout mutation | No command except read-only Git identity/status/archive operations targeted the legacy checkout. All writes occurred in disposable snapshots; source archive hashes were identical; post-probe status is recorded in Section 8. | `E1 verified` — before/after Git checks and snapshot locations | Oracle operator |
| Network access | Every build/run process used an OS policy denying `network*`; denial was tested before execution. | `E1 verified` — `PermissionError [Errno 1]` preflight and exact invocation above | Oracle operator |
| Script mutation | Only reviewed `scripts/build.sh` and `scripts/fidelity.sh` were run. `bin/setup_sf.sh`, CLI programs, expression evaluation, plotting, shell-backed runtime operations, and arbitrary-file surfaces were not run. | `E1 verified / E3 code-derived` — executed command set; `scripts/build.sh:1-99`; `scripts/fidelity.sh:1-186` | Oracle operator |
| Generated content | Each canonical snapshot generated 226 files across local build/output/home paths. No archived tracked file changed or disappeared after either run. All snapshots, archives, binaries, logs, generated references, and captures were deleted after evidence extraction. | `E1 verified` — post-run archive comparison and cleanup verification | Oracle operator |
| Host data exposure | Clean environment redirected `HOME`/`TMPDIR` into each snapshot and provided no credential variables. The deny-network policy did not blanket-deny host filesystem reads, so this is bounded-script containment rather than a hardened VM/container. | `E1 verified / E4 inferred` — invocation and policy semantics | Security/oracle operator |
| Resource isolation | No explicit CPU, memory, PID, or filesystem quota was applied. Both builds completed in about 30 seconds and runs in about 3 seconds, but stronger resource containment is still required before broad/untrusted inputs. | `E1 verified` for observed execution / `E5 unknown` for hostile-input behavior | Security/oracle operator |
| Proprietary or unclear source/dependency licensing | No binary/source/output was retained or redistributed. Resolve Intel-confidential, NR, and mixed-provenance material before broader oracle packaging or target translation. | `E3 code-derived / E5 unknown` — `docs/modernization/dependency-ledger.md:121-131`; `docs/modernization/translation-gaps.md:56` | Legal/product owner |

## 4. Invocation contract

No `BEH-NNN` catalog existed at probe time. **2026-08-19:** BEH-001 accepted; BEH-002–BEH-004 record the other executed library calls; the `arange-5` driver loop is not BEH-005.

| Behavior | Legacy command / interaction | Inputs | Outputs | Exit / error behavior |
|----------|------------------------------|--------|---------|-----------------------|
| `BUILD-TBD` — core plus driver | `<clean-environment-and-sandbox-prefix> /bin/bash <snapshot>/scripts/build.sh` | Accepted source archive; `FC=gfortran`; `FFT_BACKEND=NR`; local OpenBLAS | `lib/libscifor.a`, `lib/libscifor_deb.a`, modules, `.bin/scifor-fidelity` | Exit `0` in both canonical clean snapshots. One expected no-`.git` diagnostic, 310 compiler warning lines, and one duplicate-library linker warning per build. |
| `BEH-TBD` — fidelity corpus | `<clean-environment-and-sandbox-prefix> /bin/bash <snapshot>/scripts/fidelity.sh` | Built driver; tracked driver literals and `numutils/test/xy2.data`; `TOL=1e-10` | `.fidelity-out/full.out` plus five extracted numeric sections | Exit `0` twice; `5 passed, 0 failed` each run. The script rewrites candidate expected files before comparison, so its PASS result is not fixture-provenance evidence. |
| `BEH-001` — `linspace-5` | Fidelity driver calls `linspace(0.d0, 1.d0, 5)` | Driver literals | Five parsed real values accepted as FIX-001 | E1 execution output; byte-identical across builds. Managed-API contract ignores `es24.17` text. `fidelity/driver.f90:12-19`; ADR-001–003. |
| `BEH-002` — `logspace-5` | Fidelity driver calls `logspace(1,1000,5)` | Driver literals | Five formatted real values | E1 execution output; byte-identical across builds. Parsed values not retained. `fidelity/driver.f90:19-25`. |
| `BEH-005` — not this row | Fidelity driver prints `real(i,8)` for `i=1..5` | Driver literals | Five formatted real values | E1 driver execution output, but **not** evidence for the legacy `arange` implementation. `fidelity/driver.f90:27-30`. |
| `BEH-003` — `fermi-beta100` | Fidelity driver calls `fermi(x,100)` for five values | `[-2,-1,0,1,2]` | Five two-column rows | E1 execution output; byte-identical across builds. Parsed values not retained. `fidelity/driver.f90:32-39`. |
| `BEH-004` — `deriv-xy2` | Fidelity driver reads `xy2.data` and calls `deriv(y,dh)` | 1,024 tracked X/Y rows | 1,024 two-column rows | E1 execution output; byte-identical across builds. File-open failure is coded but was not exercised. Parsed values not retained. `fidelity/driver.f90:41-67`. |

## 5. Fixture corpus

No fixture was retained or promoted in probe mode. The rows below record hashes of ephemeral E1 execution captures only; they are reproducibility evidence, not frozen expected files.

| Fixture ID | Behavior | Inputs | Expected legacy output | Evidence grade | Determinism notes |
|------------|----------|--------|------------------------|----------------|-------------------|
| `CAP-20260810-FULL` | Entire fidelity-driver execution | Driver literals plus `numutils/test/xy2.data` | Observed full stdout SHA-256 `14a40532e4e308265dfd09cca956ab7f4249c80de884a1264bc96377d3688dd5`; not retained | `E1 verified` | Same hash in both independent builds. |
| `CAP-20260810-LINSPACE` | `linspace-5` / BEH-001 | `0,1,5` | Parsed values accepted as FIX-001; original stdout not retained. Section SHA-256 `dabd07f92b83714a0e0740223261fc457c517025cf3f4b08cd51380a0082c35c` | `E1 verified` | 5×1 rows; byte-identical; parsed max difference `0`. Promoted 2026-08-19: `docs/modernization/fixtures/FIX-001-linspace-5.md` |
| `CAP-20260810-LOGSPACE` | `logspace-5` | `1,1000,5` | Observed section SHA-256 `c5b198afbc3ccea1a27ded3ac9f3919952a6662c2465000c84e35e8f964db4fe`; not retained | `E1 verified` | 5×1 rows; byte-identical; parsed max difference `0`. |
| `CAP-20260810-ARANGE-LABEL` | Driver loop labeled `arange-5` | Integers 1–5 | Observed section SHA-256 `2e104659f3bed55ec95de1c6b444990a60856fc1de30a3ab8aad51e4abda3c17`; not retained | `E1 verified` for driver output only | 5×1 rows; byte-identical; does not exercise legacy `arange`. |
| `CAP-20260810-FERMI` | `fermi-beta100` | Five X values; beta 100 | Observed section SHA-256 `6f35eadc7f917064b110353cebf6ab6468999625ceae0394744d68be67a1810a`; not retained | `E1 verified` | 5×2 rows; byte-identical; parsed max difference `0`. |
| `CAP-20260810-DERIV` | `deriv-xy2` | 1,024 rows from `xy2.data` | Observed section SHA-256 `8a8879bc89a240338320275532f418c332b4120a536c23a8cb9428ca99b30b6f`; not retained | `E1 verified` | 1024×2 rows; byte-identical; parsed max difference `0`. |
| `CAND-EXPECTATIONS` | Script comparison inputs | `fidelity/golden/*.txt` and `numutils/test/xy2.deriv` | **Not accepted as legacy expected output:** four files are Python-generated and one is copied from an unprovenanced historical file | `E3 code-derived / E4 inferred` — `scripts/fidelity.sh:105-159` | Both runs reported the same comparison errors, but that does not improve expectation provenance. |

Promotion to T2/frozen fixtures still requires behavior/defect IDs, owner acceptance, immutable retained captures, and approved comparison rules. The executable T1 scope can regenerate observed output under the pinned environment.

## 6. Tolerances and normalization

| Output kind | Rule | Applies to | Rationale | ADR / decision |
|-------------|------|------------|-----------|----------------|
| Numeric execution repeatability | Exact parsed value equality across the two captures; observed cross-build maximum absolute difference `0` for every case | Six ephemeral `CAP-20260810-*` records | This demonstrates repeatability in the pinned environment, not target parity or an accepted tolerance. | Probe observation only |
| Script self-check | Absolute-only `1e-10` against regenerated/copied candidate expectations | Five fidelity sections | Both runs reported max errors: linspace `0`, logspace `1.023e-12`, arange-label `0`, fermi `0`, deriv `4.885e-15`. The expectation provenance remains E3/E4. `scripts/fidelity.sh:11,27-78,105-175`. | Not accepted as a parity rule |
| Configured numeric defaults | Relative `1e-6` and absolute `1e-6` | Future approved `BEH`/`FIX` records only | `E2 documented` — `.cursor/workflow.config.yml:44-47`; these defaults conflict with the script threshold and were not needed for cross-build equality. | `TBD` — behavior-specific numerical comparison decision |
| Text repeatability | Exact byte comparison for full and per-case captures | Six ephemeral `CAP-20260810-*` records | All hashes matched under `LC_ALL=C`; no whitespace or line-ending normalization was needed for this probe. | Probe observation only |
| Configured text normalization | Trim surrounding whitespace, normalize line endings, preserve case | Future approved text fixtures | `E2 documented` — `.cursor/workflow.config.yml:48-52`; broader Fortran formatting and complex ordering remain unknown. | `TBD` — per-surface text/culture/complex-column decision |
| Time / random | `TZ=UTC`; no RNG/time-bearing behavior executed | Random, timestamp/version, and time-reporting candidates | Clock-seeded RNG and runtime timestamp/version functions remain outside the verified corpus. `E3` — `src/RANDOM.f90:197-208`; `src/COMVARS.f90:112-124`. | `TBD` — RNG sequence vs statistical-equivalence and time controls |

Passing the script's formula/copy comparison is not a parity claim. T1 establishes repeatable execution; it does not by itself establish accepted compatibility values.

## 7. Determinism hazards

| Hazard | Affected behavior | Control | Evidence |
|--------|-------------------|---------|----------|
| Build products are not byte-reproducible | Archives and fidelity executable | Treat source/environment/output hashes as oracle provenance; investigate deterministic archive/link settings before retaining binaries | `E1 verified` — clean-build SHA-256 differed: optimized archive `1b260db26d45161dd6dcb851a092c8463d82eadc8ffe27a9e3911ff636838bd8` vs `9381df74bdd6095de077ce8e47663657e0ca063df8dbf1a17a4ac1db42afd75b`; debug archive `3a035f60ca5973bfb158538c5b8821278cd4a847316790bd9612b5443bb6387f` vs `360dcab298f2738f3285ea25e91e826775735ed7a21e774a45bb56a0176e5c46`; driver `3f638354e53f51617e57c57bfab84684d43771e7fb02fd689b7df7a70de8c88d` vs `b9c8c04121b7cc98d56f814e3bb8299ed9b548066b5680be3336187b90f120ae`. Runtime captures were identical. |
| Revision stamping cannot use Git in archive snapshots | Version diagnostics and any caller of `sf_version` | Keep external source commit/tree/archive hashes; decide a non-mutating revision injection contract | `E1 verified / E3 code-derived` — one Git diagnostic per build and generated `sf_version=""`; `src/Makefile:2,24-25`; `docs/modernization/translation-gaps.md:61`. |
| Compiler diagnostics indicate latent numeric/state risk | Uncovered routines and edge cases | Preserve warning inventory; characterize affected callable surfaces before parity claims | `E1 verified` — 310 compiler warning lines per build, including 61 `-Wconversion`, 55 `-Wmaybe-uninitialized`, 12 `-Wuninitialized`, one `-Wtarget-lifetime`, and unused-code categories; one duplicate-`-lgfortran` linker warning. |
| Compiler, optimization, architecture, numeric kind, and IEEE-mode differences | All numerical routines | Pin exact source/compiler/libraries/flags/architecture as above; test other environments separately | `E1 verified` for this environment / `E5 unknown` elsewhere — `include/options.inc:1-16`; `docs/modernization/translation-gaps.md:35,42-43`. |
| BLAS/LAPACK provider, threading, pivoting, and ordering | Matrix/statistics and downstream algorithms | OpenBLAS was pinned and thread counts were set to 1; add matrix/provider fixtures before extending T1 scope | `E1 verified` for linkage / `E5 unknown` for behavior — no matrix case ran; `docs/modernization/translation-gaps.md:44`. |
| FFT backend sign/normalization/index conventions | `fftgf` and FFT library calls | Record `NR` as build selection only; require impulse/tone/round-trip/domain fixtures and legal decision | `E1 verified` for selected compile / `E5 unknown` for behavior — fidelity driver has no FFT call; `docs/modernization/translation-gaps.md:45`. |
| Locale, formatted I/O, signed zero, and complex-column order | CLI streams/files and complex surfaces | Keep `LC_ALL=C`; capture bytes and asymmetric complex values per surface | `E1 verified` only for current real-valued driver output / `E3/E5` elsewhere — `docs/modernization/translation-gaps.md:41,47`. |
| `SYSTEM_CLOCK` RNG and shared process state | `random`, diagnostics, and stateful library consumers | Isolated serialized trials; recover seed/state contracts before scope expansion | `E3 code-derived / E5 unknown` — not executed; `src/RANDOM.f90:197-208`; `src/COMVARS.f90:59-67`. |
| Filesystem state, shell tools, and arbitrary paths | File, plotting, compression, and diagnostic surfaces | Fresh disposable scratch; stronger filesystem/resource sandbox; allowlisted fixtures only | `E3 code-derived / E5 unknown` — explicitly excluded from this probe; `docs/modernization/legacy-map.md:115-119,131-140`. |

## 8. Open oracle questions

- [x] Is accepted probe revision `master`/`e586903` also the product/parity baseline, rather than only the revision authorized for this run? **First slice (2026-08-19):** yes for BEH-001 only (ADR-001). Production-wide: still open.
- [ ] The workflow profile still declares `T3 documented-only` at `.cursor/workflow.config.yml:31`; when should it be reconciled with this scoped T1 decision?
- [ ] Which library APIs and CLI utilities are supported scope, and which downstream consumers require historical `ZEROS` versus current `OPTIMIZE` contracts?
- [ ] Which exact production compiler/OS/architecture/numeric libraries define trusted historical behavior, and should this macOS arm64/GNU 16/OpenBLAS/NR environment be accepted for future oracle runs?
- [x] Can accepted immutable fixture expectations be obtained? **FIX-001** records parsed `linspace(0,1,5)` values. Four other references remain formula/copy candidates.
- [ ] What behavior-specific tolerances, residual/identity rules, ordering, text normalization, locale, complex-column, and error/exit contracts are accepted?
- [ ] How should non-byte-reproducible archives/executables and blank archive-snapshot revision stamping be corrected without adding `.git` or mutating source?
- [ ] Which of the compiler warnings, especially conversion and possible/uninitialized-value warnings, affect retained behavior and require defect decisions?
- [ ] Can broader probes add explicit CPU/memory/PID/filesystem isolation before testing malformed input, arbitrary paths, shell-backed file operations, expression evaluation, or plotting?
- [ ] What fixtures cover FFT, matrix/BLAS/LAPACK, RNG/state, error paths, CLI formatting, and boundary/non-convergent cases? These remain outside T1 scope.
- [ ] What legal disposition permits running, retaining, or redistributing outputs/binaries involving NR, Intel-confidential, and mixed-provenance sources?
- [ ] Are exact versions available for all unexecuted dependencies so vulnerability scanning can produce supported conclusions?

**Legacy checkout integrity:** before and after the probe, the checkout was `master` at `e586903a26cc50ca8942f20ca3bccbd8814e6252`; tracked/untracked porcelain output was empty, and the same six pre-existing ignored paths listed in Section 1 remained. No legacy file was added, removed, or changed.

## 9. Links

- Behavior catalog: `docs/modernization/behaviors/BEH-001-linspace.md`
- Defect ledger: `docs/modernization/defect-ledger.md`
- Fixture: `docs/modernization/fixtures/FIX-001-linspace-5.md`
- Parity reports: `docs/modernization/parity/{STORY-ID}-parity.md`

*Created: 2026-08-09 | Probe refreshed: 2026-08-10 | First-slice promotion: 2026-08-19*
