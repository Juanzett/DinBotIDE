namespace DinBotIDE.Models
{
    /// <summary>
    /// Configuración del robot activo.
    /// </summary>
    public class RobotConfig
    {
        public string NombreRobot   { get; set; } = "DinBot v2.4";
        public string FQBN          { get; set; } = "arduino:avr:mega";
        public string PuertoCOM     { get; set; } = "COM3";
        public int    BaudRate      { get; set; } = 115200;
        public int    UmbralLinea   { get; set; } = 500;
        public int    UmbralLDR     { get; set; } = 300;
        public int    UmbralMic     { get; set; } = 600;
    }
}