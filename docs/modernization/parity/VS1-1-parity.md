PARITY SUMMARY: result=Block PAR-critical=1 PAR-high=0 PROV-critical=0 PROV-high=0 mismatches=1 blocked=1 oracleTier=T1

<!-- Parity report contract:
- First non-comment line must be PARITY SUMMARY.
- One row per Phase P criterion and covered BEH-NNN.
- Do not claim parity above the configured oracle tier.
-->

# Parity Report: VS1-1 — Inclusive linspace at the managed port

**Date:** `2026-08-20`
**Oracle tier:** `T1 executable`, scoped to `BEH-001` at probe revision `e586903a26cc50ca8942f20ca3bccbd8814e6252` by ADR-001. The profile-wide `modernization.oracleTier` remains `T3 documented-only`, and this report claims nothing outside the scoped `T1` boundary.
**Result:** `Block`
**Mode:** `targeted` — Phase P evidence only
**Story-ID resolution:** invoked as `/verify-parity VS-1`; `VS-1` is a migration-plan slice ID, and the configured story glob resolves to the slice's one story, `VS1-1`.

> **`P1` itself passes.** The single Phase P criterion matches its oracle exactly, with no
> tolerance applied. The overall result is `Block` for a different reason: `DEF-005`, a mismatch on
> the covered behavior `BEH-001`, has no owner disposition. The configured result semantics list a
> missing defect decision as a blocking condition, and the command's hard rules forbid treating a
> user-visible mismatch as acceptable without one. `Z8` is therefore not passable today, and
> becomes passable on the disposition alone — no code change is required for `reproduce-faithfully`.

---

## 1. Scope

| Story criterion | Behavior | Oracle source | New-system command / test |
|-----------------|----------|---------------|---------------------------|
| P1 | `BEH-001` inclusive `linspace` | `FIX-001` / `CAP-20260810-LINSPACE` at `e586903`, scoped `T1` (ADR-001, ADR-003) | `tests/SciFor.Tests/Parity/Fix001LinspaceParityTests.cs::parity_BEH_001_P1` via `dotnet test SciFor.sln --filter FullyQualifiedName~parity_BEH_001_P1` |

`BEH-001` is the only covered behavior, so there is exactly one Phase P criterion. `REQ-001`
`S2`–`S6` are covered in Phase A with no `P` criterion, which is correct: no fixture exists for
them, and giving them one would let Phase P evidence exceed the oracle tier. Those rows appear in
the matrix below as scope statements, not as parity claims.

## 2. Results matrix

| BEH | AC | Fixture / data | Tolerance | Legacy result | New result | Result | Defect decision |
|-----|----|----------------|-----------|---------------|------------|--------|-----------------|
| BEH-001 | P1 | `FIX-001` (`docs/modernization/fixtures/FIX-001-linspace-5.md`), transcribed to `tests/SciFor.Tests/Parity/Fixtures/FIX-001-linspace-5.expected.txt` | exact parsed binary64 equality; no relative or absolute tolerance; no text comparison | `0, 0.25, 0.5, 0.75, 1` (parsed, index origin 1 in the legacy call) | `0, 0.25, 0.5, 0.75, 1` (zero-based, via `Grids.Linspace(0.0, 1.0, 5)`) | **match** | `DEF-001` reproduce-faithfully |
| BEH-001 | A2, A2a (no `P` criterion) | none — endpoint behavior at lengths where the formula misses `stop` | n/a | **unknown** — `E5`; no capture exists at any affected length | `0.9999999999999999` as the last sample of `Linspace(0.0, 1.0, 50)` | **blocked** | `DEF-005` **undecided** |
| BEH-001 | A2 (no `P` criterion) | none — general inclusive formula, `E3` code-derived from `src/tools_grids.f90:11-14` | n/a | not captured beyond `FIX-001` | formula reproduced without fixup | not claimed | none |
| BEH-001 | A3, A4 (no `P` criterion) | none — decreasing interval and equal endpoints, `E4` inferred, never executed by the probe | n/a | not captured | formula reproduced | not claimed | none; decision recorded in `REQ-001` Q6 |
| BEH-001 | A5, A6, A7 (no `P` criterion) | none — abort classification, `E3` code-derived, never executed | n/a | not captured | typed domain failure, two distinguishable reasons | not claimed | `DEF-002` fix-now |

