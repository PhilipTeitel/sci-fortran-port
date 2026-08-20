# ADR-004: Port the whole SciFortran surface to C#

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

Recovery through 2026-08-19 treated this repository as a framework exercise whose first (and only authorized) slice was library `linspace`. PURPOSE, the assessment, and ADRs 001–003 therefore kept every other SciFortran module and CLI out of scope and directed the next command to `/refine-feature` on BEH-001 rather than `/plan-migration`.

The owner has now corrected that purpose: the project must port the Fortran library’s functionality to usable C# that reproduces legacy behavior. Licensing and provenance are accepted as out of scope because this is a private POC, not a redistribution. The immediate need is to reach the normal ADD planning gate (`/plan-migration`) rather than implementing only the linspace walking skeleton.

---

## Decision

1. The product is a **C# / .NET 8 port of the retained SciFortran surface** at probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`. Success is usable C# that reproduces accepted legacy behavior, not a documentation-only ADD demonstration.
2. **Whole-library scope is authorized.** The retained surface is the public `SCIFOR` facade (its imported modules and their public procedures) plus the CLI utilities that can be built from this checkout, except surfaces explicitly retired in ADR-005.
3. This remains a **private POC**. Licensing, Intel-confidential headers, Numerical Recipes provenance, and mixed vendored notices do **not** block planning or C# implementation. They still forbid publishing or redistributing restricted source, and Intel interface files must not be copied into the target tree.
4. Hexagonal architecture and a host-neutral managed API remain the product boundary (ADR-002, extended by ADR-005). ASP.NET Core is a later optional driving adapter, not the library contract.
5. BEH-001 / FIX-001 remain the **first implementation slice** after `/plan-migration` sequences the work. They are no longer the only retained behavior.
6. `/plan-migration` is unblocked. `/refine-feature` on BEH-001 is the first code-production slice inside that plan, not a substitute for library-wide planning.

---

## Consequences

**Positive**

- PURPOSE, domain, catalog, and assessment can describe the real product.
- Migration planning can sequence module and CLI slices instead of stopping at linspace.

**Negative / costs**

- Most surfaces are still T3 (documented-only) or T1-without-recovered-fixtures. The plan must schedule `/document-legacy` and `/refine-feature` per slice before implementation-ready stories.
- Missing source (`DLPLOT` / FFTPACK callees) cannot be ported; those surfaces are retired rather than guessed (ADR-005).

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Keep the linspace-only exercise thesis | Owner stated the whole surface must be ported and C# must be produced |
| Wait for production legal clearance before planning | Owner accepted licensing as irrelevant for this POC |
| Plan a product-wide ASP.NET rewrite first | No legacy web topology; hexagonal core must exist before any host |

---

## Explicit non-decisions

- Per-procedure C# signatures, NuGet packaging, and solution layout remain design/story work.
- Behavior-specific tolerances beyond FIX-001 remain per-slice ADRs.
- Downstream Fortran ABI compatibility (`.mod` / `libscifor.a`) is not required (ADR-005).

---

## Later amendment (2026-08-19)

`/plan-migration` produced `docs/modernization/migration-plan.md` (SL-001–SL-025). `/refine-feature` produced `docs/requirements/REQ-001-linspace.md` (`Draft`, S1–S6) on 2026-08-20. Next command is `/design-application` for VS-1, then `/plan-port-story`.

## Second amendment (2026-08-19)

This ADR's **authorization** of the whole retained library stands: no module is forbidden, and the reserve remains available. What changed is **planned work**.

- **ADR-008** narrows the build to representative vertical slices, because the objective is demonstrating that Artifact-Driven Development extends to migration, not shipping a SciFortran replacement. Decision 1's "usable C# that reproduces accepted legacy behavior" holds for the slices built; it is no longer a claim about the library as a whole.
- **ADR-006** retires the CLI programs referenced in decision 2 from build scope.
- Decision 6's `/plan-migration` output is superseded in sequencing by ADR-008.

## Links

- Assessment: `docs/modernization/ASSESSMENT.md` Conditions 1–11
- Catalog: `docs/modernization/behavior-catalog.md`
- Migration plan: `docs/modernization/migration-plan.md`
- Related: ADR-001, ADR-002, ADR-003, ADR-005
