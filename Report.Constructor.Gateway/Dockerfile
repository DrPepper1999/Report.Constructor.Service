ARG HP_DOCKER_REGISTRY=10.178.3.55:5000
FROM $HP_DOCKER_REGISTRY/dotnet/aspnet:7.0 AS base

WORKDIR /app

FROM $HP_DOCKER_REGISTRY/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Report.Constructor.Gateway/Report.Constructor.Gateway.csproj", "Report.Constructor.Gateway/"]
COPY ["Report.Constructor.Application/Report.Constructor.Application.csproj", "Report.Constructor.Application/"]
COPY ["Report.Constructor.Core/Report.Constructor.Core.csproj", "Report.Constructor.Core/"]
COPY ["Report.Constructor.DAL/Report.Constructor.DAL.csproj", "Report.Constructor.DAL/"]
COPY ["Report.Constructor.Infrastructure/Report.Constructor.Infrastructure.csproj", "Report.Constructor.Infrastructure/"]
COPY ["Adapters/WebServiceRestAdapter/WebServiceRestAdapter.csproj", "Adapters/WebServiceRestAdapter/"]
RUN dotnet restore "Report.Constructor.Gateway/Report.Constructor.Gateway.csproj"
COPY . .
WORKDIR "/src/Report.Constructor.Gateway"
ARG CONFIG
RUN dotnet build "Report.Constructor.Gateway.csproj" -c $CONFIG -o /app/build

FROM build AS publish
ARG CONFIG
RUN dotnet publish "Report.Constructor.Gateway.csproj" -c $CONFIG -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY openssl.conf /
ENV OPENSSL_CONF=/openssl.conf

EXPOSE 6143/tcp
ENV ASPNETCORE_URLS http://*:6143

ENV TZ=Europe/Moscow
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

ENV WEBSITE_SITE_NAME Report.Constructor
ENTRYPOINT ["dotnet", "Report.Constructor.Gateway.dll"]
