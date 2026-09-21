# Grounded.Hexagonal.Host.Worker

Este proyecto es el Composition Root ejecutable para procesos iniciados en background.

---

## Caso actual

```text
UC-SPOILAGE-001
ProcessFoodSpoilage
```

Composición lógica:

```text
FoodSpoilageWorker
        ↓
IProcessFoodSpoilageUseCase
        ↓
ProcessFoodSpoilageHandler
        ├── IFoodRepository
        └── IClock
```

---

## Selección de persistencia

El Host decide qué implementación concreta conectar a:

```text
IFoodRepository
```

mediante:

```text
Persistence:Provider
```

Valores soportados:

```text
InMemory
Sqlite
```

### InMemory

```text
IFoodRepository
    → InMemoryFoodRepository
```

### Sqlite

```text
IFoodRepository
    → EfCoreFoodRepository
    → SQLite
```

Cuando SQLite está activo, el Host inicializa el schema antes de arrancar el proceso en background.

---

## Clock

Independientemente de la persistencia:

```text
IClock
    → SystemClock
```

El tiempo sigue siendo otro outbound adapter y no se mezcla con persistencia.

---

## Worker configuration

```text
FoodSpoilage:Interval
FoodSpoilage:RunImmediately
```

Persistencia:

```text
Persistence:Provider
ConnectionStrings:Grounded
```

---

## Qué NO hace

El Host no contiene:

```text
RULE-SPOILAGE-001
comparaciones de SpoilsAt
mutación de Food
queries SQL
mapeo EF Core
```

---

## Regla principal

El Host conecta e inicia.

El Worker dispara.

Application coordina.

Domain decide.
