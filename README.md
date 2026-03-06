# 🤖 DinBot IDE

> IDE de escritorio en C# (WPF / .NET 8) para programar el robot **DinBot v2.4** de RobotGroup Argentina, en reemplazo del sistema original **miniBloq** (abandonado ~2016).

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![WPF](https://img.shields.io/badge/UI-WPF-0078D7?logo=windows)
![Arduino](https://img.shields.io/badge/Backend-arduino--cli-00979D?logo=arduino)
![Estado](https://img.shields.io/badge/Estado-Fase%200%20Setup-orange)

---

## 📋 ¿Qué es DinBot IDE?

El IDE permite programar el robot DinBot mediante **bloques visuales** (drag & drop), genera código **Arduino (.ino)** automáticamente, y lo compila y sube al robot via USB usando **arduino-cli** como backend.

---

## 🖥️ Requisitos del sistema

| Software | Versión | Link |
|---|---|---|
| Visual Studio Community | 2022 | [Descargar](https://visualstudio.microsoft.com/es/vs/community/) |
| .NET SDK | 8.0+ | incluido en VS 2022 |
| arduino-cli | última | [Instalar](https://arduino.github.io/arduino-cli/) |
| Driver CH340 | — | buscar "CH340 driver Windows 10" |

### Configurar arduino-cli (una sola vez)
```bash
arduino-cli core install arduino:avr
```

---

## 🚀 Cómo empezar

```bash
git clone https://github.com/Juanzett/DinBotIDE.git
cd DinBotIDE
start DinBotIDE.sln
```

1. Abrir `DinBotIDE.sln` en Visual Studio 2022
2. Click derecho → **Restaurar paquetes NuGet**
3. Presionar **F5** para compilar y ejecutar

---

## 🗂️ Estructura del proyecto

```
DinBotIDE/
├── DinBotIDE.sln
└── DinBotIDE/
    ├── App.xaml / App.xaml.cs         ← Colores y estilos globales
    ├── MainWindow.xaml / .cs          ← Ventana principal del IDE
    ├── BlockEditor/
    │   ├── BlockTypes.cs              ← Enum con los 17 tipos de bloques
    │   ├── BlockFactory.cs            ← Paleta organizada por categorías
    │   └── BloqueVisual.cs            ← Control drag & drop con parámetros
    ├── CodeGenerator/
    │   └── ArduinoCodeGenerator.cs    ← Bloques → código .ino
    ├── ArduinoCLI/
    │   ├── ArduinoRunner.cs           ← Compilar / subir via arduino-cli
    │   └── BoardDetector.cs           ← Detectar puertos COM
    ├── SerialMonitor/
    │   └── SerialMonitor.cs           ← Monitor serial async (115200 baud)
    └── Models/
        ├── RobotConfig.cs             ← Config del robot activo
        └── BlockProgram.cs            ← Modelo serializable del programa
```

---

## 🧱 Bloques disponibles (MVP)

| Categoría | Bloques |
|---|---|
| 🟢 Movimiento | Adelante, Atrás, Girar izquierda, Girar derecha, Detener |
| 🔵 Sensores | CNY70 izq/der, Choque, LDR, Micrófono, IR |
| 🟠 Control | Repetir N, Repetir siempre, Si/Sino, Esperar |
| 🟣 Comunicación | Enviar serial, Leer serial |

---

## 🔧 Hardware compatible

- **DinBot v2.4** — Arduino Mega compatible (ATmega2560)
- **FQBN:** `arduino:avr:mega`
- **Conexión:** USB Mini-B → CH340
- **Velocidad serial:** 115200 baud

---

## 🗺️ Roadmap

- **Fase 1 (MVP):** Seguidor de línea, esquiva obstáculos, control IR, LDR, micrófono
- **Fase 2:** Integración Android (pantalla, TTS, cámara, WiFi)
- **Fase 3:** IA conversacional, GPS, visión artificial, voz offline

---

## 📜 Licencia

MIT © 2026 — Juanzett
