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
    → MigrateAsync
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


---

## Development: SQLite + demo data

La configuración base sigue siendo:

```text
Persistence:Provider = InMemory
DemoData:Seed = false
```

Sin embargo, `appsettings.Development.json` cambia deliberadamente el entorno local a:

```json
{
  "Persistence": {
    "Provider": "Sqlite"
  },
  "ConnectionStrings": {
    "Grounded": "Data Source=grounded-hexagonal.db"
  },
  "DemoData": {
    "Seed": true
  }
}
```

Con el perfil `http` de `launchSettings.json`, `dotnet run` utiliza `Development`.

El arranque queda:

```text
Host.Api
    ↓
selecciona SQLite
    ↓
MigrateAsync()
    ↓
EfCoreDemoDataSeeder.SeedIfMissingAsync()
    ↓
Map endpoints
    ↓
Run
```

El seed utiliza IDs estables para que el laboratorio pueda probarse manualmente:

```text
Player
11111111-1111-1111-1111-111111111111

Mint Mace Recipe
22222222-2222-2222-2222-222222222222

Mint Shard
33333333-3333-3333-3333-333333333333

Tough Gunk
44444444-4444-4444-4444-444444444444

Flower Petal
55555555-5555-5555-5555-555555555555

Mint Mace
66666666-6666-6666-6666-666666666666
```

El inventario inicial contiene:

```text
12 Mint Shards
5 Tough Gunk
8 Flower Petals
```

y la receta consume:

```text
10 Mint Shards
5 Tough Gunk
3 Flower Petals
```

para producir:

```text
1 Mint Mace
```

El seed es idempotente: volver a iniciar el Host no restaura las cantidades originales si el inventario ya existe.

Consulta la guía:

```text
docs/guides/SQLiteDemo.md
```
