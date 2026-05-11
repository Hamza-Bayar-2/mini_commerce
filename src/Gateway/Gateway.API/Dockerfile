# Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

# Gateway ve bağımlı projeleri kopyalayalım
COPY ["src/Gateway/Gateway.API/Gateway.API.csproj", "src/Gateway/Gateway.API/"]

RUN dotnet restore "src/Gateway/Gateway.API/Gateway.API.csproj"

# Tüm kaynak kodları kopyalayalım
COPY . .

# Projeyi derleyelim
WORKDIR "/app/src/Gateway/Gateway.API"
RUN dotnet publish "Gateway.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Gateway.API.dll"]
