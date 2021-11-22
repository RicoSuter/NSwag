using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(settings =>
{
    settings.Title = "Minimal API";
    settings.Version = "v1";
});

var app = builder.Build();
app.UseDeveloperExceptionPage();

app.UseOpenApi();
app.UseSwaggerUi3();

app.MapGet("/", (Func<string>)(() => "Hello World!"))
    .WithTags("General");

app.MapGet("/sum/{a}/{b}", (Func<int, int, int>)((a, b) => a + b))
    .WithName("CalculateSum")
    .WithTags("Calculator");

app.Run();