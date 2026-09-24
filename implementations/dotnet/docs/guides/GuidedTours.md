# Guided Tours — sigue el flujo en código real

Estos tours complementan `docs/traceability/`.

La trazabilidad responde:

```text
Architecture ID → implementación → tests
```

Este documento responde otra pregunta:

> Si abro el código, ¿qué archivo miro después y por qué?

No necesitas abrir todos los archivos de cada proyecto.

---

# Tour 1 — CraftItem

Tipo:

```text
Command
HTTP trigger
Domain mutation
Persistence
```

Architecture ID:

```text
UC-CRAFT-001
```

## 1. Entrada HTTP

Archivo:

```text
src/Grounded.Hexagonal.Adapters.Inbound.Http/CraftItem/CraftItemEndpoint.cs
```

Métodos:

```text
MapCraftItemEndpoint(...)
HandleAsync(...)
```

Observa cómo:

```text
HTTP request
→ valida detalles de transporte
→ crea CraftItemCommand
→ llama ICraftItemUseCase
```

No busca recetas directamente.

No modifica inventario directamente.

No conoce EF Core.

## 2. Input Port

Archivo:

```text
src/Grounded.Hexagonal.Application/Ports/Inbound/ICraftItemUseCase.cs
```

Método:

```text
ExecuteAsync(CraftItemCommand, ...)
```

Pregunta:

> ¿Qué operación expone el core sin hablar de HTTP?

El port expresa `CraftItem`.

No expresa `POST /api/...`.

## 3. Application

Archivo:

```text
src/Grounded.Hexagonal.Application/UseCases/CraftItem/CraftItemHandler.cs
```

Método:

```text
ExecuteAsync(...)
```

Sigue este orden:

```text
IRecipeRepository.GetByIdAsync
        ↓
IInventoryRepository.GetByPlayerIdAsync
        ↓
Inventory.Craft(recipe)
        ↓
IInventoryRepository.SaveAsync
        ↓
CraftItemResult
```

Application coordina.

La regla de crafting no se implementa aquí.

## 4. Domain

Archivo:

```text
src/Grounded.Hexagonal.Domain/Inventory/Inventory.cs
```

Método:

```text
Craft(Recipe recipe)
```

Aquí se protegen:

```text
RULE-CRAFT-001
validar todos los requisitos antes de mutar

RULE-CRAFT-002
consumir materiales y agregar el resultado
```

Éste es el salto clave del tour:

```text
CraftItemHandler
    coordina

Inventory.Craft
    decide
```

## 5. Output Ports

Archivos:

```text
src/Grounded.Hexagonal.Application/Ports/Outbound/IRecipeRepository.cs
src/Grounded.Hexagonal.Application/Ports/Outbound/IInventoryRepository.cs
```

Pregunta:

> ¿Qué necesita Application sin saber cómo se implementará?

Los contratos hablan de recetas e inventarios.

No hablan de SQLite.

## 6. Outbound Adapters

Dos familias implementan esos ports:

```text
src/Grounded.Hexagonal.Adapters.Outbound.Persistence/InMemory/
src/Grounded.Hexagonal.Adapters.Outbound.Persistence/EntityFrameworkCore/
```

Ejemplos:

```text
InMemoryInventoryRepository
EfCoreInventoryRepository
InMemoryRecipeRepository
EfCoreRecipeRepository
```

Aquí sí aparecen detalles concretos de almacenamiento.

## 7. Composition Root

Revisa:

```text
src/Grounded.Hexagonal.Host.Api/Program.cs
src/Grounded.Hexagonal.Host.Api/Composition/PersistenceComposition.cs
```

El Host puede conocer:

```text
port + adapter concreto
```

porque su trabajo es conectarlos.

## 8. Regreso a HTTP

`CraftItemResult` regresa al `CraftItemEndpoint`, que traduce estados de Application a:

```text
200
404
409
400
```

El core no devuelve `IResult`.

---

# Tour 2 — GetInventory

Tipo:

```text
Query
HTTP trigger
Read-only
```

Architecture ID:

```text
UC-INVENTORY-001
```

## Recorrido

