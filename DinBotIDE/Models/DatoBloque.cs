using System.Collections.Generic;

namespace DinBotIDE.Models
{
    /// <summary>
    /// Datos de un bloque extraídos para generación de código Arduino.
    /// </summary>
    public class DatoBloque
    {
        public string Tipo { get; set; } = "";
        public double PosX { get; set; }
        public double PosY { get; set; }
        public Dictionary<string, string> Parametros { get; set; } = new();
    }
}