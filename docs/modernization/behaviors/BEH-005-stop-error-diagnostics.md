# BEH-005: `STOP` / error / diagnostic termination contract

**Status:** Draft
**Evidence grade:** `E3 code-derived` (overall; process exit status, stderr vs stdout, and partial side effects largely `E5 unknown`)
**Legacy surfaces:** `COMMON_VARS` `msg`/`warning`/`error`/`abort`; `PARSE_CMD` help `STOP`; utility/`MATRIX` callers; fidelity driver `stop 1`
**Date:** `2026-08-10`

---

## 1. Summary

User-visible failure and help paths typically print diagnostics to Fortran unit `*` (stdout) and terminate with bare `STOP` (or occasionally `stop <code>`). `warning` and `msg` print without stopping. `abort` is an alias of `error`. There is no recovered structured status taxonomy, stderr separation, or stable exit-code contract for most library/CLI failures. Host mapping (CLI exit codes, HTTP Problem Details) is out of scope for this artifact.

## 2. Actors and triggers

| Actor / system | Trigger | Preconditions | Evidence |
|----------------|---------|---------------|----------|
| Library/CLI code | Calls `error`/`abort`/`warning`/`msg` | Optional MPI id gating | `E3` — `src/COMVARS.f90:86-88,192-254` |
| CLI user | Passes `--help`/`-h`/`help`/`info`/`--h` | Help buffer registered | `E3` — `src/PARSECMD.f90:36-58,70-78` |
| Numeric routine | Invalid dimension / LAPACK `info` | Error path reached | `E3` — `numutils/src/fftgf.f90:77,118,138`; `src/MATRIX.f90:100-103` |
| Grid helpers | `num<0` / invalid endpoint combo | `linspace`/`logspace` | `E3` — `src/tools_grids.f90:7-12,41` |
| Fidelity driver | Missing `xy2.data` | Open `iostat /= 0` | `E3` — `fidelity/driver.f90:43-46` |

## 3. Inputs

| Input | Type / format | Units | Range / constraints | Required? | Evidence |
|-------|---------------|-------|----------------------|-----------|----------|
| Diagnostic text | `character(len=*)` | n/a | Free text | yes for error/warn/msg | `E3` — `src/COMVARS.f90:192-247` |
| Optional CPU/`id` | integer | n/a | Compared to `mpiID`/`mpiSIZE` | no | `E3` — `src/COMVARS.f90:197-199,216-218,239-241` |
| Help flags | exact string match on argv tokens | n/a | `--help`, `-h`, `info`, `--h`, `help` | no | `E3` — `src/PARSECMD.f90:45-49` |
| Help `status` optional | logical out-param | n/a | If present, set `.true.` and return instead of `STOP` | no | `E3` — `src/PARSECMD.f90:40-58` |

## 4. Outputs and side effects

| Output / side effect | Type / format | Precision / ordering | Destination | Evidence |
|----------------------|---------------|----------------------|-------------|----------|
| Error line | ANSI-styled `"error:"` prefix + `bg_red(text)` | Prefix then message | unit `*` (stdout) | `E3` — `src/COMVARS.f90:199-207` |
| Warning line | `"warning:"` + yellow styling | non-fatal | stdout | `E3` — `src/COMVARS.f90:218-226` |
| Message line | `"msg:"` + text; optional blank lines | non-fatal | stdout | `E3` — `src/COMVARS.f90:241-252` |
| Process termination | bare `stop` after `error` | exit code **unspecified** in source | process | `E3`/`E5` — `src/COMVARS.f90:208`; GAP-026 |
| Help text then stop | trimmed help buffer lines | then `stop` unless status mode | stdout | `E3` — `src/PARSECMD.f90:50-57,74-78` |
| Fidelity open failure | `write(*,*) 'error: ...'`; `stop 1` | numeric stop code 1 | stdout + exit | `E3` — `fidelity/driver.f90:45-46` |
| PARSE update msgs | `msg("Variable ... update to ...")` | on successful parse | stdout | `E3` — `src/PARSECMD.f90:121,142` |

## 5. Rules and invariants

| Rule | Evidence | Open question? |
|------|----------|----------------|
| `error` always executes `stop` after optional print (when `mpiID` matches). | `E3` — `src/COMVARS.f90:199-208` | no |
| `abort` is bound to `error`. | `E3` — `src/COMVARS.f90:86-88,94` | no |
| `warning` and `msg` do not stop. | `E3` — `src/COMVARS.f90:211-254` | no |
| Diagnostics are written with `write(*,...)`, not a dedicated stderr unit. | `E3` — `src/COMVARS.f90:201-247`; GAP-026 | yes — compatibility of stdout mixing |
| Help without `status` argument stops the process after printing. | `E3` — `src/PARSECMD.f90:53-57,77-78` | no |
| ANSI escape wrappers decorate error/warning prefixes. | `E3` — `src/COMVARS.f90:201-225,290-323` | yes — whether styling is part of contract |
| MPI/OpenMP-named globals gate which rank prints; default `MPISIZE=1`, `MPIID=0`. | `E3`/`E5` — `src/COMVARS.f90:59-67,199-208` | yes — operational or scaffolding |
| No project-wide mapping from LAPACK `info` to non-terminating status exists in inspected wrappers (`error` on nonzero). | `E3` — `src/MATRIX.f90:100-103,125-128` | yes |

## 6. Error handling and edge cases

