using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace DinBotIDE.ArduinoCLI
{
    /// <summary>
    /// Detecta los puertos COM disponibles en el sistema.
    /// En una versión futura puede usar 'arduino-cli board list' para identificar la placa.
    /// </summary>
    public class BoardDetector
    {
        /// <summary>
        /// Devuelve los puertos COM disponibles en el sistema.
        /// </summary>
        public List<string> GetAvailablePorts()
        {
            try
            {
                var puertos = new List<string>(SerialPort.GetPortNames());
                puertos.Sort(StringComparer.OrdinalIgnoreCase);
                return puertos;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}