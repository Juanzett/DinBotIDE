using System.Collections.Generic;
using System.Text;
using DinBotIDE.BlockEditor;

namespace DinBotIDE.CodeGenerator
{
    /// <summary>
    /// Convierte una lista ordenada de BloqueVisual en código Arduino (.ino)
    /// compatible con el DinBot v2.4 (Arduino Mega / ATmega2560).
    /// </summary>
    public class ArduinoCodeGenerator
    {
        // ── Pinout DinBot v2.4 ───────────────────────────────────────────────
        private const string PINOUT = @"// ── Pinout DinBot v2.4 ─────────────────────────
#define MOTOR_IZQ_A   4
#define MOTOR_IZQ_B   5
#define MOTOR_DER_A   6
#define MOTOR_DER_B   7
#define CNY70_IZQ     A0
#define CNY70_DER     A1
#define SENSOR_CHOQUE 2
#define LDR_PIN       A2
#define MIC_PIN       A3
#define IR_PIN        11
#define UMBRAL_LINEA  500
// ─────────────────────────────────────────────────";

        public string Generar(List<BloqueVisual> bloques)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Código generado por DinBot IDE v1.0");
            sb.AppendLine("// Robot: DinBot v2.4 — RobotGroup Argentina");
            sb.AppendLine("// https://github.com/Juanzett/DinBotIDE");
            sb.AppendLine();
            sb.AppendLine(PINOUT);
            sb.AppendLine();
            sb.AppendLine("// ── Helpers de movimiento ───────────────────────");
            sb.AppendLine("void adelante(int vel) {");
            sb.AppendLine("  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);");
            sb.AppendLine("  analogWrite(MOTOR_DER_A, vel); digitalWrite(MOTOR_DER_B, LOW);");
            sb.AppendLine("}");
            sb.AppendLine("void atras(int vel) {");
            sb.AppendLine("  digitalWrite(MOTOR_IZQ_A, LOW); analogWrite(MOTOR_IZQ_B, vel);");
            sb.AppendLine("  digitalWrite(MOTOR_DER_A, LOW); analogWrite(MOTOR_DER_B, vel);");
            sb.AppendLine("}");
            sb.AppendLine("void izquierda(int vel) {");
            sb.AppendLine("  analogWrite(MOTOR_IZQ_B, vel); digitalWrite(MOTOR_IZQ_A, LOW);");
            sb.AppendLine("  analogWrite(MOTOR_DER_A, vel); digitalWrite(MOTOR_DER_B, LOW);");
            sb.AppendLine("}");
            sb.AppendLine("void derecha(int vel) {");
            sb.AppendLine("  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);");
            sb.AppendLine("  analogWrite(MOTOR_DER_B, vel); digitalWrite(MOTOR_DER_A, LOW);");
            sb.AppendLine("}");
            sb.AppendLine("void detener() {");
            sb.AppendLine("  digitalWrite(MOTOR_IZQ_A, LOW); digitalWrite(MOTOR_IZQ_B, LOW);");
            sb.AppendLine("  digitalWrite(MOTOR_DER_A, LOW); digitalWrite(MOTOR_DER_B, LOW);");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("// ── setup() ─────────────────────────────────────");
            sb.AppendLine("void setup() {");
            sb.AppendLine("  pinMode(MOTOR_IZQ_A, OUTPUT); pinMode(MOTOR_IZQ_B, OUTPUT);");
            sb.AppendLine("  pinMode(MOTOR_DER_A, OUTPUT); pinMode(MOTOR_DER_B, OUTPUT);");
            sb.AppendLine("  pinMode(SENSOR_CHOQUE, INPUT_PULLUP);");
            sb.AppendLine("  Serial.begin(115200);");
            sb.AppendLine("  Serial.println(\"DinBot v2.4 listo!\");");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("// ── loop() ──────────────────────────────────────");
            sb.AppendLine("void loop() {");

            if (bloques.Count == 0)
            {
                sb.AppendLine("  // ← Arrastrá bloques al canvas para generar código");
            }
            else
            {
                foreach (var bloque in bloques)
                    sb.AppendLine("  " + GenerarLinea(bloque));
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerarLinea(BloqueVisual b)
        {
            var v = string.IsNullOrWhiteSpace(b.Param1) ? "150" : b.Param1;
            var t = string.IsNullOrWhiteSpace(b.Param2) ? "500" : b.Param2;

            return b.TipoBloque switch
            {
                TipoBloque.MoverAdelante    => $"adelante({v}); delay({t});",
                TipoBloque.MoverAtras       => $"atras({v}); delay({t});",
                TipoBloque.GirarIzquierda   => $"izquierda({v}); delay({t});",
                TipoBloque.GirarDerecha     => $"derecha({v}); delay({t});",
                TipoBloque.Detener          => "detener();",
                TipoBloque.SiCNY70Izquierdo => $"if (analogRead(CNY70_IZQ) > UMBRAL_LINEA) {{ /* acción */ }}",
                TipoBloque.SiCNY70Derecho   => $"if (analogRead(CNY70_DER) > UMBRAL_LINEA) {{ /* acción */ }}",
                TipoBloque.SiChoque         => $"if (digitalRead(SENSOR_CHOQUE) == LOW) {{ /* acción */ }}",
                TipoBloque.SiLDRBajo        => $"if (analogRead(LDR_PIN) < {v}) {{ /* acción */ }}",
                TipoBloque.SiMicrofonoAlto  => $"if (analogRead(MIC_PIN) > {v}) {{ /* acción */ }}",
                TipoBloque.SiIRRecibe       => $"// IR: si recibe código {v} → acción",
                TipoBloque.Repetir          => $"for (int i = 0; i < {v}; i++) {{ /* bloque */ }}",
                TipoBloque.RepetirSiempre   => "while (true) { /* bloque */ }",
                TipoBloque.Si               => "if (/* condición */) { /* bloque */ }",
                TipoBloque.Esperar          => $"delay({v});",
                TipoBloque.EnviarSerial     => $"Serial.println(\"{b.Param1}\");",
                TipoBloque.LeerSerial       => "if (Serial.available()) { String datos = Serial.readStringUntil('\n'); }",
                _                           => "// bloque desconocido"
            };
        }
    }
}