# Code Review: NSwag — Newtonsoft.Json to System.Text.Json Migration

## Scope

Branch `feature/migrate-core-to-stj` — 146 files, ~3800 insertions / ~3200 deletions across 7 commits.

---

## What Was Done Well

- Clean separation: `NSwag.Generation.NewtonsoftJson` as opt-in package follows ASP.NET Core `AddNewtonsoftJson()` pattern
- `EnumMemberStringEnumConverter<T>` correctly handles `[EnumMember]` attributes with AOT-compatible generic approach
- `OpenApiParameterJsonConverter` correctly resolves the `required` property collision (bool vs string[])
- `OpenApiPathItemConverter` rewrite correctly handles dictionary-like serialization with HTTP methods as keys
- New snapshot tests cover minimal/full/security/round-trip scenarios
- `NSwagDocumentBase.GetSerializerOptions()` correctly returns static readonly instance
- Conditional `UseLocalNJsonSchemaProjects` for development with sibling NJsonSchema repo
- `NSwagServiceCollectionExtensions` uses reflection for optional Newtonsoft detection
- All Newtonsoft `using` statements removed from core projects
- `TransformLegacyDocument` correctly uses `.DeepClone()` for JsonNode parent ownership
- `[JsonObjectCreationHandling(Populate)]` correctly replaces Newtonsoft's auto-population on `OpenApiDocument.Components`
- `[JsonConstructor]` on `OpenApiComponents` for STJ deserialization
- `Lazy<SchemaSerializationConverter>` caching in `OpenApiDocument.Serialization.cs`
- Property casing (`"DocumentGenerator"` → `"documentGenerator"`) is NOT a breaking change — Newtonsoft's `CamelCasePropertyNamesContractResolver` already produced camelCase

---

## Critical Issues

### 1. Recursive reverse rename destroys `deprecated` on `OpenApiOperation`
**Root cause:** NJsonSchema's `ApplyReverseRenamesRecursively` in `SchemaSerializationConverter`.

The renames registered for `JsonSchema` include `x-deprecated` → `deprecated` (OpenApi3). During deserialization, the reverse rename `deprecated` → `x-deprecated` is applied to the **entire** JSON tree — including `OpenApiOperation` objects where `deprecated` is a native OpenAPI property mapped to `[JsonPropertyName("deprecated")]`.

**Impact:** `OpenApiOperation.IsDeprecated` is never populated. All `[System.Obsolete]` and `@deprecated` annotations are dropped from generated code. Same issue affects `example` properties on non-schema objects.

**Fix required in NJsonSchema.** See NJsonSchema review.md.

### 2. `CodeGeneratorCollection` PascalCase breaks .nswag backward compatibility
**File:** `NSwag.Commands/CodeGeneratorCollection.cs:12,17,22`

```csharp
[JsonPropertyName("OpenApiToTypeScriptClient")]  // PascalCase
```

On master, Newtonsoft's `CamelCasePropertyNamesContractResolver` overrode `[JsonProperty("OpenApiToTypeScriptClient")]` to produce `"openApiToTypeScriptClient"` (camelCase). In STJ, `[JsonPropertyName]` takes precedence over `PropertyNamingPolicy` — so the output is now PascalCase.

Existing `.nswag` files use camelCase keys (confirmed in `sample.nswag`). They will fail to load.

**Fix:** Change to `[JsonPropertyName("openApiToTypeScriptClient")]`, etc.

### 3. `UseLocalNJsonSchemaProjects` defaults to `true`
**File:** `Directory.Build.props:40`

Every build attempts to resolve local NJsonSchema projects at `../NJsonSchema/`. Fails for CI, other developers, and NuGet packing.

**Fix:** Default to `false`.

---

## Major Issues

### 4. `NSwag.Generation.NewtonsoftJson` not in solution file
**File:** `NSwag.sln`

The new project is not listed in the solution. Won't appear in IDE, can't be independently built from solution.

### 5. Breaking public API: `ToJson(SchemaType, Formatting)` → `ToJson(SchemaType, bool)`
**File:** `NSwag.Core/OpenApiDocument.cs:151`

Parameter type changed from `Newtonsoft.Json.Formatting` enum to `bool writeIndented`. Compile-breaking for consumers. Intentional but must be documented.

### 6. Breaking public API: `GetJsonSerializerContractResolver` → `GetSchemaSerializationConverter`
Method renamed with different return type. Compile-breaking for consumers.

### 7. `JsonExceptionFilterAttribute` behavioral change
**File:** `NSwag.AspNetCore/JsonExceptionFilterAttribute.cs:129-160`

- Lost polymorphic exception serialization (no longer uses `JsonExceptionConverter`)
- `_searchedNamespaces` constructor parameter is now dead code
- `JsonValue.Create()` at line 150 only supports primitive types — complex exception properties will fail (caught by try-catch but silently dropped)

### 8. `OpenApiPathItem.ExtensionData` type changed
**File:** `NSwag.Core/OpenApiPathItem.cs:71`

Changed from `IDictionary<string, object>` to `IDictionary<string, JsonNode>`. Breaking for consumers accessing extension data.

### 9. No `PropertyNameCaseInsensitive` for .nswag deserialization
**File:** `NSwag.Commands/NSwagDocumentBase.cs:287-295`

