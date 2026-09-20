# Inbound Ports — Application

Esta carpeta contiene las operaciones que Application expone a inbound adapters.

---

## Concepto arquitectónico

Un inbound port representa:

```text
una capacidad que el sistema permite invocar
```

No representa HTTP, UI, CLI ni ningún mecanismo concreto.

---

## CraftItem

El primer inbound port será:

```text
PORT-IN-CRAFT-001
```

Representación prevista en C#:

```text
ICraftItemUseCase
```

---

## Posibles adapters

El mismo inbound port podría ser invocado desde:

```text
HTTP
CLI
UI
message consumer
test
otro adapter de entrada
```

sin cambiar el comportamiento del caso de uso.

---

## Regla principal

La interface C# representa el port.

La interface C# no define qué significa un port en Arquitectura Hexagonal.
