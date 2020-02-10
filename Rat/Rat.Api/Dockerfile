#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Rat.Api/Rat.Api.csproj", "Rat.Api/"]
COPY ["Rat/Rat.csproj", "Rat/"]
RUN dotnet restore "Rat.Api/Rat.Api.csproj"
COPY . .
WORKDIR "/src/Rat.Api"
RUN dotnet build "Rat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rat.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rat.Api.dll"]
