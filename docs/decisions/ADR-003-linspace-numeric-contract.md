# ADR-003: BEH-001 numeric representation and comparison

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

Public SciFortran numerics use `real(8)` / kind-8. The workflow profile still lists relative and absolute `1e-6`, while the merged fidelity script uses absolute-only `1e-10`. Neither threshold was an accepted parity rule. The 2026-08-10 probe compared two independent `linspace(0,1,5)` captures with parsed maximum absolute difference `0`, and the script's formula comparison for that section also reported error `0`.

The first slice is a managed API (ADR-002), so Fortran `es24.17` / list-directed text is not the product contract.

---

## Decision

For **BEH-001** only:

- Map legacy `real(8)` sequence values to IEEE-754 binary64 (`double`) in the managed API.
- The accepted fixture `FIX-001` (`linspace(start=0, stop=1, num=5)` with default inclusive endpoints) requires **exact parsed numeric equality** with the observed probe values:

  `0`, `0.25`, `0.5`, `0.75`, `1`

- Do **not** use profile `1e-6` or script `1e-10` as the pass/fail rule for this fixture.
- General default-endpoint evaluation follows the legacy formula  
  `array(i) = start + (i-1) * (stop-start)/(num-1)`  
  for `i = 1..num` with `num >= 2`. Additional fixtures are required before claiming parity beyond `FIX-001`.
- Text/locale/complex-column codecs are out of scope for this managed-API slice.

---

## Consequences

**Positive**

- This fixture is exactly representable in binary64, so exact equality is meaningful.
- Parity cannot be quietly loosened to `1e-6`.

**Negative / costs**

- Other `linspace` inputs, especially non-dyadic spacings, need their own fixtures and may need a later tolerance ADR.
- Optional `istart`/`iend`/`mesh` branches remain unaccepted for parity.

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Profile relative/absolute `1e-6` | Unproven; would hide exact-match evidence for this fixture |
| Script absolute `1e-10` | Script choice, not owner-approved; unnecessary here |
| Byte-level Fortran formatted output | Conflicts with managed-API boundary (ADR-002); capture bytes were not retained |

---

## Explicit non-decisions

- This ADR does not set a global numeric tolerance for the library.
- This ADR does not accept `logspace`, `fermi`, `deriv`, or CLI formatting contracts.
- This ADR does not decide signed-zero, NaN, or overflow behavior; those paths were not executed.

---

## Later amendment (2026-08-19)

The decision line "text/locale/complex-column codecs are out of scope for this managed-API slice" was written scoped to BEH-001. **ADR-007 generalizes it library-wide**: with `IOTOOLS` reclassified as a driven port with adapters, no retained surface carries a text or complex-column fidelity requirement. The numeric contract for BEH-001 is otherwise unchanged.

Per-slice numeric contracts in the shape of this ADR are still required for each new vertical slice (ADR-008), notably for `MATRIX`.

---

## Links

- Oracle: `docs/modernization/oracle.md` CAP-20260810-LINSPACE
- Fixture: `docs/modernization/fixtures/FIX-001-linspace-5.md`
- Behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Related: ADR-001, ADR-002; generalized by ADR-007; slice scope per ADR-008
