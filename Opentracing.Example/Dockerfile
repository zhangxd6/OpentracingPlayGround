FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Opentracing.Example/Opentracing.Example.csproj Opentracing.Example/
COPY Opentracing.Lib/Opentracing.Lib.csproj Opentracing.Lib/
COPY Opentracing.DataAccess/Opentracing.DataAccess.csproj Opentracing.DataAccess/
RUN dotnet restore Opentracing.Example/Opentracing.Example.csproj
COPY . .
WORKDIR /src/Opentracing.Example
RUN dotnet build Opentracing.Example.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Opentracing.Example.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Opentracing.Example.dll"]
