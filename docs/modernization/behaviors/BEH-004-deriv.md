# BEH-004: Numerical derivative of tabulated Y(X)

**Status:** Ready for Legacy Documentation (T1 characterized; not fixture-recovered)
**Evidence grade:** `E1 verified` for probe section hash only; parsed 1024-row output was not retained
**Legacy surfaces:** Library function `TOOLS.deriv`. CLI `numutils/src/deriv.f90` is an adapter (BEH-205).
**Date:** `2026-08-19`

---

## 1. Summary

A caller supplies tabulated samples and a step `dh`. The library returns a derivative sequence. The 2026-08-10 driver read `numutils/test/xy2.data` (1,024 rows) and called `deriv(y,dh)`, producing a byte-identical section (SHA-256 `8a8879bc89a240338320275532f418c332b4120a536c23a8cb9428ca99b30b6f`). The checked-in `xy2.deriv` / golden copy is **not** E1.

## 2. Oracle

| Capture | Input | Evidence |
|---------|-------|----------|
| `CAP-20260810-DERIV` | 1,024 rows from `xy2.data` plus driver `dh` | `E1` hash; `fidelity/driver.f90:41-67`; `docs/modernization/oracle.md:100` |

## 3. Open questions

- [ ] Recover parsed output rows or a retained capture
- [ ] Stencil, endpoint rules, and `dh` source
- [ ] Comparison rule (script reported formula error `4.885e-15`; that is not the parity rule)

## 4. Links

- Catalog: `docs/modernization/behavior-catalog.md`
- Oracle: `docs/modernization/oracle.md`
- Related: ADR-005

*Created: 2026-08-19*
