FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["notification-service/NotificationService.Api/NotificationService.Api.csproj", "notification-service/NotificationService.Api/"]
COPY ["notification-service/NotificationService.Application/NotificationService.Application.csproj", "notification-service/NotificationService.Application/"]
COPY ["notification-service/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "notification-service/NotificationService.Infrastructure/"]
COPY ["notification-service/NotificationService.Domain/NotificationService.Domain.csproj", "notification-service/NotificationService.Domain/"]
COPY ["shared/Contracts/Shared.Contracts.csproj", "shared/Contracts/"]
RUN dotnet restore "notification-service/NotificationService.Api/NotificationService.Api.csproj"

COPY ["notification-service/", "notification-service/"]
COPY ["shared/", "shared/"]

WORKDIR /src/notification-service/NotificationService.Api
RUN dotnet build "NotificationService.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "NotificationService.Api.csproj" -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]
