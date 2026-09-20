# Items — Domain

Esta carpeta contiene conceptos de dominio que identifican los tipos de objetos que pueden existir en el sistema.

---

## ItemId

`ItemId` es un Value Object.

Representa la identidad lógica de un tipo de item dentro del dominio.

No representa necesariamente:

```text
primary key SQL
route parameter HTTP
EF Core key
```

Esos mecanismos externos pueden mapearse posteriormente hacia este valor.

---

## Por qué existe fuera de Crafting

`ItemId` no pertenece exclusivamente a una receta.

Puede ser utilizado por:

```text
Inventory
Crafting
Food
otros conceptos futuros del dominio
```

Por eso se modela como un concepto propio del dominio en lugar de colocarlo dentro de un adapter o caso de uso.

---

## Regla principal

Los mecanismos externos pueden traducir sus identificadores hacia `ItemId`.

`ItemId` no debe adaptarse a los mecanismos externos.
