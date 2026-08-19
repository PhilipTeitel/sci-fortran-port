# Purpose: SciFortran C# port (POC)

**Source material:**
- Owner correction dated 2026-08-19: port the whole Fortran library surface to usable C#; licensing is irrelevant for this private POC
- `docs/modernization/ASSESSMENT.md` (go-with-conditions, now narrowed to a whole-library POC)
- Probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252`
- ADRs 001–005
- Recovered BEH-001 / FIX-001 (first implementation slice, not the product boundary)

**Date:** 2026-08-19
**Status:** Accepted (whole-library POC thesis)

---

## Thesis

This repository exists to **port SciFortran’s retained library functionality to C#** so callers get the same numerical results the legacy library produced at the accepted probe baseline. It is a private proof of concept. It is not a license to redistribute restricted Fortran, and it is not an ASP.NET rewrite of a system that had no web topology.

## The job it does

A numerical-library maintainer needs a host-neutral C# library that reproduces the public `SCIFOR` surface (grids, functions, integration, matrix, FFT, optimization, splines, statistics, Green/Padé/lattice helpers, and I/O ports) plus CLI adapters over those same ports. The job matters because a linspace-only exercise cannot plan code production for the rest of the library.

## North-star outcome

A C# caller of the managed port can exercise retained SciFortran behavior and obtain legacy-faithful results, starting with exact `linspace(0,1,5)` and expanding module by module under `/plan-migration`. Surfaces that cannot be built from this checkout (`vfplot`/`DLPLOT`, FFTPACK callees) are retired rather than faked.

## Trade-off rule

When goals conflict, optimize for **usable C# with honest parity of retained behavior** over **Fortran ABI compatibility, ASP.NET hosting, or expanding into missing/unexported source**.

Do not silently “fix” unexecuted branches. Schedule `/document-legacy` and `/refine-feature` for each slice before claiming implementation-ready stories.

## Anti-thesis

- A documentation-only ADD exercise that never ships C#.
- A linspace-only product presented as a SciFortran port.
- A full SciFortran-on-ASP.NET rewrite treated as the first deliverable.
- Copying Intel-confidential headers or Numerical Recipes source into the target tree.
- Treating Python-generated golden files as legacy truth.

## Success signals

- [x] `/plan-migration` produces an ordered slice plan covering the retained catalog (`docs/modernization/migration-plan.md`).
- Each slice yields C# behind hexagonal ports, with CLI adapters calling the same use cases.
- FIX-001 (and later recovered T1 fixtures) pass with their accepted comparison rules.
- Retired surfaces are listed as out of product, not half-translated.

## Open purpose questions

- [ ] After the POC library exists, is a production thesis (legal clearance, packaging, support) ever required?
- [ ] Should unexported bundled special-function internals later join the public C# API?
- [ ] Are historical Fortran consumers of `libscifor.a` in scope after the managed API exists?

## Links

- Related domain model: `docs/DOMAIN.md`
- Related catalog: `docs/modernization/behavior-catalog.md`
- Related plan: `docs/modernization/migration-plan.md`
- Related ADRs: ADR-001–005
- Related behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Related requirements: `docs/requirements/REQ-001-linspace.md`

---

*Updated: 2026-08-19 | Owner purpose correction: whole-library C# port | Plan: `/plan-migration` | Refine: REQ-001*
