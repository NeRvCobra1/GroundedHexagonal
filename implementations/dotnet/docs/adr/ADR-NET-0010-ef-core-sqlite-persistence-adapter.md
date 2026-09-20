# ADR-NET-0010 — Add EF Core + SQLite as a Replaceable Persistence Adapter

- Status: Accepted
- Scope: .NET implementation
- Decision type: Outbound adapter technology

---

## Context

The reference implementation already contains In-Memory implementations for:

```text
PORT-OUT-INVENTORY-001
PORT-OUT-RECIPE-001
PORT-OUT-FOOD-001
```

Those adapters are useful for fast tests and educational composition, but they do not demonstrate durable relational persistence.

A central promise of the architecture is that Application can depend on ports while concrete storage technologies remain replaceable.

The project therefore needs a second implementation of the same persistence ports.

---

## Decision

Add an EF Core + SQLite implementation inside:

```text
Grounded.Hexagonal.Adapters.Outbound.Persistence
    └── EntityFrameworkCore/
```

using:

```text
Microsoft.EntityFrameworkCore.Sqlite 10.0.12
```

The existing In-Memory adapters remain available.

---

## Port implementations

```text
IInventoryRepository
    ├── InMemoryInventoryRepository
    └── EfCoreInventoryRepository

IRecipeRepository
    ├── InMemoryRecipeRepository
    └── EfCoreRecipeRepository

IFoodRepository
    ├── InMemoryFoodRepository
    └── EfCoreFoodRepository
```

No Application port changes are required.

No Handler changes are required.

---

## Persistence models are separate

EF Core does not map Domain entities directly.

The adapter owns relational persistence models:

```text
InventoryRecord
InventoryItemRecord
RecipeRecord
RecipeIngredientRecord
FoodRecord
```

and explicit mapping code.

This prevents EF Core concerns from entering Domain.

---

## Domain rehydration

Persistence must sometimes reconstruct state that was produced by earlier domain behavior.

For example, an already spoiled `Food` must be restored as `Spoiled` without replaying the spoilage transition.

Domain therefore exposes:

```text
Food.Restore(...)
```

This method is framework-independent and represents domain rehydration rather than ORM support.

---

## SQLite DateTimeOffset decision

Domain retains:

```text
DateTimeOffset
```

for `Food.SpoilsAt`.

The SQLite adapter persists the value as UTC `DateTime`:

```text
SpoilsAtUtc
```

and converts at the adapter boundary.

The persistence limitation does not change the domain model.

---

## Recipe requirement identity

A recipe may contain repeated requirements for the same item.

Therefore relational recipe ingredients are identified by:

```text
RecipeId + Position
```

rather than:

```text
RecipeId + ItemId
```

This preserves the exact domain collection.

---

## DbContext lifetime

Repositories create a short-lived `GroundedDbContext` for each operation.

The public adapter configuration surface is:

```text
SqlitePersistenceOptions
```

rather than leaking `DbContext` into Application or Domain.

---

## Database creation

This milestone uses:

```text
Database.EnsureCreatedAsync()
```

through `EfCoreDatabaseInitializer`.

EF Core migrations are intentionally deferred to a separate milestone.

The goal is to teach:

```text
adapter implementation
```

before introducing:

```text
schema evolution
```

as another concern.

---

## Integration testing

SQLite tests use a real temporary SQLite database file.

They verify:

```text
repository round trips
domain rehydration
duplicate recipe requirements
CraftItem through EF repositories
GetInventory through EF repositories
ProcessFoodSpoilage through EF repositories
```

The existing In-Memory tests remain in place.

---

## Consequences

### Positive

```text
real relational persistence is demonstrated
ports remain unchanged
handlers remain unchanged
Domain remains free of EF attributes
adapter replacement becomes observable
SQLite tests remain local and deterministic
```

### Negative

```text
persistence project now depends on EF Core
explicit mappings add code
SQLite has provider-specific limitations
EnsureCreated is not a production schema evolution strategy
```

---

## Important clarification

EF Core, SQLite, Repository implementations and persistence record classes are not Hexagonal Architecture concepts.

They are concrete outbound technology choices.

The architectural concept is the dependency through outbound ports.
