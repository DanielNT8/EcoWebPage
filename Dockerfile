# ===========================
# STAGE 1: BUILD
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY EcoWebPage.sln ./
COPY EcoBO/EcoBO.csproj EcoBO/
COPY EcoRepository/EcoRepository.csproj EcoRepository/
COPY EcoService/EcoService.csproj EcoService/
COPY EcoAPI/EcoAPI.csproj EcoAPI/

# Restore dependencies
RUN dotnet restore EcoWebPage.sln

# Copy all source code
COPY . .

# Build and publish API project
RUN dotnet publish EcoAPI/EcoAPI.csproj -c Release -o /app/publish --no-restore

# ===========================
# STAGE 2: RUNTIME
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 (Render dùng port này)
EXPOSE 8080

# Set environment variable for ASP.NET
ENV ASPNETCORE_URLS=http://+:8080

# Start the app
ENTRYPOINT ["dotnet", "EcoAPI.dll"]
