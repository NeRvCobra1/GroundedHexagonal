# Anti-patterns — errores que este laboratorio intenta hacer visibles

No son reglas universales de estilo.

Son señales de que las fronteras arquitectónicas pueden estar perdiendo significado.

---

## 1. Regla de negocio dentro del Endpoint

Ejemplo problemático:

```csharp
if (inventory.Quantity < recipe.RequiredQuantity)
{
    return Results.Conflict();
}
```

Problema:

```text
HTTP está decidiendo una regla del negocio.
```

En este proyecto:

```text
CraftItemEndpoint
    traduce HTTP

CraftItemHandler
    coordina

Inventory.Craft
    protege la regla
```

---

## 2. Application depende de EF Core

Ejemplo problemático:

```csharp
public sealed class CraftItemHandler
{
    private readonly GroundedDbContext _db;
}
```

Problema:

```text
el caso de uso ya conoce el mecanismo de persistencia
```

Consecuencia:

```text
cambiar EF Core
→ obliga a tocar Application
```

En este proyecto Application depende de:

```text
IInventoryRepository
IRecipeRepository
IFoodRepository
```

---

## 3. Un port llamado como la tecnología

Ejemplo sospechoso:

```text
ISqliteInventoryRepository
IEfCoreRecipeStore
IAzureClock
```

Un port debería expresar la capacidad que necesita el core.

Mejor:

```text
IInventoryRepository
IRecipeRepository
IClock
```

La tecnología pertenece al adapter.

---

## 4. Domain como bolsa de datos

Ejemplo problemático:

```text
Inventory
    sólo getters/setters

CraftItemHandler
    contiene toda la regla de crafting
```

Eso hace que las invariantes dependan de que todos los callers recuerden aplicarlas.

En este proyecto:

```text
Inventory.Craft(...)
Food.AdvanceSpoilage(...)
```

mantienen comportamiento donde vive el estado relevante.

---

## 5. “Interface = Port”

Crear una interface no crea automáticamente una frontera arquitectónica.

```csharp
public interface IEfDatabaseHelper
```

puede seguir siendo una abstracción tecnológica.

Pregunta mejor:

> ¿Este contrato expresa una operación/capacidad del core o sólo oculta una clase concreta?

---

## 6. Adapter → Adapter como atajo

Ejemplo problemático:

```text
HTTP Endpoint
    ↓
EfCoreInventoryRepository
```

El inbound adapter estaría saltándose Application y usando directamente un outbound adapter.

En el laboratorio:

```text
Inbound Adapter
    ↓
Input Port
    ↓
Application
    ↓
Output Port
    ↓
Outbound Adapter
```

---

## 7. Usar `DateTimeOffset.UtcNow` directamente en Application

Para lógica sensible al tiempo, hacerlo directamente dificulta controlar el entorno.

En `ProcessFoodSpoilage`:

```text
Application
    ↓
IClock
    ↓
SystemClock
```

Esto hace explícita la dependencia exterior y facilita tests deterministas.

---

## 8. Confundir DI con arquitectura

Esto:

```csharp
services.AddScoped<IThing, Thing>();
```

sólo demuestra wiring.

No demuestra que:

```text
la interface esté en la capa correcta
las reglas vivan en Domain
Application sea independiente del framework
```

Dependency Injection ayuda a componer la solución, pero no sustituye el diseño de fronteras.

---

## 9. Crear una arquitectura distinta para cada trigger

HTTP y Worker pueden parecer aplicaciones diferentes.

Aquí ambos llegan al core mediante inbound ports:

```text
HTTP → Input Port
Worker → Input Port
```

La diferencia está en el adapter de entrada, no en las reglas centrales.

---

## 10. Dejar las fronteras sólo en documentación

Una regla escrita puede olvidarse.

Por eso existe:

```text
tests/Grounded.Hexagonal.ArchitectureTests
```

La intención es que dependencias prohibidas fallen automáticamente.

---

## Pregunta práctica

Cuando agregues algo nuevo, pregunta:

```text
¿Esto es una regla?
    → Domain

¿Coordina una intención?
    → Application

¿Traduce una entrada externa?
    → Inbound Adapter

¿Habla con una tecnología exterior?
    → Outbound Adapter

¿Conecta implementaciones?
    → Composition Root
```

No es una fórmula absoluta, pero es un buen detector de diseño accidental.
