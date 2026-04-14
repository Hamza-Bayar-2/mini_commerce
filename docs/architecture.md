# Mini Commerce Architecture and Development Guide

## Onion Architecture Layers

In this project, we follow the **Onion Architecture** principles to ensure a decoupled and testable codebase.

### Solution Structure
```
MicroCommerce/
├── src/
│   ├── AuthService/
│   │   ├── AuthService.API/
│   │   ├── AuthService.Application/
│   │   ├── AuthService.Domain/
│   │   └── AuthService.Infrastructure/
│   ├── ProductService/
│   │   ├── ProductService.API/
│   │   ├── ProductService.Application/
│   │   ├── ProductService.Domain/
│   │   └── ProductService.Infrastructure/
│   ├── LogService/
│   │   ├── LogService.API/
│   │   ├── LogService.Application/
│   │   ├── LogService.Domain/
│   │   └── LogService.Infrastructure/
│   └── Gateway/
│       └── Gateway.API/
├── docker-compose.yml
├── docker-compose.override.yml
└── README.md
```

### Layer Definitions

1.  **Domain (Core):**
    - Contains Entities, Value Objects, Enums, and Domain Exceptions.
    - **Dependency:** None.
2.  **Application:**
    - Contains Business Logic, Interfaces (Repositories/Services), DTOs, Mappers, and Validators.
    - **Dependency:** Reference only **Domain**.
3.  **Infrastructure:**
    - Contains External Service implementations, Persistence (Entity Framework, Dapper), and Logging.
    - **Dependency:** Reference **Application**.
4.  **Presentation (API):**
    - Contains Controllers, Middlewares, and Program.cs (DI Configuration).
    - **Dependency:** Reference **Application** (for usage) and **Infrastructure** (strictly for DI registration).

---

## Project Setup Commands

When creating a new microservice under `src/`, follow these steps:

### 1. Create Projects

```powershell
cd src/LogService
dotnet new sln -n LogService

dotnet new webapi -n LogService.API --no-openapi false
dotnet new classlib -n LogService.Application
dotnet new classlib -n LogService.Domain
dotnet new classlib -n LogService.Infrastructure

dotnet sln add LogService.API/LogService.API.csproj
dotnet sln add LogService.Application/LogService.Application.csproj
dotnet sln add LogService.Domain/LogService.Domain.csproj
dotnet sln add LogService.Infrastructure/LogService.Infrastructure.csproj
```

### 2. Establish Layer Connections

```powershell
cd src/LogService

# API references Application and Infrastructure
dotnet add LogService.API/LogService.API.csproj reference LogService.Application/LogService.Application.csproj LogService.Infrastructure/LogService.Infrastructure.csproj

# Infrastructure references Application
dotnet add LogService.Infrastructure/LogService.Infrastructure.csproj reference LogService.Application/LogService.Application.csproj

# Application references Domain
dotnet add LogService.Application/LogService.Application.csproj reference LogService.Domain/LogService.Domain.csproj
```

### 3. Add Essential NuGet Packages

For each service, you should add these base packages to follow the architecture:

```powershell
cd src/LogService

# Infrastructure: Database and Tools
dotnet add LogService.Infrastructure/LogService.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add LogService.Infrastructure/LogService.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools

# Application: Business Logic and Validation
dotnet add LogService.Application/LogService.Application.csproj package MediatR
dotnet add LogService.Application/LogService.Application.csproj package FluentValidation

# API: Security and Documentation
dotnet add LogService.API/LogService.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add LogService.API/LogService.API.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add LogService.API/LogService.API.csproj package Swashbuckle.AspNetCore
```

---

## Best Practices

- **API Layer:** Use only `Application` types in your Controllers. Use `Infrastructure` only in `Program.cs` for Dependency Injection.
- **Interfaces:** Always define your repository and service interfaces in the `Application` layer.
- **Entities:** Keep them pure in the `Domain` layer, away from any ORM-specific logic if possible.

---

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
---

## Important Rules & Gotchas

### MediatR & Transactions
- **Pipeline Behavior:** We use a `TransactionBehavior` to automate `SaveChangesAsync` and database transactions.
- **ICommand Interface:** For a command to be included in a transaction, its `IRequest` must also implement the `ICommand` (or `ICommand<T>`) interface.
- **Why?** The pipeline filters requests. If you forget to implement `ICommand`, your changes (like adding a Refresh Token to the DB) will NOT be saved because `SaveChangesAsync` won't be called automatically.
- **Rule:** Always double-check that your Command records implement the internal `ICommand` interface.
