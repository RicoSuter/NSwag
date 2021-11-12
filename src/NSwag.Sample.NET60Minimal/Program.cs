using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

var app = BuildHost(args);
app.UseDeveloperExceptionPage();

app.UseOpenApi();
app.UseSwaggerUi3();

app.MapGet("/", (Func<string>)(() => "Hello World!"));
app.MapGet("/sum/{a}/{b}", (Func<int, int, int>)((a, b) => a + b));

app.Run();

public partial class Program
{
    public static WebApplication BuildHost(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApiDocument(settings =>
        {
            settings.Title = "Minimal API";
            settings.Version = "v1";
        });

        return builder.Build();
    }
}