# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NSwag is an OpenAPI/Swagger toolchain for .NET and TypeScript: it generates OpenAPI documents from ASP.NET / Web API controllers, generates C# and TypeScript client/controller code from OpenAPI documents, serves Swagger UI / ReDoc at runtime, and ships a Windows GUI (NSwagStudio), an MSBuild target, a CLI (including an npm wrapper), and a Chocolatey-distributed installer. It's built on top of [NJsonSchema](https://github.com/RicoSuter/NJsonSchema) (sibling repo), which provides the JSON Schema object model and code generation primitives.

## Cross-check with NJsonSchema

NSwag consumes NJsonSchema for the JSON Schema object model, `$ref` resolution, reflection-based schema generation, and C#/TypeScript type generation. A sibling clone of NJsonSchema is expected at `../NJsonSchema`.

**Direction of change.** Most NSwag work stays within NSwag. When an NSwag task reveals an NJsonSchema limitation or needs a new capability, don't paper over it — open an NJsonSchema PR first. The reverse direction (NJsonSchema changes affecting NSwag) is governed by NJsonSchema's CLAUDE.md → "Cross-check with NSwag" section.

**When an NSwag change does need NJsonSchema awareness:**

1. **Extending NJsonSchema types.** NSwag subclasses `JsonSchema` (e.g. `OpenApiParameter : JsonSchema` overrides `ActualSchema`), relies on virtual members via overrides, and uses `InternalsVisibleTo` grants in a few places. Read `../NJsonSchema/docs/references.md` for reference-resolution mechanics before adding new overrides or relying on specific `ActualSchema` / `ActualTypeSchema` behavior.
2. **Hitting an NJsonSchema API limitation.** Open an issue or PR on NJsonSchema. Don't patch NSwag to work around missing or broken NJsonSchema behavior — the workaround becomes load-bearing and blocks the eventual fix.
3. **Spec keyword parity.** If NSwag starts emitting or reading an OpenAPI keyword that has a JSON Schema counterpart, verify NJsonSchema supports the same keyword across the same draft versions. Mismatches here are usually platform-level issues worth fixing on NJsonSchema.
4. **Bumping the NJsonSchema dependency.** While v15 builds against sibling project refs (`UseLocalNJsonSchemaProjects=true`), no version bump is needed day-to-day; just commit on both sides. When v15 switches to NuGet for release, update the version in `Directory.Packages.props`.

### Supported spec targets

NSwag supports, both reading and writing:

- **Swagger 2.0** (OpenAPI 2.0) — JSON Schema Draft 4 subset.
- **OpenAPI 3.0.x** — Draft 5 subset; nullability expressed via `nullable: true`.
- **OpenAPI 3.1** — aligned with JSON Schema 2020-12 (native `const`, arbitrary `$ref` siblings, type-array nullability). Full 3.1 support depends on NJsonSchema v12 and lands on the `v15` branch.

`master` / v14.x is primarily 3.0-oriented; `v15` completes 3.1 support. Code that reads or emits an OpenAPI document should behave sensibly across all three formats — the `OpenApiDocument` abstraction paves over most differences, but spec-specific keywords (e.g. `nullable`, `const`) still need branches on `SchemaType`.

## Build and test

