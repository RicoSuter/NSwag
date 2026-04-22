# Migrate NSwag to System.Text.Json — Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Remove Newtonsoft.Json from all NSwag projects except `NSwag.Generation.NewtonsoftJson` (new) and `NSwag.AspNet.WebApi` (legacy). Add a global `UseLocalNJsonSchemaProjects` switch to toggle between local project references and NuGet packages.

**Architecture:** NSwag.Core has 28 files with Newtonsoft attributes (`[JsonProperty]`, `[JsonIgnore]`, etc.) that get mechanically swapped to STJ equivalents. One custom converter (`OpenApiPathItemConverter`) is rewritten as STJ `JsonConverter<T>`. `NSwag.Commands` migrates from `JObject`/`JsonConvert` to `JsonNode`/`JsonSerializer`. A new `NSwag.Generation.NewtonsoftJson` project isolates all Newtonsoft schema generation for opt-in use. Code generation templates (Liquid) that generate Newtonsoft user code are NOT changed — they already support both JSON libraries.

**Tech Stack:** C#, System.Text.Json, XUnit v3, Verify (snapshot testing)

**Design doc:** `docs/plans/2026-04-03-migrate-nswag-to-stj.md`

**Prerequisites:** NJsonSchema `feature/migrate-core-to-stj` branch must be available locally at `../NJsonSchema` (sibling directory).

---

## Attribute Migration Reference

These mechanical swaps apply across all NSwag.Core files (28 files):

| Newtonsoft | System.Text.Json |
|---|---|
| `using Newtonsoft.Json;` | `using System.Text.Json.Serialization;` |
| `using Newtonsoft.Json.Converters;` | _(remove)_ |
| `[JsonProperty(PropertyName = "x")]` | `[JsonPropertyName("x")]` |
| `[JsonProperty(PropertyName = "x", Order = N)]` | `[JsonPropertyName("x")] [JsonPropertyOrder(N)]` |
| `[JsonProperty(..., DefaultValueHandling = DefaultValueHandling.Ignore)]` | Add `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]` |
| `[JsonProperty(..., DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]` | Add `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]` |
| `[JsonProperty(..., NullValueHandling = NullValueHandling.Ignore)]` | Add `[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]` |
| `[JsonProperty(..., Required = Required.Always)]` | Add `[JsonRequired]` |
| `[JsonProperty(..., ItemConverterType = typeof(StringEnumConverter))]` | Apply `[JsonConverter(typeof(JsonStringEnumConverter<T>))]` on the enum type instead |
| `[JsonIgnore]` | `[JsonIgnore]` (same name, different namespace) |
| `[JsonExtensionData]` | `[JsonExtensionData]` (same name, different namespace; backing type `JToken` → `JsonNode`) |
| `[JsonConverter(typeof(StringEnumConverter))]` | `[JsonConverter(typeof(JsonStringEnumConverter<T>))]` |
| `[JsonConverter(typeof(OpenApiPathItemConverter))]` | `[JsonConverter(typeof(OpenApiPathItemConverter))]` (rewrite converter for STJ) |

---

## Phase 0: Setup — Local Reference Switch

### Task 0.1: Add UseLocalNJsonSchemaProjects to Directory.Build.props

**Files:**
- Modify: `Directory.Build.props`

**Step 1: Add the property**

Add inside the existing `<PropertyGroup>`, after `<UseArtifactsOutput>true</UseArtifactsOutput>`:

```xml
<!-- Set to true to use local NJsonSchema project references (development), false for NuGet packages (production) -->
<UseLocalNJsonSchemaProjects>false</UseLocalNJsonSchemaProjects>
```

**Step 2: Verify build still works**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj`
Expected: SUCCESS (property exists but not used yet)

**Step 3: Commit**

```bash
git add Directory.Build.props
git commit -m "feat: add UseLocalNJsonSchemaProjects switch to Directory.Build.props"
```

---

### Task 0.2: Add conditional references to NSwag.Core.csproj

**Files:**
- Modify: `src/NSwag.Core/NSwag.Core.csproj`

**Step 1: Replace the NJsonSchema PackageReference with conditional blocks**

Find the existing `<PackageReference Include="NJsonSchema" ... />` and replace with:

```xml
<!-- Local project references for development -->
<ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' == 'true'">
  <ProjectReference Include="..\..\..\NJsonSchema\src\NJsonSchema\NJsonSchema.csproj" />
</ItemGroup>
<!-- NuGet package references for production -->
<ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' != 'true'">
  <PackageReference Include="NJsonSchema" Version="11.3.2" />
</ItemGroup>
```

Keep the existing version number from the current PackageReference.

**Step 2: Verify build with NuGet (default)**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj`
Expected: SUCCESS

**Step 3: Verify build with local references**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS (requires NJsonSchema repo at `../NJsonSchema` on the `feature/migrate-core-to-stj` branch)

---

### Task 0.3: Add conditional references to remaining 5 projects

**Files:**
- Modify: `src/NSwag.Core.Yaml/NSwag.Core.Yaml.csproj` — `NJsonSchema.Yaml`
- Modify: `src/NSwag.CodeGeneration/NSwag.CodeGeneration.csproj` — `NJsonSchema.CodeGeneration`
- Modify: `src/NSwag.CodeGeneration.CSharp/NSwag.CodeGeneration.CSharp.csproj` — `NJsonSchema.CodeGeneration.CSharp`
- Modify: `src/NSwag.CodeGeneration.TypeScript/NSwag.CodeGeneration.TypeScript.csproj` — `NJsonSchema.CodeGeneration.TypeScript`
- Modify: `src/NSwag.Generation/NSwag.Generation.csproj` — `NJsonSchema.NewtonsoftJson` (temporarily; will change in Phase 7)

**Step 1: Apply the same conditional pattern to each .csproj**

For each project, find its NJsonSchema `PackageReference` and replace with conditional blocks. The local project paths are:

