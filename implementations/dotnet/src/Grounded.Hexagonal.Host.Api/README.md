# Grounded.Hexagonal.Host.Api

Este proyecto es el ejecutable que inicia la aplicación HTTP.

Actúa como Composition Root para los flujos HTTP.

---

## Responsabilidad

El Host puede encargarse de:

```text
ASP.NET Core startup
Dependency Injection
configuration
logging
middleware
registrar adapters
conectar ports con implementaciones
```

---

## Composition Root

Aquí pueden conocerse simultáneamente abstracciones e implementaciones concretas.

Actualmente conecta:

```text
IInventoryRepository
    ↓
InMemoryInventoryRepository

IRecipeRepository
    ↓
InMemoryRecipeRepository

ICraftItemUseCase
    ↓
CraftItemHandler

IGetInventoryUseCase
    ↓
GetInventoryHandler
```

Application no conoce esas implementaciones concretas.

---

## Endpoints ensamblados

```text
POST /api/crafting/items
GET  /api/inventories/{playerId}
```

El Host registra los endpoints, pero la traducción HTTP vive en:

```text
Grounded.Hexagonal.Adapters.Inbound.Http
```

---

## Puede depender de

```text
Application
Inbound.Http
Outbound.Persistence
```

---

## No debe contener

```text
reglas de negocio
casos de uso
lógica de crafting
reglas de inventario
reglas de spoilage
```

---

## Regla principal

El Host ensambla la aplicación.

No es el lugar donde vive el negocio.
