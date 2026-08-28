FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["event-service/EventService.Api/EventService.Api.csproj", "event-service/EventService.Api/"]
COPY ["event-service/EventService.Application/EventService.Application.csproj", "event-service/EventService.Application/"]
COPY ["event-service/EventService.Infrastructure/EventService.Infrastructure.csproj", "event-service/EventService.Infrastructure/"]
COPY ["event-service/EventService.Domain/EventService.Domain.csproj", "event-service/EventService.Domain/"]
COPY ["shared/Contracts/Shared.Contracts.csproj", "shared/Contracts/"]

RUN dotnet restore "event-service/EventService.Api/EventService.Api.csproj"

COPY ["event-service/EventService.Api/", "event-service/EventService.Api/"]
COPY ["event-service/EventService.Application/", "event-service/EventService.Application/"]
COPY ["event-service/EventService.Infrastructure/", "event-service/EventService.Infrastructure/"]
COPY ["event-service/EventService.Domain/", "event-service/EventService.Domain/"]
COPY ["shared/", "shared/"]

WORKDIR /src/event-service/EventService.Api
RUN dotnet publish "EventService.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EventService.Api.dll"]
