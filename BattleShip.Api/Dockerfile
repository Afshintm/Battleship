#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["BattleShip.Api/BattleShip.Api.csproj", "BattleShip.Api/"]
RUN dotnet restore "BattleShip.Api/BattleShip.Api.csproj"
COPY . .
WORKDIR "/src/BattleShip.Api"
RUN dotnet build "BattleShip.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BattleShip.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BattleShip.Api.dll"]