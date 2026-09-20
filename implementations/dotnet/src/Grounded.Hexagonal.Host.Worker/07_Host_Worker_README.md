# Grounded.Hexagonal.Host.Worker

Este proyecto es el ejecutable que inicia procesos en background.

Actúa como Composition Root para los flujos iniciados mediante Worker.

---

## Responsabilidad

Puede encargarse de:

```text
Worker Host startup
Dependency Injection
configuration
logging
registro del inbound worker adapter
registro de outbound adapters
```

---

## Flujo esperado

```text
Host.Worker
    ↓
Inbound Worker Adapter
    ↓
Inbound Port
    ↓
Application
    ↓
Outbound Port
    ↓
Persistence Adapter
```

---

## Puede depender de

```text
Application
Inbound.Worker
Outbound.Persistence
```

---

## No debe contener

```text
reglas de negocio
reglas de spoilage
persistencia directa
```

---

## Regla principal

El Host inicia y conecta componentes.

El Worker Adapter dispara el caso de uso.

Application lo coordina.

Domain contiene las reglas.