The build is driven by [NUKE](https://nuke.build/) via `build.cmd` / `build.sh` / `build.ps1` (all three bootstrap the same `build/_build.csproj`). The SDK version in `global.json` (currently `10.0.100`, `rollForward: latestMinor`) is required — the bootstrapper will download a local copy if the system SDK is missing.

Common targets (run from repo root):

```
./build.cmd Compile                # Restore + build the solution
./build.cmd Test                   # Compile + run all *.Tests projects
./build.cmd Pack                   # Build NuGet packages, MSI, NSwag.zip / NSwag.Npm.zip (Windows only)
./build.cmd Publish                # Push packages (CI only — needs *_API_KEY env vars)
```

Working directly with `dotnet` is fine for day-to-day iteration:

```
dotnet build src/NSwag.sln                                     # Windows full solution (includes WiX installer)
dotnet build src/NSwag.NoInstaller.slnf                        # Linux/macOS filtered solution (no WiX)
dotnet test src/NSwag.CodeGeneration.CSharp.Tests/NSwag.CodeGeneration.CSharp.Tests.csproj
dotnet test src/NSwag.CodeGeneration.CSharp.Tests/... --filter "FullyQualifiedName~ClientGenerationTests"
```

The NUKE `Compile` target also publishes `NSwag.Console{,.x86}` (net462) and `NSwag.ConsoleCore` (net8.0/9.0/10.0) and copies the binaries into `src/NSwag.Npm/bin/binaries` and (on Windows) the NSwagStudio output. Skipping NUKE and running `dotnet build` alone will not lay out those binaries — use the NUKE target when testing the CLI/NPM/Studio distributions.

## High-level architecture

NSwag is a layered toolchain for OpenAPI/Swagger → .NET & TypeScript. Projects under `src/` are organized by layer:

- **Specification** — `NSwag.Core`, `NSwag.Core.Yaml`, `NSwag.Annotations`: the `OpenApiDocument` object model, (de)serialization, attributes used to decorate controllers.
- **Generation (spec from code)** — `NSwag.Generation`, `NSwag.Generation.WebApi`, `NSwag.Generation.AspNetCore`: walk ASP.NET (Core) / Web API controllers to produce an `OpenApiDocument`.
- **CodeGeneration (code from spec)** — `NSwag.CodeGeneration` (shared base), `NSwag.CodeGeneration.CSharp`, `NSwag.CodeGeneration.TypeScript`: read `OpenApiDocument`, emit C# clients/controllers or TypeScript clients. Templates are `*.liquid` files embedded as resources under each project's `Templates/` folder.
- **Hosting middleware** — `NSwag.AspNetCore`, `NSwag.AspNet.Owin`, `NSwag.AspNet.WebApi`: serve the generated spec + Swagger UI / ReDoc at runtime.
- **Frontends** — `NSwag.Commands` (command object model), `NSwag.ConsoleCore` / `NSwag.Console{,.x86}` (CLIs), `NSwag.MSBuild` (MSBuild target), `NSwag.ApiDescription.Client` (`ServiceProjectReference` SDK), `NSwag.AssemblyLoader` (isolated-AppDomain loader for the CLI), `NSwag.Npm` (npm package wrapping the CLI), `NSwagStudio*` (WPF GUI + Chocolatey + WiX installer).

NSwag depends heavily on **NJsonSchema** (sibling repo [`RicoSuter/NJsonSchema`](https://github.com/RicoSuter/NJsonSchema)) for the JSON Schema object model and for C#/TypeScript type generation. Most code gen works by composing an NJsonSchema `*TypeResolver` + `*Generator` with NSwag's client/controller templates on top.

Multi-targeting: specification/generation projects typically target `netstandard2.0;net462;net8.0` (plus net9/net10 where relevant); hosting packages target `net462` or `netstandard` plus the modern `net8.0`/`net9.0`/`net10.0` ASP.NET Core targets — see `Directory.Packages.props` for the per-TFM package version table (central package management is enabled).

Tests use **xUnit v3** + **Verify.XunitV3** for snapshot assertions — expected output lives under `Snapshots/*.verified.txt` next to each test project. When generator output intentionally changes, a `.received.txt` file will appear; review, rename to `.verified.txt`, and commit. `NSwag.CodeGeneration.CSharp.Tests` also shells out to a C# compiler (`CSharpCompiler.cs`) to confirm generated code actually builds.

## v15 branch conventions (current branch: `docs/v15-setup` / PRs target `v15`)

- **NJsonSchema is consumed via local project references**, not NuGet, while v15 is in development. This is controlled by `<UseLocalNJsonSchemaProjects>true</UseLocalNJsonSchemaProjects>` in `Directory.Build.props`. Every `*.csproj` that uses NJsonSchema has paired `ItemGroup Condition=...` blocks — leave both arms in sync when adding a new NJsonSchema reference.
- **Expected local layout**: `../NJsonSchema` (on the `v12` branch) as a sibling to this checkout. CI clones it explicitly; locally you must check it out yourself.
- **Feature PRs target `v15`**, not `master`. `master` is the v14.x stable line. See `docs/plan_v15.md` for the full branch model, release plan, and pre-release cleanup checklist.
- **User-visible changes must update `docs/changelog_v15.md`** (add under `Unreleased` → Breaking / New / Fixes, plus a Migration guide section if it breaks v14 consumers).
- **CI workflow YAMLs are auto-generated by NUKE** (`build/Build.CI.GitHubActions.cs`). The sibling-NJsonSchema `git clone` step in `.github/workflows/{build,pr}.yml` is a **hand edit** on the v15 branch — regenerating via `nuke --generate-configuration ...` will delete it. Don't regenerate without re-adding the clone step (or wait until v15 flips to NuGet-based NJsonSchema).

## Repo-wide settings worth knowing

- **`TreatWarningsAsErrors=true`** for all projects (`Directory.Build.props`). NuGet audit warnings `NU1901-NU1904` are exempt. New warnings from analyzers (`AnalysisLevel=latest-Recommended`, `EnforceCodeStyleInBuild=true`) will break the build — fix them rather than suppressing locally.
- **Assemblies are strong-named** with `NSwag.snk`. `InternalsVisibleTo` entries in `.csproj` files include the full public key — copy an existing entry when adding a new internals-visible consumer.
- **`<UseArtifactsOutput>true</UseArtifactsOutput>`** — all build outputs land under the repo-root `artifacts/` directory (not per-project `bin/`), which is what the NUKE build and the packaging logic assume.
- **`<ImplicitUsings>enable</ImplicitUsings>`** and `<LangVersion>latest</LangVersion>` everywhere.

## Code style

- C# latest language version (`LangVersion=latest`) with implicit usings enabled.
- **Warnings as errors** with analyzer level `latest-Recommended` and `EnforceCodeStyleInBuild=true`. New analyzer warnings will break the build — fix them rather than adding local suppressions (the `NoWarn` lists in `Directory.Build.props` are the established exceptions).
- **No abbreviations in variable / field / parameter names** (e.g. `attribute` not `attr`, `property` not `prop`, `parameter` not `param`).
- **4-space indentation, CRLF line endings** (except `.verified.txt` snapshot files, which are LF).
- Tests follow the **AAA pattern** (`// Arrange`, `// Act`, `// Assert` comments) matching existing test style.

## Git Rules

- Never include "Claude", "Co-Authored-By", or AI attribution in commit messages, PR descriptions, or GitHub comments.
