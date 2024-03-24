FROM mcr.microsoft.com/dotnet/runtime:9.0-preview-jammy-chiseled AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WishListApp.Cli/WishListApp.Cli.csproj", "WishListApp.Cli/"]
COPY ["WishListApp.Data/WishListApp.Data.csproj", "WishListApp.Data/"]
RUN dotnet restore "./WishListApp.Cli/./WishListApp.Cli.csproj"
COPY . .
WORKDIR "/src/WishListApp.Cli"
RUN dotnet build "./WishListApp.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WishListApp.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .