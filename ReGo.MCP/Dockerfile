# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG RUNTIME_IDENTIFIER=linux-x64
WORKDIR /src
COPY ["ReGo.MCP/ReGo.MCP.csproj", "ReGo.MCP/"]
COPY ["ReGo.RegonApi/ReGo.RegonApi.csproj", "ReGo.RegonApi/"]
RUN dotnet restore "./ReGo.MCP/ReGo.MCP.csproj"
COPY . .
WORKDIR "/src/ReGo.MCP"
RUN dotnet build "./ReGo.MCP.csproj" -c $BUILD_CONFIGURATION -r $RUNTIME_IDENTIFIER -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReGo.MCP.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReGo.MCP.dll"]