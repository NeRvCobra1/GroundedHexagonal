# Crafting — Domain

Esta carpeta contendrá conceptos y reglas de dominio relacionados con fabricar objetos a partir de recetas.

No contiene el caso de uso `CraftItem` completo. El caso de uso pertenece a Application.

---

## Responsabilidad

El dominio de crafting debe poder responder preguntas como:

```text
¿Qué ingredientes requiere una receta?

¿Puede un inventario satisfacer esos requisitos?

¿Qué debe ocurrir con los materiales cuando el crafting es válido?
```

sin conocer quién solicitó la operación ni por qué mecanismo llegó.

---

## Recipe

Una `Recipe` representa la definición de algo que puede fabricarse.

Conceptualmente incluye:

```text
RecipeId
resultado producido
cantidad producida
ingredientes requeridos
```

Ejemplo:

```text
Mint Mace
    10 Mint Shards
    5 Tough Gunk
    3 Flower Petals
```

La receta es dato de dominio.

No debe existir una condición hardcodeada específica para Mint Mace dentro del algoritmo de crafting.

---

## IngredientRequirement

Representa:

```text
Item requerido
Cantidad requerida
```

Debe expresar un requisito válido del dominio.

---

## RULE-CRAFT-001

Todos los requisitos deben comprobarse antes de consumir materiales.

Una receta no debe producir modificaciones parciales cuando algún ingrediente es insuficiente.

---

## RULE-CRAFT-002

Un crafting exitoso:

```text
consume materiales
        +
agrega el resultado producido
```

al inventario.

---

## Relación con Inventory

Crafting necesita interactuar con el inventario como concepto de dominio.

Eso no significa que Crafting conozca:

```text
IInventoryRepository
EF Core
SQL
DbContext
```

Esas responsabilidades pertenecen a Application o adapters.

---

## Regla principal

Esta carpeta responde a:

> ¿Cuáles son las reglas del crafting?

No responde a:

> ¿Cómo llegó la solicitud?

ni:

> ¿Cómo se persiste el inventario?
