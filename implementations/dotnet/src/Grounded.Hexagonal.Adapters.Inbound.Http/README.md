# Grounded.Hexagonal.Adapters.Inbound.Http

Este proyecto contiene inbound adapters que permiten iniciar casos de uso mediante HTTP.

Su responsabilidad es traducir entre el protocolo HTTP y los inbound ports definidos por Application.

---

## Responsabilidad arquitectónica

Representa:

```text
Inbound Adapter
```

usando:

```text
ASP.NET Core Minimal APIs
```

como tecnología concreta.

Minimal APIs son una decisión de esta implementación .NET.

No forman parte de la definición de Arquitectura Hexagonal.

---

## Implementaciones actuales

```text
CraftItem/
    ADAPTER-IN-HTTP-CRAFT-001

GetInventory/
    ADAPTER-IN-HTTP-INVENTORY-001
```

Endpoints:

```text
POST /api/crafting/items
GET  /api/inventories/{playerId}
```

Los dos adapters entran por ports de Application diferentes:

```text
CraftItemEndpoint
    ↓
ICraftItemUseCase
```

```text
GetInventoryEndpoint
    ↓
IGetInventoryUseCase
```

---

## Dirección de dependencia

```text
HTTP Adapter
      ↓
Application
```

Este proyecto puede conocer:

```text
Application
ASP.NET Core
HTTP
```

No debe conocer:

```text
Persistence Adapter
SQL
EF Core
Host implementation details
```

---

## Modelos HTTP vs modelos de Application

Los adapters pueden definir modelos de transporte como:

```text
CraftItemHttpRequest
CraftItemHttpResponse
GetInventoryHttpResponse
InventoryItemHttpResponse
```

Estos se traducen hacia o desde modelos de Application como:

```text
CraftItemCommand
CraftItemResult
GetInventoryQuery
GetInventoryResult
```

La separación es deliberada.

Una modificación del contrato HTTP no debe convertir los modelos de Application en DTOs de transporte.

---

## Traducción de resultados

Application expresa significado.

HTTP expresa protocolo.

Ejemplos:

```text
CraftItemStatus.InsufficientIngredients
        ↓
409 Conflict
```

```text
GetInventoryStatus.InventoryNotFound
        ↓
404 Not Found
```

Esos códigos sólo tienen significado dentro del adapter HTTP.

---

## FrameworkReference

Este proyecto es una class library basada en:

```text
Microsoft.NET.Sdk
```

y utiliza APIs de ASP.NET Core.

Por ello referencia explícitamente:

```xml
<FrameworkReference Include="Microsoft.AspNetCore.App" />
```

Esto es configuración específica de .NET y no una decisión de Arquitectura Hexagonal.

---

## Qué no debe ocurrir

Un endpoint no debe:

```text
consultar SQL
usar DbContext
conocer InMemoryInventoryRepository
calcular ingredientes
modificar directamente Domain salvo mediante el caso de uso correspondiente
```

Su frontera termina al invocar el inbound port.

---

## Regla principal

HTTP entra por este adapter.

HTTP no entra al núcleo.
