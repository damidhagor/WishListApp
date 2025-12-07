FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/WishListApp/WishListApp.csproj", "WishListApp/"]
COPY ["src/WishListApp.Data/WishListApp.Data.csproj", "WishListApp.Data/"]
COPY ["src/WishListApp.ProductCrawling/WishListApp.ProductCrawling.csproj", "WishListApp.ProductCrawling/"]
COPY ["src/WishListApp.ServiceDefaults/WishListApp.ServiceDefaults.csproj", "WishListApp.ServiceDefaults/"]
COPY ["src/SharedLibraries/src/Blazor/Shared.Blazor.Dialogs/Shared.Blazor.Dialogs.csproj", "SharedLibraries/src/Blazor/Shared.Blazor.Dialogs/"]
COPY ["src/SharedLibraries/src/Blazor/Shared.Blazor.SourceGeneration/Shared.Blazor.SourceGeneration.csproj", "SharedLibraries/src/Blazor/Shared.Blazor.SourceGeneration/"]
RUN dotnet restore "./WishListApp/WishListApp.csproj"
COPY ./src .
WORKDIR "/src/WishListApp"
RUN dotnet publish "./WishListApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WishListApp.dll"]
