
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "src/CabinetBooking.Api/CabinetBooking.Api.csproj"
WORKDIR "/src/src/CabinetBooking.Api"
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CabinetBooking.Api.dll"]
