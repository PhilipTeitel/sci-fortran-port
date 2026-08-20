# ADR-007: File I/O, CLI parsing, timing, and console diagnostics are adapters, not domain

**Status:** Accepted
**Date:** 2026-08-19

---

## Context

Two accepted ADRs contradict each other.

ADR-002 §2 states that domain and use-case code for retained numerical behavior is host-neutral and **"must not depend on ASP.NET, a CLI parser, Fortran formatted I/O, or filesystem/shell adapters."**

ADR-005 §3 then lists four modules among the retained **library** modules whose public procedures are to be ported:

| Module | ADR-005 §3 description | What it actually is |
|--------|------------------------|---------------------|
| `IOTOOLS` (`IOFILE`, `SLPLOT`, `SLREAD`) | "file metadata, `splot`/`sread` data helpers" | Formatted file I/O and filesystem/gzip helpers |
| `PARSE_CMD` | "CLI adapter support" | A CLI argument parser |
| `COMMON_VARS` | "diagnostics mapped at the port; ANSI helpers as adapter concerns" | Console diagnostics and ANSI styling |
| `TIMER` | "`start_timer`/`stop_timer`/`eta`/`print_bar` as adapter/progress ports" | Wall-clock timing and console progress output |

Every one of these is precisely what ADR-002 §2 excludes from the host-neutral core, yet ADR-005 §3 places them inside the product surface and says "port every public procedure of" it. ADR-005 §3 hedges three of the four with the word "adapter" in their descriptions while still listing them as library modules, which is the contradiction in miniature.

This is not academic. It is what forced the awkward carve-out in `docs/PURPOSE.md`, where text codecs and complex-column order had to remain product contracts *only for IOTOOLS* while being adapter concerns everywhere else. The carve-out was preserving the contradiction rather than resolving it.

The owner has confirmed the hexagonal intent: ports exist so that today's managed API can later be exposed as services. That intent decides the question — I/O format is exactly the kind of thing that must sit outside the domain if the same use case is to serve a library caller today and an HTTP caller later.

---

## Decision

1. **ADR-002 §2 is authoritative.** The host-neutral core does not depend on formatted I/O, the filesystem, CLI parsing, wall-clock timing, or console output.

2. **`IOTOOLS` is reclassified as a driven port with adapters.** The domain does not read or write files. Use cases exchange numeric values through a driven port; an adapter decides how those values are persisted or transported. The Fortran `splot`/`sread` procedures are evidence of *what data* the legacy library moved across that boundary, not a specification of the bytes the port must emit.

3. **No legacy file-format fidelity is required.** The file adapter serializes idiomatically for .NET. Delimiters, `es24.17` versus list-directed forms, external complex-column order, and gzip behavior are adapter implementation choices. This effort does not need to read or write existing legacy data files.

4. **`PARSE_CMD` is dropped.** ADR-006 retired its only consumer.

5. **`TIMER` is dropped from the product surface.** Timing and progress reporting are host observability concerns. If needed later, they are adapter or cross-cutting infrastructure, not ported Fortran procedures.

6. **`COMMON_VARS` splits.** The *classification* of a condition as fatal or non-fatal stays in the domain as a typed domain failure (ADR-002 §4). The message text, ANSI styling, output channel, and process exit status are adapter concerns.

7. **ADR-003's codec exclusion now generalizes.** "Text/locale/complex-column codecs are out of scope" was scoped to the BEH-001 slice. With IOTOOLS reclassified, it holds library-wide, and the IOTOOLS carve-out recorded in `docs/PURPOSE.md` is withdrawn.

8. **Cross-cutting contract dispositions.** Of the recovered contracts:

   | Contract | Disposition |
   |----------|-------------|
   | `BEH-301` numeric kind / representation | **Domain.** binary64 mapping at the port (ADR-003). |
   | `BEH-302` array layout and bounds | **Domain.** Index base, shape, and non-default lower bounds are part of the call contract, and matter for MATRIX/FFT. |
   | `BEH-305` failure classification | **Domain.** Which conditions fail, as typed failures. |
   | `BEH-303` text codecs | **Adapter.** No fidelity requirement. |
   | `BEH-304` external complex-column order | **Adapter.** In-memory complex component pairing remains domain under `BEH-301`. |
   | `BEH-305` channel, styling, exit codes | **Adapter.** No fidelity requirement. |

