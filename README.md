# Mini Commerce Project

A microservices-based e-commerce application built with .NET 10. This repository adheres to Onion Architecture principles to ensure a scalable and maintainable structure.

## Documentation

For a detailed guide on the architecture, project structure, and how to create new services, please refer to:
- [Architecture & Development Guide](docs/architecture.md)

## Current Services
The project consists of the following microservices:
- **AuthService:** Handles identity and authentication.
- **ProductService:** Manages product catalogs and stocks.
- **LogService:** Central logging service.
- **Gateway:** API Gateway for routing requests.

## Run Project

To run the services, you must first ensure the database is running.

### 🐳 Running the Database (Docker)
1. **Create and start the SQL Server instance:**
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=StrongPass123" -p 1433:1433 --name sqlserver_db -d mcr.microsoft.com/mssql/server:2022-latest
```

2. **Create the specific databases (`AuthDb` and `ProductDb`):**
```bash
# Create AuthDb
docker exec -it sqlserver_db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongPass123 -C -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'AuthDb') CREATE DATABASE AuthDb"

# Create ProductDb
docker exec -it sqlserver_db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P StrongPass123 -C -Q "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ProductDb') CREATE DATABASE ProductDb"
```

3. **Apply Database Migrations:**
Before running the services, you must create and apply the initial migrations to set up the database tables for both services.

*First, generate the initial migrations (if not already done):*
```powershell
# For AuthService
dotnet ef migrations add InitialCreate -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API

# For ProductService
dotnet ef migrations add InitialCreate -p src/ProductService/ProductService.Infrastructure -s src/ProductService/ProductService.API
```

*Second, apply the migrations to update the databases:*
```powershell
# Update AuthDb
dotnet ef database update -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API

# Update ProductDb
dotnet ef database update -p src/ProductService/ProductService.Infrastructure -s src/ProductService/ProductService.API
```

### 🚀 Running Individual Services
Open a terminal for each service and run:

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

### Accessing Swagger
Once the services are running, you can access the Swagger UI at:
- **AuthService:** [https://localhost:5121/swagger](https://localhost:5121/swagger)
- **ProductService:** [https://localhost:5056/swagger](https://localhost:5056/swagger)
- **LogService:** [https://localhost:5243/swagger](https://localhost:5243/swagger)
- **Gateway:** [https://localhost:5292/swagger](https://localhost:5292/swagger)

## Useful Commands

### 📦 NuGet Package Management
To add a package to a specific project:
```powershell
dotnet add src/AuthService/AuthService.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
```

### 🛠 Entity Framework Core Migrations
Commands must be run from the infrastructure project directory or by specifying paths.

**Add a new migration:**
```powershell
dotnet ef migrations add <MigrationName> -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API
```

**Remove the last migration (before updating DB):**
```powershell
dotnet ef migrations remove -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API
```

**Apply migrations to the database (Update Database):**
```powershell
dotnet ef database update -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API
```

**Generate SQL Script from migrations:**
```powershell
dotnet ef migrations script -p src/AuthService/AuthService.Infrastructure -s src/AuthService/AuthService.API
```

### 🧪 Testing
To run all tests in the solution:
```powershell
dotnet test
```
