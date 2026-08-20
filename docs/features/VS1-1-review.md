<!--
Per-story review contract:
- Produced by /review-story. Focused audit of the changed surface, not a full-repo audit.
- First non-comment line is the configured machine-checkable summary line.
- Parity (`PAR-#`) and Provenance (`PROV-#`) subsections are present because this is a port
  story; the configured profile lists them in `review.perStoryCategories`.
-->

REVIEW SUMMARY: result=Block TEST-critical=0 TEST-high=1 SEC-critical=0 SEC-high=0 REL-critical=0 REL-high=0 API-critical=0 API-high=0 MODEL-critical=0 MODEL-high=0 PAR-critical=1 PAR-high=0 PROV-critical=0 PROV-high=0

# Story Review: VS1-1 — Inclusive linspace at the managed port

**Reviewed against:** `docs/features/VS1-1-managed-linspace-port.md`
**Date:** 2026-08-20
**Mode:** `/review-story`
**Gate result:** `Block`

Two findings carry a blocking severity: `TEST-1` (`high`) and `PAR-1` (`critical`). Everything
else is `medium` or `low` and does not gate. The model-fidelity gate (`Z7`) has zero `high` or
`critical` `MODEL-#` findings and is satisfied on its own terms; `Z6` is not, because its wording
also counts `TEST-#` and `PAR-#`.

**Story-ID resolution.** The command was invoked as `/review-story VS-1`. `VS-1` is a
migration-plan slice ID, and the configured story glob `docs/features/{STORY-ID}-*.md` matches
nothing under that name. The slice has exactly one story, `VS1-1`, so the target resolved
unambiguously and the review proceeded against it. Reviewing is non-destructive, which is why
this resolution was made rather than refused (contrast `INT-013`, where writing a new story
under a slice ID would have corrupted the backlog).

---

## Scope

- Story ID: `VS1-1`
- Purpose artifact: `docs/PURPOSE.md` — present (`Status: Accepted`)
- Domain artifact: `docs/DOMAIN.md` — present (`Status: Draft` — see `MODEL-1`)
- Domain terms/entities in scope:
  - `Linear sequence`, `Start / Stop / Length`, `Domain failure` — from `docs/DOMAIN.md` §2a
  - `LinearSequenceRequest`, `LinearSequence` — from `docs/DOMAIN.md` §4a, §5a
  - `Linear sequence evaluation (VS-1)` consistency boundary — from `docs/DOMAIN.md` §7a
  - `LinearSequenceProduced`, `LinearSequenceRejected` — from `docs/DOMAIN.md` §9a
  - `NumericValue` / `NumericKind` (kind-8 → binary64) — from `docs/DOMAIN.md` §2b, §11b
  - `ComparisonPolicy` — from `docs/DOMAIN.md` §2b, §7b
- Linked refined requirements (Sn IDs in scope): `docs/requirements/REQ-001-linspace.md` — `S1`, `S2`, `S3`, `S4`, `S5`, `S6`
- Files in scope (Section 7 intersected with `git diff 2518a07..cc33244`, the squash-merge that implemented the story):
  - `SciFor.sln` — created
  - `src/SciFor.Domain/SciFor.Domain.csproj` — created
  - `src/SciFor.Domain/DomainFailureException.cs` — created
  - `src/SciFor.Domain/Grids/LinearSequenceRequest.cs` — created
  - `src/SciFor.Domain/Grids/LinearSequence.cs` — created
  - `src/SciFor.Domain/Grids/LinearSequenceRejection.cs` — created
  - `src/SciFor.Domain/Grids/LinearSequenceRejectedException.cs` — created
  - `src/SciFor.Application/SciFor.Application.csproj` — created
  - `src/SciFor.Application/Grids/IGenerateLinearSequence.cs` — created
  - `src/SciFor.Application/Grids/GenerateLinearSequence.cs` — created
  - `src/SciFor.Managed/SciFor.Managed.csproj` — created
  - `src/SciFor.Managed/Grids.cs` — created
  - `tests/SciFor.Tests/SciFor.Tests.csproj` — created
  - `tests/SciFor.Tests/Unit/LinearSequenceTests.cs` — created
  - `tests/SciFor.Tests/Contract/GenerateLinearSequenceContractTests.cs` — created
  - `tests/SciFor.Tests/Integration/GridsLinspaceTests.cs` — created
  - `tests/SciFor.Tests/Parity/Fix001LinspaceParityTests.cs` — created
  - `tests/SciFor.Tests/Parity/Fixtures/FIX-001-linspace-5.expected.txt` — created
  - `.gitignore` — created (Section 7 "created but not planned")
  - `tests/SciFor.Tests/Parity/Fix001Fixture.cs` — created (Section 7 "created but not planned")
  - `tests/SciFor.Tests/Integration/RepositoryLayout.cs` — created (Section 7 "created but not planned")
  - `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs` — created (Section 7 "created but not planned")
  - `README.md` — modified
  - `docs/modernization/behavior-catalog.md` — modified
  - `docs/modernization/migration-plan.md` — modified
  - `.cursor/workflow.config.yml` — modified
  - `docs/modernization/defect-ledger.md` — modified (Section 7 records this under "leave UNCHANGED" as "modified after all")