```text
GetInventoryEndpoint.HandleAsync
        ↓
IGetInventoryUseCase.ExecuteAsync
        ↓
GetInventoryHandler.ExecuteAsync
        ↓
IInventoryRepository.GetByPlayerIdAsync
        ↓
Inventory.GetItems
        ↓
GetInventoryResult
        ↓
HTTP response
```

Archivos principales:

```text
src/Grounded.Hexagonal.Adapters.Inbound.Http/GetInventory/GetInventoryEndpoint.cs
src/Grounded.Hexagonal.Application/Ports/Inbound/IGetInventoryUseCase.cs
src/Grounded.Hexagonal.Application/UseCases/GetInventory/GetInventoryHandler.cs
src/Grounded.Hexagonal.Application/Ports/Outbound/IInventoryRepository.cs
src/Grounded.Hexagonal.Domain/Inventory/Inventory.cs
```

La observación más importante:

```text
NO SaveAsync
```

Compara con `CraftItem`.

Ambos usan el mismo tipo de fronteras, pero uno muta estado y el otro sólo construye un resultado de lectura.

No hace falta inventar otro repositorio sólo porque sea una Query.

---

# Tour 3 — ProcessFoodSpoilage

Tipo:

```text
Background Command
Worker trigger
Domain mutation
```

Architecture ID:

```text
UC-SPOILAGE-001
```

## 1. Inbound Adapter sin HTTP

Archivo:

```text
src/Grounded.Hexagonal.Adapters.Inbound.Worker/FoodSpoilage/FoodSpoilageWorker.cs
```

Métodos:

```text
ExecuteAsync(...)
RunOnceAsync(...)
```

El Worker decide:

```text
cuándo ejecutar
cada cuánto ejecutar
```

No decide cuándo un alimento debe considerarse spoiled.

## 2. Input Port

Archivo:

```text
src/Grounded.Hexagonal.Application/Ports/Inbound/IProcessFoodSpoilageUseCase.cs
```

La entrada al core sigue siendo un port aunque no exista HTTP.

## 3. Application

Archivo:

```text
src/Grounded.Hexagonal.Application/UseCases/ProcessFoodSpoilage/ProcessFoodSpoilageHandler.cs
```

Método:

```text
ExecuteAsync(...)
```

Secuencia:

```text
IClock.UtcNow
        ↓
IFoodRepository.GetAllAsync
        ↓
Food.AdvanceSpoilage(currentTime)
        ↓
IFoodRepository.SaveAsync sólo si cambió
```

## 4. Domain

Archivo:

```text
src/Grounded.Hexagonal.Domain/Food/Food.cs
```

Método:

```text
AdvanceSpoilage(DateTimeOffset currentTime)
```

La regla es:

```text
Fresh + currentTime < SpoilsAt
    → sin cambio

Fresh + currentTime >= SpoilsAt
    → Spoiled

Spoiled
    → sin cambio
```

## 5. Un Output Port que no es persistencia

Archivo:

```text
src/Grounded.Hexagonal.Application/Ports/Outbound/IClock.cs
```

Adapter:

```text
Grounded.Hexagonal.Adapters.Outbound.Time/SystemClock
```

Ésta es una pieza especialmente útil:

```text
Outbound Adapter != Database Adapter
```

El reloj del sistema también es una dependencia exterior.

---

# Qué comparar al terminar

Haz una tabla mental:

| Pregunta | CraftItem | GetInventory | ProcessFoodSpoilage |
|---|---|---|---|
| ¿Quién inicia? | HTTP POST | HTTP GET | Worker |
| ¿Muta Domain? | Sí | No | Sí |
| ¿Guarda? | Sí | No | Sólo cambios |
| ¿Usa reloj externo? | No | No | Sí |
| ¿Necesita HTTP? | Sí como adapter actual | Sí como adapter actual | No |
| ¿El core conoce el trigger? | No | No | No |

Después usa el Visualizer para recorrer los mismos saltos sin buscar manualmente los archivos.

Trazabilidad detallada:

```text
docs/traceability/CraftItem.md
docs/traceability/GetInventory.md
docs/traceability/ProcessFoodSpoilage.md
```
