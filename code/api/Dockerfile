# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/IcompCare.Api/IcompCare.Api.csproj", "src/IcompCare.Api/"]
COPY ["src/IcompCare.Application/IcompCare.Application.csproj", "src/IcompCare.Application/"]
COPY ["src/IcompCare.Domain/IcompCare.Domain.csproj", "src/IcompCare.Domain/"]
COPY ["src/IcompCare.Infrastructure/IcompCare.Infrastructure.csproj", "src/IcompCare.Infrastructure/"]
RUN dotnet restore "./src/IcompCare.Api/IcompCare.Api.csproj"
COPY . .
WORKDIR "/src/src/IcompCare.Api"
RUN dotnet build "./IcompCare.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IcompCare.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IcompCare.Api.dll"]