- Tests in scope (from Section 8a Test Plan): all 21 rows. Every cited file exists; the suite runs
  34 tests with 0 failures (`dotnet test SciFor.sln`, re-run 2026-08-20 on .NET SDK 8.0.424).
- Adapters in scope (from Section 4b):
  - `SciFor.Grids` (`src/SciFor.Managed/Grids.cs`) for port `SciFor.Application.Grids.IGenerateLinearSequence`

No driven (outbound) port exists on this slice, so the adapter rubric applies once, to the
driving adapter.

### Out-of-plan changes

All are documentation. None adds runtime behavior, so no `TEST-#` attaches to them under the
out-of-plan rubric. Several are owner gate actions bundled into the same squash-merge rather
than implementer drift, which is why they are listed rather than challenged.

- `docs/requirements/REQ-001-linspace.md` — Section 7 lists this under "leave UNCHANGED"; it was
  modified (`Status` → `Ready for Design`). Legitimate: this is the owner closing Gate 2 (`INT-011`).
  Recommend Section 7 distinguish "the implementer must not edit" from "the owner may re-status".
- `docs/decisions/ADR-009-vs1-managed-port-and-layout.md`, `docs/decisions/ADR-010-typed-domain-failure.md` —
  same situation: listed as "leave UNCHANGED", modified to `Accepted` by the owner (`INT-012`).
- `docs/modernization/behaviors/BEH-001-linspace.md` — Section 7 lists the behaviors directory
  under "leave UNCHANGED" as recovered evidence; its status line was edited to point forward at
  `VS1-1`. This is the one deviation that is implementer-side rather than owner-side. The edit is
  harmless (a forward pointer, no recovered fact changed), but recovered evidence acquiring
  forward references to product artifacts is the drift the "leave UNCHANGED" list exists to catch.
- `docs/PURPOSE.md`, `docs/DOMAIN.md`, `docs/decisions/ADR-002-hexagonal-managed-api.md`,
  `docs/decisions/ADR-004-whole-library-csharp-port.md`, `docs/modernization/ASSESSMENT.md`,
  `docs/modernization/intent-ledger.md` — not named in Section 7 at all. Each was refreshed to
  retire stale `Proposed`/`blocked` wording. Recommend adding them to Section 7 "Files to MODIFY"
  rather than leaving the story's touchpoint list narrower than the change it produced.

---

## Findings

### Test Coverage {`TEST-#`}

#### TEST-1. Scenario IDs `S3`–`S6` are not traceable from any test name or per-test annotation

