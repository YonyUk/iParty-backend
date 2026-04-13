FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar toda la solución (desde la raíz)
COPY . .

# Restaurar el proyecto de API (con ruta completa)
RUN dotnet restore "Services/Users/API/UsersApi.csproj"

# Publicar
WORKDIR "/src/Services/Users/API"
RUN dotnet publish "UsersApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Copia el script de inicio y da permisos de ejecución
COPY start.sh .
RUN chmod +x start.sh

# Establece el script como punto de entrada
ENTRYPOINT ["./start.sh"]