# ���������� .NET SDK ��� ������
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# �������� Shared
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Shared/Shared.csproj

# �������� NotificationService.csproj
COPY VideoService/VideoService.csproj ./VideoService/
RUN dotnet restore VideoService/VideoService.csproj

# �������� ��� (������� Shared)
COPY Shared ./Shared
COPY VideoService ./VideoService

# �������� ������
WORKDIR /App/VideoService
RUN dotnet publish -c Release -o out

# ������ runtime-���������
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/VideoService/out ./

# ����������, ��� ���� ���������� appsettings.Docker.json
COPY VideoService/appsettings.Docker.json ./

ENTRYPOINT ["dotnet", "VideoService.dll"]
