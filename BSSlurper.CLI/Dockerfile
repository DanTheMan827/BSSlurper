#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BSSlurper.CLI/BSSlurper.CLI.csproj", "BSSlurper.CLI/"]
COPY ["BSSlurper.Core/BSSlurper.Core.csproj", "BSSlurper.Core/"]
RUN dotnet restore "./BSSlurper.CLI/BSSlurper.CLI.csproj"
COPY . .
WORKDIR "/src/BSSlurper.CLI"
RUN dotnet build "./BSSlurper.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BSSlurper.CLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN echo '#!/bin/bash' > /app/entrypoint.sh && \
	echo '[[ ! -z "$UMASK" ]] && umask "$UMASK"' >> /app/entrypoint.sh && \
	echo 'echo "Running with umask `umask`"' >> /app/entrypoint.sh && \
	echo 'id' >> /app/entrypoint.sh && \
	echo 'exec dotnet BSSlurper.CLI.dll "$@"' >> /app/entrypoint.sh && \
	chmod +rx /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]