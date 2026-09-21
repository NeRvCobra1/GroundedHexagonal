# EntityFrameworkCore — SQLite Outbound Persistence Adapter

Esta carpeta contiene una segunda implementación concreta de los outbound ports de persistencia utilizando:

```text
Entity Framework Core
SQLite
```

El objetivo educativo es demostrar que el Core no cambia cuando cambia la tecnología de almacenamiento.

---

## Implementaciones

```text
EfCoreInventoryRepository
    implements IInventoryRepository
    PORT-OUT-INVENTORY-001

EfCoreRecipeRepository
    implements IRecipeRepository
    PORT-OUT-RECIPE-001

EfCoreFoodRepository
    implements IFoodRepository
    PORT-OUT-FOOD-001
```

Estas implementaciones conviven con los adapters In-Memory.

---

## Persistencia separada del Domain

EF Core no mapea directamente las entidades del Domain.

Esta carpeta mantiene modelos de persistencia propios:

```text
InventoryRecord
InventoryItemRecord
RecipeRecord
RecipeIngredientRecord
FoodRecord
```

y un mapper explícito:

```text
DomainPersistenceMapper
```

Por lo tanto Domain sigue sin contener:

```text
[Key]
[Table]
[Column]
DbSet
DbContext
navigation properties de EF
```

---

## DbContext

```text
GroundedDbContext
```

pertenece exclusivamente al adapter de persistencia.

Es `public` únicamente porque `dotnet-ef` necesita utilizarlo mediante la infraestructura de design-time.

Sus `DbSet` permanecen `internal` y el Core no referencia este tipo.

Los repositories siguen creando contextos de vida corta por operación.

---

# Migrations

La evolución del schema se administra mediante EF Core Migrations.

```text
EntityFrameworkCore/
└── Migrations/
    ├── 20260920220000_InitialCreate.cs
    ├── 20260920220000_InitialCreate.Designer.cs
    └── GroundedDbContextModelSnapshot.cs
```

La migración inicial crea:

```text
Foods
Inventories
InventoryItems
Recipes
RecipeIngredients
__EFMigrationsHistory
```

`__EFMigrationsHistory` pertenece a EF Core y registra qué migrations ya fueron aplicadas.

---

## Runtime initialization

Cuando un Host selecciona SQLite:

```text
EfCoreDatabaseInitializer
    ↓
Database.MigrateAsync()
```

`MigrateAsync()`:

```text
crea una base vacía si es necesario
consulta __EFMigrationsHistory
aplica sólo migrations pendientes
```

Ya no se utiliza:

```text
EnsureCreatedAsync()
```

porque `EnsureCreated` evita el sistema de migrations y no es compatible con una evolución normal del schema.

---

## Design-time factory

```text
GroundedDbContextDesignTimeFactory
```

existe para las herramientas:

```text
dotnet ef ...
```

No es un Application Port.

No participa en los casos de uso runtime.

Su conexión por defecto es:

```text
Data Source=grounded-hexagonal.design.db
```

y puede sustituirse mediante:

```text
GROUNDED_SQLITE_CONNECTION_STRING
```

---

## Tool local del repositorio

La versión de `dotnet-ef` está fijada en:

```text
.config/dotnet-tools.json
```

Por eso, después de clonar el repositorio:

```powershell
dotnet tool restore
```

restaura la versión compatible con el proyecto.

---

## Comandos principales

Ejecutados desde:

```text
implementations/dotnet
```

Restaurar tool:

```powershell
dotnet tool restore
```

Listar migrations:

```powershell
dotnet ef migrations list `
  --project src/Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src/Grounded.Hexagonal.Adapters.Outbound.Persistence
```

Comprobar si el modelo cambió sin migration:

```powershell
dotnet ef migrations has-pending-model-changes `
  --project src/Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src/Grounded.Hexagonal.Adapters.Outbound.Persistence
```

Crear una migration futura:

```powershell
dotnet ef migrations add NombreDeLaMigration `
  --project src/Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src/Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --output-dir EntityFrameworkCore/Migrations
```

Aplicar migrations manualmente:

```powershell
dotnet ef database update `
  --project src/Grounded.Hexagonal.Adapters.Outbound.Persistence `
  --startup-project src/Grounded.Hexagonal.Adapters.Outbound.Persistence
```

---

## Transición desde EnsureCreated

Las bases `.db` creadas en milestones anteriores mediante `EnsureCreatedAsync()` no contienen:

```text
__EFMigrationsHistory
```

y no deben reutilizarse como si ya fueran bases migradas.

Para este laboratorio, la transición se hace una sola vez eliminando la base local anterior y dejando que `MigrateAsync()` cree una nueva.

No se elimina ninguna base automáticamente desde el código.

En un sistema real con datos valiosos se diseñaría una estrategia explícita de baseline/migración.

---

## RecipeIngredient.Position

Una `Recipe` puede contener requisitos repetidos para el mismo `ItemId`.

Por eso la clave es:

```text
RecipeId + Position
```

en vez de:

```text
RecipeId + ItemId
```

Las migrations preservan esa decisión del modelo de persistencia.

---

## SQLite y DateTimeOffset

Domain utiliza:

```text
DateTimeOffset
```

para `SpoilsAt`.

El adapter persiste:

```text
SpoilsAtUtc : DateTime
```

y convierte explícitamente en el boundary.

Esta decisión sigue perteneciendo al adapter y no a Domain.

---

## Tests

Los integration tests verifican:

```text
migration desde base vacía
tabla __EFMigrationsHistory
schema esperado
idempotencia de MigrateAsync
repository round trips
CraftItem
GetInventory
ProcessFoodSpoilage
```

---

## Regla principal

```text
Domain/Application
    definen negocio y casos de uso.

Persistence Adapter
    decide mapping, EF Core, SQLite y evolución del schema.
```


---

## Demo data idempotente

El adapter también contiene un seeder concreto para el laboratorio local:

```text
EntityFrameworkCore/DemoData/
├── EfCoreDemoDataIds.cs
└── EfCoreDemoDataSeeder.cs
```

El Host puede ejecutarlo después de `MigrateAsync()` cuando:

```text
DemoData:Seed = true
```

El seeder crea solamente los registros que todavía no existen.

En particular, **no sobrescribe un inventario demo existente**. Esto permite:

```text
seed inicial
    ↓
CraftItem modifica Inventory
    ↓
cerrar proceso
    ↓
volver a abrir
    ↓
seed detecta Inventory existente
    ↓
estado anterior se conserva
```

El seed es una comodidad concreta del adapter SQLite para desarrollo y demostración.

No es una regla de Domain ni un Application Port.
