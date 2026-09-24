# Ejemplo mínimo — Application coordina, Domain decide

Antes de estudiar `CraftItem` completo, reduce temporalmente el problema a una sola idea:

```text
Application coordina
        ↓
Domain decide
```

Este ejemplo es **pedagógico y reducido**. No es un cuarto caso de uso del proyecto y no debe copiarse a `src/`.

---

## Problema

Imagina un item con durabilidad.

Queremos ejecutar:

```text
RepairItem
```

Application sabe que debe pedir una reparación.

Domain sabe qué significa una reparación válida.

---

## Application

Versión deliberadamente mínima:

```csharp
public sealed class RepairItemHandler
{
    public void Execute(Item item, int repairAmount)
    {
        item.Repair(repairAmount);
    }
}
```

¿Qué hace?

```text
recibe la intención
coordina la llamada
```

¿Qué no hace?

```text
no calcula la durabilidad máxima
no decide si repairAmount es válido
no implementa la regla de reparación
```

---

## Domain

```csharp
public sealed class Item
{
    public int Durability { get; private set; }

    public int MaxDurability { get; }

    public void Repair(int amount)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException();
        }

        Durability = Math.Min(
            Durability + amount,
            MaxDurability);
    }
}
```

Aquí vive la decisión:

```text
repairAmount debe ser positivo
durability nunca supera MaxDurability
```

El Handler no necesita conocer esas reglas.

---

## ¿Por qué empezar aquí?

Porque es fácil perder esta idea al ver de golpe:

```text
HTTP
DTO
interface
Handler
Repository
DbContext
Dependency Injection
SQLite
```

Todas esas piezas pueden ser necesarias en una aplicación real, pero son una segunda conversación.

Primero:

```text
Application coordina una intención
Domain protege las reglas
```

Después agregamos fronteras externas.

---

## Evolución hacia Hexagonal

Cuando el caso de uso necesita interactuar con el exterior, el dibujo crece:

```text
Inbound Adapter
      ↓
Input Port
      ↓
Application
      ↓
Domain
      ↓
Output Port
      ↓
Outbound Adapter
```

Por ejemplo, una versión completa de `RepairItem` podría necesitar:

```text
IRepairItemUseCase
IItemRepository
HTTP endpoint
EF Core adapter
```

pero la regla:

```text
Durability <= MaxDurability
```

seguiría perteneciendo a Domain.

---

## Conexión con CraftItem real

Eso es exactamente lo que ocurre conceptualmente aquí:

```text
CraftItemHandler
      ↓ coordina
Inventory.Craft(recipe)
      ↓ decide
RULE-CRAFT-001
RULE-CRAFT-002
```

El código real añade ports, adapters, persistencia y traducción HTTP alrededor de esa división.

Siguiente paso:

[`GuidedTours.md#tour-1--craftitem`](GuidedTours.md#tour-1--craftitem)
