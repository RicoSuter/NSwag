# NSwag STJ Migration — Remaining Findings

## 4 Remaining NSwag.Core.Tests Failures

All relate to reference resolution and external file loading, not core serialization.

### 1. `DocumentReferenceTests.When_parameter_references_schema_then_it_is_resolved`

**Error:** `The schema reference path '#/components/schemas/EntityId' has not been resolved.`

**Root cause:** After deserializing an OpenAPI3 document, `Components.Schemas` is empty. The `[JsonObjectCreationHandling(Populate)]` on `OpenApiDocument.Components` works for top-level deserialization (our snapshot round-trip test passes), but the reference resolution path (`UpdateSchemaReferencesAsync`) may traverse a different code path where the schemas aren't accessible. The `OpenApiComponents` parameterless `[JsonConstructor]` creates plain dictionaries (not `ObservableDictionary`), losing parent-tracking that the reference resolver relies on.

**Fix direction:** Investigate whether the parameterless constructor's plain dictionaries cause the reference resolver to miss schemas. May need to ensure `OpenApiComponents` always uses `ObservableDictionary` instances, or fix the reference resolver to not depend on parent tracking for schema lookup.

### 2. `PathItemTests.PathItem_With_External_Ref_Can_Be_Serialized`

**Error:** `Assert.True() Failure` — resolved path item missing "200" response.

**Root cause:** External file `PathItemWithRef.json` has `$ref: "./refs/PathItem.json#/aValidPath"`. The file is loaded via `JsonSchema.FromFileAsync` (NJsonSchema), which deserializes as a `JsonSchema` — not as `OpenApiPathItem`. The referenced data lands in `ExtensionData` as raw JSON, and converting it to `OpenApiPathItem` (which implements `IDictionary` and has a custom converter) may fail or produce an empty object.

**Fix direction:** The `OpenApiDocument`'s reference resolver override needs to handle external file references that resolve to non-schema types like `OpenApiPathItem`. May require overriding `ResolveFileReferenceAsync` to use NSwag-aware deserialization.

### 3. `ExternalReferenceTests.When_file_contains_parameter_reference_to_another_file_it_is_loaded`

**Error:** `The JSON value could not be converted to ICollection<string>. Path: $.required`

**Root cause:** External file `common.json` is loaded via `JsonSchema.FromFileAsync` (NJsonSchema), which doesn't have NSwag's `OpenApiParameterJsonConverter`. When the JSON contains `"required": false` (a boolean for parameter's IsRequired), it tries to deserialize as `ICollection<string>` (JsonSchema's RequiredPropertiesRaw) and crashes. This is the same `required` property collision, but in the external file loading path.

**Fix direction:** Either make the `OpenApiParameterJsonConverter` available in external file deserialization (by passing it through the reference resolver), or handle the `required` bool/array collision more broadly in NJsonSchema's deserialization (e.g., a type info modifier that ignores mismatched `required` values).

### 4. `HttpLoadingTests.When_openapi_is_loaded_without_scopes_it_should_deserialize`

**Error:** `'h' is invalid after a value` (JsonException during FixLenientJson fallback)

**Root cause:** The remote OpenAPI spec parses fine as JSON, but deserialization fails (likely due to a type mismatch somewhere in the deep object graph). This triggers `FixLenientJson` fallback, which now has improved single-quote handling but still corrupts the JSON in some edge case. The real fix is to make the initial deserialization succeed rather than relying on `FixLenientJson`.

**Fix direction:** Run the test with a breakpoint/logging to identify exactly which type/property causes the initial deserialization failure. Fix that specific issue so `FixLenientJson` is never reached. Alternatively, make `FixLenientJson`'s single-quote replacement smarter (only replace quotes used as string delimiters, never inside double-quoted strings).

## Architecture Notes

### SchemaSerializationConverter (NJsonSchema)

Key changes from original:
- **Write**: Property-by-property using `typeInfo.Properties` from stripped options, serializing values with full options (nested types get their own converters)
- **Read**: Recursive reverse renames on entire JSON tree before deserialization with stripped options
- **CanConvert**: Skips types with `[JsonConverter]` attribute (so attribute converters take precedence)
- **IsPropertyIgnored**: Exposed for `JsonPathUtilities` to skip ignored properties during `$ref` path computation
- **Empty collection filtering**: Checks `IEnumerable` on property values, excludes `string` and `JsonNode`
- **Extension data**: Written after regular properties using runtime type serialization

### OpenApiParameterJsonConverter (NSwag)

Handles `required` property collision between `OpenApiParameter.IsRequired` (bool) and `JsonSchema.RequiredPropertiesRaw` (string[]). Registered via `SchemaSerializationConverter.AddConverter()`, not as a type attribute.
