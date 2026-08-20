# ADR-008: Build representative vertical slices, not a complete library port

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

`docs/modernization/migration-plan.md` sequenced twenty-five slices covering every retained module. ADR-006 withdrew ten of them with the CLI. The remaining fifteen still describe a complete port of the SciFortran library.

That is not the objective. The owner's goal is a proof of concept that **Artifact-Driven Development extends from greenfield work to migration**, prepared as evidence for a prospective client who wants a set of thick applications converted to thin UIs backed by services. The C# library is the artifact that demonstrates the method; it is not a product anyone intends to ship or maintain.

This changes the definition of done. A complete port is measured by breadth of coverage. A methodology demonstration is measured by whether the artifact trail — legacy characterization, recovered behavior, decisions, requirements, stories, implementation, verification — holds together end to end on surfaces hard enough to be convincing. Fifteen slices of grid and scalar-function arithmetic would not be more convincing than three well-chosen ones, and would cost five times as much.

Until now no artifact stated what "enough" means, so the plan implied all fifteen.

---

## Decision

1. **Build scope is a small set of representative vertical slices**, each carried end to end through the full ADD pipeline, rather than a complete library port.

2. **The named slices are:**

   | Slice | Behavior | Why this one |
   |-------|----------|--------------|
   | VS-1 | `BEH-001` `TOOLS.linspace` | The oracle is already settled: T1 evidence, `FIX-001` accepted, ADR-001/003 in place. Proves the pipeline end to end with no argument about the baseline. |
   | VS-2 | `BEH-003` `FUNCTIONS.fermi` | T1 fidelity-corpus evidence exists but **no fixture has been recovered**. Exercises `/document-legacy` and fixture capture for real, which VS-1 does not. |
   | VS-3 | `BEH-040` `MATRIX` (inverse, diagonalize, solve) | Catalog-only, no T1 evidence, and it needs the BLAS/LAPACK dependency decided behind a numeric port. This is the hard case, and the one a skeptical client actually asks about. |

3. **VS-3 is the load-bearing slice.** A demonstration that only covers pure scalar arithmetic proves nothing about migrating real applications. VS-3 must show an external numeric dependency substituted behind a port, with parity evidence, without copying vendored source.

4. **Optional fourth slice:** `BEH-004` `deriv` is cheap — T1 evidence already exists — and adds array-input coverage for the `BEH-302` layout contract. Add it only if array-shape contracts need demonstrating for a particular audience.

5. **VS-3 is swappable.** `BEH-050` `FFTGF` may replace `MATRIX` if complex-valued transforms tell a better story for a given audience. It carries the same shape of difficulty (reimplement an NR-selected contract behind a transform port, no T1 evidence). Do not build both.

6. **Everything else stays catalogued, not planned.** The remaining modules in ADR-005 §3 remain retained *in principle* and keep their catalog entries. They are not planned work and no story exists for them. They are the reserve if the demonstration needs more breadth.

7. **Done means:** each named slice passes its ADD gates with recorded evidence, and the artifact trail from legacy characterization to passing C# is complete and inspectable for that slice. Breadth of library coverage is explicitly **not** the measure of completion.

---

## Consequences

**Positive**

- Planned work drops from twenty-five slices to three, or four with the optional `deriv`.
- Each slice is a complete artifact trail rather than a partial one, which is what the demonstration is actually selling.
- VS-3 forces the dependency-substitution question into the open early, where it is a demonstration asset instead of a late surprise.
- The reserve modules mean added breadth costs planning, not re-decision.

**Negative / costs**

- **The C# result will be a fragment, not a usable SciFortran library.** No artifact may describe it as a port of the library. `PURPOSE.md` is restated accordingly.
- VS-3 has no T1 fixture. Capturing one from the legacy probe is the most expensive step in the plan and the likeliest thing to slip. It also needs its own numeric-contract ADR at the slice, in the shape of ADR-003.
- Choosing `MATRIX` makes ADR-005 §6's BLAS/LAPACK default real work rather than a planning placeholder.
- Modules left in reserve keep catalog entries that describe behavior nobody will implement. They must stay clearly marked as reserve so the artifact set does not imply committed work.

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Port all retained library modules | Fifteen slices for a demonstration nobody ships; diminishing methodological return after the third |
| Do `TOOLS` and `FUNCTIONS` first, then reassess | Every surface is easy arithmetic; would not demonstrate dependency substitution, which is the part clients doubt |
| Keep the full plan and stop when satisfied | No definition of done; the artifact set would keep asserting committed work that will never happen |
| Pick slices by legacy code size | Size does not correlate with migration difficulty; dependency and contract risk do |

---

## Explicit non-decisions

- This ADR does not change any numeric contract, oracle baseline, or retained/retired module list.
- This ADR does not choose the BLAS/LAPACK provider for VS-3. That is a slice ADR.
- This ADR does not decide whether an HTTP adapter is built to demonstrate the services direction.

---

## Links

- Supersedes in part: `docs/modernization/migration-plan.md` SL-001–SL-015 sequencing
- Related: ADR-004 (whole-library authorization, now narrowed in practice), ADR-005 §3 and §6, ADR-006, ADR-007
- Purpose: `docs/PURPOSE.md`
- Plan: `docs/modernization/migration-plan.md`
