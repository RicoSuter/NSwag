using NJsonSchema;

namespace NSwag.Core.Tests.Serialization;

public class OpenApiDocumentSnapshotTests
{
    [Fact]
    public async Task Snapshot_MinimalDocument_Swagger2()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info.Title = "My API";
        document.Info.Version = "1.0.0";

        // Act
        var json = document.ToJson(SchemaType.Swagger2, true);

        // Assert
        await Verify(json).UseDirectory("Snapshots");
    }

    [Fact]
    public async Task Snapshot_MinimalDocument_OpenApi3()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info.Title = "My API";
        document.Info.Version = "1.0.0";

        // Act
        var json = document.ToJson(SchemaType.OpenApi3, true);

        // Assert
        await Verify(json).UseDirectory("Snapshots");
    }

    [Fact]
    public async Task Snapshot_FullDocument_OpenApi3()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info.Title = "Pet Store API";
        document.Info.Version = "2.0.0";
        document.Info.Description = "A sample API for managing pets";
        document.Info.TermsOfService = "https://example.com/terms";
        document.Info.Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com",
            Url = "https://example.com/support"
        };
        document.Info.License = new OpenApiLicense
        {
            Name = "MIT",
            Url = "https://opensource.org/licenses/MIT"
        };

        document.Servers.Add(new OpenApiServer
        {
            Url = "https://api.example.com/v2",
            Description = "Production server"
        });
        document.Servers.Add(new OpenApiServer
        {
            Url = "https://staging.example.com/v2",
            Description = "Staging server"
        });

        var schema = new JsonSchema
        {
            Type = JsonObjectType.Object,
        };
        schema.Properties["id"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.Integer,
            Description = "The pet ID"
        };
        schema.Properties["name"] = new JsonSchemaProperty
        {
            Type = JsonObjectType.String,
            Description = "The pet name"
        };

        document.Definitions["Pet"] = schema;

        var getOperation = new OpenApiOperation
        {
            Summary = "List all pets",
            OperationId = "listPets",
            Description = "Returns all pets from the system"
        };
        getOperation.Tags.Add("pets");
        getOperation.Parameters.Add(new OpenApiParameter
        {
            Name = "limit",
            Kind = OpenApiParameterKind.Query,
            Description = "Maximum number of pets to return",
            IsRequired = false,
            Schema = new JsonSchema { Type = JsonObjectType.Integer }
        });
        getOperation.Responses.Add("200", new OpenApiResponse
        {
            Description = "A list of pets"
        });

        var postOperation = new OpenApiOperation
        {
            Summary = "Create a pet",
            OperationId = "createPet",
            Description = "Creates a new pet in the store",
            RequestBody = new OpenApiRequestBody
            {
                Name = "body",
                Description = "Pet to add to the store",
                IsRequired = true,
                Content =
                {
                    {
                        "application/json",
                        new OpenApiMediaType
                        {
                            Schema = new JsonSchema { Reference = schema }
                        }
                    }
                }
            }
        };
        postOperation.Tags.Add("pets");
        postOperation.Responses.Add("201", new OpenApiResponse
        {
            Description = "Pet created"
        });

        var pathItem = new OpenApiPathItem
        {
            { OpenApiOperationMethod.Get, getOperation },
            { OpenApiOperationMethod.Post, postOperation }
        };
        document.Paths["/pets"] = pathItem;

        // Act
        var json = document.ToJson(SchemaType.OpenApi3, true);

        // Assert
        await Verify(json).UseDirectory("Snapshots");
    }

    [Fact]
    public async Task Snapshot_DocumentWithSecurity_OpenApi3()
    {
        // Arrange
        var document = new OpenApiDocument();
        document.Info.Title = "Secured API";
        document.Info.Version = "1.0.0";

        document.SecurityDefinitions.Add("bearerAuth", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "JWT Bearer token authentication"
        });

        document.SecurityDefinitions.Add("apiKey", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "X-API-Key",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "API key authentication"
        });

        document.SecurityDefinitions.Add("oauth2", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.OAuth2,
            Description = "OAuth2 authentication",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = "https://example.com/oauth/authorize",
                    TokenUrl = "https://example.com/oauth/token",
                    Scopes =
                    {
                        { "read:pets", "Read pets" },
                        { "write:pets", "Write pets" }
                    }
                }
            }
        });

        var getOperation = new OpenApiOperation
        {
            Summary = "Get secured data",
            OperationId = "getSecuredData"
        };
        getOperation.Responses.Add("200", new OpenApiResponse
        {
            Description = "Success"
        });

        var pathItem = new OpenApiPathItem
        {
            { OpenApiOperationMethod.Get, getOperation }
        };
        document.Paths["/secured"] = pathItem;

        // Act
        var json = document.ToJson(SchemaType.OpenApi3, true);

        // Assert
        await Verify(json).UseDirectory("Snapshots");
    }

    [Fact]
    public async Task Snapshot_RoundTrip_ComplexDocument()
    {
        // Arrange
        var inputJson = @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Complex API"",
    ""description"": ""A complex API for testing round-trip serialization"",
    ""version"": ""3.1.0"",
    ""contact"": {
      ""name"": ""Developer"",
      ""email"": ""dev@example.com""
    }
  },
  ""servers"": [
    {
      ""url"": ""https://api.example.com"",
      ""description"": ""Production""
    }
  ],
  ""paths"": {
    ""/items"": {
      ""get"": {
        ""tags"": [""items""],
        ""summary"": ""List items"",
        ""operationId"": ""listItems"",
        ""parameters"": [
          {
            ""name"": ""offset"",
            ""in"": ""query"",
            ""description"": ""Number of items to skip"",
            ""schema"": {
              ""type"": ""integer"",
              ""default"": 0
            }
          },
          {
            ""name"": ""limit"",
            ""in"": ""query"",
            ""description"": ""Max number of items to return"",
            ""schema"": {
              ""type"": ""integer"",
              ""default"": 20
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""A list of items"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""$ref"": ""#/components/schemas/Item""
                  }
                }
              }
            }
          },
          ""400"": {
            ""description"": ""Bad request""
          }
        }
      },
      ""post"": {
        ""tags"": [""items""],
        ""summary"": ""Create item"",
        ""operationId"": ""createItem"",
        ""requestBody"": {
          ""required"": true,
          ""content"": {
            ""application/json"": {
              ""schema"": {
                ""$ref"": ""#/components/schemas/Item""
              }
            }
          }
        },
        ""responses"": {
          ""201"": {
            ""description"": ""Item created""
          }
        }
      }
    },
    ""/items/{id}"": {
      ""get"": {
        ""tags"": [""items""],
        ""summary"": ""Get item by ID"",
        ""operationId"": ""getItem"",
        ""parameters"": [
          {
            ""name"": ""id"",
            ""in"": ""path"",
            ""required"": true,
            ""description"": ""The item ID"",
            ""schema"": {
              ""type"": ""string""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""The item"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/Item""
                }
              }
            }
          },
          ""404"": {
            ""description"": ""Item not found""
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Item"": {
        ""type"": ""object"",
        ""required"": [""name""],
        ""properties"": {
          ""id"": {
            ""type"": ""string"",
            ""description"": ""The item ID""
          },
          ""name"": {
            ""type"": ""string"",
            ""description"": ""The item name""
          },
          ""tags"": {
            ""type"": ""array"",
            ""items"": {
              ""type"": ""string""
            }
          }
        }
      }
    }
  }
}";

        // Act
        var document = await OpenApiDocument.FromJsonAsync(inputJson, null, SchemaType.OpenApi3);
        var json = document.ToJson(SchemaType.OpenApi3, true);

        // Assert
        await Verify(json).UseDirectory("Snapshots");
    }

}
