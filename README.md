# Mini Commerce Project

A microservices-based e-commerce application built with .NET 10.
Implements `Onion Architecture`, `CQRS pattern`, `JWT authentication`,
`Redis caching`, `RabbitMQ event-driven messaging`, and `YARP API Gateway.

## Documentation

Check out the documentation of the project:
- [Project Documentation](docs/documentation.md)

## Quick Start

To run the services, you must first ensure the database is running.

### 🐳 Running the Dependencies (Docker)
Ensure Docker Desktop is running. The following commands should be executed in your **system terminal** (Command Prompt, Terminal, or PowerShell):

1. **Create and start the SQL Server instance:**
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=StrongPass123" -p 1433:1433 --name sqlserver_db -d mcr.microsoft.com/mssql/server:2022-latest
```

2. **Create and start the Redis instance for caching:**
```bash
docker run -d --name redis -p 6379:6379 redis:alpine
```

3. **Create and start the RabbitMQ instance for messaging:**
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

4. **Create the specific databases (`AuthDb`, `ProductDb` and `LogDb`):**
```bash
# Create AuthDb
docker exec -it sqlserver_db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongPass123 -C -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'AuthDb') CREATE DATABASE AuthDb"

# Create ProductDb
docker exec -it sqlserver_db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongPass123 -C -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ProductDb') CREATE DATABASE ProductDb"

# Create LogDb
docker exec -it sqlserver_db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongPass123 -C -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'LogDb') CREATE DATABASE LogDb"
```

4. **Apply Database Migrations:**
   
The following commands should be executed in the **VS Code Terminal**:

*First, generate the initial migrations (if not already done):*
```powershell
# For AuthService
dotnet ef migrations add InitialCreate -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API

# For ProductService
dotnet ef migrations add InitialCreate -p src/ProductService/ProductService.Infrastructure -s src/ProductService/ProductService.API

# For LogService
dotnet ef migrations add InitialCreate -p src/LogService/LogService.Infrastructure -s src/LogService/LogService.API
```

*Second, apply the migrations to update the databases:*
```powershell
# Update AuthDb
dotnet ef database update -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API

# Update ProductDb
dotnet ef database update -p src/ProductService/ProductService.Infrastructure -s src/ProductService/ProductService.API

# Update LogDb
dotnet ef database update -p src/LogService/LogService.Infrastructure -s src/LogService/LogService.API
```

### 🚀 Running Individual Services
Open a **VS Code Terminal** for each service and run:

```bash
# AuthService (Port: 5121)
dotnet run --project src/AuthService/AuthService.API/AuthService.API.csproj --launch-profile https

# ProductService (Port: 5056)
dotnet run --project src/ProductService/ProductService.API/ProductService.API.csproj --launch-profile https

# LogService (Port: 5243)
dotnet run --project src/LogService/LogService.API/LogService.API.csproj --launch-profile https

# Gateway (Port: 5292)
dotnet run --project src/Gateway/Gateway.API/Gateway.API.csproj --launch-profile https
```

### 🌐 API Gateway Usage
All external client requests must be routed through the API Gateway. The base URL structure is:
`https://localhost:5292/api/{controller}/{endpoint}`

*Example:* `GET https://localhost:5292/api/product`

### Accessing Swagger
Once the services are running, you can access the Swagger UI at:
- **AuthService:** [https://localhost:5121/swagger](https://localhost:5121/swagger)
- **ProductService:** [https://localhost:5056/swagger](https://localhost:5056/swagger)
- **LogService:** [https://localhost:5243/swagger](https://localhost:5243/swagger)
- **Gateway:** [https://localhost:5292/swagger](https://localhost:5292/swagger)

