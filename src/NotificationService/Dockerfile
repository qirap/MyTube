# Используем .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Копируем Shared
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Shared/Shared.csproj

# Копируем NotificationService.csproj
COPY NotificationService/NotificationService.csproj ./NotificationService/
RUN dotnet restore NotificationService/NotificationService.csproj

# Копируем код (включая Shared)
COPY Shared ./Shared
COPY NotificationService ./NotificationService

# Собираем проект
WORKDIR /App/NotificationService
RUN dotnet publish -c Release -o out

# Создаём runtime-контейнер
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/NotificationService/out ./

# Убеждаемся, что есть правильный appsettings.Docker.json
COPY NotificationService/appsettings.Docker.json ./

ENTRYPOINT ["dotnet", "NotificationService.dll"]
