FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore "./VoteService.API.csproj"
RUN dotnet build "./VoteService.API.csproj" -c Release -o /app/build

RUN dotnet publish "./VoteService.API.csproj" -c Release -o /app/publish -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS run

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "dotnet", "VoteService.API.dll" ]
