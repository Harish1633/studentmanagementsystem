# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore
COPY ["StudentManagementAPI/StudentManagementAPI.csproj", "StudentManagementAPI/"]
RUN dotnet restore "StudentManagementAPI/StudentManagementAPI.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/StudentManagementAPI"
RUN dotnet build "StudentManagementAPI.csproj" -c Release -o /app/build

# ── Stage 2: Publish ─────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "StudentManagementAPI.csproj" -c Release -o /app/publish \
    /p:UseAppHost=false

# ── Stage 3: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "StudentManagementAPI.dll"]
