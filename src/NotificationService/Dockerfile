# ���������� .NET SDK ��� ������
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# �������� Shared
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Shared/Shared.csproj

# �������� NotificationService.csproj
COPY NotificationService/NotificationService.csproj ./NotificationService/
RUN dotnet restore NotificationService/NotificationService.csproj

# �������� ��� (������� Shared)
COPY Shared ./Shared
COPY NotificationService ./NotificationService

# �������� ������
WORKDIR /App/NotificationService
RUN dotnet publish -c Release -o out

# ������ runtime-���������
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/NotificationService/out ./

# ����������, ��� ���� ���������� appsettings.Docker.json
COPY NotificationService/appsettings.Docker.json ./

ENTRYPOINT ["dotnet", "NotificationService.dll"]
