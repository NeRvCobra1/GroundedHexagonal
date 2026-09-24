# Ejercicios — practica sin copiar la solución

La finalidad de estos ejercicios es obligarte a decidir fronteras antes de escribir archivos.

No hay una única estructura de carpetas correcta.

Antes de implementar, escribe en papel:

```text
regla de Domain
use case
input port
output ports necesarios
inbound adapter
outbound adapters
tests
```

---

## Ejercicio 1 — RepairItem

Agrega conceptualmente:

```text
UC-REPAIR-001
RepairItem
```

Supón:

```text
un item tiene Durability
un item tiene MaxDurability
reparar consume RepairMaterial
la durabilidad nunca puede superar MaxDurability
si no hay material suficiente, no debe haber mutación parcial
```

### Tu trabajo

Decide primero:

```text
qué regla pertenece al Domain
qué debe coordinar Application
qué información debe cargar
qué debe persistirse
```

### Checklist de diseño

```text
[ ] La regla de durabilidad no vive en HTTP.
[ ] Application no conoce EF Core.
[ ] La mutación inválida no deja estado parcial.
[ ] El Input Port describe RepairItem, no HTTP.
[ ] Los Output Ports describen capacidades, no tecnologías.
[ ] El Endpoint sólo traduce request/result.
[ ] El Host hace el wiring.
```

### Tests mínimos

```text
Domain
[ ] reparación válida incrementa durabilidad
[ ] no supera MaxDurability
[ ] amount inválido falla
[ ] falta de material no muta parcialmente

Application
[ ] carga lo necesario
[ ] delega la regla a Domain
[ ] guarda sólo en éxito
[ ] devuelve un resultado explícito

Integration
[ ] adapter concreto persiste el cambio

Architecture
[ ] no aparecen nuevas dependencias prohibidas
```

Antes de empezar, relee:

[`MinimalExample.md`](MinimalExample.md)

La diferencia es que ahora debes llevar la idea mínima hasta una frontera Hexagonal completa.

---

## Ejercicio 2 — Cambia la entrada sin tocar el core

Imagina que `GetInventory` también se ejecuta desde CLI:

```text
inventory --player <id>
```

Diseña:

```text
CLI inbound adapter
        ↓
IGetInventoryUseCase
```

Restricción:

```text
GetInventoryHandler no debe modificarse.
Domain no debe modificarse.
IInventoryRepository no debe modificarse.
```

Pregunta:

> ¿Qué prueba concreta demuestra que CLI es sólo otro adapter?

---

## Ejercicio 3 — Cambia persistencia

Imagina PostgreSQL en lugar de SQLite.

Identifica qué esperarías cambiar:

```text
adapter de persistencia
configuración
composition root
integration tests específicos
```

Identifica qué no debería necesitar cambios por ese motivo:

```text
Inventory.Craft
CraftItemHandler
ICraftItemUseCase
CraftItemEndpoint
```

Si una de esas piezas necesita saber `PostgreSQL`, explica por qué.

---

## Ejercicio 4 — Nueva capacidad exterior

Imagina una regla de Application que necesita un valor de temperatura exterior.

No pongas directamente un SDK meteorológico dentro del Handler.

Diseña primero una capacidad:

```text
ITemperatureProvider
```

Después decide:

```text
quién posee el port
qué adapter lo implementa
cómo se reemplaza en tests
quién hace el wiring
```

Compara con:

```text
IClock
→ SystemClock
```

---

## Autoevaluación

Para cada ejercicio deberías poder responder:

```text
¿qué pertenece a Domain?
¿qué coordina Application?
¿cuál es la frontera de entrada?
¿qué capacidades salen del core?
¿qué tecnología concreta queda fuera?
¿qué test protege comportamiento?
¿qué test protege arquitectura?
```

Si empiezas creando controllers, repositories y carpetas antes de contestar esas preguntas, vuelve al diseño.
