# Use Cases — Application

Esta carpeta contiene las implementaciones de los comportamientos que el sistema ofrece.

Cada caso de uso coordina Domain y los ports necesarios.

---

## Casos iniciales

```text
CraftItem
GetInventory
ProcessFoodSpoilage
```

---

## Responsabilidad

Un caso de uso puede:

```text
recibir input
consultar outbound ports
cargar objetos de dominio
invocar comportamiento del dominio
persistir cambios mediante outbound ports
construir un resultado
```

---

## Lo que no debe hacer

No debe contener:

```text
HTTP status codes
SQL
EF Core
Controller logic
BackgroundService scheduling
```

---

## Handlers

En esta implementación cada caso de uso tendrá inicialmente un Handler.

Ejemplo:

```text
CraftItemHandler
```

`Handler` es una convención de implementación .NET/C#.

No es una pieza obligatoria de Arquitectura Hexagonal.

---

## Regla principal

Application coordina.

Domain gobierna las reglas del negocio.

Adapters traducen hacia y desde tecnologías externas.
