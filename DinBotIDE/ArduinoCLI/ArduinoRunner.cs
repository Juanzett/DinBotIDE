using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace DinBotIDE.ArduinoCLI
{
    /// <summary>
    /// Llama a arduino-cli para compilar y subir el sketch al DinBot.
    /// Requiere que arduino-cli esté instalado y en el PATH del sistema.
    /// </summary>
    public class ArduinoRunner
    {
        private const string FQBN       = "arduino:avr:mega";
        private const string CLI        = "arduino-cli";
        private readonly string _sketchDir;
        private readonly string _sketchFile;

        public ArduinoRunner()
        {
            _sketchDir  = Path.Combine(Path.GetTempPath(), "DinBotIDE_sketch");
            _sketchFile = Path.Combine(_sketchDir, "sketch.ino");
            Directory.CreateDirectory(_sketchDir);
        }

        /// <summary>
        /// Guarda el código .ino en disco y lo compila con arduino-cli.
        /// </summary>
        public async Task<string> CompilarAsync(string codigoArduino)
        {
            await File.WriteAllTextAsync(_sketchFile, codigoArduino);
            return await EjecutarCLIAsync($"compile --fqbn {FQBN} \"{_sketchDir}\"");
        }

        /// <summary>
        /// Sube el último sketch compilado al DinBot en el puerto indicado.
        /// </summary>
        public async Task<string> SubirAsync(string puerto)
        {
            return await EjecutarCLIAsync($"upload -p {puerto} --fqbn {FQBN} \"{_sketchDir}\"");
        }

        private static async Task<string> EjecutarCLIAsync(string argumentos)
        {
            var psi = new ProcessStartInfo(CLI, argumentos)
            {
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute        = false,
                CreateNoWindow         = true
            };

            using var proceso = new Process { StartInfo = psi };
            proceso.Start();

            var stdout = await proceso.StandardOutput.ReadToEndAsync();
            var stderr = await proceso.StandardError.ReadToEndAsync();
            await proceso.WaitForExitAsync();

            var resultado = stdout + (string.IsNullOrWhiteSpace(stderr) ? "" : "\n" + stderr);
            return string.IsNullOrWhiteSpace(resultado)
                ? $"arduino-cli finalizó (código: {proceso.ExitCode})"
                : resultado.Trim();
        }
    }
}