# syntax=docker/dockerfile:1

# Build from the REPOSITORY ROOT, not from Backend/. The root .editorconfig carries
# analyzer settings the build enforces (EnforceCodeStyleInBuild), so it has to be in
# the context:
#   docker build -t clerkapi -f Backend/Dockerfile .

# ---- build ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

# Layout mirrors the repository so .editorconfig, global.json and the
# Directory.*.props files all resolve exactly as they do on a developer machine.
WORKDIR /src
COPY .editorconfig ./
COPY Backend/global.json Backend/Directory.Build.props Backend/Directory.Packages.props ./Backend/
COPY Backend/ClerkAPI/ClerkAPI.csproj ./Backend/ClerkAPI/

# Run from Backend/ so the SDK finds global.json, which it resolves by walking up
# from the working directory.
WORKDIR /src/Backend

# Restore before copying sources, so this layer stays cached until a dependency changes.
RUN dotnet restore ClerkAPI/ClerkAPI.csproj

COPY Backend/ClerkAPI/ /src/Backend/ClerkAPI/
RUN dotnet publish ClerkAPI/ClerkAPI.csproj \
    --no-restore \
    --configuration Release \
    --output /app

# ---- runtime --------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# The .NET images define a non-root "app" user. Run as it rather than as root.
USER $APP_UID

# Kestrel listens on 8080 in these images. Platforms that inject their own PORT
# map onto it; this line documents the default.
EXPOSE 8080

COPY --from=build /app .

# Configuration comes from the environment — never bake Clerk keys into a layer:
#   Clerk__SecretKey, Clerk__Authority, Clerk__AuthorizedParty
# Point your platform's health probe at /health.
ENTRYPOINT ["dotnet", "ClerkAPI.dll"]