## 3. Mismatches

| ID | BEH | Description | Severity | Reconciled? | Required action |
|----|-----|-------------|----------|-------------|-----------------|
| PAR-1 | `BEH-001` | The recovered formula does not reach `stop` at every length, and the port reproduces it without a fixup. `Linspace(0, 1, 50)` returns `0.9999999999999999` as its last sample, while owner-accepted `REQ-001` `S2` asserts "the last sample is T". The ledger's enumeration puts the divergence at 2504 of the lengths in `2..20000` over `[0,1]`. | critical | **no** — `DEF-005` exists as a row but its Decision column reads "undecided — owner input needed" | Owner records `reproduce-faithfully`, `fix-now`, or `fix-later` for `DEF-005`, then `REQ-001` `S2` is amended to match. `reproduce-faithfully` ratifies the shipped behavior and needs no code change; `fix-now` adds the endpoint fixup and requires `P1` to be re-run. |

`PAR-1` does not affect `P1`. `FIX-001` uses `num=5`, whose step `0.25` is a dyadic rational, so
the formula lands on `1.0` exactly — confirmed by execution for this report, not assumed.

## 4. Provenance gaps

| ID | BEH / artifact | Gap | Severity | Required decision |
|----|----------------|-----|----------|-------------------|
| PROV-1 | `FIX-001` / `CAP-20260810-LINSPACE` | The probe retained no capture bytes ("Harness or fixture creation: None", `oracle.md` §1), so the recorded section SHA-256 `dabd07f9…2c35c` cannot be recomputed from either repository. The parity chain therefore terminates at a transcription. The transcription is faithful — the five value lines in the test fixture are byte-identical to `FIX-001` — but that is the deepest verifiable step. | low | None required for VS-1. For VS-2 and VS-3, retain capture bytes alongside parsed values so the chain stays checkable end to end. |
| — | `BEH-001` endpoint behavior at affected lengths | `E5 unknown` on the legacy side: no capture exists at any length where the formula misses `stop`, so nothing shows what legacy Fortran actually produced there. This is the evidence half of `PAR-1` and is not counted separately in the summary line. | (counted in `PAR-1`) | Recovering one legacy capture at an affected length — for example `linspace(0, 1, 50)` under the ADR-001 probe recipe — would likely settle `DEF-005` outright rather than merely inform it. |
| — | Global oracle tier | `modernization.oracleTier` in `.cursor/workflow.config.yml` still reads `T3 documented-only`, while ADR-001 scopes `T1` for `BEH-001`. Tracked as an open question in `oracle.md` §8. Disclosed in `VS1-1` §8b, so this report states the scoped tier explicitly rather than inheriting the profile value. | low | Reconcile the profile value or keep citing ADR-001 per behavior. |

## 5. Commands and evidence

