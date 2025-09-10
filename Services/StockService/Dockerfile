# Services/StockService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Services/StockService/StockService.csproj", "Services/StockService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Services/StockService/StockService.csproj"
COPY . .
WORKDIR "/src/Services/StockService"
RUN dotnet build "StockService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StockService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StockService.dll"]