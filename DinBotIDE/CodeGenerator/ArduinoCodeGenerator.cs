using System.Collections.Generic;
using System.Text;
using DinBotIDE.BlockEditor;

namespace DinBotIDE.CodeGenerator
{
    /// <summary>
    /// Traduce la lista de bloques del canvas a código Arduino (.ino) válido.
    /// </summary>
    public class ArduinoCodeGenerator
    {
        // ── Pinout DinBot v2.4 ─────────────────────────────────────────
        private const string Header = @"// Código generado por DinBot IDE v0.1
// Hardware: DinBot v2.4 — Arduino Mega (ATmega2560)
// ¡No editar manualmente — regenerar desde el IDE!

#define MOTOR_IZQ_A   4
#define MOTOR_IZQ_B   5
#define MOTOR_DER_A   6
#define MOTOR_DER_B   7
#define CNY70_IZQ     A0
#define CNY70_DER     A1
#define SENSOR_CHOQUE 2
#define SENSOR_LDR    A2
#define SENSOR_MIC    A3
#define SENSOR_IR     3
#define UMBRAL_LINEA  500
";

        private const string SetupFijo = @"
void setup() {
  pinMode(MOTOR_IZQ_A, OUTPUT);
  pinMode(MOTOR_IZQ_B, OUTPUT);
  pinMode(MOTOR_DER_A, OUTPUT);
  pinMode(MOTOR_DER_B, OUTPUT);
  pinMode(SENSOR_CHOQUE, INPUT_PULLUP);
  Serial.begin(115200);
  Serial.println(\"DinBot IDE — Iniciado\"
);
}

// ── Funciones de movimiento ─────────────────────────────────
void adelante(int vel) {
  analogWrite(MOTOR_IZQ_A, vel); digitalWrite(MOTOR_IZQ_B, LOW);
  analogWrite(MOTOR_DER_A, vel); digitalWrite(MOTOR_DER_B, LOW);
}
void atras(int vel) {
  digitalWrite(MOTOR_IZQ_A, LOW); analogWrite(MOTOR_IZQ_B, vel);
  digitalWrite(MOTOR_DER_A, LOW); analogWrite(MOTOR_DER_B, vel);
}
void girarIzq(int vel) {
  digitalWrite(MOTOR_IZQ_A, LOW); analogWrite(MOTOR_IZQ_B, vel);
  analogWrite(MOTOR_DER_A, vel);  digitalWrite(MOTOR_DER_B, LOW);
}
void girarDer(int vel) {
  analogWrite(MOTOR_IZQ_A, vel);  digitalWrite(MOTOR_IZQ_B, LOW);
  digitalWrite(MOTOR_DER_A, LOW); analogWrite(MOTOR_DER_B, vel);
}
void detener() {
  digitalWrite(MOTOR_IZQ_A, LOW); digitalWrite(MOTOR_IZQ_B, LOW);
  digitalWrite(MOTOR_DER_A, LOW); digitalWrite(MOTOR_DER_B, LOW);
}
";

        public string GenerarCodigo(List<BloqueInstancia> bloques)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Header);
            sb.AppendLine(SetupFijo);
            sb.AppendLine("void loop() {");

            foreach (var bloque in bloques)
                sb.AppendLine(GenerarBloque(bloque, indentacion: 2));

            sb.AppendLine("}");
            return sb.ToString();
        }

        private static string GenerarBloque(BloqueInstancia b, int indentacion)
        {
            var ind = new string(' ', indentacion);
            string Get(string key, string defVal = "0") =>
                b.Parametros.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? v : defVal;

            return b.Tipo switch
            {
                TipoBloque.MoverAdelante   => $"{ind}adelante({Get("velocidad", "150")}); delay({Get("tiempo_ms", "500")});",
                TipoBloque.MoverAtras      => $"{ind}atras({Get("velocidad", "150")}); delay({Get("tiempo_ms", "500")});",
                TipoBloque.GirarIzquierda  => $"{ind}girarIzq({Get("velocidad", "150")}); delay({Get("tiempo_ms", "400")});",
                TipoBloque.GirarDerecha    => $"{ind}girarDer({Get("velocidad", "150")}); delay({Get("tiempo_ms", "400")});",
                TipoBloque.Detener         => $"{ind}detener();",

                TipoBloque.SiCNY70Izq      => $"{ind}if (analogRead(CNY70_IZQ) < UMBRAL_LINEA) {{",
                TipoBloque.SiCNY70Der      => $"{ind}if (analogRead(CNY70_DER) < UMBRAL_LINEA) {{",
                TipoBloque.SiChoque        => $"{ind}if (digitalRead(SENSOR_CHOQUE) == LOW) {{",
                TipoBloque.SiLDR           => $"{ind}if (analogRead(SENSOR_LDR) < {{Get("umbral", "300")}}) {{",
                TipoBloque.SiMicrofono     => $"{ind}if (analogRead(SENSOR_MIC) > {{Get("umbral", "600")}}) {{",
                TipoBloque.SiIR            => $"{ind}if (/* IR code */ false) {{ // TODO: código IR = {{Get("codigo")}}",

                TipoBloque.RepetirN        => $"{ind}for (int _i = 0; _i < {{Get("veces", "3")}}; _i++) {{",
                TipoBloque.RepetirSiempre  => $"{ind}while (true) {{",
                TipoBloque.SiSino          => $"{ind}if ({{Get("condicion", "true")}}) {{",
                TipoBloque.Esperar         => $"{ind}delay({{Get("tiempo_ms", "1000")}});",

                TipoBloque.EnviarSerial    => $"{ind}Serial.println(\"{{Get("mensaje", "hola")}}\");",
                TipoBloque.LeerSerial      => $"{ind}if (Serial.available()) {{ String _s = Serial.readStringUntil('\n'); }}",

                _ => $"{ind}// Bloque desconocido: {{b.Tipo}}"
            };
        }
    }
}