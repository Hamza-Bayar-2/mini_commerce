# Clean Architecture, CQRS, and Unit of Work (UoW) Guidelines

This project leverages Clean Architecture principles, MediatR for CQRS (Command Query Responsibility Segregation), and the Repository + Unit of Work patterns for data access.

Whenever you generate new code (Commands, Queries, Services, Interfaces, Repositories), you MUST adhere strictly to the following architectural rules.

## 1. Separation of Concerns (CQRS)

We have explicitly separated our write (Commands) and read (Queries) operations using custom marker interfaces (e.g., `ICommand<T>` and `IQuery<T>`).

*   **Commands:** Change the state of the system (Create, Update, Delete). Must implement `ICommand<TResponse>`.
*   **Queries:** Read the state of the system but do NOT change it. Must implement `IQuery<TResponse>`.
*   **Handlers:** Every Command or Query has a corresponding `IRequestHandler` that executes the core logic.

## 2. Unit of Work (UoW) and Transaction Management

To ensure data consistency and integrity, direct calls to `SaveChangesAsync()` from Services or Repositories are strictly **FORBIDDEN**. 

*   **Change Tracker:** Whenever an entity is added, updated, or removed using a Repository, EF Core's Change Tracker simply tracks its new state (`Added`, `Modified`, `Deleted`) in memory. We do **not** persist it immediately.
*   **Unit of Work:** The `IUnitOfWork` interface is the single source of truth for committing transactions to the database. It wraps EF Core's `SaveChangesAsync`, `BeginTransactionAsync`, `CommitTransactionAsync`, and `RollbackTransactionAsync`.
*   **Automatic Commits (Pipeline Behavior):** We use a MediatR pipeline behavior called `TransactionBehavior`. It automatically wraps any incoming Command inside a database transaction:
    *   It begins a transaction via `IUnitOfWork`.
    *   If the command handler runs successfully (all validations pass, all logic succeeds), it automatically calls `CommitTransactionAsync` which calls `SaveChangesAsync`.
    *   **CRITICAL RULE:** For the `TransactionBehavior` to intercept your request, the request MUST implement the `ICommand` (or `ICommand<TResponse>`) interface. If you forget this, `SaveChangesAsync` will NOT be called, and your changes will not be persisted.
    *   **Result Pattern & Rollback:** If the handler returns a `Result<T>` with `IsSuccess = false`, the `TransactionBehavior` MUST catch this via `IResult` interface, issue a `RollbackTransactionAsync`, and return the failed result.
    *   If an exception occurs anywhere in the pipeline or handler, the behavior catches it and safely issues a `RollbackTransactionAsync`.

### What Does This Mean For You?
When you write business logic inside a Service or a Command Handler, you only deal with repositories (e.g., `await _userRepo.AddAsync(user, ct)`). You **DO NOT** manually save changes. The system automatically inserts/updates the DB safely when your handler finishes its execution without errors.

## 3. Error Handling and Result Pattern

We strictly avoid using Exceptions for flow control (e.g., throwing an exception when a user is not found). Instead, we use a uniform `Result<T>` pattern.

*   **Result<T>:** All Commands and Queries MUST return a `Result<T>` (or `Result<Unit>`).
*   **IResult Interface:** The `Result<T>` type implements the `IResult` interface to allow non-generic access to the `IsSuccess` state within pipeline behaviors.
*   **Controller Logic:** Controllers check `result.IsSuccess` and return appropriate HTTP status codes (e.g., `BadRequest`, `NotFound`, `Unauthorized`) based on the `ErrorMessage`.

## 4. Repositories and Entity Configuration

The project uses the Generic Repository Pattern and Entity Framework Core for data access.

*   **Fluent Configuration (IEntityTypeConfiguration):** Direct configuration of entities within `AppDbContext` using `onModelCreating` is strictly **FORBIDDEN**. 
    *   Every entity MUST have its own configuration class implementing `IEntityTypeConfiguration<T>` (e.g., `UserConfiguration : IEntityTypeConfiguration<User>`).
    *   Configurations MUST be placed in the `Infrastructure.Persistence.Configurations` folder.
    *   `AppDbContext` uses `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)` to automatically register these configurations.
*   **IGenericRepository<T>:** Contains only basic method definitions like `GetById`, `AddAsync`, `Update`, `Remove`. Note that these methods simply interact with `DbContext.Set<T>()` and do not execute `SaveChanges`.
*   **Specific Repositories:** Entities should have their own interfaces (e.g., `IUserRepository : IGenericRepository<User>`) and implementation classes (e.g., `UserRepository : GenericRepository<User>, IUserRepository`) if they have specific querying needs (like `GetByEmailAsync`).
*   **CancellationToken:** Every single async method traversing the application down to the repositories MUST accept and pass the `CancellationToken (ct)` to support cooperative cancellation of asynchronous operations.

## 4. Input Validations

We use **FluentValidation** integrated with MediatR behavior (`ValidationBehavior`).

*   When creating a new Command/Query that requires validation, simply create a class inheriting from `AbstractValidator<TCommand>` and define the validation rules.
*   **Result Pattern Validation:** The `ValidationBehavior` automatically intercepts the request. If validation fails, it captures all errors and returns a `Result<T>.Failure` (using reflection to call the static Failure method) instead of throwing an exception. This ensures a consistent error flow.

## Checklist for Adding a New Capability:

1.  **Define Request:** Create the Record class implementing `ICommand<TResponse>` or `IQuery<TResponse>`.
2.  **Define Validator (optional):** Create an `AbstractValidator<TCommand>` for your request if it takes user input.
3.  **Define Handler:** Create a class implementing `IRequestHandler<YourCommand, TResponse>`.
4.  **Inject Dependencies:** Inject your desired domain Repositories and external Services into the handler. Do not inject `AppDbContext` directly.
5.  **Execute Logic:** Use repository methods like `AddAsync` or `Update` to mutate state. Do **not** call arbitrary save methods.
6.  **Dependency Injection (DI):** If you create a new Service or Repository, remember to register it in `DependencyInjection.cs` (e.g., `services.AddScoped<INewRepo, NewRepo>()`) under the respective layer (Application vs Infrastructure).
7.  **DI Coding Pattern:** When injecting dependencies into Handlers or Services, you MUST use `private readonly` fields with an underscore prefix (e.g., `_userRepo`) and initialize them via a classic constructor. Do NOT use C# 12 Primary Constructors for dependency injection.
