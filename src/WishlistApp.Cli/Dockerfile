FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WishlistApp.Cli/WishlistApp.Cli.csproj", "WishlistApp.Cli/"]
COPY ["WishlistApp.Data/WishlistApp.Data.csproj", "WishlistApp.Data/"]
RUN dotnet restore "./WishlistApp.Cli/./WishlistApp.Cli.csproj"
COPY . .
WORKDIR "/src/WishlistApp.Cli"
RUN dotnet build "./WishlistApp.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WishlistApp.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WishlistApp.Cli.dll"]