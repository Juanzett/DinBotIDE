using DinBotIDE.Models;
using System.Text;

namespace DinBotIDE.CodeGenerator
{
    /// <summary>
    /// Traduce el programa en bloques a código Arduino (.ino) listo para compilar.
    /// </summary>
    public static class ArduinoCodeGenerator
    {
        public static string Generar(BlockProgram programa)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// ════════════════════════════════════════");
            sb.AppendLine("// Código generado por DinBot IDE");
            sb.AppendLine("// Robot: DinBot v2.4 — RobotGroup Argentina");
            sb.AppendLine("// ════════════════════════════════════════");
            sb.AppendLine();
            sb.AppendLine(GenerarCabecera());
            sb.AppendLine();
            sb.AppendLine("void setup() {");
            sb.AppendLine("  Serial.begin(115200);");
            sb.AppendLine("  pinMode(MOTOR_IZQ_A, OUTPUT);");
            sb.AppendLine("  pinMode(MOTOR_IZQ_B, OUTPUT);");
            sb.AppendLine("  pinMode(MOTOR_DER_A, OUTPUT);");
            sb.AppendLine("  pinMode(MOTOR_DER_B, OUTPUT);");
            sb.AppendLine("  Serial.println(\"DinBot listo!\");");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("void loop() {");

            foreach (var bloque in programa.Bloques)
                sb.AppendLine(GenerarBloque(bloque));

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine(GenerarFunciones());

            return sb.ToString();
        }

        private static string GenerarCabecera() => @"// ── Pinout DinBot v2.4 ──\n#define MOTOR_IZQ_A  4\n#define MOTOR_IZQ_B  5\n#define MOTOR_DER_A  6\n#define MOTOR_DER_B  7\n#define CNY70_IZQ    A0\n#define CNY70_DER    A1\n#define PIN_CHOQUE   2\n#define PIN_LDR_IZQ  A2\n#define PIN_LDR_DER  A3\n#define PIN_MIC      A4\n#define PIN_IR       3\n#define UMBRAL_LINEA 500";

        private static string GenerarBloque(DatoBloque b)
        {
            string P(string key, string def = "0") =>
                b.Parametros.TryGetValue(key, out var v) ? v : def;

            return b.Tipo switch
            {
                "MoverAdelante"  => $"  adelante({P("Velocidad", "150")}); delay({P("Tiempo", "1000")});",
                "MoverAtras"     => $"  atras({P("Velocidad", "150")}); delay({P("Tiempo", "1000")});",
                "GirarIzquierda" => $"  girarIzq({P("Velocidad", "120")}); delay({P("Tiempo", "500")});",
                "GirarDerecha"   => $"  girarDer({P("Velocidad", "120")}); delay({P("Tiempo", "500")});",
                "Detener"        => "  detener();",

                "SiCNY70Izquierdo" => "  if (analogRead(CNY70_IZQ) < UMBRAL_LINEA) {\n    // TODO: acción\n  }",
                "SiCNY70Derecho"   => "  if (analogRead(CNY70_DER) < UMBRAL_LINEA) {\n    // TODO: acción\n  }",
                "SiChoque"         => "  if (digitalRead(PIN_CHOQUE) == LOW) {\n    // TODO: acción\n  }",
                "SiLDR"            => $"  if (analogRead(PIN_LDR_IZQ) < {P("Umbral", "400")}) {{\n    // TODO: acción\n  }}",
                "SiMicrofono"      => $"  if (analogRead(PIN_MIC) > {P("Umbral", "600")}) {{\n    // TODO: acción\n  }}",
                "SiIR"             => $"  // IR: verificar código {P("Codigo", "0xFF30CF")}",

                "Repetir"        => $"  for (int i = 0; i < {P("Veces", "3")}; i++) {{\n    // TODO: bloques internos\n  }}",
                "RepetirSiempre" => "  // 'Repetir siempre' → usar loop() directamente",
                "Si"             => "  if (/* condición */) {\n    // TODO: acción\n  } else {\n    // TODO: sino\n  }",
                "Esperar"        => $"  delay({P("Ms", "1000")});",

                "EnviarSerial"   => $"  Serial.println(\"{P("Mensaje", "Hola DinBot")}\");",
                "LeerSerial"     => "  if (Serial.available()) { String data = Serial.readStringUntil('\n'); }",

                _ => $"  // Bloque desconocido: {b.Tipo}"
            };
        }

        private static string GenerarFunciones() => @"// ── Funciones de movimiento ──\nvoid adelante(int vel) {\n  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);\n  analogWrite(MOTOR_DER_A, vel); digitalWrite(MOTOR_DER_B, LOW);\n}\n\nvoid atras(int vel) {\n  digitalWrite(MOTOR_IZQ_A, LOW); analogWrite(MOTOR_IZQ_B, vel);\n  digitalWrite(MOTOR_DER_A, LOW); analogWrite(MOTOR_DER_B, vel);\n}\n\nvoid girarIzq(int vel) {\n  digitalWrite(MOTOR_IZQ_A, LOW); digitalWrite(MOTOR_IZQ_B, LOW);\n  analogWrite(MOTOR_DER_A, vel);  digitalWrite(MOTOR_DER_B, LOW);\n}\n\nvoid girarDer(int vel) {\n  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);\n  digitalWrite(MOTOR_DER_A, LOW); digitalWrite(MOTOR_DER_B, LOW);\n}\n\nvoid detener() {\n  digitalWrite(MOTOR_IZQ_A, LOW); digitalWrite(MOTOR_IZQ_B, LOW);\n  digitalWrite(MOTOR_DER_A, LOW); digitalWrite(MOTOR_DER_B, LOW);\n}\n";
    }
}