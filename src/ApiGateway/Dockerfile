# Используем .NET SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Копируем Shared
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Shared/Shared.csproj

# Копируем NotificationService.csproj
COPY ApiGateway/ApiGateway.csproj ./ApiGateway/
RUN dotnet restore ApiGateway/ApiGateway.csproj

# Копируем код (включая Shared)
COPY Shared ./Shared
COPY ApiGateway ./ApiGateway

# Собираем проект
WORKDIR /App/ApiGateway
RUN dotnet publish -c Release -o out

# Создаём runtime-контейнер
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/ApiGateway/out ./

# Убеждаемся, что есть правильный appsettings.Docker.json
COPY ApiGateway/appsettings.Docker.json ./

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