- Severity: high
- AC / Sn / Adapter affected: `S3`, `S4`, `S5`, `S6` (criteria `A3`, `A4`, `A5`, `A6`, `A7`)
- Missing or weak test: The behavior is tested; the scenario-to-test link is not present in the
  test sources. Searching every `tests/**/*.cs` file for each scenario ID gives:
  `S1` — one hit (`Fix001LinspaceParityTests.cs:16`, on the `P1` test);
  `S2` — three hits, including `GenerateLinearSequenceContractTests.cs:46` on the `A2a` test;
  `S3`, `S4` — one shared hit at `GenerateLinearSequenceContractTests.cs:11`, a class-level remark
  about evidence grades, not attached to `Generate_DecreasingInterval_A3` or
  `Generate_EqualEndpoints_A4`;
  `S5` — **zero hits anywhere in the test tree**;
  `S6` — one hit, inside the range expression `S2-S6` at `Fix001LinspaceParityTests.cs:5`, in a
  sentence stating that those scenarios are *not* covered by that class.
- Why it matters: the configured methodology sets `gherkinScenarioTraceability: required`, and the
  auditor rubric requires each implemented `Sn` to be reachable from a test name or annotation, not
  only from the story's Section 8a table. Today the sole path from `REQ-001` `S5` to its test runs
  through a markdown table. If the table is edited, renumbered, or the story is superseded, the
  link is gone, and `REQ-001` §4 already notes that this behavior has had two numbering schemes.
  Test names in this suite reference acceptance-criterion IDs (`_A5`, `_P1`, `_Y2`) but never
  scenario IDs, so the convention is uniformly absent rather than accidentally missed once.
- Lightest-weight way to add it: append the scenario ID to the existing test names
  (`Generate_NegativeLength_Rejected_A5_S5`) or add one `// @scenario S5` comment per test method.
  No new test and no behavior change is needed.
- Verification gap: none in behavior — `S1`–`S6` are all exercised and all pass. The gap is in
  mechanical traceability, which is what this rubric check exists to protect.

#### TEST-2. Phase Z criteria `Z1`, `Z2`, `Z4`–`Z8` have no Section 8a row

- Severity: low
- AC / Sn / Adapter affected: `Z1`, `Z2`, `Z4`, `Z5`, `Z6`, `Z7`, `Z8`
- Missing or weak test: Section 8a's 21 rows cover every Phase A, P, and Y criterion plus `Z3`
  (row 21). The remaining Phase Z criteria appear only in Section 8.
- Why it matters: graded `low`, not `high`, for three reasons that were checked rather than
  assumed. First, these criteria are command- or gate-evidenced rather than test-evidenced: `Z1`
  is a build, `Z2` is a format check, `Z6`–`Z8` are the review and parity commands themselves, and
  a test row for "the review passes" would be circular. Second, the port-story template's
  Definition of Ready does not carry the plain story template's "every AC ID including Phase Z is
  referenced by a test row" item, and `VS1-1` §3 matches the port template. Third, no verification
  gap actually exists: `Z1`, `Z2`, and `Z3` were independently re-run green for this review, and
  `Z4` and `Z5` are recorded as not applicable with reasons in Section 8.
- Lightest-weight way to add it: either add rows whose `File::test name` cell names the command
  (`dotnet build SciFor.sln`), or state the exemption once in Section 8a's preamble so a future
  reader does not read the absence as an oversight.
- Verification gap: none.

#### TEST-3. `A7`'s "no samples allocated" clause has no real assertion

- Severity: low
- AC / Sn / Adapter affected: `A7` (`S5`, `S6`), and the `DEF-002` fix-now decision it encodes
- Missing or weak test: `tests/SciFor.Tests/Unit/LinearSequenceTests.cs:32` closes
  `Rejection_CarriesRequest_NoSamples_A7` with `Assert.IsType<LinearSequenceRejectedException>(exception)`.
  `exception` came from `Assert.Throws<LinearSequenceRejectedException>` on line 24, so it is
  already that type and the assertion cannot fail. The criterion's second clause — "no
  `LinearSequence` instance is constructed on the failure path" — is carried by the comment on
  lines 30–31, not by an assertion.
- Why it matters: `DEF-002` is a `fix-now` disposition and one of the few places this slice
  deliberately departs from legacy behavior, so it is worth a real guard. Graded `low` because an
  effective guard exists elsewhere by accident of construction: `Generate_NegativeLength_Rejected_A5`
  includes `int.MinValue`, and if validation ever moved after allocation, `new double[int.MinValue]`
  would throw `OverflowException` instead of `LinearSequenceRejectedException` and that test would
  fail.
