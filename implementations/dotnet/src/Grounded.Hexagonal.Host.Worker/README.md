# Grounded.Hexagonal.Host.Worker

Este proyecto es el Composition Root ejecutable para procesos iniciados en background.

---

## Caso actual

```text
UC-SPOILAGE-001
ProcessFoodSpoilage
```

Composición:

```text
FoodSpoilageWorker
        ↓
IProcessFoodSpoilageUseCase
        ↓
ProcessFoodSpoilageHandler
        ├── IFoodRepository
        │       ↓
        │   InMemoryFoodRepository
        │
        └── IClock
                ↓
            SystemClock
```

---

## Responsabilidad del Host

El Host conoce implementaciones concretas porque necesita conectarlas mediante Dependency Injection.

Actualmente registra:

```text
IFoodRepository
    → InMemoryFoodRepository

IClock
    → SystemClock

IProcessFoodSpoilageUseCase
    → ProcessFoodSpoilageHandler

IHostedService
    → FoodSpoilageWorker
```

También enlaza configuración:

```text
FoodSpoilage:Interval
FoodSpoilage:RunImmediately
```

---

## Qué NO hace

El Host no contiene:

```text
RULE-SPOILAGE-001
comparaciones de SpoilsAt
mutación de Food
persistencia
```

---

## Regla principal

El Host conecta e inicia.

El Worker dispara.

Application coordina.

Domain decide.
