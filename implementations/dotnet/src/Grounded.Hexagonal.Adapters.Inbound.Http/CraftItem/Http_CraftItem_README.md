# CraftItem — HTTP Inbound Adapter

```text
Architecture ID:
ADAPTER-IN-HTTP-CRAFT-001

Use Case:
UC-CRAFT-001
```

Esta carpeta contiene la traducción HTTP del caso de uso `CraftItem`.

---

## Endpoint

```text
POST /api/crafting/items
```

Request:

```json
{
  "playerId": "GUID",
  "recipeId": "GUID"
}
```

---

## Flujo

```text
HTTP Request
      ↓
CraftItemHttpRequest
      ↓
CraftItemEndpoint
      ↓
CraftItemCommand
      ↓
ICraftItemUseCase
      ↓
Application
```

El adapter no ejecuta las reglas de crafting.

---

## Modelos de transporte

```text
CraftItemHttpRequest
CraftItemHttpResponse
```

pertenecen exclusivamente al adapter HTTP.

No se reutilizan como modelos de Application o Domain.

---

## Traducción de identificadores

HTTP utiliza:

```text
Guid
```

El adapter traduce estos valores a:

```text
PlayerId
RecipeId
```

antes de invocar Application.

Eso evita que Application reciba directamente detalles del protocolo.

---

## Traducción de resultados

Application devuelve:

```text
Success
RecipeNotFound
InventoryNotFound
InsufficientIngredients
```

El adapter decide su representación HTTP:

```text
Success
    → 200 OK

RecipeNotFound
    → 404 Not Found

InventoryNotFound
    → 404 Not Found

InsufficientIngredients
    → 409 Conflict
```

Además:

```text
Guid.Empty
    → 400 Bad Request
```

Estas correspondencias son decisiones del adapter HTTP.

No son reglas del caso de uso.

---

## Regla principal

Application expresa el significado del resultado.

El adapter HTTP expresa cómo ese significado se representa mediante HTTP.
