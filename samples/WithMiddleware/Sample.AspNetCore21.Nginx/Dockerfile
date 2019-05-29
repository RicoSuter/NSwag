FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["Sample.AspNetCore21.Nginx.csproj", ""]
RUN dotnet restore "Sample.AspNetCore21.Nginx.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Sample.AspNetCore21.Nginx.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Sample.AspNetCore21.Nginx.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sample.AspNetCore21.Nginx.dll"]