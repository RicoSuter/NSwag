# Changelog

## Release v11.2

### TypeScript

**Breaking changes:**

- Return FileResponse instead of Blob for file responses with additional properties (data, fileName, headers) [#834](https://github.com/NSwag/NSwag/issues/834): Just use `response.data` to access the response blob. 
- Render union types (null/undefined) for nullable operation parameters (9c23e1485cd90b39369356d223428ab4586d0ee5)[https://github.com/NSwag/NSwag/commit/9c23e1485cd90b39369356d223428ab4586d0ee5]


- Exclude more types with ExcludedTypeNames (FileParameter, FileResponse, etc.), [#825](https://github.com/NSwag/NSwag/issues/825)

### CSharp

- Added GenerateUpdateJsonSerializerSettingsMethod setting to customize the JSON.NET serializer in the base class (or partial class)
- 

### Web API

- Fixed wildcard path parameter handling [#831](https://github.com/NSwag/NSwag/issues/831)
- 

## Release v11.0

See https://github.com/NSwag/NSwag/releases/tag/NSwag-Build-829

## Release v10.0

See https://github.com/NSwag/NSwag/releases/tag/NSwag-Build-813
