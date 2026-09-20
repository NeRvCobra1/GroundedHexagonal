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
```

Endpoint:

```text
POST /api/crafting/items
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

## HTTP Request vs Application Command

El adapter utiliza modelos propios del transporte:

```text
CraftItemHttpRequest
```

y los traduce a modelos de Application:

```text
CraftItemCommand
```

La separación es deliberada.

Una modificación del contrato HTTP no debe obligar al caso de uso a convertirse en un modelo de transporte.

---

## HTTP Response vs Application Result

Application devuelve:

```text
CraftItemResult
```

El adapter traduce ese significado a:

```text
HTTP status code
HTTP response body
```

Por ejemplo:

```text
InsufficientIngredients
        ↓
409 Conflict
```

Ese `409` sólo tiene significado dentro del adapter HTTP.

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

El endpoint no debe:

```text
consultar SQL
usar DbContext
conocer InMemoryInventoryRepository
calcular ingredientes
modificar Inventory directamente
```

Su frontera termina al invocar el inbound port.

---

## Regla principal

HTTP entra por este adapter.

HTTP no entra al núcleo.
