FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WishlistApp/WishlistApp.csproj", "WishlistApp/"]
COPY ["WishlistApp.ProductCrawling/WishlistApp.ProductCrawling.csproj", "WishlistApp.ProductCrawling/"]
RUN dotnet restore "./WishlistApp/./WishlistApp.csproj"
COPY . .
WORKDIR "/src/WishlistApp"
RUN dotnet build "./WishlistApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WishlistApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WishlistApp.dll"]