FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY AuthECAPI.slnx ./
COPY AuthECAPI/AuthECAPI.csproj ./AuthECAPI/
RUN dotnet restore ./AuthECAPI/AuthECAPI.csproj

COPY AuthECAPI/. ./AuthECAPI/
WORKDIR /src/AuthECAPI
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "AuthECAPI.dll"]