| NSwag .csproj | NuGet Package | Local ProjectReference path |
|---|---|---|
| `NSwag.Core.Yaml` | `NJsonSchema.Yaml` | `..\..\..\NJsonSchema\src\NJsonSchema.Yaml\NJsonSchema.Yaml.csproj` |
| `NSwag.CodeGeneration` | `NJsonSchema.CodeGeneration` | `..\..\..\NJsonSchema\src\NJsonSchema.CodeGeneration\NJsonSchema.CodeGeneration.csproj` |
| `NSwag.CodeGeneration.CSharp` | `NJsonSchema.CodeGeneration.CSharp` | `..\..\..\NJsonSchema\src\NJsonSchema.CodeGeneration.CSharp\NJsonSchema.CodeGeneration.CSharp.csproj` |
| `NSwag.CodeGeneration.TypeScript` | `NJsonSchema.CodeGeneration.TypeScript` | `..\..\..\NJsonSchema\src\NJsonSchema.CodeGeneration.TypeScript\NJsonSchema.CodeGeneration.TypeScript.csproj` |
| `NSwag.Generation` | `NJsonSchema.NewtonsoftJson` | `..\..\..\NJsonSchema\src\NJsonSchema.NewtonsoftJson\NJsonSchema.NewtonsoftJson.csproj` |

**Step 2: Verify full solution build with local references**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true && dotnet build src/NSwag.Core.Yaml/NSwag.Core.Yaml.csproj -p:UseLocalNJsonSchemaProjects=true && dotnet build src/NSwag.CodeGeneration/NSwag.CodeGeneration.csproj -p:UseLocalNJsonSchemaProjects=true && dotnet build src/NSwag.CodeGeneration.CSharp/NSwag.CodeGeneration.CSharp.csproj -p:UseLocalNJsonSchemaProjects=true && dotnet build src/NSwag.CodeGeneration.TypeScript/NSwag.CodeGeneration.TypeScript.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.Core/NSwag.Core.csproj src/NSwag.Core.Yaml/NSwag.Core.Yaml.csproj src/NSwag.CodeGeneration/NSwag.CodeGeneration.csproj src/NSwag.CodeGeneration.CSharp/NSwag.CodeGeneration.CSharp.csproj src/NSwag.CodeGeneration.TypeScript/NSwag.CodeGeneration.TypeScript.csproj src/NSwag.Generation/NSwag.Generation.csproj
git commit -m "feat: add conditional NJsonSchema project/NuGet references to all projects"
```

---

## Phase 1: Snapshot Tests (Before Migration)

Add Verify snapshot tests for OpenAPI document serialization to lock down current behavior before changing anything.

### Task 1.1: Add serialization snapshot tests for NSwag.Core

**Files:**
- Create: `src/NSwag.Core.Tests/Serialization/OpenApiDocumentSnapshotTests.cs`

**Step 1: Write snapshot tests for representative OpenAPI documents**

These tests serialize OpenAPI documents in both Swagger2 and OpenApi3 formats and snapshot the output. Any serialization drift during migration will be caught.

```csharp
namespace NSwag.Core.Tests.Serialization;

[UsesVerify]
public class OpenApiDocumentSnapshotTests
{
    [Fact]
    public async Task Snapshot_MinimalDocument_Swagger2()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info = new OpenApiInfo { Title = "Test API", Version = "1.0.0" };

        // Act
        var json = document.ToJson(SchemaType.Swagger2, Formatting.Indented);

        // Assert
        await Verify(json);
    }

    [Fact]
    public async Task Snapshot_MinimalDocument_OpenApi3()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info = new OpenApiInfo { Title = "Test API", Version = "1.0.0" };

        // Act
        var json = document.ToJson(SchemaType.OpenApi3, Formatting.Indented);

        // Assert
        await Verify(json);
    }

    [Fact]
    public async Task Snapshot_FullDocument_OpenApi3()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info = new OpenApiInfo
        {
            Title = "Pet Store",
            Version = "1.0.0",
            Description = "A sample API",
            Contact = new OpenApiContact { Name = "Test", Email = "test@test.com" },
            License = new OpenApiLicense { Name = "MIT" }
        };
        document.Servers.Add(new OpenApiServer { Url = "https://api.example.com/v1" });

        var pathItem = new OpenApiPathItem();
        var operation = new OpenApiOperation
        {
            Summary = "List pets",
            OperationId = "listPets",
            Tags = { "pets" }
        };
        operation.Responses["200"] = new OpenApiResponse { Description = "A list of pets" };
        pathItem["get"] = operation;
        document.Paths["/pets"] = pathItem;

        var postOperation = new OpenApiOperation
        {
            Summary = "Create a pet",
            OperationId = "createPet"
        };
        postOperation.RequestBody = new OpenApiRequestBody
        {
            Description = "Pet to add",
            IsRequired = true
        };
        postOperation.Responses["201"] = new OpenApiResponse { Description = "Pet created" };
        pathItem["post"] = postOperation;

        // Act
        var json = document.ToJson(SchemaType.OpenApi3, Formatting.Indented);

        // Assert
        await Verify(json);
    }

    [Fact]
    public async Task Snapshot_DocumentWithSecurity_OpenApi3()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info = new OpenApiInfo { Title = "Secure API", Version = "1.0.0" };
        document.Components.SecuritySchemes["bearer"] = new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Bearer token"
        };

        // Act
        var json = document.ToJson(SchemaType.OpenApi3, Formatting.Indented);

        // Assert
        await Verify(json);
    }

    [Fact]
    public async Task Snapshot_RoundTrip_ComplexDocument()
    {
        // Arrange — load a real-world-ish spec
        var json = @"{
            ""openapi"": ""3.0.0"",
            ""info"": { ""title"": ""Test"", ""version"": ""1.0"" },
            ""paths"": {
                ""/items"": {
                    ""get"": {
                        ""operationId"": ""getItems"",
                        ""parameters"": [
                            { ""name"": ""limit"", ""in"": ""query"", ""schema"": { ""type"": ""integer"" } }
                        ],
                        ""responses"": {
                            ""200"": { ""description"": ""OK"" }
                        }
                    }
                }
            }
        }";

        // Act
        var document = await OpenApiDocument.FromJsonAsync(json);
        var roundTripped = document.ToJson(SchemaType.OpenApi3, Formatting.Indented);

        // Assert
        await Verify(roundTripped);
    }
}
```

**Step 2: Run tests to generate initial snapshots**

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj --filter "OpenApiDocumentSnapshotTests"`
Expected: FAIL (no verified snapshots yet)

**Step 3: Accept the snapshots**

Review the generated `.received.txt` files. If they look correct, rename to `.verified.txt`.

