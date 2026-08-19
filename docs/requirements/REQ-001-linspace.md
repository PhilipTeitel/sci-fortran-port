<!-- Requirements contract:
- Preserve headings and order from the requirements template.
- Do not invent answers. Trace every goal, non-goal, persona, constraint, and scenario to source material or a resolved question.
- Do not produce design (no architecture, stack table, C# signatures, namespaces, or project layout).
- Scenario IDs are S1, S2, … and later port-story Test Plans must map each in-scope Sn.
- Status starts at Draft; the owner marks Ready for Design after review.
- If a section has no content yet, write `None yet.`.
-->

# REQ-001: Inclusive linear sequence (linspace)

**Source material:**
- `docs/modernization/behaviors/BEH-001-linspace.md` — recovered library `linspace` behavior
- `docs/modernization/flows/BEH-001-linspace.md` — library evaluation flow
- `docs/modernization/fixtures/FIX-001-linspace-5.md` — accepted parsed fixture
- `docs/modernization/oracle.md` — CAP-20260810-LINSPACE
- `docs/PURPOSE.md`, `docs/DOMAIN.md`
- `docs/modernization/migration-plan.md` (SL-001)
- `docs/modernization/defect-ledger.md` (DEF-001, DEF-002)
- ADRs: `docs/decisions/ADR-001-first-slice-oracle-baseline.md`, `docs/decisions/ADR-002-hexagonal-managed-api.md`, `docs/decisions/ADR-003-linspace-numeric-contract.md`, `docs/decisions/ADR-004-whole-library-csharp-port.md`, `docs/decisions/ADR-005-planning-gate-scope.md`

**Date:** `2026-08-19`
**Status:** Draft

---

## 1. Goals

- Give a C# caller a host-neutral way to request an inclusive linear sequence and receive the same numerical job the legacy `TOOLS.linspace` library function performed: evenly spaced binary64 samples from start through stop. `E2 documented` — `docs/PURPOSE.md`; ADR-002; ADR-004; BEH-001 §1.
- Make the first walking-skeleton proof the accepted FIX-001 case: `linspace(start=0, stop=1, length=5)` with default inclusive endpoints returns exactly `0, 0.25, 0.5, 0.75, 1`. `E1 verified / E2 documented` — FIX-001; ADR-001; ADR-003.
- Keep that arithmetic in a hexagonal port so later CLI or HTTP adapters call the same use case and do not redefine spacing. `E2 documented` — ADR-002; ADR-005.
- Reject invalid lengths as typed domain failures instead of Fortran `error`/`STOP`, without claiming CLI exit codes or HTTP Problem Details in this slice. `E2 documented / E3 code-derived` — ADR-002; BEH-001 §6; GAP-026.

## 2. Non-goals

- The `linspace` CLI (`numutils/src/linspace.f90`, BEH-201 / SL-017), including help text, `wmin`/`wmax`/`L` defaults, `RANGE` parsing, program-unit name `linsp`, list-directed stdout, and process exit codes. `E2 documented / E3 code-derived` — ADR-002; BEH-001 §2, §11; migration plan SL-017.
- HTTP / ASP.NET as a driving adapter for this slice. `E2 documented` — ADR-002; ADR-005.
- Fortran `.mod` / `libscifor.a` ABI compatibility, and any uninventoried downstream Fortran callers of `linspace`. `E2 documented / E5 unknown` — ADR-005; BEH-001 §10.
- Optional legacy flags `istart`, `iend`, and `mesh` on the first managed port. Those branches are unexecuted and unaccepted for parity; they wait for a later REQ with fixtures. `E3 code-derived / E5 unknown / E2 documented` — BEH-001 §3–§6; ADR-003.
- Byte-level Fortran formatted output (`es24.17` or list-directed text), locale codecs, or complex-column contracts. `E2 documented` — ADR-003; DEF-004.
- Treating `fidelity/golden/linspace-5.txt` as E1 authority. `E2 documented` — ADR-001; DEF-001.
- Profile relative/absolute `1e-6` or script absolute `1e-10` as the FIX-001 pass/fail rule. `E2 documented` — ADR-003.
- Claiming numeric parity for any linspace input other than FIX-001. The default-inclusive formula is the evaluation rule; additional exact-equality fixtures are required before further parity claims. `E2 documented` — ADR-003.
- Signed-zero, NaN, Infinity, and overflow behavior. Those paths were not executed. `E2 documented` — ADR-003 explicit non-decisions.
- Other SciFortran modules, CLI adapters, or SL-002+ slices. `E2 documented` — ADR-004; migration plan.

## 3. Personas / actors

- **Target managed-API caller** — A C# consumer of the host-neutral linspace port. Supplies start, stop, and length; receives an ordered sequence or a typed domain failure. Does not go through Fortran stdout. `E2 documented` — ADR-002; `docs/DOMAIN.md` §2; BEH-001 §2.

- **Numerical-library maintainer** — Owns the POC port. Needs the first slice to prove recovered behavior behind hexagonal ports so later families can follow the same strangler loop. `E2 documented` — `docs/PURPOSE.md`.

Fortran library callers (`use TOOLS` / fidelity driver) and CLI users are legacy/recovery actors. They are not first-slice product personas. `E2 documented` — ADR-002; ADR-005.

## 4. User scenarios (Gherkin)

### S1 — Inclusive five-point unit interval

Accepted first-slice parity fixture. Exact parsed equality; no text compare.

```gherkin
Given a linspace request with start 0, stop 1, and length 5
And   default inclusive endpoints
When  the host-neutral linspace port is invoked
Then  the result has length 5
And   the values equal 0, 0.25, 0.5, 0.75, and 1 exactly
```

Evidence: `E1 verified` — FIX-001; `docs/modernization/oracle.md` CAP-20260810-LINSPACE; ADR-001; ADR-003. Covers BEH-001 default-inclusive happy path.

### S2 — Default inclusive evaluation follows the recovered step formula

Specifies the evaluation rule for the default-inclusive path when length is at least 2. This is the contract for generating samples, including cases such as a decreasing interval or `start == stop`. It is **not** an additional exact-equality parity fixture.

```gherkin
Given a linspace request with start S, stop T, and integer length N greater than or equal to 2
And   default inclusive endpoints
When  the host-neutral linspace port is invoked
Then  the result has length N
And   the 1-based sample i equals S + (i-1) * (T-S)/(N-1) in binary64
And   the first sample is S and the last sample is T
```

Evidence: `E3 code-derived` formula at `src/tools_grids.f90:11-14`; `E2 documented` — ADR-003. `E1` confirms the formula on FIX-001 only. `E4 inferred` that decreasing and equal-endpoint cases follow the same formula; those specific inputs were not executed. Parity claims beyond FIX-001 remain out of this REQ.

### S3 — Negative length is a typed domain failure

```gherkin
Given a linspace request whose length is less than 0
When  the host-neutral linspace port is invoked
Then  the call fails as a typed domain failure
And   no linear sequence is returned
```

Evidence: `E3 code-derived` — `src/tools_grids.f90:7`; `src/COMVARS.f90:189-199` (`error("linspace: N<0, abort.")` then `STOP`). Mapping: `E2 documented` — ADR-002 (typed domain failure at the port; host exit codes / HTTP Problem Details are adapter concerns). Exact Fortran message text and process termination are **not** first-slice parity; the path was not executed. DEF-002’s Fortran `array(num)` sizing-before-check hazard is not a managed-API contract (Fortran ABI is not retained, ADR-005).

### S4 — Inclusive endpoints reject length less than 2

```gherkin
Given a linspace request with default inclusive endpoints
And   length 0 or length 1
When  the host-neutral linspace port is invoked
Then  the call fails as a typed domain failure
And   no linear sequence is returned
```

Evidence: `E3 code-derived` — `src/tools_grids.f90:12` (`error("linspace: N<2 with both start and end points")`). Mapping: ADR-002. Unexecuted. Callers of the managed port must be able to distinguish this failure class from S3 (negative length) without requiring byte-identical Fortran diagnostic text.

### S5 — Successful evaluation has no I/O, network, or RNG side effects

```gherkin
Given a valid default-inclusive linspace request with length at least 2
When  the host-neutral linspace port is invoked
Then  the port returns the sequence and does not write files, open a network connection, or consume a RNG
And   the port does not terminate the host process
```

Evidence: `E3 code-derived` — `src/tools_grids.f90:1-30` (no file/network/RNG); abort is only on invalid length. Process `STOP` is replaced by typed failure (ADR-002), so success must not abort.

## 5. Constraints

- Host-neutral hexagonal port: domain/use-case code must not depend on ASP.NET, a CLI parser, Fortran formatted I/O, or filesystem/shell adapters. `E2 documented` — ADR-002.
- First driving adapter is the managed C# API. CLI and HTTP, if added later, call this use case. `E2 documented` — ADR-002; ADR-005.
- Map legacy `real(8)` samples to IEEE-754 binary64 (`double`). `E2 documented` — ADR-003.
- FIX-001 comparison is exact parsed numeric equality. Do not use profile `1e-6` or script `1e-10`. `E2 documented` — ADR-003; DEF-001.
- Probe baseline for this slice: revision `e586903a26cc50ca8942f20ca3bccbd8814e6252` and the recorded 2026-08-10 GNU Fortran 16.1.0 / OpenBLAS 0.3.34 / NR environment. `E1 verified / E2 documented` — ADR-001; ADR-005.
- Defect policy: reproduce-then-refactor. Do not silently “fix” unexecuted branches. `E2 documented` — PURPOSE; `.cursor/workflow.config.yml` `defectPolicy`; BEH-001 §6.
- DEF-001: reproduce the probe parsed values, not the Python-generated golden file. `E2 documented` — defect ledger.
- Fortran `error`/`STOP` maps to a typed domain failure at the port. Concrete exception or result type names, namespaces, and package IDs are **not** this REQ (ADR-002 explicit non-decisions). `E2 documented`.
- No Intel-confidential headers or Numerical Recipes source in the target tree (not required for this slice’s arithmetic). `E2 documented` — ADR-004; ADR-005.
- Structure fidelity for this slice is preserve-then-refactor: implement the recovered inclusive formula first. `E2 documented` — migration plan; workflow profile.

## 6. Resolved questions

| # | Question | Resolution | Source |
|---|----------|------------|--------|
| 1 | Is `linspace` the first retained behavior? | Yes. It is the first **code** slice (SL-001), not the product boundary. | owner 2026-08-19; ADR-004; BEH-001 §10 |
| 2 | Is the 2026-08-10 probe accepted for this behavior? | Yes, as the POC/oracle baseline. T1 execution still covers only the fidelity corpus. | ADR-001; ADR-005 |
| 3 | Managed API vs CLI vs HTTP for the first driving adapter? | Managed API. CLI is a later adapter (BEH-201 / SL-017). HTTP is optional later. | ADR-002; ADR-005 |
| 4 | Should optional `istart` / `iend` / `mesh` be on the first managed port? | **Deferred.** Unexecuted (`E5`); ADR-003 leaves them unaccepted for parity; PURPOSE forbids silently porting unexecuted branches. First-slice request is start, stop, and length with default inclusive endpoints. | BEH-001 §3, §10; ADR-003; PURPOSE; this refine 2026-08-19 |
| 5 | What typed exception/result type and message stability are required? | Invalid length is a typed domain failure and returns no sequence (ADR-002). Concrete C# type names are design, not this REQ. Exact Fortran `N<0` / `N<2` stdout strings are not first-slice parity (unexecuted). S3 and S4 must remain distinguishable failure classes. | ADR-002; BEH-001 §6; this refine 2026-08-19 |
| 6 | Must `start == stop` and decreasing intervals be specified before implementation, or only FIX-001? | The default-inclusive formula in S2 is the evaluation rule whenever length ≥ 2 (ADR-003). FIX-001 remains the only accepted exact-equality parity fixture. Do not invent extra goldens; do not claim parity for unexecuted numeric cases. | ADR-003; BEH-001 §6; this refine 2026-08-19 |
| 7 | Are downstream Fortran `linspace` callers in supported scope besides the fidelity driver? | No. Fortran ABI is not retained. Unknown `libscifor.a` consumers are not required for this POC. | ADR-005; BEH-001 §10 |
| 8 | How does DEF-002 (`array(num)` before `num<0`) affect the managed port? | The observable abort is a typed domain failure with no sequence (S3). Reproducing Fortran allocation of a negative-length result array is out of product (ABI not retained). The defect-ledger **label** (`reproduce-faithfully` vs `fix-now`) remains an M3 human decision and does not change S3. | ADR-002; ADR-005; DEF-002; this refine 2026-08-19 |

## 7. Open questions

None that block this REQ’s default-inclusive managed-port scope.

Still unspecified **outside this REQ** (do not implement as if decided):

- Signed-zero, NaN, Infinity, and overflow (ADR-003 non-decision).
- Optional endpoint flags and `mesh` (deferred; see resolved question 4).
- CLI text, locale, and exit mapping (BEH-201 / DEF-003 / DEF-004).
- DEF-002’s ledger classification of the Fortran sizing-before-check hazard (does not change S3).

## 8. Suggested ADR triggers

| Trigger | Why it likely needs an ADR | Related Sn |
|---------|----------------------------|------------|
| Domain-failure vocabulary | ADR-002 maps `STOP` to a typed domain failure but leaves exception vs result type, type names, and message/code stability to design. GAP-026. | S3, S4 |
| Non-dyadic linspace spacings | ADR-003 authorizes exact equality only for FIX-001. Later fixtures may need a tolerance ADR. | S2 |
| Optional endpoint flags | If a later slice exposes `includeStart` / `includeStop` / step-out, that is a new contract plus fixtures, not a silent addition to this REQ. | deferred from S1–S2 |
| DEF-002 ledger label | Human M3 should record `reproduce-faithfully`, `fix-now`, or `fix-later` for the Fortran allocation-before-check hazard. | S3 |

Do **not** create those ADRs in this command. `/design-application` or `/plan-port-story` owns them.

## 9. Links

- Source material: see header
- Related REQ files: none yet
- Related ADRs: ADR-001, ADR-002, ADR-003, ADR-004, ADR-005
- Behavior: `docs/modernization/behaviors/BEH-001-linspace.md`
- Flow: `docs/modernization/flows/BEH-001-linspace.md`
- Fixture: `docs/modernization/fixtures/FIX-001-linspace-5.md`
- Defects: `docs/modernization/defect-ledger.md`
- Plan: `docs/modernization/migration-plan.md` (SL-001)
- Purpose / domain: `docs/PURPOSE.md`, `docs/DOMAIN.md`

---

*Created: 2026-08-19 | Refined by: architect in Discovery Mode | Command: `/refine-feature` | Input: `docs/modernization/behaviors/BEH-001-linspace.md`*
