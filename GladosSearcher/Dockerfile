FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build

WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet build GladosSearcher.sln -c Release -o /app/build

FROM build AS publish

RUN dotnet publish GladosSearcher.sln -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS final

WORKDIR /app

COPY --from=publish /app/publish .

CMD ["dotnet", "GladosSearcher.dll"]