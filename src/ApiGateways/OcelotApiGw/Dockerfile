FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "./OcelotApiGw.csproj"
RUN dotnet build "./OcelotApiGw.csproj" -c Release -o /app/build

RUN dotnet publish "./OcelotApiGw.csproj" -c Release -o /app/publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "OcelotApiGw.dll" ]
