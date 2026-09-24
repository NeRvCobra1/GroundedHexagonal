# Arquitectura de la implementación .NET

Este documento conecta los conceptos de Arquitectura Hexagonal con su representación concreta en la implementación .NET.

> Regla de lectura: **concepto arquitectónico** y **mecanismo tecnológico** no son sinónimos.

---

## Tres vistas distintas del mismo sistema

Para comprender esta implementación conviene mantener separadas tres preguntas.

### 1. ¿Por dónde viaja la información?

Es la **dirección de ejecución en runtime**.

Ejemplo `CraftItem`:

```text
HTTP Request
    ↓
CraftItemEndpoint
    ↓
ICraftItemUseCase
    ↓
CraftItemHandler
    ↓
Inventory.Craft(...)
    ↓
IInventoryRepository.SaveAsync(...)
    ↓
EfCoreInventoryRepository
    ↓
SQLite
```

### 2. ¿Quién depende de quién en código?

Es la **dirección de dependencias de compilación**.

```text
Domain
  ▲
  │
Application
  ▲      ▲
  │      │
Inbound  Outbound
Adapters Adapters
  ▲      ▲
   \    /
    Hosts
```

Aunque `CraftItemHandler` termina provocando una escritura en SQLite, Application no referencia `EfCoreInventoryRepository`.

Application sólo conoce:

```text
IInventoryRepository
```

El adapter concreto implementa ese contrato desde fuera.

### 3. ¿Dónde vive físicamente cada pieza?

Es el **mapa del repositorio**.

```text
src/
├── Grounded.Hexagonal.Domain
├── Grounded.Hexagonal.Application
├── Grounded.Hexagonal.Adapters.Inbound.Http
├── Grounded.Hexagonal.Adapters.Inbound.Worker
├── Grounded.Hexagonal.Adapters.Outbound.Persistence
├── Grounded.Hexagonal.Adapters.Outbound.Time
├── Grounded.Hexagonal.Host.Api
└── Grounded.Hexagonal.Host.Worker
```

El Visualizer muestra las tres vistas por separado para evitar mezclarlas.

---

## Core

### Domain

Proyecto:

```text
Grounded.Hexagonal.Domain
```

Responsabilidad:

```text
reglas del negocio
entidades
value objects
invariantes
comportamiento del dominio
```

Ejemplos:

```text
Inventory
Recipe
Food
ItemId
PlayerId
RecipeId
```

No conoce:

```text
HTTP
ASP.NET Core
EF Core
SQLite
BackgroundService
Dependency Injection
```

Reglas representativas:

```text
RULE-CRAFT-001
validar todos los ingredientes antes de mutar el inventario

RULE-CRAFT-002
un crafting exitoso consume materiales y agrega el resultado

RULE-SPOILAGE-001
Fresh → Spoiled cuando currentTime >= SpoilsAt
```

### Application

Proyecto:

```text
Grounded.Hexagonal.Application
```

Responsabilidad:

```text
orquestar casos de uso
exponer inbound ports
definir outbound ports
traducir resultados de aplicación
coordinar Domain
```

No decide cómo funciona HTTP ni cómo persiste EF Core.

---

## Ports

### Inbound Ports

Representan operaciones que el core permite ejecutar.

```text
PORT-IN-CRAFT-001
    ICraftItemUseCase

PORT-IN-INVENTORY-001
    IGetInventoryUseCase

PORT-IN-SPOILAGE-001
    IProcessFoodSpoilageUseCase
```

En este proyecto se expresan con `interface`, pero:

```text
Port ≠ interface por definición
```

La interface es la representación elegida en C#.

### Outbound Ports

Representan capacidades exteriores que Application necesita.

```text
PORT-OUT-INVENTORY-001
    IInventoryRepository

PORT-OUT-RECIPE-001
    IRecipeRepository

PORT-OUT-FOOD-001
    IFoodRepository

PORT-OUT-CLOCK-001
    IClock
```

Esto permite que el core diga **qué necesita** sin decidir **cómo se obtiene**.

---

## Adapters

### Inbound Adapters

Traducen un trigger exterior hacia un inbound port.

```text
HTTP
  → Minimal API endpoints

Background execution
  → FoodSpoilageWorker / BackgroundService
```

