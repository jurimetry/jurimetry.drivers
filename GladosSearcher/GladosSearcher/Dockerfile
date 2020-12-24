#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GladosSearcher/GladosSearcher.csproj", "GladosSearcher/"]
COPY ["GladosSearcher.Messager/GladosSearcher.Messager.csproj", "GladosSearcher.Messager/"]
COPY ["GladosSearcher.Messager.Domain/GladosSearcher.Messager.Domain.csproj", "GladosSearcher.Messager.Domain/"]
COPY ["GladosSearcher.Service.Tjmg/GladosSearcher.Service.Tjmg.csproj", "GladosSearcher.Service.Tjmg/"]
COPY ["GladosSearcher.Domain/GladosSearcher.Domain.csproj", "GladosSearcher.Domain/"]
COPY ["GladosSearcher.Helper/GladosSearcher.Helper.csproj", "GladosSearcher.Helper/"]
RUN dotnet restore "GladosSearcher/GladosSearcher.csproj"
COPY . .
WORKDIR "/src/GladosSearcher"
RUN dotnet build "GladosSearcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GladosSearcher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GladosSearcher.dll"]