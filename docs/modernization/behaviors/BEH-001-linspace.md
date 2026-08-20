# BEH-001: Generate an inclusive linear sequence (linspace)

**Status:** Recovered; first code slice (VS-1, formerly SL-001). Ready for `/refine-feature`.
**Evidence grade:** `E1 verified` for `FIX-001`; `E3 code-derived` for default-endpoint formula and abort conditions; `E5 unknown` for unexecuted optional flags, `mesh`, invalid `num`, and the CLI surface (out of first-slice scope)
**Legacy surfaces:** Library function `TOOLS.linspace`; exercised by `fidelity/driver.f90`. CLI `numutils/src/linspace.f90` is documented as related and **out of first-slice scope**.
**Date:** `2026-08-19`

---

## 1. Summary

A caller supplies a start value, a stop value, and a length. The library returns that many evenly spaced `real(8)` numbers over the interval. With the default inclusive endpoints, the first value is start, the last value is stop, and interior values are equally spaced.

The 2026-08-10 probe observed `linspace(0,1,5)` → `0, 0.25, 0.5, 0.75, 1`. The owner accepted that result as the first-slice contract (ADR-001, ADR-003). The first target driving surface is a managed API, not Fortran stdout (ADR-002).

## 2. Actors and triggers

| Actor / system | Trigger | Preconditions | Evidence |
|----------------|---------|---------------|----------|
| Fortran library caller (fidelity driver) | Call `linspace(start,stop,num)` | Linked `TOOLS`; `num` is a positive length used as the result shape | `E1 verified` — `fidelity/driver.f90:12-19`; `docs/modernization/oracle.md:83` |
| Downstream Fortran consumer | `use TOOLS` / `use SCIFOR` then call `linspace` | `libscifor.a` available | `E3 code-derived` — `src/TOOLS.f90:18`; `src/SCIFOR.f90:14`; consumers `E5 unknown` |
| Target managed-API caller | Invoke the host-neutral linspace port | First-slice adapter only | `E2 documented` — ADR-002; owner decision 2026-08-19 |
| CLI user | Invoke `linspace` executable | Built `numutils` binary | Out of first-slice scope. `E3 code-derived` — `numutils/src/linspace.f90:1-51` |

## 3. Inputs

| Input | Type / format | Units | Range / constraints | Required? | Evidence |
|-------|---------------|-------|----------------------|-----------|----------|
| `start` | `real(8)` | caller-defined | No additional range check in the function | yes | `E3 code-derived` — `src/tools_grids.f90:1-2`; `E1` value `0` in FIX-001 |
| `stop` | `real(8)` | same as `start` | No additional range check in the function | yes | `E3 code-derived` — `src/tools_grids.f90:1-2`; `E1` value `1` in FIX-001 |
| `num` | `integer` | count | `num < 0` aborts; default inclusive endpoints also abort when `num < 2` | yes | `E3 code-derived` — `src/tools_grids.f90:7,11-13`; unexecuted. FIX-001 uses `5` (`E1`) |
| `istart` | `logical`, optional | n/a | Default `.true.` | no | `E3 code-derived` — `src/tools_grids.f90:4-8`; not passed in FIX-001 |
| `iend` | `logical`, optional | n/a | Default `.true.` | no | `E3 code-derived` — `src/tools_grids.f90:4-9`; not passed in FIX-001 |
| `mesh` | `real(8)`, optional intent undocumented in signature (assigned if present) | same as `start`/`stop` | Receives the step used | no | `E3 code-derived` — `src/tools_grids.f90:6,29`; not exercised |

## 4. Outputs and side effects

