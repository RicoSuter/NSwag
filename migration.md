# NSwag Migration Guide: Newtonsoft.Json to System.Text.Json

## Overview

Starting with this release, **NSwag** core packages use **System.Text.Json** instead of Newtonsoft.Json for all serialization, deserialization, and document handling.

If you need Newtonsoft.Json integration for schema generation (e.g., types decorated with `[JsonProperty]`), install **`NSwag.Generation.NewtonsoftJson`** and call `.UseNewtonsoftJson()`.

This migration also requires the updated **NJsonSchema** packages (which underwent the same migration). See the [NJsonSchema migration guide](https://github.com/RicoSuter/NJsonSchema/blob/feature/migrate-core-to-stj/migration.md) for NJsonSchema-specific changes.

---

## Quick Start

### Default (System.Text.Json)

No changes needed if your types use `[JsonPropertyName]` and other STJ attributes:

```csharp
services.AddOpenApiDocument(settings => {
    // Uses SystemTextJsonSchemaGeneratorSettings by default
});
```

### Opt-in Newtonsoft.Json support

Install `NSwag.Generation.NewtonsoftJson`, then:

```csharp
services.AddOpenApiDocument(settings => {
    settings.UseNewtonsoftJson();
});
```

---

## Breaking Changes

### 1. `OpenApiDocument.ToJson(SchemaType, Formatting)` → `ToJson(SchemaType, bool)`

```csharp
// Before
var json = document.ToJson(SchemaType.OpenApi3, Formatting.Indented);

// After
var json = document.ToJson(SchemaType.OpenApi3, writeIndented: true);
```

### 2. `GetJsonSerializerContractResolver` → `GetSchemaSerializationConverter`

```csharp
// Before
var resolver = OpenApiDocument.GetJsonSerializerContractResolver(schemaType);

// After
var converter = OpenApiDocument.GetSchemaSerializationConverter(schemaType);
```

Returns `SchemaSerializationConverter` (a `JsonConverterFactory`) instead of `IContractResolver`.

### 3. `AspNetCoreOpenApiDocumentGenerator.GetJsonSerializerSettings` deprecated

```csharp
// Before
JsonSerializerSettings settings = generator.GetJsonSerializerSettings();

// After — method is [Obsolete], returns object
// Use NSwag.Generation.NewtonsoftJson for Newtonsoft support
```

### 4. `OpenApiPathItem.ExtensionData` type changed

```csharp
// Before
IDictionary<string, object> extensionData = pathItem.ExtensionData;

// After
IDictionary<string, JsonNode> extensionData = pathItem.ExtensionData;
```

### 5. NSwag.Generation no longer depends on NJsonSchema.NewtonsoftJson

If you use `NewtonsoftJsonSchemaGeneratorSettings`, you must now install `NSwag.Generation.NewtonsoftJson`:

```csharp
// Before (implicit):
settings.SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { ... };

// After (explicit package required):
// Install NSwag.Generation.NewtonsoftJson
settings.UseNewtonsoftJson();
```

### 6. `JsonExceptionFilterAttribute` behavioral change

The exception serialization no longer uses `JsonExceptionConverter` for polymorphic exception handling. Custom exception properties are serialized as primitives only. The `searchedNamespaces` constructor parameter is no longer used for deserialization.

### 7. Schema generation defaults to System.Text.Json

`OpenApiDocumentGeneratorSettings.SchemaSettings` now defaults to `SystemTextJsonSchemaGeneratorSettings` instead of `NewtonsoftJsonSchemaGeneratorSettings`. Types decorated with `[JsonProperty]` will no longer have those attributes recognized unless you opt in via `UseNewtonsoftJson()`.

---

## .nswag Document Compatibility

Existing `.nswag` configuration files should continue to load. The serialization format uses `JsonNamingPolicy.CamelCase` to match the existing camelCase convention.

If you encounter loading issues with manually edited `.nswag` files, ensure property names use camelCase (e.g., `"openApiToCSharpClient"`, not `"OpenApiToCSharpClient"`).

---

## Behavioral Differences

### Extension Data Types

Extension data values may differ in type:
- Integers: `int` (if in range) instead of `long`
- Dates: remain as `string` instead of being auto-parsed to `DateTime`
- Objects: `JsonElement` or `Dictionary<string, object?>` instead of `JObject`

### Property Ordering

JSON property ordering in serialized OpenAPI documents may differ slightly. The semantic content is identical.

### Lenient JSON Parsing

Non-standard JSON (single quotes, unquoted property names, comments) is handled via:
- `AllowTrailingCommas = true`
- `ReadCommentHandling = JsonCommentHandling.Skip`
- Automatic fallback fixing for common issues

---

## NSwag.Generation.NewtonsoftJson Package

Provides opt-in Newtonsoft.Json support for NSwag document generation:

- **`UseNewtonsoftJson()`** — extension method on `OpenApiDocumentGeneratorSettings`
- **`NewtonsoftJsonSettingsResolver`** — resolves Newtonsoft `JsonSerializerSettings` from DI

### ASP.NET Core Setup

```csharp
// In Program.cs / Startup.cs:
builder.Services.AddOpenApiDocument(settings => {
    settings.UseNewtonsoftJson();
    // Optionally configure Newtonsoft settings:
    settings.UseNewtonsoftJson(serializerSettings => {
        serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    });
});
```

---

## Namespace Quick Reference

| Old (Newtonsoft) | New (System.Text.Json) |
|---|---|
| `using Newtonsoft.Json` | `using System.Text.Json` |
| `using Newtonsoft.Json.Linq` | `using System.Text.Json.Nodes` |
| `Formatting.Indented` | `true` (bool) |
| `Formatting.None` | `false` (bool) |
| `IContractResolver` | `SchemaSerializationConverter` |
| `NewtonsoftJsonSchemaGeneratorSettings` | `SystemTextJsonSchemaGeneratorSettings` (default) |

---

*Last updated: 2026-04-07*
*Branch: `feature/migrate-core-to-stj`*
