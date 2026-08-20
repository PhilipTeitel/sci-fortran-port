# AGENTS.md

## Cursor Cloud specific instructions

SciFor is a single .NET 8 (`net8.0`) solution (`SciFor.sln`) — a hexagonal class
library (`SciFor.Domain`, `SciFor.Application`, `SciFor.Managed`) plus one xUnit test
project (`SciFor.Tests`). There is no web server, CLI, or UI in VS-1; the product
surface is the managed `SciFor.Grids.Linspace(start, stop, length)` API. See
`README.md` for the full stack and architecture.

Standard commands are already documented in `README.md` ("Available Scripts"):
`dotnet build SciFor.sln`, `dotnet test SciFor.sln`, and
`dotnet format SciFor.sln --verify-no-changes` (the `Z2` lint stand-in).

Non-obvious notes:

- The .NET 8 SDK is provided by the base environment (installed from Ubuntu's
  `dotnet-sdk-8.0` apt package). The startup update script only runs
  `dotnet restore SciFor.sln`; it does not reinstall the SDK.
- All projects set `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, so any
  compiler warning fails the build.
- `dotnet format SciFor.sln --verify-no-changes` currently exits non-zero (2) because
  of pre-existing whitespace deviations in
  `tests/SciFor.Tests/Integration/ArchitectureBoundaryTests.cs`. This is committed
  state, not an environment problem. Do not auto-reformat unrelated code just to make
  this pass; only run `dotnet format` (without `--verify-no-changes`) when a change
  intentionally targets those files.
- There is no application to "serve". To exercise the library ad hoc, reference
  `src/SciFor.Managed/SciFor.Managed.csproj` from a throwaway console project and call
  `new SciFor.Grids().Linspace(...)`; keep such scratch projects outside the repo tree.
