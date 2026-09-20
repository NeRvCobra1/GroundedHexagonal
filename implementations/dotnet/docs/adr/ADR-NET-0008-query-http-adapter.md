# ADR-NET-0008 — Reuse the Existing Inventory Port for the GetInventory Query

- Status: Accepted
- Scope: .NET implementation
- Decision type: Application and inbound adapter design

---

## Context

`UC-INVENTORY-001` needs to read the current inventory for a player.

The Application layer already owns:

```text
PORT-OUT-INVENTORY-001
    ↓
IInventoryRepository
```

and that port already provides the required capability:

```text
GetByPlayerIdAsync(PlayerId)
```

Creating another repository abstraction exclusively for the query would duplicate the same external capability.

---

## Decision

`GetInventoryHandler` will reuse `IInventoryRepository`.

The HTTP adapter will expose:

```text
GET /api/inventories/{playerId}
```

through:

```text
GetInventoryEndpoint
    ↓
IGetInventoryUseCase
```

No new outbound port is introduced.

---

## Query semantics

The use case is read-only:

```text
load Inventory
read snapshot
map result
return
```

It does not call:

```text
SaveAsync
```

This behavioral difference from `CraftItem` is protected by Application tests.

---

## Empty vs missing inventory

The implementation distinguishes:

```text
existing Inventory with zero items
    → Success
    → HTTP 200 with items: []
```

from:

```text
no Inventory for PlayerId
    → InventoryNotFound
    → HTTP 404
```

An empty aggregate is still an existing aggregate.

---

## Consequences

### Positive

```text
no duplicate outbound abstraction
clear Command vs Query contrast
same persistence adapter can serve multiple use cases
HTTP remains outside Application
```

### Negative

```text
IInventoryRepository serves both read and write use cases
future read-performance needs may justify a dedicated read port or read model
```

That future optimization should be introduced only when the required capability actually differs.

---

## Important clarification

Reusing the same repository is a decision for the current capability and scale of this reference implementation.

Hexagonal Architecture does not require one repository per aggregate, nor one port per use case. Ports model capabilities required by the inside of the application.
