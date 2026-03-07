using DinBotIDE.Models;
using System.Text;

namespace DinBotIDE.CodeGenerator
{
    /// <summary>
    /// Traduce el programa en bloques a código Arduino (.ino) listo para compilar.
    /// </summary>
    public static class ArduinoCodeGenerator
    {
        /// <summary>
        /// Genera el código Arduino a partir de una lista de DatoBloque.
        /// </summary>
        public static string Generar(List<DatoBloque> bloques)
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

            foreach (var bloque in bloques)
                sb.AppendLine(GenerarBloque(bloque));

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine(GenerarFunciones());

            return sb.ToString();
        }

        private static string GenerarCabecera()
        {
            return """
                // ── Pinout DinBot v2.4 ──
                #define MOTOR_IZQ_A  4
                #define MOTOR_IZQ_B  5
                #define MOTOR_DER_A  6
                #define MOTOR_DER_B  7
                #define CNY70_IZQ    A0
                #define CNY70_DER    A1
                #define PIN_CHOQUE   2
                #define PIN_LDR_IZQ  A2
                #define PIN_LDR_DER  A3
                #define PIN_MIC      A4
                #define PIN_IR       3
                #define UMBRAL_LINEA 500
                """;
        }

        private static string GenerarBloque(DatoBloque b)
        {
            string P(string key, string def = "0") =>
                b.Parametros.TryGetValue(key, out var v) ? v : def;

            return b.Tipo switch
            {
                "MoverAdelante"    => $"  adelante({P("Velocidad", "150")}); delay({P("Tiempo", "1000")});",
                "MoverAtras"       => $"  atras({P("Velocidad", "150")}); delay({P("Tiempo", "1000")});",
                "GirarIzquierda"   => $"  girarIzq({P("Velocidad", "120")}); delay({P("Tiempo", "500")});",
                "GirarDerecha"     => $"  girarDer({P("Velocidad", "120")}); delay({P("Tiempo", "500")});",
                "Detener"          => "  detener();",

                "SiCNY70Izquierdo" => "  if (analogRead(CNY70_IZQ) < UMBRAL_LINEA) {\n    // TODO: acción\n  }",
                "SiCNY70Derecho"   => "  if (analogRead(CNY70_DER) < UMBRAL_LINEA) {\n    // TODO: acción\n  }",
                "SiChoque"         => "  if (digitalRead(PIN_CHOQUE) == LOW) {\n    // TODO: acción\n  }",
                "SiLDR"            => $"  if (analogRead(PIN_LDR_IZQ) < {P("Umbral", "400")}) {{\n    // TODO: acción\n  }}",
                "SiMicrofono"      => $"  if (analogRead(PIN_MIC) > {P("Umbral", "600")}) {{\n    // TODO: acción\n  }}",
                "SiIR"             => $"  // IR: verificar código {P("Codigo", "0xFF30CF")}",

                "Repetir"          => $"  for (int i = 0; i < {P("Veces", "3")}; i++) {{\n    // TODO: bloques internos\n  }}",
                "RepetirSiempre"   => "  // 'Repetir siempre' → usar loop() directamente",
                "Si"               => "  if (/* condición */) {\n    // TODO: acción\n  } else {\n    // TODO: sino\n  }",
                "Esperar"          => $"  delay({P("Ms", "1000")});",

                "EnviarSerial"     => $"  Serial.println(\"{P("Mensaje", "Hola DinBot")}\");",
                "LeerSerial"       => "  if (Serial.available()) { String data = Serial.readStringUntil('\\n'); }",

                _ => $"  // Bloque desconocido: {b.Tipo}"
            };
        }

        private static string GenerarFunciones()
        {
            return """
                // ── Funciones de movimiento ──
                void adelante(int vel) {
                  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);
                  analogWrite(MOTOR_DER_A, vel); digitalWrite(MOTOR_DER_B, LOW);
                }

                void atras(int vel) {
                  digitalWrite(MOTOR_IZQ_A, LOW); analogWrite(MOTOR_IZQ_B, vel);
                  digitalWrite(MOTOR_DER_A, LOW); analogWrite(MOTOR_DER_B, vel);
                }

                void girarIzq(int vel) {
                  digitalWrite(MOTOR_IZQ_A, LOW); digitalWrite(MOTOR_IZQ_B, LOW);
                  analogWrite(MOTOR_DER_A, vel);  digitalWrite(MOTOR_DER_B, LOW);
                }

                void girarDer(int vel) {
                  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);
                  digitalWrite(MOTOR_DER_A, LOW); digitalWrite(MOTOR_DER_B, LOW);
                }

                void detener() {
                  digitalWrite(MOTOR_IZQ_A, LOW); digitalWrite(MOTOR_IZQ_B, LOW);
                  digitalWrite(MOTOR_DER_A, LOW); digitalWrite(MOTOR_DER_B, LOW);
                }
                """;
        }
    }
}