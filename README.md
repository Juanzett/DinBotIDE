# 🤖 DinBot IDE

> IDE de escritorio en **C# WPF (.NET 8)** para programar el robot **DinBot v2.4** de RobotGroup Argentina.  
> Reemplaza al sistema original **miniBloq** (abandonado ~2016).

---

## 📋 ¿Qué es DinBot IDE?

DinBot IDE permite programar el robot DinBot v2.4 mediante **bloques visuales** (drag & drop), genera código **Arduino (.ino)** automáticamente, y compila/sube el firmware al robot via USB usando **arduino-cli** como backend.

---

## 🔧 Hardware Soportado

- **DinBot v2.4** — Arduino Mega compatible (ATmega2560), fabricado por RobotGroup Argentina
- **MatBot v1.0** — versión alternativa/simplificada del mismo sistema

---

## 💻 Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Lenguaje / UI | C# + WPF (.NET 8) |
| IDE de desarrollo | Visual Studio Community 2022 |
| Backend compilación | arduino-cli |
| Placa target | Arduino Mega (FQBN: `arduino:avr:mega`) |
| Comunicación USB | CH340 (requiere driver) |
| Librería robot | `mbq.h` (Multiplo/RobotGroup) |

---

## 🗂️ Estructura del Proyecto

```
DinBotIDE/
├── DinBotIDE.sln
├── DinBotIDE/
│   ├── App.xaml / App.xaml.cs
│   ├── MainWindow.xaml / MainWindow.xaml.cs
│   ├── BlockEditor/
│   │   ├── BlockTypes.cs
│   │   ├── BlockCanvas.cs
│   │   ├── BlockRenderer.cs
│   │   └── BlockConnection.cs
│   ├── CodeGenerator/
│   │   └── ArduinoCodeGenerator.cs
│   ├── ArduinoCLI/
│   │   ├── ArduinoRunner.cs
│   │   └── BoardDetector.cs
│   ├── SerialMonitor/
│   │   └── SerialMonitor.cs
│   └── Models/
│       ├── RobotConfig.cs
│       └── BlockProgram.cs
└── docs/
    └── DINBOT_IDE_PROJECT.md
```

---

## 🚀 Inicio Rápido

### 1. Prerequisitos

- [ ] [Visual Studio Community 2022](https://visualstudio.microsoft.com/es/vs/community/) — workload: `.NET desktop development`
- [ ] [arduino-cli](https://arduino.github.io/arduino-cli/)
- [ ] Driver CH340 (buscar "CH340 driver Windows 10")
- [ ] [Git](https://git-scm.com)

### 2. Instalar soporte Arduino AVR

```bash
arduino-cli core install arduino:avr
```

### 3. Clonar y abrir

```bash
git clone https://github.com/Juanzett/DinBotIDE.git
cd DinBotIDE
start DinBotIDE.sln
```

### 4. Compilar y ejecutar

Abrir `DinBotIDE.sln` en Visual Studio 2022 → **F5**

---

## 🧱 Bloques del IDE (MVP)

### Movimiento
- `[Mover adelante]` velocidad / tiempo
- `[Mover atrás]` velocidad / tiempo
- `[Girar izquierda/derecha]` velocidad / ángulo
- `[Detener]`

### Sensores
- `[Si CNY70 detecta línea]`
- `[Si sensor de choque activado]`
- `[Si LDR < umbral]`
- `[Si micrófono > umbral]`
- `[Si IR recibe código]`

### Control
- `[Repetir N veces]`
- `[Repetir siempre]`
- `[Si / sino]`
- `[Esperar N ms]`

### Comunicación
- `[Enviar por serial]`
- `[Leer serial]`

---

## 🎯 Fases del Proyecto

| Fase | Estado | Descripción |
|------|--------|-------------|
| **Fase 0** | 🔄 En progreso | Setup del entorno |
| **Fase 1** | ⏳ Pendiente | MVP: seguidor de línea, IR, sensores |
| **Fase 2** | ⏳ Pendiente | Integración Android (pantalla, TTS, cámara) |
| **Fase 3** | ⏳ Pendiente | IA, GPS, visión artificial, reconocimiento de voz |

---

## 🔌 Comandos arduino-cli útiles

```bash
# Listar puertos disponibles
arduino-cli board list

# Compilar sketch
arduino-cli compile --fqbn arduino:avr:mega sketch/sketch.ino

# Subir al DinBot
arduino-cli upload -p COM3 --fqbn arduino:avr:mega sketch/sketch.ino

# Monitor serial
arduino-cli monitor -p COM3 --config baudrate=115200
```

---

## 📚 Referencias

- [miniBloq (sistema original)](https://blog.minibloq.org)
- [arduino-cli docs](https://arduino.github.io/arduino-cli)
- [Multiplo (plataforma base)](http://multiplo.org)
- [RobotGroup Argentina](http://robotgroup.com.ar)

---

*Proyecto en estado: **Fase 0 — Setup del entorno** | Última actualización: 2026-03-06*