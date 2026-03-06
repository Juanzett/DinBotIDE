using System.Collections.Generic;
using DinBotIDE.BlockEditor;

namespace DinBotIDE.Models
{
    /// <summary>
    /// Modelo del programa completo en bloques.
    /// Se puede serializar a JSON para guardar/cargar proyectos.
    /// </summary>
    public class BlockProgram
    {
        public string       NombrePrograma { get; set; } = "Programa nuevo";
        public string       Version        { get; set; } = "0.1";
        public List<BloqueGuardado> Bloques { get; set; } = new();
    }

    /// <summary>
    /// Representación serializable de un bloque con su posición en el canvas.
    /// </summary>
    public class BloqueGuardado
    {
        public TipoBloque              Tipo       { get; set; }
        public double                  X          { get; set; }
        public double                  Y          { get; set; }
        public Dictionary<string, string> Parametros { get; set; } = new();
    }
}