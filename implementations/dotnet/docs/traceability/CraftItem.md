# Trazabilidad final .NET — CraftItem

```text
Use Case:
UC-CRAFT-001

Status:
Implemented end-to-end with In-Memory outbound adapters and HTTP inbound adapter.
```

---

## Specification → Domain

```text
RULE-CRAFT-001
    validar todos los ingredientes antes de mutar Inventory

Implemented by:
    Grounded.Hexagonal.Domain.Inventory.Inventory.Craft

Protected by:
    Grounded.Hexagonal.Domain.Tests.Inventory.InventoryTests
```

```text
RULE-CRAFT-002
    consumir ingredientes y agregar el resultado

Implemented by:
    Grounded.Hexagonal.Domain.Inventory.Inventory.Craft

Protected by:
    Grounded.Hexagonal.Domain.Tests.Inventory.InventoryTests
```

---

## Inbound Port

```text
PORT-IN-CRAFT-001
```

C# representation:

```text
ICraftItemUseCase
```

Implementation:

```text
CraftItemHandler
```

---

## Outbound Ports

```text
PORT-OUT-INVENTORY-001
    → IInventoryRepository
    → InMemoryInventoryRepository
```

```text
PORT-OUT-RECIPE-001
    → IRecipeRepository
    → InMemoryRecipeRepository
```

---

## HTTP Inbound Adapter

```text
ADAPTER-IN-HTTP-CRAFT-001
```

Implementation:

```text
CraftItemEndpoint
```

Route:

```text
POST /api/crafting/items
```

---

## Runtime flow

```text
HTTP Request
    ↓
CraftItemEndpoint
    ↓
ICraftItemUseCase
    ↓
CraftItemHandler
    ├── IRecipeRepository
    │       ↓
    │   InMemoryRecipeRepository
    │
    ├── IInventoryRepository
    │       ↓
    │   InMemoryInventoryRepository
    │
    └── Inventory.Craft
            ↓
      RULE-CRAFT-001
      RULE-CRAFT-002
```

---

## Dependency direction

Runtime calls move toward adapters when ports are invoked, but compile-time dependencies remain inverted:

```text
Application
    owns
    ↓
IInventoryRepository
IRecipeRepository
    ↑
    implements
Outbound Adapter
```

---

## Test coverage

```text
Domain.Tests
    crafting rules and atomic mutation

Application.Tests
    orchestration and expected outcomes

Persistence.IntegrationTests
    concrete In-Memory outbound adapters

Http.IntegrationTests
    real HTTP → Application flow

ArchitectureTests
    project and technology boundaries
```

---

## Technology separation

```text
Domain
    no ASP.NET Core
    no persistence technology

Application
    no ASP.NET Core
    no persistence technology

HTTP Adapter
    ASP.NET Core

Persistence Adapter
    In-Memory implementation

Host.Api
    composition
```

---

## Educational conclusion

`CraftItem` demonstrates a state-changing Command crossing the full hexagon.

The use case is implemented without making Domain or Application depend on HTTP or persistence details.

---

## Alternative persistence adapter — EF Core + SQLite

`UC-CRAFT-001` can now execute through either persistence implementation:

```text
IInventoryRepository
    → InMemoryInventoryRepository
    → EfCoreInventoryRepository

IRecipeRepository
    → InMemoryRecipeRepository
    → EfCoreRecipeRepository
```

The following Core types remain unchanged:

```text
CraftItemHandler
CraftItemCommand
CraftItemResult
Inventory
Recipe
```

`Grounded.Hexagonal.Persistence.IntegrationTests` verifies the complete CraftItem flow against a real temporary SQLite database.

