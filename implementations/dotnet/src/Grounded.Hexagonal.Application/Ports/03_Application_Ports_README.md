# Ports — Application

Esta carpeta contiene las fronteras mediante las cuales Application se comunica con actores externos.

Los ports son conceptos arquitectónicos.

Su representación mediante interfaces de C# es una decisión de esta implementación.

---

## Tipos

```text
Ports/
├── Inbound/
└── Outbound/
```

---

## Inbound

Expresan operaciones que actores externos pueden pedirle al sistema.

Ejemplo:

```text
PORT-IN-CRAFT-001
```

---

## Outbound

Expresan capacidades externas que Application necesita para completar sus casos de uso.

Ejemplos:

```text
PORT-OUT-INVENTORY-001
PORT-OUT-RECIPE-001
```

---

## Dirección conceptual

```text
External Actor
      ↓
Inbound Adapter
      ↓
Inbound Port
      ↓
Application
      ↓
Outbound Port
      ↓
Outbound Adapter
      ↓
External System
```

---

## Regla principal

Application posee los contratos que necesita.

Los adapters se adaptan a esos contratos.

El núcleo no debe adaptarse a tecnologías externas.
