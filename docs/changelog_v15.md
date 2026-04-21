# NSwag v15 Changelog

Running record of changes on the `v15` branch and migration guidance for users upgrading from v14.

See [`plan_v15.md`](./plan_v15.md) for the v15 scope, branch model, and release plan.

---

## Unreleased

### Breaking changes

- **Set up v15 integration branch, project-ref mechanism, CI triggers, and sibling NJsonSchema v12 checkout** (this PR) — no user-facing impact; infrastructure only.
  - Adds `UseLocalNJsonSchemaProjects` property (`Directory.Build.props`, default `true` on v15).
  - Converts 8 NSwag csprojs that reference NJsonSchema packages to conditional `ItemGroup` blocks: `NSwag.Core`, `NSwag.Core.Yaml`, `NSwag.CodeGeneration`, `NSwag.CodeGeneration.CSharp`, `NSwag.CodeGeneration.TypeScript`, `NSwag.Generation`, `NSwag.AspNet.WebApi`, `NSwag.CodeGeneration.Tests`.

Planned (not yet merged — track via linked PRs):

- **STJ core migration** — PR [#5355](https://github.com/RicoSuter/NSwag/pull/5355). Mirrors NJsonSchema v12's STJ migration.
- **Absorb NJsonSchema v12 breaking API changes** — `ValidationError.Token` type change, `SchemaType` enum expansion (OpenAPI 3.0 vs 3.1), reference-resolution semantics, etc. Items added as upstream changes land.

### New features

*(to be filled as PRs merge)*

### Fixes

*(to be filled as PRs merge)*

---

## Migration guide (v14 → v15)

Intended as a running "how do I upgrade" companion. Each section is added as breaking changes land on the `v15` branch.

### System.Text.Json migration

*(placeholder — to be filled when PR #5355 merges)*

- What changes for NSwag consumers
- Before/after code snippets
- Newtonsoft.Json escape hatch if you need to keep the old behavior

### NJsonSchema v12 dependency

*(placeholder — to be filled closer to release)*

- NSwag v15 requires NJsonSchema v12.0.0 or later.
- Breaking changes that cascade from NJsonSchema v12 — link to [NJsonSchema's migration guide](https://github.com/RicoSuter/NJsonSchema/blob/v12/docs/changelog_v12.md#migration-guide-v11--v12).

### `SchemaType` enum expansion (OpenAPI 3.0 vs 3.1)

*(placeholder — to be filled when the upstream NJsonSchema enum change lands and NSwag absorbs it)*

---

## Contributing

When merging a v15 PR that includes a user-visible change:

1. Add an entry under `Unreleased → Breaking changes` / `New features` / `Fixes`.
2. If it breaks v14 consumers, also add a section under **Migration guide** with a before/after example.
3. If the change cascades from an NJsonSchema v12 change, link to the corresponding entry in NJsonSchema's `changelog_v12.md` migration guide.
4. Keep entries concise; link to the merged PR for full detail.
