using System.Collections.Generic;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Instancia de un bloque colocado en el canvas:
    /// contiene tipo, posición y parámetros configurados por el usuario.
    /// </summary>
    public class BloqueInstancia
    {
        public TipoBloque              Tipo       { get; set; }
        public double                  X          { get; set; }
        public double                  Y          { get; set; }
        public Dictionary<string, string> Parametros { get; set; } = new();

        public BloqueInstancia() { }

        public BloqueInstancia(TipoBloque tipo, double x = 0, double y = 0)
        {
            Tipo = tipo;
            X    = x;
            Y    = y;
        }
    }
}