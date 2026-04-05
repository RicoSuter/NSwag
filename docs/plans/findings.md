# NSwag STJ Migration — Remaining Findings

## 2 Remaining NSwag.Core.Tests Failures (down from 14)

### 1. `PathItemTests.PathItem_With_External_Ref_Can_Be_Serialized`

**Error:** `Assert.True() Failure` — resolved path item missing "200" response.

**Root cause:** External file `PathItemWithRef.json` has `$ref: "./refs/PathItem.json#/aValidPath"`. The external file is loaded via `JsonSchema.FromFileAsync` (NJsonSchema), which deserializes as a `JsonSchema`. The `aValidPath` key and its value (a path item object) end up in `ExtensionData` as raw dictionary data. When the reference resolver navigates to `aValidPath`, it finds raw data but converting it to a typed `OpenApiPathItem` (which implements `IDictionary` and has a custom converter) produces an empty object.

**Fix direction:** The reference resolver's `ResolveDocumentReference` needs to handle converting raw extension data to typed NSwag objects. This may require the resolver to detect the target type and use the appropriate `JsonSerializer.Deserialize<OpenApiPathItem>()` for the raw JSON node.

### 2. `HttpLoadingTests.When_openapi_is_loaded_without_scopes_it_should_deserialize`

**Error:** `'h' is invalid after a value` (JsonException during FixLenientJson fallback)

**Root cause:** The remote SaaS API spec's initial deserialization fails with some `JsonException` (type mismatch in the deep object graph), triggering the `FixLenientJson` fallback. The fallback's single-quote replacement corrupts content that contains apostrophes in double-quoted JSON strings (e.g., `"partner's"`).

**Fix direction:** Two options:
1. Identify and fix whatever causes the initial deserialization to fail (so `FixLenientJson` is never reached)
2. Make `FixLenientJson` safe for arbitrary JSON — this requires a proper tokenizer that tracks whether the current position is inside a double-quoted string, rather than regex-based replacement
