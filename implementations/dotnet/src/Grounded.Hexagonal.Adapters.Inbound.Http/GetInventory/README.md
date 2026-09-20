# GetInventory — HTTP Inbound Adapter

```text
Architecture ID:
ADAPTER-IN-HTTP-INVENTORY-001

Use Case:
UC-INVENTORY-001
```

Esta carpeta contiene la representación HTTP del caso de uso `GetInventory`.

---

## Endpoint

```text
GET /api/inventories/{playerId}
```

El identificador llega como parte de la ruta HTTP y el adapter lo traduce a:

```text
PlayerId
    ↓
GetInventoryQuery
```

antes de invocar Application.

---

## Flujo

```text
HTTP GET
    ↓
GetInventoryEndpoint
    ↓
GetInventoryQuery
    ↓
IGetInventoryUseCase
    ↓
GetInventoryHandler
```

El endpoint no consulta directamente ningún repositorio.

---

## Respuesta exitosa

Ejemplo conceptual:

```json
{
  "playerId": "00000000-0000-0000-0000-000000000001",
  "items": [
    {
      "itemId": "00000000-0000-0000-0000-000000000002",
      "quantity": 4
    }
  ]
}
```

Los modelos:

```text
GetInventoryHttpResponse
InventoryItemHttpResponse
```

pertenecen al adapter HTTP.

No son modelos de Domain ni Application.

---

## Inventario vacío

Un inventario existente puede no contener items.

En ese caso:

```text
200 OK
items: []
```

Esto es diferente de que el inventario no exista.

---

## Traducción de resultados

```text
Application
    Success
        ↓
HTTP
    200 OK
```

```text
Application
    InventoryNotFound
        ↓
HTTP
    404 Not Found
```

```text
HTTP playerId = Guid.Empty
        ↓
400 Bad Request
```

Las decisiones `200`, `404` y `400` pertenecen a este adapter.

---

## Query

`GetInventory` es una Query:

```text
lee estado
no modifica Inventory
no llama SaveAsync
```

El hecho de entrar por HTTP no cambia esa naturaleza.

---

## Regla principal

El adapter entiende HTTP.

El caso de uso entiende la intención de consultar un inventario.

Cada responsabilidad permanece en su frontera.
