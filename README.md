# Mini Commerce Project

A microservices-based e-commerce application built with **.NET 10**.
Implements `Onion Architecture`, `CQRS pattern`, `JWT authentication`,
`Redis caching`, `RabbitMQ event-driven messaging`, and `YARP API Gateway`.

## Documentation

Check out the documentation of the project:
- [Project Documentation](docs/documentation.md)

## Quick Start

### 1. Environment Configuration (`.env`)
Project root dizininde bir `.env` dosyası oluşturun ve aşağıdaki şablonu doldurun:

```env
DB_USERNAME=sqlserver_kayra_db
DB_PASSWORD=buraya_guclu_bir_sifre_yazin

JWT_SECRET=buraya_en_az_32_karakterli_gizli_anahtar_yazin
JWT_EXPIRATION=3600000
REFRESH_EXPIRATION=1209600000

REDIS_USERNAME=redis_kayra
RABBITMQ_HOST=rabbitmq_kayra
RABBITMQ_USERNAME=guest
RABBITMQ_PASSWORD=guest

ADMIN_TOKEN=buraya_admin_token_yazin
SELLER_TOKEN=buraya_seller_token_yazin
```

### 2. Database Migrations
Eğer `Migrations` klasörleri mevcut değilse, aşağıdaki komutlarla ilk migrasyonları oluşturun. Uygulama ayağa kalkarken bu migrasyonlar otomatik olarak veritabanına uygulanacaktır.

```powershell
# For AuthService
dotnet ef migrations add InitialCreate -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API
```

```powershell
# For ProductService
dotnet ef migrations add InitialCreate -p src/ProductService/ProductService.Infrastructure -s src/ProductService/ProductService.API
```

```powershell
# For LogService
dotnet ef migrations add InitialCreate -p src/LogService/LogService.Infrastructure -s src/LogService/LogService.API
```

### 3. 🐳 Running the Project (Docker Compose)
Tüm servisleri (Veritabanları, RabbitMQ, Redis, Gateway ve Microservice'ler) tek bir komutla başlatın:

```bash
docker-compose up -d --build
```

Bu komut:
- Gerekli altyapı servislerini (SQL Server, Redis, RabbitMQ) ayağa kaldırır.
- Microservice imajlarını oluşturur.
- Bağımlılıklar hazır olduğunda servisleri başlatır ve veritabanı migrasyonlarını otomatik uygular.

### 🌐 API Gateway Usage
Tüm dış istekler API Gateway üzerinden yönlendirilir. Base URL:
`http://localhost:5292/api/{service-route}/{endpoint}`

*Not: Docker içinde servisler 8080 portunda çalışır, Gateway dışarıya 5292 portundan hizmet verir.*

### Accessing Swagger
Servisler çalıştıktan sonra aşağıdaki linklerden API dökümantasyonuna erişebilirsiniz:
- **Gateway:** [http://localhost:5292/swagger](http://localhost:5292/swagger)

