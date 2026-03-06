using System.Collections.Generic;
using System.IO.Ports;

namespace DinBotIDE.ArduinoCLI
{
    /// <summary>
    /// Detecta los puertos COM disponibles en el sistema (donde puede estar conectado el DinBot).
    /// </summary>
    public class BoardDetector
    {
        /// <summary>
        /// Retorna la lista de puertos seriales disponibles (ej: COM3, COM4).
        /// </summary>
        public List<string> ObtenerPuertos()
        {
            var puertos = new List<string>(SerialPort.GetPortNames());
            return puertos;
        }
    }
}