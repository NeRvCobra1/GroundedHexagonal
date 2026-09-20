# Trazabilidad .NET — ProcessFoodSpoilage

```text
Use Case:
UC-SPOILAGE-001

Inbound Port:
PORT-IN-SPOILAGE-001

Clock Port:
PORT-OUT-CLOCK-001

Supporting Food Port:
PORT-OUT-FOOD-001

Domain Rule:
RULE-SPOILAGE-001
```

---

## Runtime flow

```text
BackgroundService timer
        ↓
FoodSpoilageWorker
        ↓
IProcessFoodSpoilageUseCase
        ↓
ProcessFoodSpoilageHandler
        │
        ├── IClock
        │      ↓
        │  SystemClock
        │
        └── IFoodRepository
               ↓
          InMemoryFoodRepository
               ↓
              Food
               ↓
      AdvanceSpoilage(currentTime)
```

---

## Domain

```text
RULE-SPOILAGE-001
```

Implementation:

```text
Food.AdvanceSpoilage(DateTimeOffset currentTime)
```

Behavior:

```text
Fresh before SpoilsAt
    → no change

Fresh at/after SpoilsAt
    → Spoiled

already Spoiled
    → no change
```

---

## Application

```text
PORT-IN-SPOILAGE-001
    → IProcessFoodSpoilageUseCase
    → ProcessFoodSpoilageHandler
```

The Handler:

```text
gets current time once
loads food
delegates rule to Domain
saves only changed entities
returns execution counts
```

---

## Outbound adapters

Food storage:

```text
IFoodRepository
    → InMemoryFoodRepository
```

Time:

```text
PORT-OUT-CLOCK-001
IClock
    → SystemClock
```

---

## Inbound adapter

```text
FoodSpoilageWorker
```

Technology:

```text
Microsoft.Extensions.Hosting.BackgroundService
```

This is a .NET implementation choice, not a Hexagonal Architecture primitive.

---

## Host

```text
Grounded.Hexagonal.Host.Worker
```

Composition Root wiring:

```text
FoodSpoilageWorker
ProcessFoodSpoilageHandler
InMemoryFoodRepository
SystemClock
```

---

## Tests

```text
Domain.Tests
    RULE-SPOILAGE-001

Application.Tests
    orchestration and save-only-on-change

Persistence.IntegrationTests
    InMemoryFoodRepository

Worker.IntegrationTests
    Worker → Application → Domain → Persistence

ArchitectureTests
    worker/time adapter dependency boundaries
```

---

## Main educational point

`ProcessFoodSpoilage` reaches the same hexagonal core without an HTTP request.

```text
Inbound
```

means:

```text
something initiating a use case
```

not:

```text
Controller
Presentation
HTTP
```


---

## Specification synchronization note

Este ZIP contiene únicamente `implementations/dotnet`, no la carpeta común `specification/`.

Por eso este milestone materializa explícitamente dos detalles que deben mantenerse iguales en las demás implementaciones:

```text
RULE-SPOILAGE-001
    Fresh cambia a Spoiled cuando currentTime >= SpoilsAt.

PORT-OUT-FOOD-001
    capacidad para obtener y persistir Food.
```

Antes de implementar otro lenguaje, estos mismos conceptos deben existir o confirmarse en la especificación independiente del lenguaje para conservar la trazabilidad común.

---

## Alternative persistence adapter — EF Core + SQLite

`PORT-OUT-FOOD-001` now has:

```text
IFoodRepository
    ├── InMemoryFoodRepository
    └── EfCoreFoodRepository
```

Persisted `Food` state is reconstructed through:

```text
Food.Restore(...)
```

so an already spoiled entity is rehydrated without replaying `RULE-SPOILAGE-001`.

A persistence integration test verifies the complete use case against SQLite while `IClock` remains independently replaceable.

