FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["JobSearch.Server/JobSearch.Server.csproj", "JobSearch.Server/"]
COPY ["JobSearch.Data/JobSearch.Data.csproj", "JobSearch.Data/"]

RUN dotnet restore "JobSearch.Server/JobSearch.Server.csproj"

COPY . .

WORKDIR "/src/JobSearch.Server"
RUN dotnet build "JobSearch.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobSearch.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobSearch.Server.dll"]