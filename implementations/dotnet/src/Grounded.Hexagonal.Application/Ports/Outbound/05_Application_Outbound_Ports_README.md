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
