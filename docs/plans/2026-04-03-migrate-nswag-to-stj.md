# Migrate NSwag Core to System.Text.Json with Local NJsonSchema Project References

## Goal

Remove `Newtonsoft.Json` from all NSwag projects except `NSwag.Generation.NewtonsoftJson` (new) and `NSwag.AspNet.WebApi` (legacy). The core NSwag libraries use only `System.Text.Json`. A global MSBuild switch allows toggling between local NJsonSchema project references (development) and NuGet packages (production).

This is a **major breaking version** aligned with NJsonSchema v11's STJ migration.

## Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Local/NuGet switch | `UseLocalNJsonSchemaProjects` property in `Directory.Build.props` | Same pattern as Namotion.Interceptor. Global property, per-csproj conditional ItemGroups. Local refs are for development/testing only. |
| Newtonsoft isolation | New `NSwag.Generation.NewtonsoftJson` project | Opt-in package for users whose types use Newtonsoft attributes. Mirrors ASP.NET Core's `AddNewtonsoftJson()` pattern. |
| NSwag.AspNet.WebApi | Keeps Newtonsoft | Old ASP.NET is inherently Newtonsoft-based. References `NSwag.Generation.NewtonsoftJson`. |
| ASP.NET Core Newtonsoft opt-in | Via `NSwag.Generation.NewtonsoftJson` package + service registration | Users who call `AddNewtonsoftJson()` on MVC also add this package. |
| OpenAPI model serialization | STJ attributes directly on NSwag.Core types | Same approach as NJsonSchema core migration. Clean break. |
| .nswag config files | Migrate to STJ serialization with backwards-compatible reading | Must round-trip existing .nswag files correctly. |

## Project Structure After Migration

| Project | Newtonsoft? | NJsonSchema Reference |
|---|---|---|
| NSwag.Core | No | `NJsonSchema` |
| NSwag.Core.Yaml | No | `NJsonSchema.Yaml` |
| NSwag.Annotations | No | None |
| NSwag.CodeGeneration | No | `NJsonSchema.CodeGeneration` |
| NSwag.CodeGeneration.CSharp | No | `NJsonSchema.CodeGeneration.CSharp` |
| NSwag.CodeGeneration.TypeScript | No | `NJsonSchema.CodeGeneration.TypeScript` |
| NSwag.Generation | No | `NJsonSchema` (via NSwag.Core) |
| NSwag.Generation.AspNetCore | No | None (via NSwag.Generation) |
| NSwag.Generation.WebApi | No | None (via NSwag.Generation) |
| NSwag.AspNetCore | No | None (via NSwag.Core) |
| NSwag.Commands | No | None (via other projects) |
| NSwag.ConsoleCore | No | None (via NSwag.Commands) |
| NSwag.Generation.NewtonsoftJson *(new)* | **Yes** | `NJsonSchema.NewtonsoftJson` |
| NSwag.AspNet.WebApi | **Yes** | Via `NSwag.Generation.NewtonsoftJson` |

## Local/NuGet Reference Switch

### Directory.Build.props (NSwag root)

```xml
<PropertyGroup>
  <!-- Set to true to use local NJsonSchema project references (development), false for NuGet packages (production) -->
  <UseLocalNJsonSchemaProjects>false</UseLocalNJsonSchemaProjects>
</PropertyGroup>
```

### Per-csproj conditional blocks

Each project that references NJsonSchema gets conditional ItemGroups:

```xml
<!-- Local project references for development -->
<ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' == 'true'">
  <ProjectReference Include="..\..\..\NJsonSchema\src\NJsonSchema\NJsonSchema.csproj" />
</ItemGroup>
<!-- NuGet package references for production -->
<ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' != 'true'">
  <PackageReference Include="NJsonSchema" Version="11.x.x" />
</ItemGroup>
```

### Reference mapping

| NSwag project | NuGet package | Local project path (relative from src/) |
|---|---|---|
| NSwag.Core | `NJsonSchema` | `../../../NJsonSchema/src/NJsonSchema/NJsonSchema.csproj` |
| NSwag.Core.Yaml | `NJsonSchema.Yaml` | `../../../NJsonSchema/src/NJsonSchema.Yaml/NJsonSchema.Yaml.csproj` |
| NSwag.CodeGeneration | `NJsonSchema.CodeGeneration` | `../../../NJsonSchema/src/NJsonSchema.CodeGeneration/NJsonSchema.CodeGeneration.csproj` |
| NSwag.CodeGeneration.CSharp | `NJsonSchema.CodeGeneration.CSharp` | `../../../NJsonSchema/src/NJsonSchema.CodeGeneration.CSharp/NJsonSchema.CodeGeneration.CSharp.csproj` |
| NSwag.CodeGeneration.TypeScript | `NJsonSchema.CodeGeneration.TypeScript` | `../../../NJsonSchema/src/NJsonSchema.CodeGeneration.TypeScript/NJsonSchema.CodeGeneration.TypeScript.csproj` |
| NSwag.Generation.NewtonsoftJson | `NJsonSchema.NewtonsoftJson` | `../../../NJsonSchema/src/NJsonSchema.NewtonsoftJson/NJsonSchema.NewtonsoftJson.csproj` |

Assumes `NJsonSchema` and `NSwag` repos are sibling directories.

## Breaking Changes

