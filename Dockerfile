#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 4321

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["FlexSearch.Api/FlexSearch.Api.csproj", "SearchApi/"]
COPY ["FlexSearch.Core/FlexSearch.Core.csproj", "Core/"]
RUN dotnet restore "SearchApi/FlexSearch.Api.csproj"
COPY . .
WORKDIR "/src/SearchApi"
RUN dotnet build "FlexSearch.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlexSearch.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlexSearch.Api.dll"]