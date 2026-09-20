# CraftItem — Application Use Case

```text
Architecture ID:
UC-CRAFT-001
```

Este directorio contiene la materialización .NET/C# del caso de uso `CraftItem`.

---

## Responsabilidad

Application coordina el caso.

Domain aplica las reglas de crafting.

Los outbound ports proporcionan las capacidades externas necesarias.

```text
CraftItemCommand
      ↓
ICraftItemUseCase
      ↓
CraftItemHandler
      │
      ├── IRecipeRepository
      ├── IInventoryRepository
      └── Inventory.Craft(Recipe)
      ↓
CraftItemResult
```

---

## Input

La especificación define:

```text
PlayerId
RecipeId
```

La representación de Application es:

```text
CraftItemCommand
```

`CraftItemCommand` no es un HTTP request DTO.

No contiene:

```text
route
headers
status code
JSON concerns
```

---

## Inbound Port

```text
PORT-IN-CRAFT-001
```

Representación C#:

```text
ICraftItemUseCase
```

Implementación inicial:

```text
CraftItemHandler
```

---

## Outbound Ports

### Inventory

```text
PORT-OUT-INVENTORY-001
```

Representación C#:

```text
IInventoryRepository
```

Permite:

```text
obtener el Inventory de un PlayerId
persistir el Inventory actualizado
```

Application no conoce el mecanismo real de almacenamiento.

### Recipe

```text
PORT-OUT-RECIPE-001
```

Representación C#:

```text
IRecipeRepository
```

Permite obtener una `Recipe` por `RecipeId`.

Application no conoce dónde están almacenadas las recetas.

---

## Handler

`CraftItemHandler` implementa la orquestación de `UC-CRAFT-001`.

Secuencia actual:

```text
1. consultar Recipe
2. si no existe → RecipeNotFound
3. consultar Inventory
4. si no existe → InventoryNotFound
5. ejecutar Inventory.Craft(recipe)
6. si Domain reporta materiales insuficientes
      → InsufficientIngredients
7. guardar Inventory
8. devolver Success
```

---

## Separación Application / Domain

El Handler NO contiene:

```text
10 Mint Shards
5 Tough Gunk
3 Flower Petals
```

Tampoco contiene el algoritmo que consume materiales.

Eso pertenece a:

```text
Recipe
Inventory
Domain Rules
```

El Handler únicamente coordina esas piezas.

---

## Resultado de Application

`CraftItemResult` expresa estados propios del caso de uso:

```text
Success
RecipeNotFound
InventoryNotFound
InsufficientIngredients
```

No expresa:

```text
200 OK
404 Not Found
409 Conflict
```

porque esas decisiones pertenecen al adapter que utilice un protocolo concreto.

Por ejemplo, el futuro adapter HTTP podrá decidir cómo mapear cada estado.

---

## Errores esperables vs. errores inesperados

El Handler transforma:

```text
InsufficientIngredientsException
```

en un resultado esperado de Application:

```text
CraftItemStatus.InsufficientIngredients
```

Pero no captura indiscriminadamente todas las excepciones.

Un fallo inesperado de infraestructura o programación debe seguir siendo visible para los mecanismos externos de manejo de errores.

---

## Tests

`Grounded.Hexagonal.Application.Tests` utiliza fakes hechos a mano para comprobar el caso sin infraestructura real.

Se verifican:

```text
RecipeNotFound
InventoryNotFound
InsufficientIngredients
Success
persistencia sólo después de éxito
delegación del comportamiento de crafting al Domain
```

No se levanta:

```text
HTTP server
database
EF Core
```

---

## Trazabilidad

```text
UC-CRAFT-001
    → ICraftItemUseCase
    → CraftItemHandler

PORT-IN-CRAFT-001
    → ICraftItemUseCase

PORT-OUT-INVENTORY-001
    → IInventoryRepository

PORT-OUT-RECIPE-001
    → IRecipeRepository

RULE-CRAFT-001
RULE-CRAFT-002
    → Inventory.Craft
```

---

## Regla principal

Application sabe **qué necesita hacer para cumplir el caso de uso**.

No sabe **cómo HTTP recibió la solicitud** ni **cómo una base de datos guarda el resultado**.
