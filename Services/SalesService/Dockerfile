# Services/SalesService/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Services/SalesService/SalesService.csproj", "Services/SalesService/"]
COPY ["Shared/Shared.csproj", "Shared/"]
RUN dotnet restore "Services/SalesService/SalesService.csproj"
COPY . .
WORKDIR "/src/Services/SalesService"
RUN dotnet build "SalesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesService.dll"]