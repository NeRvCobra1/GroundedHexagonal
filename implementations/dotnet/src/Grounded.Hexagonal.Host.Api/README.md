# Grounded.Hexagonal.Host.Api

Este proyecto es el ejecutable que inicia la aplicación HTTP.

Actúa como **Composition Root** para los flujos HTTP.

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
inicializar infraestructura seleccionada
```

---

## Composition Root

Aquí pueden conocerse simultáneamente abstracciones e implementaciones concretas.

Application no toma esta decisión.

Actualmente el Host siempre conecta:

```text
ICraftItemUseCase
    → CraftItemHandler

IGetInventoryUseCase
    → GetInventoryHandler
```

La persistencia se selecciona mediante configuración.

---

## Selección de persistencia

Configuración:

```json
{
  "Persistence": {
    "Provider": "InMemory"
  },
  "ConnectionStrings": {
    "Grounded": "Data Source=grounded-hexagonal.db"
  }
}
```

Valores soportados:

```text
InMemory
Sqlite
```

### InMemory

```text
IInventoryRepository
    → InMemoryInventoryRepository

IRecipeRepository
    → InMemoryRecipeRepository
```

### Sqlite

```text
IInventoryRepository
    → EfCoreInventoryRepository

IRecipeRepository
    → EfCoreRecipeRepository
```

Cuando `Sqlite` está activo, el Host ejecuta:

```text
EfCoreDatabaseInitializer
    → EnsureCreatedAsync
```

antes de aceptar tráfico.

---

## Por qué la elección vive aquí

El Host es el lugar donde se ensamblan piezas concretas:

```text
Port
  +
Adapter
  +
Framework startup
```

Por eso puede conocer simultáneamente:

```text
IInventoryRepository
EfCoreInventoryRepository
```

Application sólo conoce el primero.

---

## Endpoints ensamblados

```text
POST /api/crafting/items
GET  /api/inventories/{playerId}
```

La traducción HTTP vive en:

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

El Host **elige, conecta e inicia**.

No es el lugar donde vive el negocio.