- Lightest-weight way to add it: assert on the observable instead of the tautology — for example
  that `exception.Request.Length` is negative and that `Generate` never returned a value — or state
  in the criterion that `A5`'s `int.MinValue` case is the structural guard.
- Verification gap: a regression that allocated before validating would be caught by `A5`, not by
  `A7`, which is the criterion that claims to cover it.

#### TEST-4. `A2` computes its expected values with the implementation's own expression

- Severity: low
- AC / Sn / Adapter affected: `A2` (`S2`)
- Missing or weak test: `GenerateLinearSequenceContractTests.cs:33-37` derives `step` and
  `start + i * step` in the test, which is character-for-character the arithmetic in
  `GenerateLinearSequence.cs:66-71`. The interior-sample oracle is therefore the code itself.
- Why it matters: the story's own §9 risk row anticipates this ("Four exact dyadic values make S1
  look trivially passable") and mitigates it by pointing at `A2`'s non-dyadic triples — but `A2`
  cannot detect a wrong-but-self-consistent formula, only a differently-ordered one. Graded `low`
  because the test does retain real power against the substitution the code comments warn about (a
  running accumulator or fused multiply-add would produce different interior roundings and fail),
  and because `A1` and `P1` pin five independent hard-coded values.
- Lightest-weight way to add it: add two or three hand-computed literal expectations for a
  non-dyadic case, so at least one row of `A2` has an oracle that is not the implementation.
- Verification gap: interior samples at non-dyadic spacings have no independent expected value
  anywhere in the suite.

### Reliability {`REL-#`}

#### REL-1. The null-request guard in `LinearSequenceRejectedException` is unreachable

- Severity: low
- Confidence: high
- AC / Sn affected: `A7` (indirectly); no `Sn` depends on it
- Files and lines: `src/SciFor.Domain/Grids/LinearSequenceRejectedException.cs:17-23`
- Evidence checked: constructing the exception with a null request throws `NullReferenceException`,
  not `ArgumentNullException`. Confirmed by direct execution against the built assemblies:
  `null-request-throws: NullReferenceException`.
- Failure mode: a caller or future sibling type that passes a null request gets an opaque
  `NullReferenceException` from inside the base constructor chain instead of the intended
  `ArgumentNullException` naming the parameter.
- Root cause: C# evaluates the base-constructor arguments before the derived constructor body, so
  `MessageFor(reason, request)` on line 18 dereferences `request.Length` before
  `ArgumentNullException.ThrowIfNull(request)` on line 20 can run.
- Minimal safe fix: move the check ahead of the base call. Either make `MessageFor` tolerate a null
  request (formatting the length only when one exists) and keep the body guard for the field
  assignment, or add a private static helper that validates and returns the request, and call it in
  the base-argument position: `base(CodeFor(reason), MessageFor(reason, Validated(request)))`.
- Regression test idea: assert `ArgumentNullException` for a null request in
  `LinearSequenceTests`.
- Verification: the production path is unaffected — `GenerateLinearSequence.Generate` null-checks
  its request first (`GenerateLinearSequence.cs:16`) and always passes a non-null request — which
  is why this is `low` rather than higher.

#### REL-2. The port contract omits a third outcome that large lengths actually produce

- Severity: low
- Confidence: high
- AC / Sn affected: `A2` (`S2`); the documented contract of `IGenerateLinearSequence.Generate`
- Files and lines: `src/SciFor.Application/Grids/GenerateLinearSequence.cs:68`;
  contract documented at `src/SciFor.Application/Grids/IGenerateLinearSequence.cs:20-28`
- Evidence checked: `Linspace(0.0, 1.0, int.MaxValue)` throws `OutOfMemoryException`. Confirmed by
  direct execution: `maxvalue-length-throws: OutOfMemoryException`.
- Failure mode: the port documents exactly two outcomes — a sequence, or
  `LinearSequenceRejectedException`. A length that passes both length rules but cannot be allocated
  produces a third, and a caller written to the documented contract will not handle it.
