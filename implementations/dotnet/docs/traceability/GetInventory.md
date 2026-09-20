# Trazabilidad .NET — GetInventory

```text
Use Case:
UC-INVENTORY-001

Status:
Implemented end-to-end with an HTTP inbound adapter and the existing
In-Memory inventory outbound adapter.
```

---

## Input

```text
PlayerId
```

---

## Domain read model

`Inventory` expone:

```text
GetItems()
```

que devuelve:

```text
IReadOnlyCollection<InventoryItemQuantity>
```

El diccionario mutable interno no se expone fuera de la Entity.

---

## Inbound Port

```text
PORT-IN-INVENTORY-001
```

Representación C#:

```text
IGetInventoryUseCase
```

Implementación:

```text
GetInventoryHandler
```

---

## Outbound Port reutilizado

El caso de uso reutiliza:

```text
PORT-OUT-INVENTORY-001
    ↓
IInventoryRepository
```

Implementación concreta actual:

```text
InMemoryInventoryRepository
```

No se creó otro repositorio sólo por tratarse de una Query.

---

## HTTP Inbound Adapter

```text
ADAPTER-IN-HTTP-INVENTORY-001
```

Implementación:

```text
GetInventoryEndpoint
```

Route:

```text
GET /api/inventories/{playerId}
```

Mapping inicial:

```text
Success
    → 200 OK

InventoryNotFound
    → 404 Not Found

Guid.Empty
    → 400 Bad Request
```

Un inventario existente sin items sigue siendo un resultado exitoso:

```text
200 OK
items: []
```

---

## Runtime flow

```text
HTTP GET
    ↓
GetInventoryEndpoint
    ↓
IGetInventoryUseCase
    ↓
GetInventoryHandler
    ↓
IInventoryRepository
    ↓
InMemoryInventoryRepository
    ↓
Inventory.GetItems()
    ↓
GetInventoryResult
    ↓
HTTP response
```

---

## Command vs Query

```text
CraftItem
    Command
    obtiene Inventory
    modifica Domain state
    SaveAsync
```

```text
GetInventory
    Query
    obtiene Inventory
    lee Domain state
    NO SaveAsync
```

Ambos atraviesan las mismas fronteras arquitectónicas sin necesitar arquitecturas paralelas.

---

## Tests

```text
Domain.Tests
    protege el snapshot de Inventory

Application.Tests
    protege lectura y ausencia de SaveAsync

Http.IntegrationTests
    protege 200, inventario vacío, 404 y request inválido

ArchitectureTests
    protege las fronteras entre Core, adapters y frameworks
```

---

## Alternative persistence adapter — EF Core + SQLite

`UC-INVENTORY-001` reuses `PORT-OUT-INVENTORY-001`.

That port now has two implementations:

```text
IInventoryRepository
    ├── InMemoryInventoryRepository
    └── EfCoreInventoryRepository
```

`GetInventoryHandler` does not change when the adapter changes.

A persistence integration test verifies the use case against a real temporary SQLite database.

