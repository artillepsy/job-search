FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["JobSearch.DataScraper/JobSearch.DataScraper.csproj", "JobSearch.DataScraper/"]
COPY ["JobSearch.Data/JobSearch.Data.csproj", "JobSearch.Data/"]
RUN dotnet restore "JobSearch.DataScraper/JobSearch.DataScraper.csproj"
COPY . .
WORKDIR "/src/JobSearch.DataScraper"
RUN dotnet build "JobSearch.DataScraper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobSearch.DataScraper.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JobSearch.DataScraper.dll"]
