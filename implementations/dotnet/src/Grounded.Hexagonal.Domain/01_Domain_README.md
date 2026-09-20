# Grounded.Hexagonal.Domain

Este proyecto contiene el modelo y las reglas centrales del dominio del sistema.

Su objetivo es representar el conocimiento del negocio de forma independiente de mecanismos externos como HTTP, bases de datos, frameworks, procesos en background o infraestructura.

> Este proyecto representa el núcleo de negocio. No representa Application, infraestructura ni mecanismos de entrada/salida.

---

## Responsabilidad arquitectónica

Dentro de esta implementación, `Grounded.Hexagonal.Domain` representa la parte más interna y estable del sistema.

Aquí viven conceptos como:

```text
Item
Inventory
Recipe
Food
Crafting rules
Spoilage rules
Value Objects
Domain Errors
Invariants
```

El dominio expresa **qué es válido en el negocio** y **cómo se comportan sus conceptos principales**.

---

## Qué puede contener

Este proyecto puede contener:

```text
Entities
Value Objects
Domain Rules
Domain Services
Domain Errors
Domain Events, si más adelante son necesarios
Enums o tipos propios del dominio
```

Siempre que representen conocimiento del negocio.

---

## Qué no debe contener

Este proyecto no debe conocer detalles como:

```text
HTTP
ASP.NET Core
Controllers
Endpoints
EF Core
SQL
DbContext
BackgroundService
Dependency Injection
Configuration
Logging framework específico
JSON
DTOs de transporte
```

El dominio debe poder compilarse y probarse sin levantar infraestructura externa.

---

## Entidades

Una Entity representa un concepto del dominio con identidad propia.

Ejemplo conceptual:

```text
Inventory
```

Dos instancias pueden representar el mismo inventario aunque ciertos valores internos cambien con el tiempo.

La identidad y el comportamiento son más importantes que comparar únicamente todas sus propiedades.

---

## Value Objects

Un Value Object representa un valor definido por sus datos y reglas, no por una identidad independiente.

Ejemplos potenciales:

```text
ItemId
Quantity
Durability
SpoilageLevel
```

Un Value Object debe ayudar a evitar estados inválidos y expresar mejor el lenguaje del dominio.

---

## Invariantes

Una invariante es una regla que debe mantenerse verdadera para que el modelo permanezca válido.

Ejemplo conceptual:

```text
La cantidad de un item en el inventario no puede ser negativa.
```

Las invariantes deben protegerse dentro del dominio, no dejarse únicamente en adapters o UI.

---

## Domain Rules

Las reglas de dominio expresan decisiones del negocio.

Ejemplo conceptual:

```text
Para fabricar un Mint Mace deben existir determinados ingredientes
en cantidades suficientes.
```

Estas reglas no deben depender de:

```text
HTTP
SQL
EF Core
ASP.NET Core
```

---

## Domain Services

Un Domain Service puede utilizarse cuando una regla de negocio:

- pertenece al dominio;
- involucra varios conceptos;
- no encaja naturalmente dentro de una única Entity o Value Object.

No debe utilizarse como una carpeta genérica para poner lógica que no sabemos dónde ubicar.

---

## Domain Errors

Los errores del dominio representan violaciones o situaciones relevantes para el negocio.

Ejemplos conceptuales:

```text
InsufficientIngredients
InvalidQuantity
ItemNotFound
```

No deben expresarse inicialmente como:

```text
HTTP 400
HTTP 404
SQL Exception
```

porque esos significados pertenecen a adapters externos.

---

## Dependencias permitidas

`Grounded.Hexagonal.Domain`:

```text
puede depender de:
    bibliotecas base de .NET cuando sean necesarias

no puede depender de:
    Application
    Inbound Adapters
    Outbound Adapters
    Hosts
```

La regla buscada es:

```text
Domain
  ↓
ningún otro proyecto productivo
```

---

## Relación con Application

Application puede utilizar Domain.

Domain no conoce Application.

```text
Application
    ↓
Domain
```

Nunca:

```text
Domain
    ↓
Application
```

---

## Relación con persistencia

Domain no sabe cómo se almacena su estado.

Por ejemplo:

```text
Inventory
```

no debe conocer:

```text
DbContext
SQL table
EF Entity
Repository implementation
```

La persistencia será responsabilidad de un outbound adapter.

---

## Relación con los casos de uso

Los casos de uso de Application utilizarán el dominio para ejecutar comportamiento.

Ejemplo:

```text
CraftItemHandler
      ↓
Inventory / Recipe
      ↓
Domain Rules
```

El handler coordina.

El dominio decide qué es válido desde el punto de vista del negocio.

---

## Organización futura

Las carpetas internas se crearán a medida que aparezcan responsabilidades reales.

Una posible evolución:

```text
Grounded.Hexagonal.Domain/
│
├── Crafting/
├── Inventory/
└── Food/
```

No se crearán carpetas genéricas como:

```text
Helpers/
Utils/
Common/
Services/
```

sin una responsabilidad de dominio claramente justificable.

---

## Trazabilidad

Cuando un tipo de dominio implemente directamente una regla o concepto documentado en la especificación, debe indicarse su Architecture ID correspondiente.

Ejemplos:

```text
RULE-CRAFT-001
RULE-SPOILAGE-001
```

La trazabilidad buscada es:

```text
Specification
    ↓
Architecture ID
    ↓
Domain Type / Rule
    ↓
Domain Test
```

---

## Pruebas

Las reglas de este proyecto se prueban principalmente en:

```text
Grounded.Hexagonal.Domain.Tests
```

Las pruebas deben poder ejecutarse sin:

```text
servidor HTTP
base de datos
worker
infraestructura externa
```

---

## Regla principal

Si un concepto deja de tener sentido cuando eliminamos ASP.NET Core, EF Core o la base de datos, probablemente no pertenece al Domain.

Domain representa el negocio, no la tecnología utilizada para ejecutarlo.