- Root cause: `new double[request.Length]` allocates after validation, and validation only rejects
  `Length < 2`.
- Minimal safe fix: none required in code. Document the allocation-failure outcome on the port, or
  record explicitly that resource exhaustion is a host concern outside the domain contract, in the
  same spirit as `ADR-007`.
- Regression test idea: not recommended — a test that allocates near `int.MaxValue` is hostile to CI.
- Verification: legacy Fortran would also fail to allocate at that size, so this is a documentation
  gap rather than a parity divergence, and no fixture or scenario reaches it.

### Security {`SEC-#`}

None.

### API Contracts {`API-#`}

#### API-1. `dotnet pack` produces a `SciFor` package that cannot be consumed

- Severity: medium
- Confidence: high
- AC / Sn affected: none directly — no acceptance criterion covers packaging
- Files and lines: `src/SciFor.Managed/SciFor.Managed.csproj:13-22` (`IsPackable`, `PackageId`);
  `src/SciFor.Domain/SciFor.Domain.csproj:9` and `src/SciFor.Application/SciFor.Application.csproj:9`
  (`IsPackable=false`); advertised at `README.md` "Available Scripts"
- Evidence checked: `dotnet pack src/SciFor.Managed/SciFor.Managed.csproj` succeeds, and the
  resulting `SciFor.1.0.0.nupkg` contains exactly one assembly, `lib/net8.0/SciFor.Managed.dll`.
  Its nuspec declares `<dependency id="SciFor.Application" version="1.0.0" />` — a package that
  does not exist and, because `SciFor.Application` is `IsPackable=false`, never will.
  `SciFor.Domain.dll` is absent entirely.
- Affected behavior or contract: the distributable form of the product surface. Every type in the
  story's §5 API table other than `Grids` itself lives in the two assemblies the package omits, so
  a consumer cannot restore the package, and could not use `Linspace`'s return type if they did.
- Failure mode: `dotnet add package SciFor` fails to restore `SciFor.Application`; a manually
  installed package throws `FileNotFoundException` on first call.
- Minimal safe fix: either stop advertising the command until packaging is in scope, or set
  `IsPackable=true` on all three projects, or pack the dependency assemblies into the one package
  (`TargetsForTfmSpecificBuildOutput` / `PrivateAssets="all"` with `IncludeReferencedProjects`).
- Backward-compatibility or migration notes: none — nothing was ever published, and `B11` forbids
  publication.
- Regression test idea: a script step that packs and then restores the package into a scratch
  project, if packaging ever becomes a claim.
- Verification: graded `medium` rather than `high` precisely because `B11` places NuGet publication
  out of scope and `README.md` says "do not publish as SciFortran". The defect is that the README
  lists the command among working scripts. If the POC ever offers the package to a reader, this
  becomes `high`.

### Model Fidelity {`MODEL-#`}

#### MODEL-1. The domain model still lists as live blockers two questions this slice shipped through

- Severity: medium
- Confidence: high
- Purpose / domain section affected: `docs/DOMAIN.md` §7b (`VerificationBoundary`), §10b, §11b;
  `docs/DOMAIN.md` header `Status: Draft`
