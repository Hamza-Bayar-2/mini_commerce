# Event-Driven Architecture & Messaging Guidelines

This project uses **MassTransit** over **RabbitMQ** for event-driven communication between microservices. When adding new events, publishers, or consumers, you MUST adhere strictly to the following rules.

## 1. Shared Event Definitions

All events used for inter-service communication MUST be defined in the `Shared.Events` project to ensure consistent schemas across services.

- **Storage:** Place events in `Shared.Events/<EntityName>/<EventName>.cs`.
- **Format:** Use C# `record` types for events as they are immutable by design.
- **Naming:** Events should be named in the past tense (e.g., `ProductCreatedEvent`, `OrderPlacedEvent`).
- **Required Data:** Always include the primary key (`Guid`), essential fields for logging/display, and a timestamp.

## 2. Event Publishing (Source Service)

To publish events from a service (e.g., `ProductService`):

- **Interface:** Every service must have an `IEventPublisherService` in its `Application.Interfaces.Services` layer.
- **Implementation:** The `EventPublisherService` in the `Infrastructure.Services` layer should wrap MassTransit's `IPublishEndpoint`.
- **Workflow:**
  1. Inject `IEventPublisherService` into the Command Handler (CQRS).
  2. After a successful database operation (and before returning the result), call `await _eventPublisher.PublishAsync(new YourEvent(...), ct)`.
  3. **Null-Safety:** Ensure data fields being passed to the event are not null (use `!` and `.Value` if necessary, as Command Handlers work with `Result<T>`).

## 3. Event Consuming (Target Service)

To consume events in a service (e.g., `LogService`):

- **Consumer Class:** Place consumers in the `Infrastructure.Consumers` folder.
- **Naming:** Follow the pattern `<EventName>Consumer` (e.g., `ProductCreatedConsumer`).
- **Handling:** Implement `IConsumer<TEvent>`. The `Consume` method must perform the logic (e.g., logging to DB via Repository).
- **CRITICAL - Persistence:** Since Consumers operate outside of the MediatR `TransactionBehavior`, you MUST manually call `await repository.SaveChangesAsync()` (or use a Unit of Work) inside the `Consume` method to persist changes to the database.
- **CancellationToken:** Always pass `context.CancellationToken` to repository calls.

## 4. Infrastructure Registration (Dependency Injection)

Every new event system component must be registered in `DependencyInjection.cs`:

- **Publisher Side:**
  ```csharp
  services.AddMassTransit(x => {
      x.UsingRabbitMq((ctx, cfg) => {
          cfg.Host(...);
      });
  });
  services.AddScoped<IEventPublisherService, EventPublisherService>();
  ```
- **Consumer Side:**
  ```csharp
  services.AddMassTransit(x => {
      x.AddConsumer<YourConsumer>();
      x.UsingRabbitMq((ctx, cfg) => {
          cfg.ReceiveEndpoint("service-name-queue", e => {
              e.ConfigureConsumer<YourConsumer>(ctx);
          });
      });
  });
  ```

## Checklist for Adding a New Event:

1.  **Shared:** Create the `record` in `Shared.Events`.
2.  **Publisher:**
    - Inject `IEventPublisherService` into the relevant Command Handler.
    - Publish the event after business logic success.
3.  **Consumer:**
    - Create the Consumer class in the target service's `Infrastructure.Consumers`.
    - Inject necessary Repositories/Services into the Consumer constructor.
4.  **Register:** Update `DependencyInjection.cs` in both services to include the new components.