| Command / inspection | Result | Evidence excerpt |
|----------------------|--------|------------------|
| `dotnet test SciFor.sln --filter FullyQualifiedName~parity_BEH_001_P1` | pass | `Passed!  - Failed: 0, Passed: 1, Skipped: 0, Total: 1` |
| `dotnet test SciFor.sln` | pass | `Passed!  - Failed: 0, Passed: 34, Skipped: 0, Total: 34` |
| `dotnet build SciFor.sln` | pass | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| `diff` of the five value lines in `FIX-001-linspace-5.md` against `tests/SciFor.Tests/Parity/Fixtures/FIX-001-linspace-5.expected.txt` | pass | no differences; both are `0.00000000000000000e+00` / `2.50000000000000000e-01` / `5.00000000000000000e-01` / `7.50000000000000000e-01` / `1.00000000000000000e+00` |
| Direct execution of `Grids.Linspace(0.0, 1.0, 5)` against the built assemblies, values printed round-trip (`R`) | pass | `fix001: 0, 0.25, 0.5, 0.75, 1` |
| Direct execution of `Grids.Linspace(0.0, 1.0, 50)` | mismatch recorded | `len50-last: 0.9999999999999999  equals-stop: False` |
| Fixture provenance header contains the capture ID and probe revision (asserted by `no_legacy_text_or_golden_oracle_Y4`) | pass | header names `CAP-20260810-LINSPACE` and `e586903a26cc50ca8942f20ca3bccbd8814e6252` |
| Golden-file oracle not read by any test | pass | `Y4` scans the test tree for `fidelity/golden` and `linspace-5.txt` and finds neither outside its own guard definition |

Toolchain: .NET SDK 8.0.424 on linux-x64, installed for this run; the environment had no SDK. All
rows above were executed for this report rather than copied from the story.

**Not re-verified:** `VS1-1` §8b records the red-first transition for `P1` — a first failing run of
`dotnet build` with `error CS0246: The type or namespace name 'Grids' could not be found`. That is a
historical state of the working tree and cannot be reproduced now that the adapter exists. It is
accepted here as story-asserted (`E2 documented`), not as re-verified evidence.

## 6. Notes

- **What `P1` does and does not establish.** It establishes that the managed port reproduces the
  accepted parsed values for `linspace(0, 1, 5)` under exact binary64 equality, at the product
  surface `Grids.Linspace` rather than at the use case. It does not establish anything about other
  lengths, other intervals, the optional legacy flags, or the abort paths, because no fixture covers
  them. The story is explicit about this and the code comments reinforce it.
- **The exact-equality rule is narrow on purpose.** ADR-003 authorizes exact parsed equality for
  `FIX-001` only. The profile's `1e-6` and the fidelity script's `1e-10` are both unused here, and
  `DEF-308` — the unresolved three-way comparison-policy tension — stays open for VS-2 and VS-3
  without binding this criterion.
- **Four dyadic values are a weak proof of a formula.** `0, 0.25, 0.5, 0.75, 1` are exactly
  representable, so `P1` would pass for several wrong implementations. The story mitigates this in
  Phase A with non-dyadic triples in `A2`, though `A2` derives its expectations from the same
  expression the implementation uses; that is filed as `TEST-4` in
  `docs/features/VS1-1-review.md`.
- **UAT implication.** A user-acceptance claim for VS-1 can say that the managed `linspace`
  reproduces the accepted legacy fixture exactly. It cannot yet say that the port's endpoint
  behavior is the accepted behavior, because nobody has decided what the accepted behavior is. That
  is one owner decision, not a body of work.
- **Residual risk if `DEF-005` is dispositioned `reproduce-faithfully`.** The port would ship a
  documented one-ULP endpoint divergence from `REQ-001` `S2` as written, so `S2` must be amended in
  the same pass or the requirement stays internally inconsistent. `A2a` already pins the behavior in
  both directions, so a later silent fixup would fail a test.
- **Residual risk if `DEF-005` is dispositioned `fix-now`.** Adding an endpoint fixup improves
  precision relative to the recovered code. `docs/PURPOSE.md` names silent precision improvement as
  a trail hole, so a `fix-now` decision needs the ADR-003 amendment to say so out loud, and `P1`
  must be re-run to confirm `num=5` is unaffected.

---

*Created: 2026-08-20 | Command: `/verify-parity` (invoked as `/verify-parity VS-1`, resolved to `VS1-1`) | QA role, targeted mode | Oracle: `FIX-001` scoped `T1` per ADR-001*
