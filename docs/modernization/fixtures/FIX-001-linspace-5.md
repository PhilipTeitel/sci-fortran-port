# FIX-001: linspace(0, 1, 5) default endpoints

**Behavior:** BEH-001
**Status:** Accepted for first-slice parity
**Date:** 2026-08-19
**Evidence grade:** `E1 verified` for parsed values; capture bytes were not retained

---

## Provenance

| Field | Value |
|-------|-------|
| Source revision | `e586903a26cc50ca8942f20ca3bccbd8814e6252` |
| Probe date | 2026-08-10 |
| Capture ID | `CAP-20260810-LINSPACE` |
| Section SHA-256 (ephemeral stdout extract) | `dabd07f92b83714a0e0740223261fc457c517025cf3f4b08cd51380a0082c35c` |
| Cross-build parsed max abs diff | `0` |
| Script formula max abs error | `0` (does **not** make `fidelity/golden/linspace-5.txt` E1) |
| Authority | ADR-001, ADR-003 |

The original stdout bytes were deleted after the probe. This fixture records the **parsed numeric sequence**, which is the managed-API contract. Do not treat the Python-generated golden file as the source of truth.

## Inputs

| Name | Value |
|------|-------|
| `start` | `0` |
| `stop` | `1` |
| `num` | `5` |
| `istart` | omitted (legacy default `.true.`) |
| `iend` | omitted (legacy default `.true.`) |
| `mesh` | omitted |

## Expected output

Parsed `real(8)` / binary64 values, index origin 1 in the legacy call, stored here as a zero-based managed sequence:

```
0.00000000000000000e+00
2.50000000000000000e-01
5.00000000000000000e-01
7.50000000000000000e-01
1.00000000000000000e+00
```

## Comparison rule

Exact parsed numeric equality (ADR-003). No relative/absolute tolerance. Text formatting is out of scope.

## Legacy invocation that produced the capture

Fidelity driver: `x = linspace(0.d0, 1.d0, n)` with `n = 5`, then `write (*, '(es24.17)')` per element. `E1 verified` — `fidelity/driver.f90:12-19`; `docs/modernization/oracle.md:83,96`.
