# Outbound Ports — Application

Esta carpeta contiene contratos que Application necesita para interactuar con capacidades externas.

Application define qué necesita.

Los outbound adapters deciden cómo proporcionar esa capacidad.

---

## CraftItem

El caso de uso inicial necesita:

```text
PORT-OUT-INVENTORY-001
PORT-OUT-RECIPE-001
```

Representaciones previstas:

```text
IInventoryRepository
IRecipeRepository
```

---

## Inventory Port

Debe permitir a Application trabajar con el inventario asociado al jugador sin conocer dónde está almacenado.

Conceptualmente:

```text
obtener inventory por PlayerId
guardar inventory
```

---

## Recipe Port

Debe permitir obtener una receta por `RecipeId`.

Application no debe conocer si esa receta proviene de:

```text
SQL
archivo
memoria
API
configuración
```

---

## Regla principal

Un outbound port pertenece al lado que necesita la capacidad:

```text
Application
```

No al adapter que termina implementándola.


---

## ProcessFoodSpoilage

El proceso automático agrega dos capacidades externas:

```text
PORT-OUT-FOOD-001
    → IFoodRepository

PORT-OUT-CLOCK-001
    → IClock
```

`IFoodRepository` desacopla el caso de uso del almacenamiento de alimentos.

`IClock` desacopla el caso de uso del reloj del sistema.

Esto permite que Application trabaje con una hora determinista durante tests y que Domain reciba el tiempo como dato.
