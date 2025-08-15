FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .
RUN dotnet restore "./PollService.API.csproj"
RUN dotnet build "./PollService.API.csproj" -c Release -o /app/build

RUN dotnet publish "./PollService.API.csproj" -c Release -o /app/publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "PollService.API.dll" ]