**Step 4: Run tests again to confirm they pass**

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj --filter "OpenApiDocumentSnapshotTests"`
Expected: PASS

**Step 5: Commit**

```bash
git add src/NSwag.Core.Tests/Serialization/OpenApiDocumentSnapshotTests.cs src/NSwag.Core.Tests/Snapshots/
git commit -m "test: add OpenAPI document serialization snapshot tests"
```

---

## Phase 2: NSwag.Core Attribute Migration (Mechanical)

Migrate all 28 files from Newtonsoft attributes to STJ attributes. This is mechanical work following the attribute migration reference table above.

### Task 2.1: Migrate simple model files (no custom converters, no extension data)

**Files to modify** (one `using` swap + attribute swaps each):
- `src/NSwag.Core/OpenApiTag.cs`
- `src/NSwag.Core/OpenApiServerVariable.cs`
- `src/NSwag.Core/OpenApiServer.cs`
- `src/NSwag.Core/OpenApiOAuthFlows.cs`
- `src/NSwag.Core/OpenApiOAuthFlow.cs`
- `src/NSwag.Core/OpenApiMediaType.cs`
- `src/NSwag.Core/OpenApiLink.cs`
- `src/NSwag.Core/OpenApiLicense.cs`
- `src/NSwag.Core/OpenApiInfo.cs`
- `src/NSwag.Core/OpenApiExternalDocumentation.cs`
- `src/NSwag.Core/OpenApiExample.cs`
- `src/NSwag.Core/OpenApiEncoding.cs`
- `src/NSwag.Core/OpenApiContact.cs`
- `src/NSwag.Core/JsonExpectedSchema.cs`

**Step 1: For each file, apply the mechanical attribute swap**

Example transformation for `OpenApiTag.cs`:

Before:
```csharp
using Newtonsoft.Json;

// ...
[JsonProperty(PropertyName = "name", DefaultValueHandling = DefaultValueHandling.Ignore)]
public string Name { get; set; }
```

After:
```csharp
using System.Text.Json.Serialization;