| Output / side effect | Type / format | Precision / ordering | Destination | Evidence |
|----------------------|---------------|----------------------|-------------|----------|
| Returned sequence | `real(8)` array of length `num` | Index 1 is the first sample; order is start-toward-stop | Function result / managed-API sequence | `E1 verified` for FIX-001; `E3` for general default-endpoint formula |
| Optional `mesh` | `real(8)` scalar step | Equal to `(stop-start)/(num-1)` when both endpoints are included | Output argument if present | `E3 code-derived` — `src/tools_grids.f90:14,29` |
| Abort diagnostics | stdout ANSI-styled `error:` plus message, then `STOP` | Process termination | Console; process exit | `E3 code-derived` — `src/COMVARS.f90:189-199`; not executed for linspace |
| Files / network / RNG | none observed | n/a | n/a | `E3 code-derived` — `src/tools_grids.f90:1-30` |

## 5. Rules and invariants

| Rule | Evidence | Open question? |
|------|----------|----------------|
| Default `istart` and `iend` are true: both endpoints are included. | `E3 code-derived` — `src/tools_grids.f90:8-14` | no for first-slice default path |
| With both endpoints included and `num >= 2`, `step = (stop-start)/real(num-1,8)` and `array(i) = start + real(i-1,8)*step`. | `E3 code-derived` — `src/tools_grids.f90:11-14`; `E1` confirms FIX-001 | no |
| FIX-001 values are `0, 0.25, 0.5, 0.75, 1` with exact parsed equality. | `E1 verified` — `docs/modernization/oracle.md:96,109`; ADR-003 | no |
| `num < 0` calls `error("linspace: N<0, abort.")` then `STOP`. | `E3 code-derived` — `src/tools_grids.f90:7`; `src/COMVARS.f90:189-199` | yes — managed exception mapping accepted in principle (ADR-002); message/process compatibility not specified |
| Both endpoints included and `num < 2` calls `error("linspace: N<2 with both start and end points")`. | `E3 code-derived` — `src/tools_grids.f90:12` | yes — unexecuted |
| If only start is included, `step = (stop-start)/num` and samples are `start + (i-1)*step` for `i=1..num` (stop excluded). | `E3 code-derived` — `src/tools_grids.f90:16-18` | yes — unexecuted; out of first-slice parity |
| If only stop is included, `step = (stop-start)/num` and samples are `start + i*step` for `i=1..num` (start excluded). | `E3 code-derived` — `src/tools_grids.f90:20-22` | yes — unexecuted; out of first-slice parity |
| If neither endpoint is included, `step = (stop-start)/(num+1)` and samples are `start + i*step` for `i=1..num`. | `E3 code-derived` — `src/tools_grids.f90:24-27` | yes — unexecuted; out of first-slice parity |
| Result array is sized as `array(num)` in the function header before the `num<0` check. | `E3 code-derived` — `src/tools_grids.f90:1-7` | yes — see DEF-002 |
| CLI help describes the same job (“evenly spaced numbers over a specified interval”) with defaults `wmin=-5`, `wmax=5`, `L=1024`. | `E3 code-derived` — `numutils/src/linspace.f90:17-25` | yes — CLI not in first slice |

## 6. Error handling and edge cases

| Case | Legacy behavior | Evidence | Defect decision |
|------|-----------------|----------|-----------------|
| `num < 0` | `error` + `STOP` | `E3` — `src/tools_grids.f90:7` | TBD — DEF-002 notes sizing-before-check; decision open |
| Inclusive endpoints and `num < 2` | `error` + `STOP` | `E3` — `src/tools_grids.f90:12` | none for happy-path slice; parity out of scope until executed |
| `start == stop`, `num >= 2`, inclusive | Constant sequence at `start` by the formula | `E4 inferred` from formula; not executed | TBD — not in FIX-001 |
| Decreasing interval (`start > stop`) | Negative step; still includes both ends | `E4 inferred` from formula; not executed | TBD |
| Optional endpoint flags | Four mutually exclusive spacing rules | `E3` — `src/tools_grids.f90:11-27` | Out of first-slice parity |
| CLI malformed `RANGE` / missing args | PARSE_CMD + defaults; unverified | `E5 unknown` | Out of scope |

## 7. Draft Gherkin

