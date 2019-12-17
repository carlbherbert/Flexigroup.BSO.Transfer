#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Flexigroup.BSO.Transfer/Flexigroup.BSO.Transfer.csproj", "Flexigroup.BSO.Transfer/"]
RUN dotnet restore "Flexigroup.BSO.Transfer/Flexigroup.BSO.Transfer.csproj"
COPY . .
WORKDIR "/src/Flexigroup.BSO.Transfer"
RUN dotnet build "Flexigroup.BSO.Transfer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Flexigroup.BSO.Transfer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flexigroup.BSO.Transfer.dll"]