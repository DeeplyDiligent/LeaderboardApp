#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install sqlite3
# RUN apt-get update && apt-get install -y sqlite3

# Install curl - for testing and debugging 
RUN apt-get update && apt-get install -y curl

# Install net-tools - for testing and debugging 
RUN apt-get update && apt-get install -y net-tools

# Install Azure CLI
RUN curl -sL https://aka.ms/InstallAzureCLIDeb | bash

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LeaderboardApp.csproj", "."]
RUN dotnet restore "./LeaderboardApp.csproj"
# Copy the rest of the backend application code
COPY . .
RUN dotnet build "./LeaderboardApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LeaderboardApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
# Copy certificate file into the image

USER root
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*

COPY --chmod=755 setup_db.sh /scripts/setup_db.sh

ENTRYPOINT ["/bin/bash", "-c", "/scripts/setup_db.sh && exec dotnet /app/LeaderboardApp.dll"]