# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

# Tüm projelerin .csproj dosyalarını kopyalayıp "restore" (paket yükleme) yapalım
COPY ["src/LogService/LogService.API/LogService.API.csproj", "src/LogService/LogService.API/"]
COPY ["src/LogService/LogService.Application/LogService.Application.csproj", "src/LogService/LogService.Application/"]
COPY ["src/LogService/LogService.Domain/LogService.Domain.csproj", "src/LogService/LogService.Domain/"]
COPY ["src/LogService/LogService.Infrastructure/LogService.Infrastructure.csproj", "src/LogService/LogService.Infrastructure/"]
COPY ["src/Shared/Shared.Events/Shared.Events.csproj", "src/Shared/Shared.Events/"]

RUN dotnet restore "src/LogService/LogService.API/LogService.API.csproj"

# Tüm kaynak kodları kopyalayalım
COPY . .

# Projeyi derleyelim
WORKDIR "/app/src/LogService/LogService.API"
RUN dotnet publish "LogService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "LogService.API.dll"]
