# ���������� .NET SDK ��� ������
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# �������� Shared
COPY Shared/Shared.csproj ./Shared/
RUN dotnet restore Shared/Shared.csproj

# �������� NotificationService.csproj
COPY ApiGateway/ApiGateway.csproj ./ApiGateway/
RUN dotnet restore ApiGateway/ApiGateway.csproj

# �������� ��� (������� Shared)
COPY Shared ./Shared
COPY ApiGateway ./ApiGateway

# �������� ������
WORKDIR /App/ApiGateway
RUN dotnet publish -c Release -o out

# ������ runtime-���������
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/ApiGateway/out ./

# ����������, ��� ���� ���������� appsettings.Docker.json
COPY ApiGateway/appsettings.Docker.json ./

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
