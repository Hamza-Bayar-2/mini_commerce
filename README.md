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

To run the services, you can use the `dotnet run` command for each API project.

### Running Individual Services

Open a terminal for each service and run:

```bash
# AuthService (Port: 5121)
dotnet run --project src/AuthService/AuthService.API/AuthService.API.csproj

# ProductService (Port: 5056)
dotnet run --project src/ProductService/ProductService.API/ProductService.API.csproj

# LogService (Port: 5243)
dotnet run --project src/LogService/LogService.API/LogService.API.csproj

# Gateway (Port: 5292)
dotnet run --project src/Gateway/Gateway.API/Gateway.API.csproj
```

### Accessing Swagger
Once the services are running, you can access the Swagger UI at:
- **AuthService:** [http://localhost:5121/swagger](http://localhost:5121/swagger)
- **ProductService:** [http://localhost:5056/swagger](http://localhost:5056/swagger)
- **LogService:** [http://localhost:5243/swagger](http://localhost:5243/swagger)
- **Gateway:** [http://localhost:5292/swagger](http://localhost:5292/swagger)

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
