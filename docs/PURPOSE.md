# Purpose: SciFortran port exercise (first slice)

**Source material:**
- Owner decisions dated 2026-08-19: select `linspace`, accept the 2026-08-10 probe environment, hexagonal architecture, first driving adapter = managed API
- `docs/modernization/ASSESSMENT.md` (go-with-conditions for a private framework exercise)
- `docs/modernization/intent-ledger.md` INT-001 (checkout is an experiment / POC)
- Repository commit `7c64bd6` (“must never be used for anything but a POC”)
- Recovered BEH-001 and FIX-001
- ADRs 001–003

**Date:** 2026-08-19
**Status:** Draft (first-slice recovery; not a production product thesis)

---

## Thesis

This repository exists to prove that Artifact-Driven Development can recover, bound, and re-host a real Fortran numerical behavior as a host-neutral C# capability — not to ship a production SciFortran replacement.

## The job it does

A numerical-library maintainer (or framework evaluator) needs one actually executed, dependency-light operation — generating an inclusive linear sequence — translated behind a managed API so later CLI or HTTP adapters can be added without changing the arithmetic. The job matters because the assessment forbade implementing a walking skeleton until a behavior, baseline, and boundary were chosen.

## North-star outcome

A caller of the managed linspace port gets the same parsed `linspace(0,1,5)` values the 2026-08-10 probe observed, with evidence graded and fixture-backed, while every other SciFortran surface remains explicitly out of scope.

## Trade-off rule

When goals conflict, optimize for **honest, host-neutral parity of the selected behavior** over **surface-area coverage, Fortran ABI compatibility, or a premature web host**.

The assessment is a controlled POC. Expanding to 17 CLIs, FFT/BLAS providers, or ASP.NET before this slice has recovered documentation would recreate the blockers that stopped progress.

## Anti-thesis

- A full SciFortran-on-ASP.NET rewrite presented as a drop-in production port.
- A CLI-format clone of `write(*,*)` treated as the product because that is what Fortran printed.
- Treating Python-generated golden files as legacy truth.
- Silently “fixing” unexecuted branches, CLI name mismatches, or `STOP` semantics during the first slice.

## Success signals

- BEH-001 stays the only implementation-ready behavior until further owner selections.
- FIX-001 is compared with exact parsed equality, not profile `1e-6`.
- Domain code has no ASP.NET or CLI dependency (ADR-002).
- Unselected modules remain labeled out of scope rather than half-translated.

## Open purpose questions

- [ ] If this exercise succeeds, is the next retained behavior another grid helper (`logspace`) or a different family?
- [ ] Does a later production thesis replace this POC thesis, or is the port permanently exercise-only?
- [ ] Are historical Fortran consumers of `linspace` in scope after the managed API exists?

## Links

- Related domain model: `docs/DOMAIN.md`
- Related ADRs: ADR-001, ADR-002, ADR-003
- Related behavior: `docs/modernization/behaviors/BEH-001-linspace.md`

---

*Created: 2026-08-19 | Modeled by: modeler in Legacy recovery mode (in-chat fallback; no subagent delegation)*
