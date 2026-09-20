# Inventory — Domain

Esta carpeta contiene el concepto de inventario y las reglas de dominio necesarias para modificar su contenido de forma válida.

---

## Inventory

`Inventory` es una Entity.

Está asociado a un `PlayerId` y mantiene cantidades de items del dominio.

No conoce:

```text
SQL
EF Core
DbContext
IInventoryRepository
HTTP
```

---

## PlayerId

`PlayerId` es un Value Object que identifica al propietario del inventario.

No representa mecanismos de autenticación.

El dominio no necesita saber cómo fue autenticado el jugador.

---

## Craft

`Inventory.Craft(Recipe)` aplica las dos reglas iniciales de crafting:

```text
RULE-CRAFT-001
    validar todos los materiales antes de modificar estado

RULE-CRAFT-002
    consumir materiales y agregar el resultado
```

---

## Atomicidad de dominio

Antes de modificar el inventario se calculan y verifican todos los requisitos.

Si alguno es insuficiente:

```text
Inventory antes
    =
Inventory después
```

y se produce:

```text
InsufficientIngredientsException
```

Esto evita un estado parcialmente modificado.

---

## Ingredientes repetidos

El algoritmo agrupa los requisitos por `ItemId`.

Por ejemplo, si una receta contuviera:

```text
2 x Item A
3 x Item A
```

el inventario debe tener al menos:

```text
5 x Item A
```

antes de comenzar la modificación.

Esto protege la regla de validación completa aunque la fuente de recetas entregue requisitos repetidos.

---

## Persistencia

La persistencia de `Inventory` no pertenece a esta carpeta.

Application definirá un outbound port y un outbound adapter implementará el mecanismo concreto de almacenamiento.

---

## Regla principal

Inventory sabe mantener su estado válido.

No sabe cómo fue cargado ni cómo será guardado.
