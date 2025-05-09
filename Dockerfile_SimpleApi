FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY Directory.Build.props Directory.Build.props
COPY Directory.Packages.props Directory.Packages.props
COPY nuget.config nuget.config

COPY ./src/Coda.SimpleWebApi ./

RUN dotnet restore

COPY ./src .

RUN dotnet build Coda.SimpleWebApi/Coda.SimpleWebApi.csproj --configuration Release --output /app/build
RUN dotnet publish Coda.SimpleWebApi/Coda.SimpleWebApi.csproj --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

ENV ASPNETCORE_URLS=http://*:5080
EXPOSE 5080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Coda.SimpleWebApi.dll"]