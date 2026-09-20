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

Estas implementaciones conviven con:

```text
InMemoryInventoryRepository
InMemoryRecipeRepository
InMemoryFoodRepository
```

---

## Sustitución de adapter

Application sigue viendo exactamente los mismos ports:

```text
                 IInventoryRepository
                    ▲             ▲
                    │             │
              InMemory...     EfCore...
                                  │
                                  ▼
                                SQLite
```

Ni `CraftItemHandler` ni `GetInventoryHandler` saben cuál implementación está detrás del port.

---

## Persistencia separada del Domain

EF Core no mapea directamente las entidades del Domain.

Esta carpeta tiene modelos de persistencia propios:

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

Por lo tanto Domain no contiene:

```text
[Key]
[Table]
[Column]
DbSet
DbContext
navigation properties de EF
```

---

## Rehidratación

Leer una base de datos no significa volver a ejecutar reglas de negocio históricas.

Por ejemplo, un `Food` ya persistido como:

```text
Spoiled
```

debe reconstruirse como `Spoiled`.

Para eso Domain expone:

```text
Food.Restore(...)
```

Es una capacidad del modelo de dominio para rehidratar estado válido, no una dependencia de EF Core.

---

## SQLite y DateTimeOffset

Domain utiliza:

```text
DateTimeOffset
```

para representar `SpoilsAt`.

SQLite tiene limitaciones para ciertas operaciones con `DateTimeOffset`, por lo que este adapter persiste:

```text
SpoilsAtUtc : DateTime
```

y realiza la conversión explícita en el boundary de persistencia:

```text
Domain DateTimeOffset
        ↕
Persistence DateTime UTC
```

La decisión pertenece al adapter, no a Domain.

---

## RecipeIngredient.Position

Una `Recipe` puede contener requisitos repetidos para el mismo `ItemId`.

Por eso la tabla de ingredientes no usa:

```text
RecipeId + ItemId
```

como clave.

Utiliza:

```text
RecipeId + Position
```

Esto conserva exactamente la colección definida por Domain.

---

## DbContext

```text
GroundedDbContext
```

es `internal`.

El resto de la aplicación no necesita conocerlo.

Los repositories reciben únicamente:

```text
SqlitePersistenceOptions
```

y crean contextos de vida corta por operación.

---

## Inicialización

Este milestone utiliza deliberadamente:

```text
Database.EnsureCreatedAsync()
```

mediante:

```text
EfCoreDatabaseInitializer
```

Todavía no se introducen migrations.

La razón es educativa: adapter/mapping y evolución de schema son conceptos distintos y se incorporarán por separado.

---

## Seeder

`EfCoreDataSeeder` es una herramienta específica del adapter para cargar datos concretos.

No es un Application Port.

Application nunca necesita una operación:

```text
SeedRecipe
```

para ejecutar sus casos de uso normales.

Agregar ese método a `IRecipeRepository` solamente para facilitar tests o bootstrap deformaría el port.

---

## Regla principal

```text
Ports
    describen capacidades del Core.

EF Core + SQLite
    son una forma reemplazable de implementar esas capacidades.
```