- Files and lines: `docs/DOMAIN.md:338` ("Blocks parity stories until approved"); `docs/DOMAIN.md:407`
  (kind-8 / IEEE-edge question, under "Still blocking for design/story planning in the built
  slices"); `docs/DOMAIN.md:441` ("Kind-8 declarations abundant; portable equivalence to C#
  `double`/`Complex` unproven … Blocks numeric representation claims", listed under "Live — these
  block built slices")
- Evidence checked: `VS1-1` `B3` maps legacy `real(8)` to IEEE-754 binary64 and shipped it
  (`LinearSequence.cs:26-29`), and `P1` claims parity on an exact-equality rule. Both are
  authorized by `ADR-003`, which is `Accepted`, and `REQ-001` Q10 records the edge cases as out of
  scope. `docs/DOMAIN.md` was modified in the same change (`git diff 2518a07..cc33244`) but none of
  these three rows was narrowed, and the file's status is still `Draft`.
- Fidelity break: the domain model, which the profile treats as canonical for these contracts,
  asserts that a numeric-representation claim and a parity story are blocked, while the repository
  contains both. Nothing is wrong in the code; the canonical artifact simply was not reconciled
  when the blockers were narrowed by `ADR-003` and `ADR-008`.
- Why it matters: this is the trail gap the purpose cares most about. `docs/PURPOSE.md` sets the
  trade-off rule as "completeness and honesty of the artifact trail over breadth", and a reader
  following `DOMAIN.md` today cannot tell whether `VS1-1` violated a live blocker or the blocker
  had been scoped away. It is graded `medium`, not `high`, because the authorizing decision does
  exist and is linked — `ADR-003` and `REQ-001` Q10 — so the trail is stale rather than broken.
- Minimal safe fix: run `/model-domain` to scope the two `§11b` rows and the `§10b` question to
  VS-2 and VS-3, narrow §7b's "Blocks parity stories until approved" to surfaces without an
  accepted comparison rule, and decide whether `DOMAIN.md` leaves `Draft` now that a slice has
  shipped against it.
- Regression or model update needed: domain-model update. No code change.
- Verification: after the update, `DOMAIN.md` §11b's live rows should be readable against the built
  set without contradicting `ADR-003`.

#### MODEL-2. `LinearSequence` does not enforce the value-object semantics the domain model asserts

- Severity: medium
- Confidence: high
- Purpose / domain section affected: `docs/DOMAIN.md` §5a ("value objects, no persistence"); §4a
  data dictionary row `LinearSequence.samples` ("ordered binary64 list"); `VS1-1` §1a, which
  restates the value-object claim, and §6b ("immutable value objects")
- Files and lines: `src/SciFor.Domain/Grids/LinearSequence.cs:19-29`;
  `src/SciFor.Application/Grids/GenerateLinearSequence.cs:33,64-74`
- Evidence checked: `EvaluateInclusive` returns `double[]`, and the constructor stores that
  reference as `IReadOnlyList<double>` without copying or wrapping it, so the array is reachable
  through a downcast. Confirmed by direct execution against the built assemblies:
  `mutation-before: 0, 0.25, 0.5, 0.75, 1` then `((double[])seq.Samples)[0] = 42.0` then
  `mutation-after : 42, 0.25, 0.5, 0.75, 1`, with `downcast-succeeded: True`.
- Fidelity break: a caller can mutate a returned sequence in place, which a value object must not
  permit. Separately, the type's own documentation says "Count equals the requested length"
  (`LinearSequence.cs:27`) but the public constructor accepts any list, so that relationship is
  guaranteed only by the use case, not by the type. `GridsLinspaceTests.cs:66` already constructs a
  `LinearSequence` whose contents bear no relation to any request.
- Why it matters: the sequence is the product's entire output. Graded `medium` rather than `high`
  because exploiting it requires a deliberate downcast, nothing is persisted or crosses a trust
  boundary, and the parity claim is unaffected — but "value object" is the domain model's word for
  this type, and the code does not make it true.
- Minimal safe fix: wrap the array before handing it out — `new ReadOnlyCollection<double>(samples)`
  in the constructor, or `samples.ToArray()` to break the alias. Both are one line and neither
  changes any sample value.
- Regression or model update needed: a unit test asserting that a downcast either fails or cannot
  affect `Samples`. If mutability is instead intended for performance, say so in `DOMAIN.md` §5a
  rather than leaving the two artifacts disagreeing.
- Verification: re-run the mutation probe; `Samples` must be unchanged after the attempt.

### Parity {`PAR-#`}

#### PAR-1. `DEF-005` has no disposition while the behavior it describes ships and is user-visible

- Severity: critical
- Confidence: high
- AC / Sn affected: `A2`, `A2a` (`S2`); `REQ-001` `S2`; does **not** affect `P1`
- Files and lines: `src/SciFor.Application/Grids/GenerateLinearSequence.cs:64-74` (no endpoint
  fixup); `docs/modernization/defect-ledger.md:32` (the `DEF-005` row) and `:80` (listed under
  "Open defect decisions"); `docs/requirements/REQ-001-linspace.md:77` (`S2`: "the first sample is
  S and the last sample is T"); `docs/features/VS1-1-managed-linspace-port.md:402`
- Evidence checked: the shipped port and the accepted requirement disagree at lengths where the
  recovered formula does not land on `stop`. Confirmed by direct execution:
  `len50-last: 0.9999999999999999  equals-stop: False`. `DEF-005`'s Decision column reads
  "**undecided — owner input needed**", and the ledger's own policy note states that "mismatches
  without decisions block affected Phase P parity criteria".
- Affected behavior or contract: `Grids.Linspace(0, 1, 50)` returns a final sample one ULP below
  the `stop` the owner-accepted requirement promises. The divergence affects 2504 of the lengths in
  `2..20000` over `[0,1]` by the ledger's enumeration.
- Failure mode: the auditor's parity rubric requires every mismatch to link to a `DEF-NNN`
  *decision* — one of `reproduce-faithfully`, `fix-now`, or `fix-later`. `DEF-005` is documented,
  which is good practice, but documenting a mismatch is not disposing of it, and the disposition is
  what the gate requires. Until it exists, no reviewer can say whether the shipped behavior is the
  intended outcome or an accepted defect.
- Minimal safe fix: the owner records a disposition. `reproduce-faithfully` ratifies what shipped
  and requires amending `REQ-001` `S2` so the accepted requirement stops asserting something the
  code does not do. `fix-now` requires the endpoint fixup, an `S2` amendment, and re-running `P1`.
  Nothing else in the slice moves either way.
- Backward-compatibility or migration notes: `P1` is unaffected — `FIX-001` uses `num=5`, which is
  dyadic and lands on `1.0` exactly, verified in this review.
- Regression test idea: already present. `Generate_EndpointFollowsFormulaNotFixup_A2a` asserts the
  divergence in both directions, so whichever disposition is chosen, silently changing the endpoint
  behavior later will fail a test.
- Verification: this finding also carries the provenance half of the problem, so it is not filed
  twice. The legacy side of `DEF-005` is `E5 unknown` — no capture exists at any affected length —
  and the auditor's provenance rubric would grade implemented behavior resting on unresolved `E5`
  evidence `critical` on its own. Both framings describe one defect and one remedy, so `PROV-#`
  is not incremented for it. Note that recovering a legacy capture at an affected length would
  likely collapse the decision rather than merely inform it: if the probe also misses `1.0` at
  `num=50`, `reproduce-faithfully` is the only defensible answer.

### Provenance {`PROV-#`}

#### PROV-1. `FIX-001`'s authority is a transcription whose capture bytes no longer exist

- Severity: low
- Confidence: high
- Purpose / domain section affected: `docs/modernization/oracle.md` §1, §5;
  `docs/modernization/fixtures/FIX-001-linspace-5.md` provenance table
- Files and lines: `docs/modernization/fixtures/FIX-001-linspace-5.md:6,22`;
  `docs/modernization/oracle.md:39,96`; `tests/SciFor.Tests/Parity/Fixtures/FIX-001-linspace-5.expected.txt:1-18`
- Evidence checked: the oracle doc records that the probe retained no capture ("Harness or fixture
  creation: None"), and `FIX-001` states "capture bytes were not retained". The section SHA-256
  `dabd07f9…2c35c` therefore cannot be recomputed from anything in either repository. The
  transcription itself is faithful: the five value lines in the test fixture are byte-identical to
  the five in `FIX-001`, verified by `diff` for this review.
- Fidelity break: none, and nothing is hidden — the limitation is disclosed in the fixture doc, the
  oracle doc, the test fixture's own header, and `VS1-1` §8b. It is recorded here because a parity
  gate should state the depth of its evidence: re-verification can prove the port matches the
  transcription, and cannot prove the transcription matches the probe's bytes.
- Why it matters: `FIX-001` is the only `T1` evidence in the repository, and it is one document
  away from being unverifiable. Graded `low` because the probe recorded cross-build parsed
  agreement of `0` and the values are exact dyadic rationals that the formula reproduces
  independently, so the risk of a transcription error surviving undetected is small.
- Minimal safe fix: none available retroactively. For VS-2 and VS-3, retain the capture bytes
  alongside the parsed values so the chain stays checkable end to end.
- Regression or model update needed: none.
- Verification: `diff` of the value lines in `FIX-001-linspace-5.md` against
  `FIX-001-linspace-5.expected.txt` — identical.

---

## Required actions before QA

- `PAR-1` — the owner records a `DEF-005` disposition (`reproduce-faithfully`, `fix-now`, or
  `fix-later`) in `docs/modernization/defect-ledger.md`, and `REQ-001` `S2` is amended to match
  whichever is chosen. This is the only finding that needs a decision rather than an edit.
- `TEST-1` — make `S3`, `S4`, `S5`, and `S6` reachable from the tests that implement them, either
  by appending the scenario ID to the test names or by adding one `// @scenario Sn` annotation per
  test method. No behavior change.

Re-run `/review-story VS1-1` after both. The remaining findings are `medium` or `low` and do not
gate; `API-1` and `MODEL-1`/`MODEL-2` are worth folding into the same pass since each is a
one-line or documentation-only change.

---

## Notes

- **Verification performed for this review, not taken from the story.** On .NET SDK 8.0.424
  (installed for this run; the environment had none): `dotnet build SciFor.sln` succeeded with 0
  warnings and 0 errors; `dotnet test SciFor.sln` reported 34 passed, 0 failed;
  `dotnet test --filter FullyQualifiedName~parity_BEH_001_P1` reported 1 passed;
  `dotnet format SciFor.sln --verify-no-changes` exited 0. `Z1`, `Z2`, and `Z3` therefore hold
  independently of the story's own claims.
- **The adapter rubric passes cleanly.** Section 4b's one adapter has a contract test against the
  port (`GenerateLinearSequenceContractTests`), an integration test that does not mock the boundary
  it owns (`GridsLinspaceTests::real_use_case_through_adapter_Y2`, which wires the real use case),
  and a `(binding)` Phase Y criterion citing that integration test. The one test that does
  substitute the port, `Linspace_DelegatesToThePort`, is an extra guard against the adapter
  restating the formula, and no binding criterion rests on it.
- **Phase P discipline is right, and unusually so.** `S2`–`S6` deliberately carry no `P` criterion
  because no fixture supports them. That is the rule "do not let Phase P evidence exceed the oracle
  tier" being followed at a cost, and it is the correct call.
- **The Phase Y guards are stronger than boilerplate.** `ArchitectureBoundaryTests` inspects the
  project files, the compiled assembly references, and the source text, and `Y4` scans the test
  tree for the golden-file path and the two legacy diagnostic strings. One residual weakness worth
  knowing: those are substring guards, so repointing the parity test at the golden file through a
  variable, or with backslash separators, would not trip them. Not filed as a finding — the guard
  is materially better than none and the rubric does not require guards to be unspoofable.
- `docs/DOMAIN.md` §9a lists `LinearSequenceProduced` and `LinearSequenceRejected` as domain
  events. `LinearSequenceRejected` is realized as the typed exception; `LinearSequenceProduced` has
  no code counterpart. `VS1-1` §1a discloses this and states that no requirement asks for an event
  bus, so it is not filed as a finding.
- `Fix001Fixture` parses with `NumberStyles.Float` and `CultureInfo.InvariantCulture`, which is the
  right choice for a fixture that must not depend on the host locale, and it throws rather than
  falling back to inlined literals when the fixture file is missing. Both directly serve `DEF-001`.
- Story-ID resolution and the `VS-1`-versus-`VS1-1` namespace confusion are recorded in
  `docs/modernization/intent-ledger.md` `INT-013`.

---

*Created: 2026-08-20 | Command: `/review-story` (invoked as `/review-story VS-1`, resolved to `VS1-1`) | Auditor, per-story review mode | Changed surface: `git diff 2518a07..cc33244`*