Newtonsoft's `CamelCasePropertyNamesContractResolver` was case-insensitive during deserialization. STJ's `CamelCase` naming policy only affects serialization. Manually edited `.nswag` files with mixed casing may fail.

**Fix:** Add `PropertyNameCaseInsensitive = true` to serializer options.

---

## Minor Issues

- `NSwag.CodeGeneration.Tests` still references `NJsonSchema.NewtonsoftJson` but doesn't use it
- `hasSchemaAnnotations` check in `OpenApiDocumentGenerator.cs:128` uses default serializer options (fragile empty-schema detection)
- No STJ integration tests for AspNetCore generation (only Newtonsoft tests exist in `NSwag.Generation.AspNetCore.Tests`)
- Removed `UseRequiredKeywordNewtonsoftJsonSchemaGeneratorTests` without STJ replacement
- Blank line removal in generated controller code (cosmetic output change)
- `NSwagStudio` still uses `JsonConvert` in 3 files — relies on transitive Newtonsoft
- Several files use fully-qualified `System.Text.Json.JsonSerializer.Serialize(...)` instead of adding `using`
- `OpenApiOperation.ParametersRaw` removed `IsWriting` guard — changes serialization behavior for body parameters (likely an improvement but undocumented)
- `ConvertJsonNodeToExpandoObject` falls back to `null` for unhandled node types (could silently drop data)

---

## Resolved / Non-Issues

| Finding | Status | Notes |
|---------|--------|-------|
| `OpenApiParameterJsonConverter` omits `required: false` | **CORRECT** | Matches OpenAPI spec default |
| NSwag.AspNet.WebApi still references NJsonSchema.NewtonsoftJson | **INTENTIONAL** | Legacy net462 project |
| Test files still using NewtonsoftJsonSchemaGeneratorSettings | **CORRECT** | Tests backward-compat path via NSwag.Generation.NewtonsoftJson |
| `JsonStringEnumConverter` not AOT-compatible | **ACCEPTABLE** | NSwag CLI isn't AOT-compiled |

---

## Known Open Issues (pre-existing, not from migration)

### `PathItemTests.PathItem_With_External_Ref_Can_Be_Serialized`
External file reference resolution produces empty `OpenApiPathItem` because raw extension data isn't converted to typed objects. The reference resolver's `ResolveDocumentReference` needs to handle converting raw extension data to typed NSwag objects.

### `HttpLoadingTests.When_openapi_is_loaded_without_scopes_it_should_deserialize`
Remote SaaS API spec deserialization fails, triggering `FixLenientJson` fallback which corrupts apostrophes in strings. Fix direction: identify root deserialization failure or make `FixLenientJson` safe with a proper tokenizer.

### `NSwag.ConsoleCore.Tests` (6 tests)
Pre-existing `FileNotFoundException: openapi.json` — console tool integration tests require NSwag CLI to be built and generate sample specs. Not caused by migration.

---

## Already Fixed During Review

| # | Item | Fix |
|---|------|-----|
| 1 | NSwag.AspNetCore build error — hard Newtonsoft import | Replaced with reflection-based `TryCreateNewtonsoftJsonSchemaGeneratorSettings()` |
| 2 | NSwagDocumentBase null references (lines 181, 518) | Added `obj != null &&` guards and `?.` on `GetValue<int>()` |
| 3 | JsonExceptionFilterAttribute safety | Added `MaxInnerExceptionDepth = 10`, wrapped property serialization in try-catch |
| 4 | `JsonSerializerOptions` allocation per call | Cached as `static readonly` field |
| 5 | YAML boolean/number type loss | Added `.WithAttemptingUnquotedStringTypeDeserialization()` in both repos |
| 6 | YAML null paths deserialization | Added `RemoveNullCollectionProperties()` in NJsonSchema converter |
| 7 | Namotion.Reflection crash on indexer properties | Added `GetIndexParameters().Length == 0` guards (in NJsonSchema) |
| 8 | YAML test Newtonsoft dependency | Migrated `YamlDocumentTests.cs` from `JObject` to `JsonNode` |
| 9 | AspNetCore.Tests missing project reference | Added `ProjectReference` to `NSwag.Generation.NewtonsoftJson` |
| 10 | QueryParametersTests JToken assertion | Changed to `Assert.Equal("42", ...?.ToString())` |
| 11 | Namotion.Reflection version | Bumped to 3.5.0 in both repos |

---

## Priority Action Items

### Must Fix Before Merge
1. `CodeGeneratorCollection` — change `[JsonPropertyName]` values to camelCase
2. `UseLocalNJsonSchemaProjects` — default to `false` in `Directory.Build.props`
3. Recursive reverse rename fix (in NJsonSchema, but blocks NSwag correctness)

### Should Fix Before Release
4. Add `NSwag.Generation.NewtonsoftJson` to solution file
5. Add `PropertyNameCaseInsensitive = true` to NSwagDocumentBase options
6. Document all public API breaking changes in release notes

### Nice to Have
7. Remove unused `NJsonSchema.NewtonsoftJson` reference from `NSwag.CodeGeneration.Tests`
8. Add STJ integration tests for AspNetCore generation
9. Clean up fully-qualified `System.Text.Json` references

---

*Last updated: 2026-04-07*
*Branch: `feature/migrate-core-to-stj`*
*Tests: 854 passed, 0 failed (last full run)*
