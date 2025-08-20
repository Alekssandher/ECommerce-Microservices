#!/bin/bash
set -e

dotnet run --project Gateway/Gateway.csproj &
dotnet run --project Services/SalesService/SalesService.csproj &
dotnet run --project Services/StockService/StockService.csproj &

wait