- OpenAPI model types use STJ attributes instead of Newtonsoft attributes
- Extension data dictionaries become `IDictionary<string, JsonNode?>` instead of `IDictionary<string, JToken?>`
- `NSwag.Generation` no longer transitively pulls in Newtonsoft.Json
- Users relying on Newtonsoft-aware schema generation must add `NSwag.Generation.NewtonsoftJson`
- `.nswag` config files are serialized with STJ (reading existing files must remain backwards-compatible)
- Custom OpenAPI converters rewritten for STJ

## Migration Details

### NSwag.Core (28 files, bulk of work)

**Attribute swaps (mechanical):**
- `[JsonProperty("name")]` → `[JsonPropertyName("name")]`
- `[JsonIgnore]` (Newtonsoft) → `[JsonIgnore]` (STJ)
- `[JsonConverter(typeof(StringEnumConverter))]` → `[JsonConverter(typeof(JsonStringEnumConverter<T>))]`
- `[JsonExtensionData]` — same name, backing type: `IDictionary<string, JToken?>` → `IDictionary<string, JsonNode?>`
- `DefaultValueHandling.Ignore` → `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]`

**Custom converter rewrite:**
- `OpenApiPathItemConverter` (in `OpenApiPathItem.cs`) — full custom Read/Write with `$ref` handling, extension data, HTTP operation mapping. Rewrite as STJ `JsonConverter<OpenApiPathItem>`.

**Enum converters:**
- `OpenApiParameterStyle`, `OpenApiParameterKind`, `OpenApiParameterCollectionFormat`, `OpenApiOAuth2Flow`, `OpenApiSecurityScheme.In` — all use `[JsonConverter(typeof(StringEnumConverter))]` → `[JsonConverter(typeof(JsonStringEnumConverter<T>))]`

**Serialization infrastructure:**
- Uses NJsonSchema's `JsonSchemaSerialization` which has already been migrated to STJ
- Migrate from `CreateJsonSerializerContractResolver()` to `ConfigureJsonSerializerOptions()`

### NSwag.Commands

- `NSwagDocumentBase.cs`: `JObject.FromObject()` → `JsonSerializer.SerializeToNode()`, `JToken` → `JsonNode`, `JsonConvert` → `JsonSerializer`
- Other command files: lighter mechanical swaps

### NSwag.Core.Yaml

- Has its own direct Newtonsoft usage (not via NJsonSchema.Yaml): `JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter)`
- Replace with STJ-based JSON→dynamic conversion for YamlDotNet compatibility

### NSwag.AspNetCore

- `SwaggerUiSettings.cs`: `JsonConvert.SerializeObject()` → `JsonSerializer.Serialize()`
- `JsonExceptionFilterAttribute.cs`: same swap

### NSwag.Generation.NewtonsoftJson (new project)

- Newtonsoft-aware schema generation subclass/extension
- Service registration extensions (e.g. `UseNewtonsoftJson()` on swagger options)
- References: `NSwag.Generation` + `NJsonSchema.NewtonsoftJson`

## Risk Areas

### OpenAPI document serialization fidelity
Switching from Newtonsoft to STJ can subtly change JSON output (property ordering, null handling, number formatting). Mitigate with snapshot tests for representative OpenAPI documents before and after migration.

### .nswag config file backwards compatibility
Existing `.nswag` files serialized with Newtonsoft must still deserialize correctly with STJ. Mitigate with dedicated round-trip tests.

### NSwagStudio (net462 WPF app)
Transitively depends on everything via NSwag.Commands. STJ NuGet package should work on net462 (NJsonSchema already does), but needs build verification.

### OpenApiPathItemConverter complexity
Moderate-complexity converter with $ref handling and extension data. Needs careful porting and test coverage.

## Implementation Phases

### Phase 0 — Setup & test hardening
- Add `UseLocalNJsonSchemaProjects` switch to `Directory.Build.props`
- Add conditional ItemGroups to all 6 NJsonSchema-referencing .csproj files
- Verify build works with local references (switch = true)
- Add snapshot tests for OpenAPI document serialization round-trips
- Add .nswag config file round-trip tests

### Phase 1 — NSwag.Core attribute migration
- Replace all Newtonsoft attributes with STJ equivalents across 28 files
- Change extension data types from `JToken` to `JsonNode`
- Migrate `ConfigureJsonSerializerOptions()` usage

### Phase 2 — OpenApiPathItemConverter rewrite
- Port to STJ `JsonConverter<OpenApiPathItem>`
- Port enum StringEnumConverter usages to `JsonStringEnumConverter<T>`

### Phase 3 — NSwag.Commands migration
- Migrate `NSwagDocumentBase.cs` from JObject/JsonConvert to JsonNode/JsonSerializer
- Migrate other command files

### Phase 4 — NSwag.Core.Yaml migration
- Replace direct Newtonsoft ExpandoObject conversion with STJ equivalent

### Phase 5 — NSwag.AspNetCore cleanup
- Migrate SwaggerUiSettings and JsonExceptionFilterAttribute to STJ

### Phase 6 — Create NSwag.Generation.NewtonsoftJson
- New project with Newtonsoft-aware schema generation
- Service registration extensions
- Move NSwag.AspNet.WebApi to reference this

### Phase 7 — Remove Newtonsoft dependencies
- Remove `Newtonsoft.Json` PackageReferences from all projects except NSwag.Generation.NewtonsoftJson and NSwag.AspNet.WebApi
- Remove NJsonSchema.NewtonsoftJson references from NSwag.Generation
- Verify clean build on all TFMs

### Phase 8 — Test updates & verification
- Update all test projects
- Verify snapshot tests pass
- Verify .nswag config file compatibility
- Verify NSwagStudio builds and runs
- Full test pass
