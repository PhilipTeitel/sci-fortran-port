# BEH-303: Numeric text formatting and I/O codec contract

**Status:** Draft
**Evidence grade:** `E3 code-derived` (overall; partial `E1 verified` for fidelity fixed-format probe under `LC_ALL=C`; locale/NaN/EOF edge cases `E5 unknown`)
**Legacy surfaces:** CLI stdin/stdout streams; `IOTOOLS`/`SLREAD`/`SLPLOT` file codecs; fidelity driver prints; `COMMON_VARS` `txtfy`/`r8_to_s_left`
**Date:** `2026-08-10`

---

## 1. Summary

Numeric interchange is plain text via Fortran list-directed (`*`) and fixed-width formats (notably `es24.17`, `F18.10`, `g16.9`). Delimiters, exponent spelling, width, rounding, line endings, and special-value text are compiler- and locale-sensitive. Configured text normalization (trim, normalize line endings, case-sensitive) is a future comparison policy, not observed legacy formatting itself.

## 2. Actors and triggers

| Actor / system | Trigger | Preconditions | Evidence |
|----------------|---------|---------------|----------|
| CLI user / pipeline | Pipe whitespace-delimited numbers to stdin; read stdout | Utility running | `E3` — `numutils/src/deriv.f90:43-85`; `numutils/src/fftgf.f90:69-99` |
| Library file caller | `sread` / `splot` on pathnames | File present/writable | `E3` — `src/SLREAD.f90:13-29`; `src/slread_sread_V.f90:108-134,246-272`; `src/slplot_splot_V.f90:230-301` |
| Fidelity probe | Driver writes labeled sections | Built driver; `LC_ALL=C` | `E1` — `fidelity/driver.f90:10-65`; oracle §2 |
| PARSE_CMD | `NAME=value` argument parsing via `read(var%value,*)` | Args present | `E3` — `src/PARSECMD.f90:109-148` |

## 3. Inputs

| Input | Type / format | Units | Range / constraints | Required? | Evidence |
|-------|---------------|-------|----------------------|-----------|----------|
| List-directed numeric tokens | Fortran `read(unit,*)` whitespace-separated | n/a | Compiler list-directed rules; EOF ends loops | surface-specific | `E3` — `numutils/src/deriv.f90:43-44,60`; `numutils/src/fftgf.f90:70` |
| Fixed-width file fields | e.g. `F18.10`, `I15` | n/a | Width/overflow behavior compiler-defined | surface-specific | `E3` — `src/slread_sread_V.f90:108,246`; `src/slplot_splot_V.f90:230-301` |
| Command values | substring after `=` | n/a | List-directed `read` into typed variable | optional | `E3` — `src/PARSECMD.f90:97-99,119-121,140-142` |
| Locale | process environment | n/a | Probe forced `LC_ALL=C`; production locale unknown | unknown | `E1`/`E5` — oracle §2; GAP-007 |

## 4. Outputs and side effects

| Output / side effect | Type / format | Precision / ordering | Destination | Evidence |
|----------------------|---------------|----------------------|-------------|----------|
| List-directed numeric lines | `write(*,*)` / `write(ounit,*)` | Compiler-chosen width/exponent | stdout/file | `E3` — `numutils/src/deriv.f90:79-85`; `numutils/src/logspace.f90:47-48` |
| Fixed scientific lines | `es24.17` / `2(es24.17)` | 17 fractional digits in format string | fidelity stdout | `E3`/`E1` — `fidelity/driver.f90:15,23,29,37`; oracle §5 hashes |
| Fixed `F18.10` columns | file writers/readers | 10 decimal digits in format | data files | `E3` — `src/slplot_splot_V.f90:230-301` |
| `txtfy` real/complex strings | `g16.9` via `r8_to_s_left`; complex as `(re,im)` | left-adjusted; zero special-cased | diagnostics | `E3` — `src/COMVARS.f90:267-283,495-521` |
| Byte-identical captures under probe locale | full/section stdout | Exact bytes; no trim needed for probe | ephemeral | `E1` — oracle §§5-6 |

## 5. Rules and invariants

| Rule | Evidence | Open question? |
|------|----------|----------------|
| CLI numeric streams primarily use list-directed I/O. | `E3` — `numutils/src/deriv.f90:43-85`; legacy-map §6 | no |
| Fidelity driver uses explicit `es24.17` for several sections and list-directed `write(*,*)` for `deriv-xy2`. | `E3` — `fidelity/driver.f90:15-65` | no |
| `IOTOOLS` complex/real file codecs mix `*` and `F18.10`/`I15` formats by overload. | `E3` — `src/slread_sread_V.f90:108-134,246-272`; `src/slplot_splot_V.f90:130-301` | no |
| `r8_to_s_left` documents/`uses` a G-format write (`g16.9` in code; comment mentions G14.6). | `E3` — `src/COMVARS.f90:495-521` | yes — comment vs code |
| Probe repeatability required `LC_ALL=C`; other locales unverified. | `E1`/`E5` — oracle §2; GAP-007 | yes |
| Configured normalization (trim, newline normalize, case-sensitive) is a target comparison default, not legacy emission. | `E2` — `.cursor/workflow.config.yml:48-52` | yes |
| NaN/Infinity/signed-zero spellings are uncharacterized. | `E5` — legacy-map §6 caveat; GAP-007 | yes |

