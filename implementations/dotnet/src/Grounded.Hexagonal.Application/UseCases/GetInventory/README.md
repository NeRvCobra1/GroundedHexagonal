# GetInventory — Application Use Case

```text
Architecture ID:
UC-INVENTORY-001
```

Este directorio contiene la implementación .NET/C# del caso de uso de consulta `GetInventory`.

---

## Objetivo educativo

`GetInventory` permite contrastar dos tipos de casos de uso:

```text
CraftItem
    Command
    modifica estado

GetInventory
    Query
    consulta estado
```

Ambos utilizan el mismo núcleo y pueden reutilizar el mismo outbound port.

---

## Input

```text
PlayerId
```

Representación de Application:

```text
GetInventoryQuery
```

---

## Inbound Port

```text
PORT-IN-INVENTORY-001
```

Representación C#:

```text
IGetInventoryUseCase
```

Implementación:

```text
GetInventoryHandler
```

---

## Outbound Port reutilizado

`GetInventory` no introduce un repositorio nuevo.

Reutiliza:

```text
PORT-OUT-INVENTORY-001
    ↓
IInventoryRepository
```

porque la capacidad requerida ya existe:

```text
obtener Inventory por PlayerId
```

---

## Flujo

```text
GetInventoryQuery
      ↓
IGetInventoryUseCase
      ↓
GetInventoryHandler
      ↓
IInventoryRepository
      ↓
Inventory
      ↓
GetItems()
      ↓
GetInventoryResult
```

---

## Query sin persistencia

El Handler:

```text
lee Inventory
mapea su contenido
devuelve resultado
```

No ejecuta:

```text
SaveAsync
```

porque el caso de uso no modifica estado.

Esto se protege mediante tests.

---

## Domain snapshot

`Inventory.GetItems()` devuelve:

```text
IReadOnlyCollection<InventoryItemQuantity>
```

El diccionario interno de la Entity no se expone.

Application recibe una fotografía del estado y la transforma a:

```text
InventoryItemResult
```

---

## Resultado

Los estados iniciales son:

```text
Success
InventoryNotFound
```

Todavía no contienen conceptos HTTP.

El futuro HTTP adapter decidirá:

```text
Success
    → 200

InventoryNotFound
    → 404
```

---

## Trazabilidad

```text
UC-INVENTORY-001
    → IGetInventoryUseCase
    → GetInventoryHandler

PORT-IN-INVENTORY-001
    → IGetInventoryUseCase

PORT-OUT-INVENTORY-001
    → IInventoryRepository
```

---

## Regla principal

Una Query puede atravesar la misma arquitectura que un Command sin necesidad de modificar estado ni crear una arquitectura paralela.