```gherkin
Scenario: Inclusive five-point unit interval
  Given a linspace request with start 0, stop 1, and length 5
  And   default inclusive endpoints
  When  the host-neutral linspace port is invoked
  Then  the result has length 5
  And   the values equal 0, 0.25, 0.5, 0.75, and 1 exactly
```

```gherkin
Scenario: Invalid negative length
  Given a linspace request whose length is less than 0
  When  the host-neutral linspace port is invoked
  Then  the call fails as a typed domain error
  # Legacy: error("linspace: N<0, abort.") then STOP. Exact message/process mapping is open.
```

## 8. Legacy code and documentation citations

| Source | Lines / section | Claim supported | Evidence grade |
|--------|-----------------|-----------------|----------------|
| `fidelity/driver.f90` | 12–19 | Probe invocation `linspace(0.d0, 1.d0, 5)` | E1 verified / E3 code-derived |
| `docs/modernization/oracle.md` | 83, 96, 109 | Repeated capture, hash, parsed diff 0 | E1 verified |
| `src/tools_grids.f90` | 1–30 | Formula, defaults, abort conditions, `mesh` | E3 code-derived |
| `src/TOOLS.f90` | 18, 161 | Public export; include of `tools_grids.f90` | E3 code-derived |
| `src/COMVARS.f90` | 189–199 | `error` writes stdout and `STOP`s | E3 code-derived |
| `numutils/src/linspace.f90` | 1–51 | CLI help, defaults, `RANGE`, list-directed write | E3 code-derived (out of slice) |
| `fidelity/golden/linspace-5.txt` | entire file | Candidate formula reference; not E1 | E3 code-derived / E4 inferred |

## 9. Oracle fixtures

| Fixture | Input | Expected output | Tolerance / normalization | Evidence |
|---------|-------|-----------------|---------------------------|----------|
| `FIX-001` | `start=0, stop=1, num=5`, default endpoints | `0, 0.25, 0.5, 0.75, 1` | Exact parsed equality; no text compare | `E1 verified` — `docs/modernization/fixtures/FIX-001-linspace-5.md`; ADR-003 |

## 10. Open questions

- [x] Is `linspace` the first retained behavior? Yes — owner 2026-08-19.
- [x] Is the probe environment accepted for this behavior? Yes — ADR-001.
- [x] Managed API vs CLI vs HTTP for the first driving adapter? Managed API — ADR-002.
- [ ] Should optional `istart`/`iend`/`mesh` be part of the managed port in this slice or deferred?
- [ ] What typed exception/result type and message stability are required for aborting cases?
- [ ] Must `start == stop` and decreasing intervals be specified before implementation, or only FIX-001?
- [ ] Are any downstream Fortran `linspace` callers in supported scope besides the fidelity driver?

## 11. Tensions / conflicts

- CLI help names the program `linspace` while the Fortran program unit is `linsp`. First slice uses the library function, not the executable. `E3` — `numutils/src/linspace.f90:1-13`.
- Fidelity driver formats with `es24.17`; CLI uses list-directed `write(*,*)`. Managed-API parity ignores both. `E3` — `fidelity/driver.f90:17`; `numutils/src/linspace.f90:47-49`.
- Checked-in golden file matches FIX-001 numerically but is Python-generated. ADR-001 forbids treating that file as E1. `E1/E3/E4` — `docs/modernization/oracle.md:20,101`; `scripts/fidelity.sh` (legacy).

## 12. Links

- Intent ledger: `docs/modernization/intent-ledger.md` (INT-009)
- Legacy flow: `docs/modernization/flows/BEH-001-linspace.md`
- Defect ledger: `docs/modernization/defect-ledger.md`
- ADRs: `docs/decisions/ADR-001-first-slice-oracle-baseline.md`, `docs/decisions/ADR-002-hexagonal-managed-api.md`, `docs/decisions/ADR-003-linspace-numeric-contract.md`
- Migration plan: `docs/modernization/migration-plan.md` (VS-1)

*Created: 2026-08-19*
