# BEH-003: Evaluate the Fermi function

**Status:** Ready for Legacy Documentation (T1 characterized; not fixture-recovered)
**Evidence grade:** `E1 verified` for probe section hash only; parsed values were not retained
**Legacy surfaces:** Library function `FUNCTIONS.fermi`. CLI `numutils/src/fermi.f90` is an adapter (BEH-204).
**Date:** `2026-08-19`

---

## 1. Summary

A caller supplies an energy-like value and inverse temperature `beta`. The library returns the Fermi–Dirac occupation. The 2026-08-10 driver evaluated `fermi(x,100)` for `x ∈ {-2,-1,0,1,2}` and produced a byte-identical two-column section (SHA-256 `6f35eadc7f917064b110353cebf6ab6468999625ceae0394744d68be67a1810a`).

## 2. Oracle

| Capture | Input | Evidence |
|---------|-------|----------|
| `CAP-20260810-FERMI` | `x={-2,-1,0,1,2}`, `beta=100` | `E1` hash; `fidelity/driver.f90:32-39`; `docs/modernization/oracle.md:99` |

## 3. Open questions

- [ ] Recover parsed `(x, fermi)` pairs
- [ ] Exact formula, overflow/`beta` large behavior, and complex overloads if any
- [ ] Comparison rule once values are retained

## 4. Links

- Catalog: `docs/modernization/behavior-catalog.md`
- Oracle: `docs/modernization/oracle.md`
- Related: ADR-005

*Created: 2026-08-19*
