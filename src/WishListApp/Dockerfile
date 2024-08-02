FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview-noble-chiseled-extra AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WishListApp/WishListApp.csproj", "WishListApp/"]
COPY ["WishListApp.Data/WishListApp.Data.csproj", "WishListApp.Data/"]
COPY ["WishListApp.ProductCrawling/WishListApp.ProductCrawling.csproj", "WishListApp.ProductCrawling/"]
RUN dotnet restore "./WishListApp/WishListApp.csproj"
COPY . .
WORKDIR "/src/WishListApp"
RUN dotnet build "./WishListApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WishListApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WishListApp.dll"]