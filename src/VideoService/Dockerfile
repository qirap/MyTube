# Используем .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Копируем Shared
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Shared/Shared.csproj

# Копируем NotificationService.csproj
COPY VideoService/VideoService.csproj ./VideoService/
RUN dotnet restore VideoService/VideoService.csproj

# Копируем код (включая Shared)
COPY Shared ./Shared
COPY VideoService ./VideoService

# Собираем проект
WORKDIR /App/VideoService
RUN dotnet publish -c Release -o out

# Создаём runtime-контейнер
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/VideoService/out ./

# Убеждаемся, что есть правильный appsettings.Docker.json
COPY VideoService/appsettings.Docker.json ./

ENTRYPOINT ["dotnet", "VideoService.dll"]
