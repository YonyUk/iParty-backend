FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar toda la solución (necesario si el gateway también tiene referencias)
COPY . .

# Restaurar el proyecto del gateway
RUN dotnet restore "API_Gateaway/ApiGateaway.csproj"

# Publicar
WORKDIR "/src/API_Gateaway"
RUN dotnet publish "ApiGateaway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiGateaway.dll"]