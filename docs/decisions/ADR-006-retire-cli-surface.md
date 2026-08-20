# ADR-006: Retire the CLI surface from build scope; keep its catalog as recovered evidence

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

ADR-005 §4 retained sixteen `numutils/src/` CLI programs as driving adapters and the migration plan sequenced them as SL-016 through SL-025 — ten of twenty-five slices. That inventory was produced from what the legacy checkout *contains*, not from what this effort needs.

The owner has since stated the objective directly: the parts that matter are the API. The CLI was never a requirement. It entered scope because the legacy repository ships CLI programs and the planning gate catalogued everything buildable.

Retaining it carries real cost beyond the ten slices. `func` (SL-024) requires a managed expression evaluator with a grammar recovered from characterization — a parser subproject whose only consumer is a CLI program. `splot` (SL-025) requires wrapping Gnuplot. `BEH-110` (SL-016) exists to reproduce `PARSE_CMD`, timer, and ANSI diagnostic behavior that only a CLI observes.

The CLI programs are still *useful as evidence*. Several are the clearest available demonstration of how a library procedure is called and what its arguments mean, and some recovered contradictions (help text versus code) were found by reading them.

---

## Decision

1. **No CLI program is built.** All sixteen programs listed in ADR-005 §4 — `deriv`, `kdensity`, `numstat`, `splot`, `func`, `wmatsubara`, `fftgf`, `arange`, `pade`, `logspace`, `linspace`, `fermi`, `spline`, `random`, `histogram`, and `ffcmplx` — move from *retained adapters* to *retired from build scope*.
2. **Their recovered documentation is retained.** `docs/modernization/behavior-catalog.md` §4 (`BEH-200`–`BEH-216`) stays in the repository as **recovered evidence about library behavior**, not as planned work. Catalog entries are reclassified accordingly; they are not deleted.
3. **Slices SL-016 through SL-025 are withdrawn** from `docs/modernization/migration-plan.md`.
4. **The managed expression evaluator (`func` / `libmatheval`) is out of scope.** ADR-005 §6's substitution default is withdrawn with the CLI job it served.
5. **Gnuplot wrapping is out of scope.** ADR-005 §6's `splot` plotting default is withdrawn. The IOTOOLS *data* helpers are handled separately by ADR-007.
6. **CLI-program defect rows are retired, not deferred.** `DEF-003`, `DEF-301`, `DEF-302`, `DEF-303`, `DEF-312`, and `DEF-313` describe contradictions observable only inside `numutils/src/` programs. They are recorded as **retired with evidence**: preserved with their findings, closed for this effort, and reopened only if a CLI adapter is ever added.

Rows about IOTOOLS readers and writers (`DEF-004`, `DEF-304`, `DEF-305`, `DEF-307`) and about diagnostics, channel, and exit status (`DEF-306`, `DEF-309`, `DEF-310`, `DEF-311`) are **not** retired here. ADR-007 disposes of those, on the separate ground that they are adapter concerns rather than CLI-only ones.

---

## Consequences

**Positive**

- Ten of twenty-five planned slices are withdrawn.
- An expression-grammar subproject and a Gnuplot integration disappear from the plan.
- Six defect rows close; with ADR-007's eight, only `DEF-002` and `DEF-308` remain open.
- `PARSE_CMD` loses its only consumer, which removes the last argument for treating CLI parsing as library surface.

**Negative / costs**

- If a CLI is ever wanted — for a demonstration, or because a client's legacy consumers need one — the retired catalog and defect rows must be reopened and their dispositions actually decided. Retirement is not resolution, and the recorded contradictions remain unresolved.
- Some library procedures lose their most legible usage example. Where a CLI program was the clearest evidence of how a library call is used, `/document-legacy` for that library slice must read the CLI source as evidence even though the program is not built.
- Legacy CLI users, if any exist, are not served. No inventory of such users was ever taken (ADR-002 consequences).

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Keep all CLI adapters as planned | Ten slices of work the owner does not want; `func` and `splot` add subprojects unrelated to the API |
| Delete the CLI catalog entirely | Discards recovered evidence about library usage and the contradictions found in it, for no benefit |
| Retire all but one demonstration CLI | A second driving adapter can be demonstrated more cheaply by an HTTP adapter, which is also the direction of travel (thin UI over services) |
| Retire the CLI *and* its defect rows outright | Loses the recorded findings; retirement with evidence costs nothing to keep |

---

## Explicit non-decisions

- This ADR does not decide whether an HTTP/ASP.NET driving adapter is built. That remains an optional later adapter per ADR-005 §2.
- This ADR does not dispose of the IOTOOLS reader/writer contradictions. See ADR-007.
- This ADR does not change the retained *library* module list in ADR-005 §3.

---

## Links

- Supersedes in part: ADR-005 §4 (retained CLI adapters), ADR-005 §6 rows for `func` and Gnuplot/`splot`
- Related: ADR-002 (§3 named CLI as a possible later adapter), ADR-007, ADR-008
- Catalog: `docs/modernization/behavior-catalog.md` §4
- Plan: `docs/modernization/migration-plan.md`
- Defect ledger: `docs/modernization/defect-ledger.md`
