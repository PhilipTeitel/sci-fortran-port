# BEH-002: Generate a logarithmic sequence (logspace)

**Status:** Ready for Legacy Documentation (T1 characterized; not fixture-recovered)
**Evidence grade:** `E1 verified` for probe section hash only; parsed values were not retained; formula/golden file is not authority
**Legacy surfaces:** Library function `TOOLS.logspace`. CLI `numutils/src/logspace.f90` is an adapter (BEH-202).
**Date:** `2026-08-19`

---

## 1. Summary

A caller supplies start, stop, and length. The library returns logarithmically spaced `real(8)` samples. The 2026-08-10 fidelity driver called `logspace(1,1000,5)` and produced a byte-identical section across two clean builds (SHA-256 `c5b198afbc3ccea1a27ded3ac9f3919952a6662c2465000c84e35e8f964db4fe`). Parsed values were deleted with the capture; recover them during `/document-legacy` before `/refine-feature`.

## 2. Oracle

| Capture | Input | Evidence |
|---------|-------|----------|
| `CAP-20260810-LOGSPACE` | `logspace(1.d0, 1000.d0, 5)` | `E1` hash; `fidelity/driver.f90:19-25`; `docs/modernization/oracle.md:97` |

Do not treat `fidelity/golden/logspace-5.txt` as E1 (Python formula). ADR-001 provenance rule applies.

## 3. Open questions

- [ ] Recover parsed sample values (or a retained capture) from a repeat of the probe recipe
- [ ] Base parameter and endpoint conventions
- [ ] Comparison rule: exact parsed equality vs a later tolerance ADR

## 4. Links

- Catalog: `docs/modernization/behavior-catalog.md`
- Oracle: `docs/modernization/oracle.md`
- Related: BEH-001, ADR-005

*Created: 2026-08-19*
