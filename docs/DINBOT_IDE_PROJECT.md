# 🤖 DinBot IDE — Contexto del Proyecto

> Documento de contexto completo del proyecto.  
> Usar como prompt inicial en GitHub Copilot / Claude / cualquier LLM.

---

## 📋 Resumen del Proyecto

Desarrollar un **IDE de escritorio en C# (WPF / .NET 8)** para programar el robot **DinBot v2.4** de RobotGroup Argentina, en reemplazo del sistema original **miniBloq** (abandonado ~2016).

El IDE permite programar el robot mediante **bloques visuales** (drag & drop), genera código **Arduino (.ino)** automáticamente, compila y sube el firmware al robot via USB usando **arduino-cli** como backend.

---

## 🔧 Hardware Identificado

### Placa Principal
- **DinBot v2.4** — Arduino Mega compatible, fabricado por RobotGroup Argentina
- **MatBot v1.0** — versión alternativa/simplificada del mismo sistema
- Board FQBN: `arduino:avr:mega`

### Motores
- 2x Motor DC RobotGroup (1.5–12 VDC, 400mA stall, encoder de cuadratura)

### Sensores
| Sensor | Cantidad | Uso |
|--------|----------|-----|
| CNY70 (IR reflectivo) | 2 | Seguidor de línea |
| Sensor de choque | 1 | Detección de obstáculos |
| LDR | 2 | Reacción a luz |
| Micrófono | 1 | Reacción a sonido |
| IR 38kHz | 1 | Control remoto H5002 |

---

## 💻 Stack Tecnológico

- **C# + WPF (.NET 8)** — Visual Studio Community 2022
- **arduino-cli** — compilación y carga del firmware
- **Puerto Serie / USB** — chip CH340 a 115200 baud

---

## 🎯 Fases del Proyecto

| Fase | Descripción |
|------|-------------|
| **Fase 0** | Setup del entorno |
| **Fase 1** | MVP: seguidor de línea, esquiva obstáculos, control IR |
| **Fase 2** | Integración con Android (pantalla, TTS, cámara, WiFi) |
| **Fase 3** | IA y funciones avanzadas (GPT, GPS, visión artificial) |

---

## 🔌 Pinout estimado DinBot v2.4

```
MOTOR_IZQ_A → Pin 4    MOTOR_IZQ_B → Pin 5
MOTOR_DER_A → Pin 6    MOTOR_DER_B → Pin 7
CNY70_IZQ   → A0       CNY70_DER   → A1
SENSOR_CHOQUE → Pin 2  SENSOR_LDR  → A2
SENSOR_MIC  → A3       SENSOR_IR   → Pin 3
```

---

## 🔗 Referencias

- miniBloq: https://blog.minibloq.org
- arduino-cli: https://arduino.github.io/arduino-cli
- Multiplo: http://multiplo.org
- RobotGroup: http://robotgroup.com.ar

---

*Documento generado el 2026-03-06. Proyecto en estado: **Fase 0 — Setup del entorno**.*