El hecho de que exista un Worker demuestra que “entrada” no significa “Presentation” ni “Controller”.

Otros inbound adapters posibles en una arquitectura equivalente podrían ser:

```text
CLI
UI
scheduler
message consumer
webhook
gRPC
WebSocket
```

### Outbound Adapters

Implementan capacidades pedidas por outbound ports.

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

IClock
    └── SystemClock
```

`SystemClock` es especialmente importante porque demuestra que un outbound adapter no tiene que ser persistencia.

---

## Composition Roots

Proyectos:

```text
Grounded.Hexagonal.Host.Api
Grounded.Hexagonal.Host.Worker
```

Responsabilidades:

```text
startup
configuration
Dependency Injection
selección de adapters
framework initialization
migrations / demo startup cuando aplica
```

Aquí sí es válido conocer simultáneamente:

```text
port + implementación concreta
```

porque el Host necesita conectarlos.

Ejemplo conceptual:

```text
IInventoryRepository
        ↓ bind
EfCoreInventoryRepository
```

Application nunca realiza ese binding.

---

## Los tres casos de uso comparados

### CraftItem — Command con mutación

```text
HTTP POST
→ Inbound Adapter
→ Input Port
→ CraftItemHandler
→ Recipe + Inventory
→ Inventory.Craft
→ Output Port
→ Persistence Adapter
→ CraftItemResult
→ HTTP Response
```

Enseña:

```text
traducción de DTOs
orquestación
reglas de Domain
persistencia por ports
resultado explícito
```

### GetInventory — Query sin mutación

```text
HTTP GET
→ Inbound Adapter
→ Input Port
→ GetInventoryHandler
→ IInventoryRepository
→ Inventory.GetItems
→ GetInventoryResult
→ HTTP Response
```

No hay `SaveAsync()`.

Enseña que Command/Query es una propiedad del caso de uso, no una razón para romper las fronteras.

### ProcessFoodSpoilage — entrada por Worker

```text
Background cycle
→ FoodSpoilageWorker
→ IProcessFoodSpoilageUseCase
→ ProcessFoodSpoilageHandler
→ IClock → SystemClock
→ IFoodRepository
→ Food.AdvanceSpoilage
→ IFoodRepository.SaveAsync
```

Enseña:

```text
HTTP no es obligatorio
IClock es un outbound port
SystemClock es un outbound adapter
Domain sigue siendo dueño de la regla
```

---

## Hexagonal vs .NET

| Elemento | Arquitectura Hexagonal | Decisión concreta .NET |
|---|---:|---:|
| Input Port | Sí | `interface` con prefijo `I` |
| Output Port | Sí | `interface` tipo repository/clock |
| Inbound Adapter | Sí | Minimal API / `BackgroundService` |
| Outbound Adapter | Sí | EF Core / InMemory / SystemClock |
| Domain | Sí | clases/records C# |
| Composition Root | Conceptualmente necesario | `Program.cs` en Hosts |
| Handler | No | convención elegida para use cases |
| Repository | No como nombre obligatorio | patrón usado para ports de persistencia |
| Controller/Endpoint | No | mecanismo ASP.NET Core |
| `I...` | No | convención de interfaces C# |
| EF Core Migration | No | detalle del adapter de persistencia |

---

## Persistencia sin contaminar Domain

La implementación EF Core no mapea directamente las entidades de Domain.

Utiliza modelos de persistencia separados y mapping explícito:

```text
Domain model
    ↕ mapping
Persistence records
    ↕
EF Core / SQLite
```

Esto mantiene decisiones de schema y ORM fuera de Domain.

---

## Architecture Tests

La arquitectura no depende sólo de disciplina humana.

`Grounded.Hexagonal.ArchitectureTests` comprueba el grafo de proyectos y límites tecnológicos para detectar degradación estructural.

El objetivo es que una violación como:

```text
Application → EF Core
```

falle automáticamente durante tests en lugar de convertirse en una “convención olvidada”.

---

## Siguiente lectura

- [`../guides/HowToReadThisImplementation.md`](../guides/HowToReadThisImplementation.md)
- [`../guides/Visualizer.md`](../guides/Visualizer.md)
- [`../traceability/`](../traceability/)
- [`../adr/`](../adr/)
