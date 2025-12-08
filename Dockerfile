# --- DOCKERFILE CHO ECO API ---

# Stage 1: Base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copy file csproj của tất cả các project liên quan
# Dựa trên hình ảnh Solution Explorer của bạn
COPY ["EcoAPI/EcoAPI.csproj", "EcoAPI/"]
COPY ["EcoBO/EcoBO.csproj", "EcoBO/"]
COPY ["EcoRepository/EcoRepository.csproj", "EcoRepository/"]
COPY ["EcoService/EcoService.csproj", "EcoService/"]

# 2. Restore dependencies
RUN dotnet restore "./EcoAPI/EcoAPI.csproj"

# 3. Copy toàn bộ source code
COPY . .
WORKDIR "/src/EcoAPI"

# 4. Build
RUN dotnet build "./EcoAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EcoAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EcoAPI.dll"]