// ...
[JsonPropertyName("name")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
public string Name { get; set; }
```

Apply the same pattern to all 14 files listed above.

**Step 2: Build to verify**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Run snapshot tests**

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj --filter "OpenApiDocumentSnapshotTests"`
Expected: Check for drift. Update snapshots if the output is semantically equivalent.

**Step 4: Commit**

```bash
git add src/NSwag.Core/
git commit -m "refactor: migrate simple NSwag.Core model files from Newtonsoft to STJ attributes"
```

---

### Task 2.2: Migrate enum types with StringEnumConverter

**Files:**
- Modify: `src/NSwag.Core/OpenApiParameterStyle.cs`
- Modify: `src/NSwag.Core/OpenApiParameterKind.cs`
- Modify: `src/NSwag.Core/OpenApiParameterCollectionFormat.cs`
- Modify: `src/NSwag.Core/OpenApiOAuth2Flow.cs`

**Step 1: Swap converters**

Before:
```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum OpenApiParameterStyle { ... }
```

After:
```csharp
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter<OpenApiParameterStyle>))]
public enum OpenApiParameterStyle { ... }
```

Apply to all 4 enum files.

**Step 2: Build and test**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.Core/
git commit -m "refactor: migrate NSwag.Core enum types to STJ JsonStringEnumConverter"
```

---

### Task 2.3: Migrate complex model files (with Order, Required, ItemConverterType)

**Files:**
- Modify: `src/NSwag.Core/OpenApiDocument.cs`
- Modify: `src/NSwag.Core/OpenApiDocument.Serialization.cs`
- Modify: `src/NSwag.Core/OpenApiComponents.cs`
- Modify: `src/NSwag.Core/OpenApiOperation.cs`
- Modify: `src/NSwag.Core/OpenApiParameter.cs`
- Modify: `src/NSwag.Core/OpenApiRequestBody.cs`
- Modify: `src/NSwag.Core/OpenApiResponse.cs`
- Modify: `src/NSwag.Core/OpenApiSecurityScheme.cs`
- Modify: `src/NSwag.Core/OpenApiCallback.cs`

**Step 1: Apply attribute swaps with Order and Required**

Example for `OpenApiDocument.cs`:

Before:
```csharp
[JsonProperty(PropertyName = "info", Order = 4, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
public OpenApiInfo Info { get; set; }
```

After:
```csharp
[JsonPropertyName("info")]
[JsonPropertyOrder(4)]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
public OpenApiInfo Info { get; set; }
```

For `Required.Always`:
```csharp
// Before
[JsonProperty(PropertyName = "title", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]

// After
[JsonPropertyName("title")]
[JsonRequired]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
```

For `ItemConverterType = typeof(StringEnumConverter)` (in `OpenApiDocument.Serialization.cs` and `OpenApiOperation.cs`):
The enum types themselves already have `[JsonConverter(typeof(JsonStringEnumConverter<T>))]` from Task 2.2, so just remove the `ItemConverterType` parameter from the property attribute.

**Step 2: Handle OpenApiDocument.cs Formatting reference**

`OpenApiDocument.cs` line 123 uses `Formatting.Indented` (Newtonsoft). Replace the `ToJson()` method signature and internals to use STJ. Check how NJsonSchema's `ToJson()` works after its migration and follow the same pattern.

**Step 3: Handle extension data type changes**

In `OpenApiResponse.cs` and `OpenApiPathItem.cs` (pathitem done in Task 2.4), change:
```csharp
// Before
[JsonExtensionData]
public IDictionary<string, JToken?> ExtensionData { get; set; }

// After
[JsonExtensionData]
public IDictionary<string, JsonNode?> ExtensionData { get; set; }
```

Add `using System.Text.Json.Nodes;` where needed.

**Step 4: Build and test**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj`
Expected: Check for failures and fix

**Step 5: Commit**

```bash
git add src/NSwag.Core/
git commit -m "refactor: migrate complex NSwag.Core model files to STJ attributes"
```

---

### Task 2.4: Rewrite OpenApiPathItemConverter for STJ

**Files:**
- Modify: `src/NSwag.Core/OpenApiPathItem.cs`

This is the most complex single file. It has a custom `JsonConverter` that manually serializes/deserializes path items with HTTP operations, extension data, and `$ref` handling.

**Step 1: Migrate the OpenApiPathItem attributes**

Same as other files — swap `[JsonProperty]` → `[JsonPropertyName]`, `[JsonIgnore]` → `[JsonIgnore]`, etc.

Change `[JsonExtensionData]` backing type from `JToken` to `JsonNode`.

**Step 2: Rewrite OpenApiPathItemConverter as STJ JsonConverter<OpenApiPathItem>**

The converter needs to handle:
- Writing: summary, description, servers, parameters, extension data, and HTTP operation methods (get, put, post, delete, options, head, patch, trace) as lowercase keys
- Reading: parsing property names, detecting `$ref`, deserializing nested objects, collecting extension data (`x-*` properties)

```csharp
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public class OpenApiPathItemConverter : JsonConverter<OpenApiPathItem>
{
    public override OpenApiPathItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var pathItem = new OpenApiPathItem();

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject");

        while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
        {
            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "$ref":
                    // Handle reference — read as string, set on pathItem
                    break;
                case "summary":
                    pathItem.Summary = reader.GetString();
                    break;
                case "description":
                    pathItem.Description = reader.GetString();
                    break;
                case "servers":
                    pathItem.Servers = JsonSerializer.Deserialize<List<OpenApiServer>>(ref reader, options);
                    break;
                case "parameters":
                    pathItem.Parameters = JsonSerializer.Deserialize<List<OpenApiParameter>>(ref reader, options);
                    break;
                default:
                    if (Enum.TryParse<OpenApiOperationMethod>(propertyName, true, out var method))
                    {
                        var operation = JsonSerializer.Deserialize<OpenApiOperation>(ref reader, options);
                        pathItem[method] = operation;
                    }
                    else if (propertyName.StartsWith("x-"))
                    {
                        pathItem.ExtensionData ??= new Dictionary<string, JsonNode?>();
                        pathItem.ExtensionData[propertyName] = JsonNode.Parse(JsonSerializer.Serialize(JsonSerializer.Deserialize<JsonNode>(ref reader, options)));
                    }
                    else
                    {
                        reader.Skip();
                    }
                    break;
            }
        }

        return pathItem;
    }

    public override void Write(Utf8JsonWriter writer, OpenApiPathItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        // Write known properties
        if (!string.IsNullOrEmpty(value.Summary))
        {
            writer.WriteString("summary", value.Summary);
        }
        if (!string.IsNullOrEmpty(value.Description))
        {
            writer.WriteString("description", value.Description);
        }

        // Write operations with lowercase keys
        foreach (var pair in value)
        {
            writer.WritePropertyName(pair.Key.ToString().ToLowerInvariant());
            JsonSerializer.Serialize(writer, pair.Value, options);
        }

        // Write parameters, servers, extension data
        if (value.Parameters?.Count > 0)
        {
            writer.WritePropertyName("parameters");
            JsonSerializer.Serialize(writer, value.Parameters, options);
        }
        if (value.Servers?.Count > 0)
        {
            writer.WritePropertyName("servers");
            JsonSerializer.Serialize(writer, value.Servers, options);
        }
        if (value.ExtensionData != null)
        {
            foreach (var ext in value.ExtensionData)
            {
                writer.WritePropertyName(ext.Key);
                if (ext.Value != null)
                    ext.Value.WriteTo(writer);
                else
                    writer.WriteNullValue();
            }
        }

        writer.WriteEndObject();
    }
}
```

**Note:** This is a starting point. The exact implementation must match the current Newtonsoft converter's behavior. Compare carefully with the existing `OpenApiPathItemConverter` (lines 111-212 of `OpenApiPathItem.cs`) and port all edge cases, especially `$ref` handling.

**Step 3: Build and test**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj`
Expected: PASS (check snapshot tests for drift)

**Step 4: Commit**

```bash
git add src/NSwag.Core/OpenApiPathItem.cs
git commit -m "refactor: rewrite OpenApiPathItemConverter for System.Text.Json"
```

---

### Task 2.5: Migrate OpenApiSecurityScheme enum converters

**Files:**
- Modify: `src/NSwag.Core/OpenApiSecurityScheme.cs`

This file has `[JsonConverter(typeof(StringEnumConverter))]` on the `In` property (of type `OpenApiSecurityApiKeyLocation`) and the `Type` property (of type `OpenApiSecuritySchemeType`). The enum types themselves may not have the converter attribute, so:

**Step 1: Check if the enum types have their own converter attributes**

If `OpenApiSecuritySchemeType` and `OpenApiSecurityApiKeyLocation` don't already have `[JsonConverter]`, add `[JsonConverter(typeof(JsonStringEnumConverter<T>))]` to the enum declarations.

**Step 2: Remove the per-property `[JsonConverter]` attributes**

Since the enums now have their own converter, the property-level converter is redundant.

**Step 3: Swap remaining attributes as per the reference table**

**Step 4: Build and test**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true && dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj`
Expected: SUCCESS

**Step 5: Commit**

```bash
git add src/NSwag.Core/
git commit -m "refactor: migrate OpenApiSecurityScheme to STJ enum converters"
```

---

### Task 2.6: Update NSwag.Core.csproj — remove Newtonsoft.Json dependency

**Files:**
- Modify: `src/NSwag.Core/NSwag.Core.csproj`

**Step 1: Remove Newtonsoft.Json PackageReference**

Find and remove:
```xml
<PackageReference Include="Newtonsoft.Json" Version="..." />
```

The project should now only depend on NJsonSchema (which itself uses STJ).

**Step 2: Add System.Text.Json PackageReference if not already present**

For `netstandard2.0` and `net462` targets, System.Text.Json needs to come from the NuGet package. Check if it's already pulled transitively via NJsonSchema. If not, add:

```xml
<PackageReference Include="System.Text.Json" Version="9.0.0" />
```

**Step 3: Build on all TFMs**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS on all target frameworks

**Step 4: Run all NSwag.Core tests**

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj`
Expected: PASS

**Step 5: Commit**

```bash
git add src/NSwag.Core/NSwag.Core.csproj
git commit -m "refactor: remove Newtonsoft.Json dependency from NSwag.Core"
```

---

## Phase 3: NSwag.Commands Migration

### Task 3.1: Migrate NSwagDocumentBase.cs

**Files:**
- Modify: `src/NSwag.Commands/NSwagDocumentBase.cs`

This is the most complex file in NSwag.Commands. It uses `JObject`, `JsonConvert`, `JsonSerializerSettings`, and `CamelCasePropertyNamesContractResolver`.

**Step 1: Replace using statements**

```csharp
// Remove:
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

// Add:
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
```

**Step 2: Replace GetSerializerSettings() with GetSerializerOptions()**

Before:
```csharp
private static JsonSerializerSettings GetSerializerSettings()
{
    return new JsonSerializerSettings
    {
        DefaultValueHandling = DefaultValueHandling.Include,
        NullValueHandling = NullValueHandling.Include,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Converters = [new StringEnumConverter()]
    };
}
```

After:
```csharp
private static JsonSerializerOptions GetSerializerOptions()
{
    return new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };
}
```

**Step 3: Replace all JsonConvert calls**

- `JsonConvert.SerializeObject(obj, Formatting.Indented, settings)` → `JsonSerializer.Serialize(obj, options)` (WriteIndented is on options)
- `JsonConvert.DeserializeObject<T>(json, settings)` → `JsonSerializer.Deserialize<T>(json, options)`
- `JsonConvert.ToString(value)` → `JsonSerializer.Serialize(value)` or manual escaping

**Step 4: Replace JObject usage**

- `JObject.FromObject(dict)` → `JsonSerializer.SerializeToNode(dict)` to get a `JsonNode`
- `JObject.Parse(data)` → `JsonNode.Parse(data)` 
- `obj["key"].Value<int>()` → `node["key"].GetValue<int>()`

**Step 5: Replace [JsonProperty] and [JsonIgnore] attributes**

Same mechanical swap as NSwag.Core files.

**Step 6: Build and test**

Run: `dotnet build src/NSwag.Commands/NSwag.Commands.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 7: Commit**

```bash
git add src/NSwag.Commands/NSwagDocumentBase.cs
git commit -m "refactor: migrate NSwagDocumentBase from Newtonsoft to STJ"
```

---

### Task 3.2: Migrate remaining NSwag.Commands files

**Files:**
- Modify: `src/NSwag.Commands/OpenApiGeneratorCollection.cs` — `[JsonIgnore]` swap only
- Modify: `src/NSwag.Commands/CodeGeneratorCollection.cs` — `[JsonProperty]` + `[JsonIgnore]` swap
- Modify: `src/NSwag.Commands/Commands/OutputCommandBase.cs` — `[JsonProperty]` swap
- Modify: `src/NSwag.Commands/Commands/InputOutputCommandBase.cs` — `[JsonIgnore]` swap
- Modify: `src/NSwag.Commands/Commands/Generation/FromDocumentCommand.cs` — `[JsonProperty]` swap
- Modify: `src/NSwag.Commands/Commands/CodeGeneration/JsonSchemaToCSharpCommand.cs` — `[JsonIgnore]` swap
- Modify: `src/NSwag.Commands/Commands/CodeGeneration/CodeGeneratorCommandBase.cs` — `[JsonIgnore]` swap
- Modify: `src/NSwag.Commands/Commands/CodeGeneration/JsonSchemaToOpenApiCommand.cs` — `[JsonProperty]` swap
- Modify: `src/NSwag.Commands/Commands/Generation/AspNetCore/AspNetCoreToOpenApiGeneratorCommandEntryPoint.cs` — `JsonConvert.DeserializeObject` → `JsonSerializer.Deserialize`
- Modify: `src/NSwag.Commands/Commands/Generation/AspNetCore/AspNetCoreToOpenApiCommand.cs` — `JsonConvert.SerializeObject` → `JsonSerializer.Serialize`

**Step 1: Apply mechanical swaps to all files**

These are all straightforward — attribute namespace changes and `JsonConvert` → `JsonSerializer` calls.

For `CodeGeneratorCollection.cs`, `NullValueHandling.Ignore`:
```csharp
// Before
[JsonProperty("OpenApiToTypeScriptClient", NullValueHandling = NullValueHandling.Ignore)]

// After
[JsonPropertyName("OpenApiToTypeScriptClient")]
[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
```

**Step 2: Build and test**

Run: `dotnet build src/NSwag.Commands/NSwag.Commands.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.Commands/
git commit -m "refactor: migrate remaining NSwag.Commands files to STJ"
```

---

## Phase 4: NSwag.Core.Yaml Migration

### Task 4.1: Migrate OpenApiYamlDocument.cs

**Files:**
- Modify: `src/NSwag.Core.Yaml/OpenApiYamlDocument.cs`

**Step 1: Replace ExpandoObject conversion**

Before:
```csharp
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
// ...
var expConverter = new ExpandoObjectConverter();
dynamic expandoObject = JsonConvert.DeserializeObject<ExpandoObject>(json, expConverter);
```

After:
```csharp
using System.Text.Json;
using System.Text.Json.Nodes;
// ...
// Convert JSON string to a dynamic-friendly structure for YamlDotNet
var jsonNode = JsonNode.Parse(json);
var expandoObject = ConvertJsonNodeToExpandoObject(jsonNode);
```

Write a helper method `ConvertJsonNodeToExpandoObject` that recursively converts `JsonNode` to `ExpandoObject`/`List<object>`/primitives for YamlDotNet compatibility:

```csharp
private static object ConvertJsonNodeToExpandoObject(JsonNode node)
{
    if (node is JsonObject jsonObject)
    {
        var expando = new ExpandoObject();
        var dict = (IDictionary<string, object>)expando;
        foreach (var property in jsonObject)
        {
            dict[property.Key] = property.Value != null ? ConvertJsonNodeToExpandoObject(property.Value) : null;
        }
        return expando;
    }
    else if (node is JsonArray jsonArray)
    {
        return jsonArray.Select(item => item != null ? ConvertJsonNodeToExpandoObject(item) : null).ToList();
    }
    else if (node is JsonValue jsonValue)
    {
        if (jsonValue.TryGetValue<bool>(out var boolValue)) return boolValue;
        if (jsonValue.TryGetValue<long>(out var longValue)) return longValue;
        if (jsonValue.TryGetValue<double>(out var doubleValue)) return doubleValue;
        if (jsonValue.TryGetValue<string>(out var stringValue)) return stringValue;
        return node.ToJsonString();
    }
    return null;
}
```

**Step 2: Build and test**

Run: `dotnet build src/NSwag.Core.Yaml/NSwag.Core.Yaml.csproj -p:UseLocalNJsonSchemaProjects=true && dotnet test src/NSwag.Core.Yaml.Tests/NSwag.Core.Yaml.Tests.csproj`
Expected: PASS

**Step 3: Commit**

```bash
git add src/NSwag.Core.Yaml/
git commit -m "refactor: migrate NSwag.Core.Yaml from Newtonsoft to STJ"
```

---

## Phase 5: NSwag.AspNetCore Migration

### Task 5.1: Migrate SwaggerUiSettings and SwaggerUiSettingsBase

**Files:**
- Modify: `src/NSwag.AspNetCore/SwaggerUiSettings.cs`
- Modify: `src/NSwag.AspNetCore/SwaggerUiSettingsBase.cs`

**Step 1: Replace JsonConvert calls**

In `SwaggerUiSettings.cs`:
```csharp
// Before
JsonConvert.SerializeObject(collection)
JsonConvert.SerializeObject(swaggerRoutes)

// After
JsonSerializer.Serialize(collection)
JsonSerializer.Serialize(swaggerRoutes)
```

In `SwaggerUiSettingsBase.cs`:
```csharp
// Before
JsonConvert.SerializeObject(pair.Value)

// After
JsonSerializer.Serialize(pair.Value)
```

In `SwaggerUiSettings.cs` inner class `SwaggerUiRoute`:
```csharp
// Before
[JsonProperty("url")]
public string Url { get; set; }
[JsonProperty("name")]
public string Name { get; set; }

// After
[JsonPropertyName("url")]
public string Url { get; set; }
[JsonPropertyName("name")]
public string Name { get; set; }
```

**Step 2: Replace using statements**

```csharp
// Remove: using Newtonsoft.Json;
// Add: using System.Text.Json; using System.Text.Json.Serialization;
```

**Step 3: Build**

Run: `dotnet build src/NSwag.AspNetCore/NSwag.AspNetCore.csproj`
Expected: SUCCESS

**Step 4: Commit**

```bash
git add src/NSwag.AspNetCore/SwaggerUiSettings.cs src/NSwag.AspNetCore/SwaggerUiSettingsBase.cs
git commit -m "refactor: migrate SwaggerUiSettings to STJ"
```

---

### Task 5.2: Migrate JsonExceptionFilterAttribute (AspNetCore)

**Files:**
- Modify: `src/NSwag.AspNetCore/JsonExceptionFilterAttribute.cs`

**Step 1: Replace JsonConvert and JsonSerializerSettings**

This file uses `JsonConvert.SerializeObject(exception, settings)` with custom settings and a `JsonExceptionConverter`. The `JsonExceptionConverter` is from NJsonSchema.

After the NJsonSchema STJ migration, check if there's a STJ equivalent of `JsonExceptionConverter`. If so, use it. If not, serialize exceptions with a simple STJ approach:

```csharp
// Before
var settings = GetSerializerSettings(context);
settings.Converters.Add(new JsonExceptionConverter(...));
var json = JsonConvert.SerializeObject(context.Exception, settings);

// After  
var options = new JsonSerializerOptions { WriteIndented = true };
// Use NJsonSchema's STJ exception converter if available, otherwise basic serialization
var json = JsonSerializer.Serialize(context.Exception, options);
```

**Step 2: Remove CopySettings method** (no longer needed with STJ)

**Step 3: Build and test**

Run: `dotnet build src/NSwag.AspNetCore/NSwag.AspNetCore.csproj`
Expected: SUCCESS

**Step 4: Commit**

```bash
git add src/NSwag.AspNetCore/JsonExceptionFilterAttribute.cs
git commit -m "refactor: migrate JsonExceptionFilterAttribute to STJ"
```

---

### Task 5.3: Migrate NSwagServiceCollectionExtensions

**Files:**
- Modify: `src/NSwag.AspNetCore/Extensions/NSwagServiceCollectionExtensions.cs`

**Step 1: Update GetJsonSerializerSettings references**

This file calls `AspNetCoreOpenApiDocumentGenerator.GetJsonSerializerSettings(services)`. After Phase 6 migrates the generation layer, this will need to use the STJ equivalent. For now, note the dependency and update when Phase 6 is complete.

---

## Phase 6: NSwag.Generation Migration

### Task 6.1: Migrate OpenApiDocumentGeneratorSettings.cs

**Files:**
- Modify: `src/NSwag.Generation/OpenApiDocumentGeneratorSettings.cs`

**Step 1: Swap [JsonIgnore] namespace**

```csharp
// Before: using Newtonsoft.Json;
// After: using System.Text.Json.Serialization;
```

The `[JsonIgnore]` attributes don't change — just the using statement.

**Step 2: Commit**

```bash
git add src/NSwag.Generation/OpenApiDocumentGeneratorSettings.cs
git commit -m "refactor: migrate OpenApiDocumentGeneratorSettings to STJ"
```

---

### Task 6.2: Migrate OpenApiDocumentGenerator.cs

**Files:**
- Modify: `src/NSwag.Generation/OpenApiDocumentGenerator.cs`

**Step 1: Replace schema annotation check**

Before:
```csharp
JsonConvert.SerializeObject(operationParameter.Schema) != "{}"
```

After:
```csharp
JsonSerializer.Serialize(operationParameter.Schema) != "{}"
```

Or better, check schema properties directly without serialization if possible.

**Step 2: Update using statements**

```csharp
// Before: using Newtonsoft.Json;
// After: using System.Text.Json;
```

**Step 3: Commit**

```bash
git add src/NSwag.Generation/OpenApiDocumentGenerator.cs
git commit -m "refactor: migrate OpenApiDocumentGenerator to STJ"
```

---

### Task 6.3: Migrate AspNetCoreOpenApiDocumentGenerator.cs

**Files:**
- Modify: `src/NSwag.Generation.AspNetCore/AspNetCoreOpenApiDocumentGenerator.cs`

This file uses reflection to load `MvcNewtonsoftJsonOptions` from ASP.NET Core. After migration, the default path should use STJ's `JsonOptions` (which is the default in modern ASP.NET Core).

**Step 1: Update GetJsonSerializerSettings to GetJsonSerializerOptions**

The method should return `JsonSerializerOptions` instead of `JsonSerializerSettings`. It should look for ASP.NET Core's `Microsoft.AspNetCore.Http.Json.JsonOptions` from DI (the default STJ configuration).

The Newtonsoft reflection path (loading `MvcNewtonsoftJsonOptions`) moves to `NSwag.Generation.NewtonsoftJson` in Phase 7.

**Step 2: Replace JsonConvert.DeserializeObject**

```csharp
// Before
JsonConvert.DeserializeObject<OpenApiOperation>(stringBuilder.ToString())

// After
JsonSerializer.Deserialize<OpenApiOperation>(stringBuilder.ToString())
```

**Step 3: Build and test**

Run: `dotnet build src/NSwag.Generation.AspNetCore/NSwag.Generation.AspNetCore.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 4: Commit**

```bash
git add src/NSwag.Generation.AspNetCore/
git commit -m "refactor: migrate AspNetCoreOpenApiDocumentGenerator to STJ"
```

---

## Phase 7: Create NSwag.Generation.NewtonsoftJson

### Task 7.1: Create the new project

**Files:**
- Create: `src/NSwag.Generation.NewtonsoftJson/NSwag.Generation.NewtonsoftJson.csproj`

**Step 1: Create the .csproj**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>netstandard2.0;net462;net8.0</TargetFrameworks>
    <Nullable>enable</Nullable>
    <PackageDescription>NSwag Newtonsoft.Json integration for OpenAPI schema generation from Newtonsoft-annotated types</PackageDescription>
  </PropertyGroup>

  <!-- Local project references for development -->
  <ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' == 'true'">
    <ProjectReference Include="..\..\..\NJsonSchema\src\NJsonSchema.NewtonsoftJson\NJsonSchema.NewtonsoftJson.csproj" />
  </ItemGroup>
  <!-- NuGet package references for production -->
  <ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' != 'true'">
    <PackageReference Include="NJsonSchema.NewtonsoftJson" Version="11.3.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\NSwag.Generation\NSwag.Generation.csproj" />
  </ItemGroup>

</Project>
```

**Step 2: Create the Newtonsoft-aware generator extension**

Create: `src/NSwag.Generation.NewtonsoftJson/NewtonsoftJsonOpenApiSchemaGeneratorExtensions.cs`

This class provides the opt-in for Newtonsoft-aware schema generation. Move the reflection-based `MvcNewtonsoftJsonOptions` detection from `AspNetCoreOpenApiDocumentGenerator` here.

**Step 3: Build**

Run: `dotnet build src/NSwag.Generation.NewtonsoftJson/NSwag.Generation.NewtonsoftJson.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 4: Commit**

```bash
git add src/NSwag.Generation.NewtonsoftJson/
git commit -m "feat: create NSwag.Generation.NewtonsoftJson project"
```

---

### Task 7.2: Update NSwag.Generation to drop NJsonSchema.NewtonsoftJson

**Files:**
- Modify: `src/NSwag.Generation/NSwag.Generation.csproj`

**Step 1: Change NJsonSchema.NewtonsoftJson reference to NJsonSchema**

The generation project should now reference just `NJsonSchema` (STJ-based), not `NJsonSchema.NewtonsoftJson`.

```xml
<!-- Local project references for development -->
<ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' == 'true'">
  <ProjectReference Include="..\..\..\NJsonSchema\src\NJsonSchema\NJsonSchema.csproj" />
</ItemGroup>
<!-- NuGet package references for production -->
<ItemGroup Condition="'$(UseLocalNJsonSchemaProjects)' != 'true'">
  <PackageReference Include="NJsonSchema" Version="11.3.2" />
</ItemGroup>
```

**Step 2: Build and verify no Newtonsoft references remain**

Run: `dotnet build src/NSwag.Generation/NSwag.Generation.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS (no Newtonsoft types should be used in this project after Phase 6 migration)

**Step 3: Commit**

```bash
git add src/NSwag.Generation/NSwag.Generation.csproj
git commit -m "refactor: drop NJsonSchema.NewtonsoftJson dependency from NSwag.Generation"
```

---

### Task 7.3: Update NSwag.AspNet.WebApi to reference NSwag.Generation.NewtonsoftJson

**Files:**
- Modify: `src/NSwag.AspNet.WebApi/NSwag.AspNet.WebApi.csproj`

**Step 1: Add project reference to NSwag.Generation.NewtonsoftJson**

This project stays Newtonsoft-based (old ASP.NET). It should reference `NSwag.Generation.NewtonsoftJson` instead of directly referencing `NJsonSchema.NewtonsoftJson`.

**Step 2: Build**

Run: `dotnet build src/NSwag.AspNet.WebApi/NSwag.AspNet.WebApi.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.AspNet.WebApi/NSwag.AspNet.WebApi.csproj
git commit -m "refactor: NSwag.AspNet.WebApi references NSwag.Generation.NewtonsoftJson"
```

---

## Phase 8: Peripheral Projects Cleanup

### Task 8.1: Migrate NSwag.CodeGeneration

**Files:**
- Modify: `src/NSwag.CodeGeneration/ClientGeneratorBase.cs`

**Step 1: Remove Newtonsoft.Json.Linq using statement**

```csharp
// Remove: using Newtonsoft.Json.Linq;
```

If any JObject/JToken usage exists, replace with JsonNode equivalents.

**Step 2: Build**

Run: `dotnet build src/NSwag.CodeGeneration/NSwag.CodeGeneration.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.CodeGeneration/
git commit -m "refactor: remove Newtonsoft from NSwag.CodeGeneration"
```

---

### Task 8.2: Migrate NSwag.CodeGeneration.CSharp

**Files:**
- Modify: `src/NSwag.CodeGeneration.CSharp/CSharpGeneratorBaseSettings.cs`

**Step 1: Swap [JsonIgnore] namespace**

```csharp
// Before: using Newtonsoft.Json;
// After: using System.Text.Json.Serialization;
```

**Note:** Do NOT modify the Liquid templates (`Client.Class.liquid`, `JsonExceptionConverter.liquid`, etc.). These generate user code that supports both Newtonsoft and STJ via the `CSharpJsonLibrary` setting. They are output code, not NSwag's own dependency.

**Step 2: Build**

Run: `dotnet build src/NSwag.CodeGeneration.CSharp/NSwag.CodeGeneration.CSharp.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.CodeGeneration.CSharp/CSharpGeneratorBaseSettings.cs
git commit -m "refactor: remove Newtonsoft from NSwag.CodeGeneration.CSharp"
```

---

### Task 8.3: Migrate NSwag.CodeGeneration.TypeScript

**Files:**
- Modify: `src/NSwag.CodeGeneration.TypeScript/TypeScriptClientGeneratorSettings.cs`

**Step 1: Swap [JsonIgnore] namespace**

```csharp
// Before: using Newtonsoft.Json;
// After: using System.Text.Json.Serialization;
```

**Step 2: Build**

Run: `dotnet build src/NSwag.CodeGeneration.TypeScript/NSwag.CodeGeneration.TypeScript.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

**Step 3: Commit**

```bash
git add src/NSwag.CodeGeneration.TypeScript/TypeScriptClientGeneratorSettings.cs
git commit -m "refactor: remove Newtonsoft from NSwag.CodeGeneration.TypeScript"
```

---

### Task 8.4: Migrate NSwag.Generation.WebApi

**Files:**
- Modify: `src/NSwag.Generation.WebApi/Processors/OperationParameterProcessor.cs`

**Step 1: Check usage**

The file checks for `JsonIgnoreAttribute` by type name string (not a direct Newtonsoft reference). This string check should work for both Newtonsoft and STJ `JsonIgnore` attributes. Verify no `using Newtonsoft.Json` exists.

**Step 2: Build**

Run: `dotnet build src/NSwag.Generation.WebApi/NSwag.Generation.WebApi.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS

---

## Phase 9: Remove Newtonsoft Dependencies from .csproj Files

### Task 9.1: Audit and remove remaining Newtonsoft PackageReferences

**Files to check:**
- `src/NSwag.Core/NSwag.Core.csproj` (should be done in Task 2.6)
- `src/NSwag.Core.Yaml/NSwag.Core.Yaml.csproj`
- `src/NSwag.Commands/NSwag.Commands.csproj`
- `src/NSwag.AspNetCore/NSwag.AspNetCore.csproj`
- `src/NSwag.Generation/NSwag.Generation.csproj` (should be done in Task 7.2)
- `src/NSwag.Generation.AspNetCore/NSwag.Generation.AspNetCore.csproj`
- `src/NSwag.CodeGeneration/NSwag.CodeGeneration.csproj`
- `src/NSwag.CodeGeneration.CSharp/NSwag.CodeGeneration.CSharp.csproj`
- `src/NSwag.CodeGeneration.TypeScript/NSwag.CodeGeneration.TypeScript.csproj`

**Step 1: For each .csproj, verify no direct Newtonsoft.Json PackageReference remains**

Search for `Newtonsoft` in all .csproj files (excluding NSwag.Generation.NewtonsoftJson, NSwag.AspNet.WebApi, and test projects).

**Step 2: Build the full solution**

Run: `dotnet build -p:UseLocalNJsonSchemaProjects=true` (from repo root, if solution file exists)
or build each project individually.

**Step 3: Commit**

```bash
git add src/
git commit -m "refactor: remove all remaining Newtonsoft.Json dependencies from core projects"
```

---

## Phase 10: Test Updates & Verification

### Task 10.1: Update test projects

**Files:**
- Modify: `src/NSwag.Core.Tests/NSwag.Core.Tests.csproj` — may need `System.Text.Json` reference
- Modify: `src/NSwag.CodeGeneration.Tests/NSwag.CodeGeneration.Tests.csproj` — remove `NJsonSchema.NewtonsoftJson` if no longer needed
- Modify: All test files that use Newtonsoft types for assertions

**Step 1: Update test project references**

Test projects may still need Newtonsoft for testing Newtonsoft-specific scenarios, but the main test paths should use STJ.

**Step 2: Update test code**

Any test that directly uses `JObject`, `JToken`, `JsonConvert` for assertions needs to be updated:
- `JObject.Parse(json)["property"]` → `JsonNode.Parse(json)["property"]`
- `JsonConvert.DeserializeObject<T>(json)` → `JsonSerializer.Deserialize<T>(json)`

---

### Task 10.2: Run full test suite

**Step 1: Run all tests with local NJsonSchema references**

Run: `dotnet test src/NSwag.Core.Tests/NSwag.Core.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`
Run: `dotnet test src/NSwag.Core.Yaml.Tests/NSwag.Core.Yaml.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`
Run: `dotnet test src/NSwag.CodeGeneration.Tests/NSwag.CodeGeneration.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`
Run: `dotnet test src/NSwag.CodeGeneration.CSharp.Tests/NSwag.CodeGeneration.CSharp.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`
Run: `dotnet test src/NSwag.CodeGeneration.TypeScript.Tests/NSwag.CodeGeneration.TypeScript.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`
Run: `dotnet test src/NSwag.Generation.Tests/NSwag.Generation.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`
Run: `dotnet test src/NSwag.Generation.AspNetCore.Tests/NSwag.Generation.AspNetCore.Tests.csproj -p:UseLocalNJsonSchemaProjects=true`

Expected: ALL PASS

**Step 2: Verify snapshot tests haven't drifted**

Check that all `.verified.txt` files match. If there's drift, review carefully — serialization order changes or whitespace differences may be acceptable.

**Step 3: Verify NSwagStudio still builds**

Run: `dotnet build src/NSwagStudio/NSwagStudio.csproj -p:UseLocalNJsonSchemaProjects=true`
Expected: SUCCESS (net462 WPF app)

---

### Task 10.3: Final verification — build with NuGet packages (switch off)

**Step 1: Verify default build still works**

Run: `dotnet build src/NSwag.Core/NSwag.Core.csproj`
Expected: SUCCESS (using NuGet packages — will only work once NJsonSchema v11 STJ is published)

**Note:** Until NJsonSchema publishes STJ-based NuGet packages, NuGet-mode builds will use the old Newtonsoft-based NJsonSchema. Local-reference mode is the primary development path for now.

**Step 2: Commit all remaining changes**

```bash
git add .
git commit -m "test: update test projects and verify full migration"
```
