# Trazabilidad .NET — GetInventory

```text
Use Case:
UC-INVENTORY-001

Current stage:
Domain read snapshot + Application implemented.
HTTP adapter not implemented yet.
```

---

## Input

```text
PlayerId
```

---

## Inbound Port

```text
PORT-IN-INVENTORY-001
```

C# representation:

```text
IGetInventoryUseCase
```

Implementation:

```text
GetInventoryHandler
```

---

## Outbound Port

The use case reuses:

```text
PORT-OUT-INVENTORY-001
```

C# representation:

```text
IInventoryRepository
```

No additional repository abstraction is introduced.

---

## Domain read model

`Inventory` exposes:

```text
GetItems()
```

returning:

```text
IReadOnlyCollection<InventoryItemQuantity>
```

This prevents Application from depending on the Entity's mutable dictionary implementation.

---

## Application flow

```text
GetInventoryQuery
      ↓
IGetInventoryUseCase
      ↓
GetInventoryHandler
      ↓
IInventoryRepository
      ↓
Inventory.GetItems()
      ↓
GetInventoryResult
```

---

## Command vs Query comparison

```text
CraftItem
    obtains Inventory
    modifies Domain state
    SaveAsync

GetInventory
    obtains Inventory
    reads Domain state
    no SaveAsync
```

This difference is behavioral.

Both still use the same hexagonal boundaries.

---

## Next adapter

The next stage will add an HTTP inbound adapter such as:

```text
GET /api/inventories/{playerId}
```

That route is not part of Application and will be documented separately when implemented.
