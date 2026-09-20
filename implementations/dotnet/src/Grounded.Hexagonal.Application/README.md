# Grounded.Hexagonal.Application

Este proyecto contiene los casos de uso y las fronteras mediante las cuales el núcleo de la aplicación se comunica con el exterior.

Application coordina comportamiento. Utiliza Domain para aplicar reglas de negocio y define los ports necesarios para recibir solicitudes y utilizar capacidades externas.

---

## Responsabilidad arquitectónica

La responsabilidad principal de Application es:

```text
orquestar casos de uso
```

Ejemplos iniciales:

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

Application decide el orden de las operaciones necesarias para cumplir cada caso de uso.

---

## Qué puede contener

Este proyecto puede contener:

```text
Use Cases
Handlers
Commands
Queries
Inbound Ports
Outbound Ports
Application Results
Application-specific errors
```

---

## Qué no debe contener

No debe contener implementaciones tecnológicas de:

```text
HTTP
ASP.NET Core Controllers
EF Core
SQL
BackgroundService
DbContext
Database repositories concretos
UI
```

Application puede expresar que necesita una capacidad externa, pero no debe implementar el detalle tecnológico.

---

## Relación con Domain

Application puede depender de Domain:

```text
Application
    ↓
Domain
```

Puede crear, consultar o modificar objetos del dominio y utilizar sus reglas.

Domain no depende de Application.

---

## Inbound Ports

Un inbound port representa una operación que el sistema ofrece al exterior.

Ejemplos:

```text
PORT-IN-CRAFT-001
PORT-IN-INVENTORY-001
PORT-IN-SPOILAGE-001
```

En C# se representarán inicialmente mediante interfaces.

Ejemplo conceptual:

```csharp
public interface ICraftItemUseCase
{
}
```

La interface es una decisión de implementación.

El concepto arquitectónico es:

```text
Inbound Port
```

---

## Outbound Ports

Un outbound port representa una capacidad externa que Application necesita.

Ejemplos:

```text
obtener inventario
guardar inventario
consultar recetas
obtener la hora actual
```

Application define el contrato.

Los outbound adapters lo implementan.

```text
Application
    │
    │ define
    ▼
Outbound Port
    ▲
    │ implements
    │
Outbound Adapter
```

---

## Commands

Un Command representa una intención de ejecutar una acción que puede modificar estado.

Ejemplo:

```text
CraftItemCommand
```

El nombre `Command` es una decisión organizativa de esta implementación.

No es un requisito de Arquitectura Hexagonal.

---

## Queries

Una Query representa una solicitud de información.

Ejemplo:

```text
GetInventoryQuery
```

`GetInventory` servirá para contrastar una operación de consulta con un command como `CraftItem`.

---

## Handlers

Un Handler implementa u orquesta un caso de uso.

Ejemplos:

```text
CraftItemHandler
GetInventoryHandler
ProcessFoodSpoilageHandler
```

`Handler` no es un término obligatorio de Arquitectura Hexagonal.

Es la forma elegida en esta implementación para hacer explícita la clase que coordina un caso de uso.

---

## Flujo conceptual

Ejemplo:

```text
Inbound Adapter
      ↓
Inbound Port
      ↓
Handler
      ↓
Domain
      ↓
Outbound Port
      ↓
Outbound Adapter
```

Application contiene:

```text
Inbound Port
Handler
Outbound Port
```

pero no contiene los adapters concretos.

---

## Dependencias permitidas

Application puede depender de:

```text
Grounded.Hexagonal.Domain
```

No debe depender de:

```text
Grounded.Hexagonal.Adapters.Inbound.Http
Grounded.Hexagonal.Adapters.Inbound.Worker
Grounded.Hexagonal.Adapters.Outbound.Persistence
Grounded.Hexagonal.Host.Api
Grounded.Hexagonal.Host.Worker
```

---

## Organización futura

La estructura inicial prevista será:

```text
Grounded.Hexagonal.Application/
│
├── Ports/
│   ├── Inbound/
│   └── Outbound/
│
└── UseCases/
    ├── CraftItem/
    ├── GetInventory/
    └── ProcessFoodSpoilage/
```

Cada carpeta tendrá documentación adicional cuando sea creada.

---

## Trazabilidad

Los casos de uso y ports deben relacionarse con IDs arquitectónicos estables.

Ejemplos:

```text
UC-CRAFT-001
PORT-IN-CRAFT-001
PORT-OUT-INVENTORY-001
```

La trazabilidad será:

```text
Specification
    ↓
Architecture ID
    ↓
Application Port / Handler
    ↓
Application Test
```

---

## Regla principal

Application coordina el negocio, pero no debe convertirse en infraestructura.

Si una clase necesita saber cómo funciona HTTP, EF Core o SQL, probablemente pertenece a un adapter y no a Application.