9. **Adapter-concern defect rows are retired with evidence.** Eight rows are closed for this effort, with their findings preserved:

   | Rows | Ground for retirement |
   |------|-----------------------|
   | `DEF-004` (`es24.17` versus list-directed), `DEF-304` (`ffcmplx` `sread` call-site anomaly), `DEF-305` (IOTOOLS `(Re,Im)`/`(Im,Re)` overload split), `DEF-307` (`sreadM_*` unallocated-`imY` and duplicate-`imY(2)` anomalies) | Fidelity of legacy file readers and writers; no legacy format is reproduced |
   | `DEF-306` (`txtfy` `(re,im)` versus file writers), `DEF-309` (bare `STOP` versus `stop 1`), `DEF-310` (diagnostics sharing stdout with data), `DEF-311` (`r8_to_s_left` `G14.6` comment versus `g16.9` code) | Diagnostic text, channel, formatting, and exit status are adapter output |

   `DEF-307` in particular documents apparent *legacy defects*. Not reproducing them is the intended outcome, and this ADR is the disposition that says so rather than leaving them silently unfixed.

   Only `DEF-002` (the `linspace` negative-`num` error path, a VS-1 concern) and `DEF-308` (comparison policy) remain open.

10. **Verification tooling is exempt.** Fixtures are captured from legacy text output, so the oracle harness still parses Fortran-formatted text. That parsing lives in the verification harness, never in the product. Parity is asserted on parsed numeric values at the managed port (ADR-003), not on bytes.

---

## Consequences

**Positive**

- The contradiction between ADR-002 §2 and ADR-005 §3 is resolved in favour of the architecture the owner actually asked for.
- The IOTOOLS carve-out disappears from `PURPOSE.md`, so a single rule now covers every surface: numeric values are the contract, representations are not.
- Eight more defect rows close. With ADR-006's six, fourteen of seventeen are retired; `DEF-001` is already decided, leaving only `DEF-002` and `DEF-308` open.
- The driven port is the seam that makes the client-facing goal work: the same use case serves a library caller now and an HTTP adapter later, with no domain change.

**Negative / costs**

- The port cannot consume or produce legacy `splot`/`sread` files. If interoperability with existing Fortran-produced data is ever needed, a compatibility adapter must be specified and `DEF-304`/`DEF-305`/`DEF-307` reopened.
- `IOTOOLS` no longer maps one-to-one onto a C# module, so `BEH-100` becomes a port-and-adapter design question rather than a translation task. `SL-015` changes shape accordingly.
- Recovered evidence in `BEH-303`/`BEH-304` and flow `BEH-304-fftgf-complex-column-io` now documents behavior the product will not reproduce. It is retained as characterization, and as the reason those contradictions were never silently "fixed".

---

## Alternatives considered

| Alternative | Why not chosen |
|-------------|----------------|
| Relax ADR-002 §2 and keep I/O in the domain | Breaks the hexagonal intent; the same use case could not later be exposed as a service without dragging file formatting with it |
| Keep IOTOOLS as domain but require legacy byte fidelity | Owner does not need legacy file interop; would resurrect the codec and complex-column contradictions as blocking work |
| Drop file I/O from scope entirely | The API still needs a way to hand results to a caller; a driven port is that, and it is the seam the services goal depends on |
| Leave the contradiction and decide per slice | It already produced one bad carve-out; deferring guarantees more |

---

## Explicit non-decisions

- This ADR does not define port names, method signatures, DTO shapes, or serialization formats. Those are design and story work, consistent with ADR-002.
- This ADR does not decide whether an HTTP adapter is built.
- This ADR does not change the numeric contracts of any computational module.

---

## Links

- Resolves: ADR-002 §2 versus ADR-005 §3
- Supersedes in part: ADR-005 §3 rows for `IOTOOLS`, `PARSE_CMD`, `COMMON_VARS`, `TIMER`
- Generalizes: ADR-003 (codec exclusion, previously BEH-001-scoped)
- Related: ADR-006, ADR-008
- Contracts: `docs/modernization/behaviors/BEH-301-*.md` … `BEH-305-*.md`
- Defect ledger: `docs/modernization/defect-ledger.md`
