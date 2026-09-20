# ProcessFoodSpoilage — Application Use Case

```text
Architecture ID:
UC-SPOILAGE-001
```

Este caso de uso demuestra que una entrada al hexágono no tiene que comenzar por HTTP.

---

## Flujo conceptual

```text
Scheduler / Background Worker
          ↓
PORT-IN-SPOILAGE-001
          ↓
IProcessFoodSpoilageUseCase
          ↓
ProcessFoodSpoilageHandler
          │
          ├── IFoodRepository
          └── IClock
          ↓
Food.AdvanceSpoilage(currentTime)
```

---

## Inbound Port

```text
PORT-IN-SPOILAGE-001
    ↓
IProcessFoodSpoilageUseCase
```

El port no sabe que actualmente será invocado por `BackgroundService`.

Otro inbound adapter podría iniciar exactamente el mismo caso de uso.

---

## Outbound Clock Port

```text
PORT-OUT-CLOCK-001
    ↓
IClock
```

El Handler no consulta directamente:

```csharp
DateTimeOffset.UtcNow
```

Esto hace que el tiempo sea una dependencia explícita y reemplazable.

---

## Food Repository

Application utiliza:

```text
IFoodRepository
```

para obtener y guardar alimentos sin conocer el mecanismo de persistencia.

En esta implementación el contrato recibe el identificador auxiliar:

```text
PORT-OUT-FOOD-001
```

---

## Responsabilidades del Handler

```text
1. obtener una única hora actual desde IClock
2. cargar los alimentos
3. pedir a cada Food que avance su estado
4. guardar únicamente los que cambiaron
5. devolver conteos de la ejecución
```

El Handler NO decide cuándo un alimento se echa a perder.

Esa decisión pertenece a Domain.

---

## Resultado

```text
ProcessFoodSpoilageResult
    EvaluatedCount
    SpoiledCount
```

Sirve para observabilidad del proceso automático sin introducir conceptos de HTTP.

---

## Diferencia frente a los casos anteriores

```text
CraftItem
    HTTP → Command → modifica estado

GetInventory
    HTTP → Query → sólo lectura

ProcessFoodSpoilage
    Worker → Command automático → modifica estado
```

Los mecanismos de entrada son diferentes.

La arquitectura interna mantiene las mismas fronteras.
