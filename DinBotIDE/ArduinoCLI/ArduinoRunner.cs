using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DinBotIDE.ArduinoCLI
{
    /// <summary>
    /// Ejecuta arduino-cli para compilar y subir sketches al DinBot.
    /// </summary>
    public class ArduinoRunner
    {
        private const string FQBN        = "arduino:avr:mega";
        private const string SketchDir   = "sketch";
        private const string SketchFile  = "sketch.ino";
        private const string ArduinoCLI  = "arduino-cli";

        public record ArduinoResult(bool Success, string Output, string ErrorMessage);

        /// <summary>
        /// Compila el código Arduino dado. Devuelve el resultado con stdout/stderr.
        /// </summary>
        public async Task<ArduinoResult> CompileAsync(string codigoArduino)
        {
            await EscribirSketch(codigoArduino);
            return await EjecutarCLI($"compile --fqbn {{FQBN}} {{SketchDir}}/{{SketchFile}});
        }

        /// <summary>
        /// Compila y sube el sketch al puerto indicado.
        /// </summary>
        public async Task<ArduinoResult> UploadAsync(string codigoArduino, string puerto)
        {
            var compile = await CompileAsync(codigoArduino);
            if (!compile.Success) return compile;

            return await EjecutarCLI($"upload -p {{puerto}} --fqbn {{FQBN}} {{SketchDir}}/{{SketchFile}});
        }

        private static async Task EscribirSketch(string codigo)
        {
            Directory.CreateDirectory(SketchDir);
            await File.WriteAllTextAsync(Path.Combine(SketchDir, SketchFile), codigo);
        }

        private static async Task<ArduinoResult> EjecutarCLI(string args)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName               = ArduinoCLI,
                    Arguments              = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    UseShellExecute        = false,
                    CreateNoWindow         = true
                };

                using var proceso = Process.Start(psi) ?? throw new InvalidOperationException("No se pudo iniciar arduino-cli");
                var stdout = await proceso.StandardOutput.ReadToEndAsync();
                var stderr = await proceso.StandardError.ReadToEndAsync();
                await proceso.WaitForExitAsync();

                bool ok = proceso.ExitCode == 0;
                return new ArduinoResult(ok, stdout, ok ? string.Empty : stderr);
            }
            catch (Exception ex)
            {
                return new ArduinoResult(false, string.Empty, $"Error al ejecutar arduino-cli: {{ex.Message}});
            }
        }
    }
}