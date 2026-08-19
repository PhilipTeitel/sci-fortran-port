# ADR-001: Accept the operational probe as the first-slice oracle baseline

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

`/assess-modernization` verified that repository `master` at `e586903a26cc50ca8942f20ca3bccbd8814e6252` built and ran twice under GNU Fortran 16.1.0, OpenBLAS 0.3.34, and the Numerical Recipes FFT compile selection. That probe is **operational**, not automatically a production/parity authority. Condition 1 blocked first-slice recovery until an owner accepted that environment for the selected behavior or supplied a different baseline.

On 2026-08-19 the owner selected library `linspace` as the first black-box slice and accepted the probe environment for that slice.

---

## Decision

For **BEH-001** (`linspace` library behavior) only:

- The accepted source revision is `e586903a26cc50ca8942f20ca3bccbd8814e6252` (tree `691333e7709f4d3396fbe714726e0a010a71355b`).
- The accepted oracle environment is the recorded 2026-08-10 probe: macOS 26.5.2 arm64, GNU Fortran 16.1.0, OpenBLAS 0.3.34, `FFT_BACKEND=NR`, `LC_ALL=C`, `LANG=C`, `TZ=UTC`, thread counts pinned to 1.
- Observed fidelity-driver output for `linspace(0,1,5)` is accepted legacy truth for this slice. The checked-in Python-generated `fidelity/golden/linspace-5.txt` is **not** the authority; it is a candidate reference that happened to match with maximum absolute error `0`.
- This baseline does **not** become the production/parity authority for any other module, CLI utility, FFT/matrix path, or unexecuted `linspace` branch.

---

## Consequences

**Positive**

- Recovery, fixtures, and later parity work for BEH-001 have a pinned revision and environment.
- The scoped T1 oracle can be cited without promoting the whole library.

**Negative / costs**

- A later production-baseline decision may invalidate or require re-capture of this slice.
- Unexecuted `linspace` branches (endpoint flags, `mesh`, invalid `num`, CLI) remain outside T1.

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Wait for a distinct production binary/environment | Owner accepted the probe for this exercise slice |
| Treat all five fidelity sections as accepted | Only `linspace` was selected; `arange-5` is not the legacy `arange` surface |
| Promote the Python golden file to E1 | Provenance remains formula-generated; E1 is the repeated probe capture |

---

## Explicit non-decisions

- This ADR does not accept OpenBLAS or Numerical Recipes as product providers.
- This ADR does not cover the 17 CLI utilities, FFT, BLAS/LAPACK, RNG, plotting, or ASP.NET hosting.
- This ADR does not choose target numeric libraries.

## Later amendment (2026-08-19)

ADR-005 extends this baseline from BEH-001-only to the **POC planning baseline for all retained surfaces**. Executable T1 coverage is still the fidelity corpus. This ADR’s non-decision about OpenBLAS/NR as product providers is replaced by ADR-005 POC substitution defaults (reimplement behind ports; do not copy NR/Intel source).

## Links

- Assessment: `docs/modernization/ASSESSMENT.md` Condition 1 (POC baseline; T1 corpus still bounded)
- Oracle: `docs/modernization/oracle.md`
- Behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Related: ADR-002, ADR-003, ADR-005