| Case | Legacy behavior | Evidence | Defect decision |
|------|-----------------|----------|-----------------|
| Help requested | Print help; `STOP` (default) | `E3` — `src/PARSECMD.f90:50-57` | none (mechanism); channel/exit via DEF-009/010 |
| Invalid FFT length | `error(...)` → stdout + `STOP` | `E3` — `numutils/src/fftgf.f90:118,138` | none (mechanism); see DEF-009/010 |
| List count mismatch | `abort(...)` → same as `error` | `E3` — `numutils/src/fftgf.f90:77` | none (mechanism); see DEF-009/010 |
| LAPACK failure in diagonalization | `error(...)` → `STOP` | `E3` — `src/MATRIX.f90:100-103` | none (mechanism); see DEF-009/010 |
| `linspace`/`logspace` invalid `num` | `error`/`abort` messages | `E3` — `src/tools_grids.f90:7-12,41` | none (mechanism); see DEF-009/010 |
| Malformed CLI value / bad stdin token | Not characterized | `E5` — GAP-020/026 | TBD |
| Bare `STOP` vs `stop 1` | Most fatals bare `STOP`; fidelity open fail uses `stop 1` | `E3`/`E5` — `COMVARS.f90:208`; `driver.f90:45-46` | **DEF-009** open/TBD |
| Diagnostics on stdout with data | `write(*,...)` / unit `*` for errors and help | `E3`/`E5` — `COMVARS.f90:201-247`; GAP-026 | **DEF-010** open/TBD |
| Partial file side effects before `STOP` | Possible (e.g. deletes) but uncatalogued here | `E3`/`E5` — `numutils/src/ffcmplx.f90:52`; GAP-021/026 | TBD |
| Fidelity missing input | `stop 1` (explicit code) | `E3` — `fidelity/driver.f90:45-46` | **DEF-009** open/TBD |

## 7. Draft Gherkin

```gherkin
Given a legacy library or CLI path that detects a fatal condition
When it calls error/abort (or help without status)
Then a diagnostic is written to Fortran unit * (stdout), possibly ANSI-styled
And the process terminates via STOP
And no portable nonzero exit code should be assumed unless the source uses stop <code>
And warnings/messages alone must not terminate the process
```

## 8. Legacy code and documentation citations

| Source | Lines / section | Claim supported | Evidence grade |
|--------|-----------------|-----------------|----------------|
| `src/COMVARS.f90` | 86-88,192-254 | `abort`/`error`/`warning`/`msg`; `STOP` | E3 |
| `src/PARSECMD.f90` | 36-78 | Help flags; `STOP` vs status return | E3 |
| `src/PARSECMD.f90` | 121,142 | Non-fatal parse `msg` | E3 |
| `numutils/src/fftgf.f90` | 77,118,138 | Utility fatal paths | E3 |
| `src/tools_grids.f90` | 7-12,41 | Grid argument fatals | E3 |
| `src/MATRIX.f90` | 100-103,125-128 | LAPACK `info` → `error` | E3 |
| `fidelity/driver.f90` | 43-46 | `stop 1` on open failure | E3 |
| `docs/modernization/translation-gaps.md` | GAP-020, GAP-026 | Host error-mapping gap | E3/E5 |
| `docs/modernization/oracle.md` | §4, §8 | Error paths not exercised in T1 probe | E1/E5 |

## 9. Oracle fixtures

| Fixture | Input | Expected output | Tolerance / normalization | Evidence |
|---------|-------|-----------------|---------------------------|----------|
| None for fatal paths | Error/help not in fidelity corpus | n/a | Need stdout bytes + process status captures | `E5` — oracle §1 scope limits |
| Happy-path probe | fidelity 5/5 | exit `0` for script | Not a failure-contract fixture | `E1` — oracle §1 |

## 10. Open questions

- [ ] What exit status does bare `STOP` produce on the accepted runtime, and must it be reproduced?
- [ ] Must ANSI color codes be preserved, stripped, or considered non-contractual?
- [ ] Should fatal diagnostics remain on stdout for CLI compatibility, or may adapters use stderr?
- [ ] Which failures must remain process-aborting versus becoming typed results for ASP.NET hosts?
- [ ] Are MPI-ranked diagnostic variants in supported scope?

## 11. Links

- Intent ledger: `docs/modernization/intent-ledger.md`
- Legacy flow: `docs/modernization/flows/` (none yet)
- Defect ledger: `docs/modernization/defect-ledger.md` — DEF-009, DEF-010 (open/TBD)
- Related gaps: GAP-020, GAP-026
- Assessment Condition 7: error/diagnostic mapping

### Tensions / conflicts

- Most fatals use bare `STOP` while the fidelity driver uses `stop 1` for one I/O failure — no uniform exit-code policy. `E3`/`E5` — `src/COMVARS.f90:208`; `fidelity/driver.f90:46`.
- Diagnostics share stdout with numeric data on CLI surfaces, conflicting with common Unix stderr conventions and with ASP.NET Problem Details mapping. `E3`/`E5` — GAP-020/026.
- Help can either `STOP` or return via optional `status`, so “help behavior” is API-dependent. `E3` — `src/PARSECMD.f90:53-58`.
- T1 oracle never exercised malformed-input or fatal library paths; failure contracts remain unverified. `E1`/`E5` — oracle §1 scope limitations.

*Created: 2026-08-10*
