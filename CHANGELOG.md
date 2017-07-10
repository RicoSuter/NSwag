# Changelog

## Release v11.3

### TypeScript

**Breaking changes:**

- Return FileResponse instead of Blob for file responses with additional properties (data, fileName, headers) [#834](https://github.com/NSwag/NSwag/issues/834): Just use `response.data` to access the response blob. 
- Render union types (null/undefined) for nullable operation parameters [9c23e1485cd90b39369356d223428ab4586d0ee5](https://github.com/NSwag/NSwag/commit/9c23e1485cd90b39369356d223428ab4586d0ee5)

**Features and fixes:**

- Exclude more types with ExcludedTypeNames (FileParameter, FileResponse, etc.), [#825](https://github.com/NSwag/NSwag/issues/825)

### CSharp

- Added GenerateUpdateJsonSerializerSettingsMethod setting to customize the JSON.NET serializer in the base class (or partial class) [#827](https://github.com/NSwag/NSwag/pull/827)
- Add support for changing default date/time parameter formatting with the ParameterDateTimeFormat setting [#826](https://github.com/NSwag/NSwag/pull/826)

### Web API

- Fixed wildcard path parameter handling [#831](https://github.com/NSwag/NSwag/issues/831)
- Use ActionNameAttribute for operation id, [#821](https://github.com/NSwag/NSwag/issues/821)

## Release v11.0

See https://github.com/NSwag/NSwag/releases/tag/NSwag-Build-829

## Release v10.0

See https://github.com/NSwag/NSwag/releases/tag/NSwag-Build-813
