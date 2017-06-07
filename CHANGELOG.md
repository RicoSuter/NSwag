# Changelog

## Release v11.0

Breaking changes: 

- Web API Swagger generator
    - NJsonSchema v9 now uses ContractResolver to reflect DTO types
    - Added ApiVersionProcessor [#655](https://github.com/NSwag/NSwag/issues/655)
    - Performance improvements (up to 3x faster)
    - Fixed loading of inherited RouteAttributes [3806388850b9ff0980726d713edfe202635f8a8a](https://github.com/NSwag/NSwag/commit/3806388850b9ff0980726d713edfe202635f8a8a)

- Middlewares
    - Renamed SwaggerOwinSettings/SwaggerUiOwinSettings to SwaggerSettings/SwaggerUiSettings [b34210a8d96f791ec90cebaa385fd729a9073534](https://github.com/NSwag/NSwag/commit/b34210a8d96f791ec90cebaa385fd729a9073534)

- Command line
    - Added --core 1.0 and --core 1.1 options [19a9951dc8a4afb34bbf4213bf6553b2a5365153](https://github.com/NSwag/NSwag/commit/19a9951dc8a4afb34bbf4213bf6553b2a5365153)

- Added more sample projects

## Release v10.0

See https://github.com/NSwag/NSwag/releases/tag/NSwag-Build-813
