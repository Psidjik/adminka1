FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release 
WORKDIR /src

COPY . .
RUN dotnet restore "src/Gateway.Api/Gateway.Api.csproj"

COPY . .
WORKDIR /src/src/Gateway.Api
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
