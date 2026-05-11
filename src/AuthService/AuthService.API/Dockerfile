# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

# Tüm projelerin .csproj dosyalarını kopyalayıp "restore" (paket yükleme) yapalım
COPY ["src/AuthService/AuthService.API/AuthService.API.csproj", "src/AuthService/AuthService.API/"]
COPY ["src/AuthService/AuthService.Application/AuthService.Application.csproj", "src/AuthService/AuthService.Application/"]
COPY ["src/AuthService/AuthService.Domain/AuthService.Domain.csproj", "src/AuthService/AuthService.Domain/"]
COPY ["src/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "src/AuthService/AuthService.Infrastructure/"]
COPY ["src/Shared/Shared.Events/Shared.Events.csproj", "src/Shared/Shared.Events/"]

RUN dotnet restore "src/AuthService/AuthService.API/AuthService.API.csproj"

# Tüm kaynak kodları kopyalayalım
COPY . .

# Projeyi derleyelim
WORKDIR "/app/src/AuthService/AuthService.API"
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

# ASP.NET Core varsayılan olarak 8080 portunda çalışır
ENTRYPOINT ["dotnet", "AuthService.API.dll"]
