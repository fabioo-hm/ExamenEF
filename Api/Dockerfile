# ---------- Runtime base ----------
    FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
    WORKDIR /app
    
    # ---------- Build ----------
    FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
    WORKDIR /src
    
    # Copiamos primero los .csproj para aprovechar caché
    COPY Api/Api.csproj Api/
    COPY Application/Application.csproj Application/
    COPY Domain/Domain.csproj Domain/
    COPY Infrastructure/Infrastructure.csproj Infrastructure/
    
    RUN dotnet restore Api/Api.csproj
    
    # Copiamos el resto del código
    COPY . .
    
    # Compilamos
    WORKDIR /src/Api
    RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
    
    # ---------- Final ----------
    FROM base AS final
    WORKDIR /app
    COPY --from=build /app/publish .
    ENTRYPOINT ["dotnet", "Api.dll"]