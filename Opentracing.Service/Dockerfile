FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Opentracing.Service/Opentracing.Service.csproj Opentracing.Service/
RUN dotnet restore Opentracing.Service/Opentracing.Service.csproj
COPY . .
WORKDIR /src/Opentracing.Service
RUN dotnet build Opentracing.Service.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Opentracing.Service.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Opentracing.Service.dll"]