## 6. Error handling and edge cases

| Case | Legacy behavior | Evidence | Defect decision |
|------|-----------------|----------|-----------------|
| EOF on stdin read loops | Exit loop via labeled `end=` / `iostat` | `E3` — `numutils/src/deriv.f90:43-48`; `fidelity/driver.f90:48-51` | none |
| Malformed numeric token | Unverified (list-directed failure modes) | `E5` — GAP-007; legacy-map §7 | TBD |
| File open failure in fidelity driver | Prints message; `stop 1` | `E3` — `fidelity/driver.f90:43-46` | see BEH-305 / **DEF-309** |
| Tolerance / comparison regimes | Workflow `1e-6` vs script `1e-10` vs probe exact | `E1`/`E2`/`E3` — oracle §6; INT-006 | **DEF-308** open/TBD |
| `r8_to_s_left` comment vs code | Comment G14.6; write `g16.9` | `E3` — `src/COMVARS.f90:495-521` | **DEF-311** open/TBD |
| Width overflow in fixed formats | Not characterized | `E5` | TBD |
| Comma-decimal locale | Not run | `E5` — GAP-007 | TBD |

## 7. Draft Gherkin

```gherkin
Given a numeric CLI or file surface that uses Fortran text I/O
When values are written with list-directed or fixed formats
Then consumers must treat the format family as part of the surface contract
And exact delimiter/exponent/special-value bytes remain unspecified until captured on the accepted compiler/locale
And comparison policies (exact vs normalized) are decided per surface, not assumed from workflow defaults
```

## 8. Legacy code and documentation citations

| Source | Lines / section | Claim supported | Evidence grade |
|--------|-----------------|-----------------|----------------|
| `numutils/src/deriv.f90` | 43-85 | List-directed CLI read/write | E3 |
| `numutils/src/fftgf.f90` | 69-99 | List-directed complex column I/O | E3 |
| `numutils/src/logspace.f90` | 47-48 | List-directed sequence print | E3 |
| `fidelity/driver.f90` | 10-65 | `es24.17` and mixed `write(*,*)` | E3 |
| `src/COMVARS.f90` | 267-283,495-521 | `txtfy` / `g16.9` stringification | E3 |
| `src/slread_sread_V.f90` | 108-134,246-272 | Fixed vs list-directed reads | E3 |
| `src/slplot_splot_V.f90` | 230-301 | Fixed/list-directed writes | E3 |
| `src/PARSECMD.f90` | 109-148 | List-directed parse of CLI values | E3 |
| `scripts/fidelity.sh` | 11 | Absolute compare `TOL` default | E3 |
| `.cursor/workflow.config.yml` | 44-52 | Provisional tolerance/normalization | E2 |
| `docs/modernization/oracle.md` | §§2,5,6 | `LC_ALL=C` byte-identical captures | E1 |

## 9. Oracle fixtures

| Fixture | Input | Expected output | Tolerance / normalization | Evidence |
|---------|-------|-----------------|---------------------------|----------|
| `CAP-20260810-FULL` (ephemeral) | fidelity driver | SHA-256 `14a40532...8dd5` | Exact bytes under probe env | `E1` — oracle §5 |
| `CAP-20260810-LINSPACE` | `es24.17` lines | section hash | Exact bytes | `E1` |
| `CAP-20260810-DERIV` | list-directed rows | section hash | Exact bytes in probe; format differs from `es24.17` sections | `E1`/`E3` |
| Configured text policy | n/a | trim + newline normalize | Future fixtures only | `E2` |

## 10. Open questions

- [ ] Per surface: exact-byte compatibility vs semantic numeric equality after parsing?
- [ ] Which locales must be supported or rejected?
- [ ] What are accepted spellings for NaN, Infinity, and signed zero?
- [ ] Should fidelity `es24.17` become the compatibility format for migrated APIs, or only a probe artifact?
- [ ] Does `r8_to_s_left` comment (`G14.6`) or code (`g16.9`) define intended diagnostic formatting?

## 11. Links

- Intent ledger: `docs/modernization/intent-ledger.md` (INT-006/007)
- Legacy flow: `docs/modernization/flows/` (none yet)
- Defect ledger: `docs/modernization/defect-ledger.md` — DEF-308, DEF-311 (open/TBD); complex-column DEF-301–306 via BEH-304
- Related gaps: GAP-007, GAP-009, GAP-013 (column order separated in BEH-304)
- Related behavior: BEH-304 (complex-column order)

### Tensions / conflicts

- Workflow tolerances `1e-6` vs script `1e-10` vs probe exact equality — three different comparison regimes. `E1`/`E2`/`E3`/`E4` — oracle §6; INT-006.
- Fidelity sections mix `es24.17` and list-directed `write(*,*)`, so one global text codec cannot describe even the probe corpus. `E3` — `fidelity/driver.f90:15-65`.
- `r8_to_s_left` comment claims `G14.6` while the write uses `g16.9`. `E3` — `src/COMVARS.f90:495-518`.
- List-directed output is not a stable cross-compiler text contract. `E5` — legacy-map §6; GAP-007.

*Created: 2026-08-10*
