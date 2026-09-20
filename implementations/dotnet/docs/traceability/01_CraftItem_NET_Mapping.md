# Trazabilidad .NET — CraftItem

Este documento conecta la especificación independiente del lenguaje del caso de uso `CraftItem` con su futura materialización en .NET/C#.

No redefine el comportamiento funcional. Su objetivo es explicar cómo los conceptos arquitectónicos serán representados en esta implementación.

---

## Caso de uso

```text
Architecture ID:
UC-CRAFT-001

Name:
CraftItem
```

El caso permite fabricar un objeto utilizando una receta y el inventario asociado al jugador.

---

## Entrada definida por la especificación

```text
PlayerId
RecipeId
```

La implementación .NET representará estos datos mediante tipos propios del núcleo en lugar de transportar conceptos HTTP hacia Application.

---

## Ejemplo educativo

La receta de referencia es:

```text
Mint Mace

Requirements:
    10 Mint Shards
    5 Tough Gunk
    3 Flower Petals
```

Esta información pertenece a la receta.

El dominio no debe contener lógica del tipo:

```text
if recipe == "Mint Mace"
```

El comportamiento de crafting debe funcionar para cualquier `Recipe` válida.

---

## Reglas principales

### RULE-CRAFT-001

Antes de fabricar, deben existir todos los ingredientes requeridos por la receta en cantidades suficientes.

La validación debe realizarse antes de modificar el inventario.

### RULE-CRAFT-002

Cuando el crafting es exitoso:

```text
1. se consumen los ingredientes requeridos;
2. se agrega al inventario el objeto producido por la receta.
```

La operación del dominio debe evitar dejar el inventario parcialmente modificado si la receta no puede fabricarse.

---

## Inbound Port

```text
PORT-IN-CRAFT-001
```

Conceptualmente representa:

```text
"ejecutar el caso de uso CraftItem"
```

En la implementación C# se representará mediante una interface de Application.

Nombre previsto:

```text
ICraftItemUseCase
```

La interface no es el concepto arquitectónico.

Es la representación C# elegida para dicho concepto.

---

## Outbound Port — Inventory

```text
PORT-OUT-INVENTORY-001
```

Application necesita una capacidad para:

```text
obtener el inventario asociado al PlayerId;
persistir el inventario actualizado.
```

Nombre C# previsto:

```text
IInventoryRepository
```

El port vivirá en Application.

Su implementación concreta vivirá en un outbound adapter.

---

## Outbound Port — Recipe

```text
PORT-OUT-RECIPE-001
```

Application necesita obtener la receta indicada por `RecipeId`.

Nombre C# previsto:

```text
IRecipeRepository
```

Application define el contrato.

El origen concreto de las recetas es responsabilidad de un outbound adapter.

---

## Domain

El dominio contendrá los conceptos necesarios para ejecutar las reglas de crafting.

Estructura prevista:

```text
Domain/
├── Crafting/
│   ├── Recipe
│   └── IngredientRequirement
│
└── Inventory/
    ├── Inventory
    ├── ItemId
    └── cantidades / stacks según evolucione el modelo
```

La estructura definitiva se introducirá únicamente cuando implementemos cada tipo.

---

## Application

Estructura prevista:

```text
Application/
│
├── Ports/
│   ├── Inbound/
│   │   └── ICraftItemUseCase
│   │
│   └── Outbound/
│       ├── IInventoryRepository
│       └── IRecipeRepository
│
└── UseCases/
    └── CraftItem/
        ├── CraftItemCommand
        ├── CraftItemResult
        └── CraftItemHandler
```

---

## Responsabilidad del Handler

`CraftItemHandler` coordinará el flujo.

Conceptualmente:

```text
CraftItemCommand
       ↓
CraftItemHandler
       │
       ├── obtiene Recipe
       │       ↓
       │   IRecipeRepository
       │
       ├── obtiene Inventory
       │       ↓
       │   IInventoryRepository
       │
       ├── delega reglas de crafting al Domain
       │
       └── persiste el Inventory actualizado
```

El Handler no debe reimplementar las reglas que pertenecen al dominio.

---

## Lo que todavía NO participa

En esta etapa no intervienen:

```text
HTTP
ASP.NET Core
Endpoint
Controller
EF Core
SQL
BackgroundService
```

Esos mecanismos se conectarán después mediante adapters.

---

## Flujo hexagonal esperado

```text
Inbound Adapter
      ↓
PORT-IN-CRAFT-001
      ↓
CraftItemHandler
      ↓
Domain
      ↓
PORT-OUT-INVENTORY-001
PORT-OUT-RECIPE-001
      ↓
Outbound Adapters
```

---

## Futura entrada HTTP

Cuando se implemente el adapter HTTP:

```text
HTTP Request
    ↓
CraftItem Endpoint
    ↓
ICraftItemUseCase
    ↓
CraftItemHandler
```

El endpoint no contendrá reglas de crafting.

---

## Trazabilidad esperada

```text
UC-CRAFT-001
    → ICraftItemUseCase
    → CraftItemHandler
    → CraftItem application tests

PORT-IN-CRAFT-001
    → ICraftItemUseCase

PORT-OUT-INVENTORY-001
    → IInventoryRepository
    → Persistence adapter

PORT-OUT-RECIPE-001
    → IRecipeRepository
    → Recipe adapter

RULE-CRAFT-001
    → Domain crafting behavior
    → Domain tests

RULE-CRAFT-002
    → Domain crafting behavior
    → Domain tests
```

---

## Regla de separación

La especificación define qué debe ocurrir.

Arquitectura Hexagonal define las fronteras.

Este documento explica cómo .NET/C# materializa esas fronteras.

Ningún nombre como `interface`, `Handler` o `Repository` debe interpretarse como requisito universal de Arquitectura Hexagonal.
