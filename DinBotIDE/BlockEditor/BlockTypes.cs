namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Todos los tipos de bloques disponibles en la paleta del IDE.
    /// </summary>
    public enum TipoBloque
    {
        // ── Movimiento ──
        MoverAdelante,
        MoverAtras,
        GirarIzquierda,
        GirarDerecha,
        Detener,

        // ── Sensores ──
        SiCNY70Izquierdo,
        SiCNY70Derecho,
        SiChoque,
        SiLDR,
        SiMicrofono,
        SiIR,

        // ── Control ──
        Repetir,
        RepetirSiempre,
        Si,
        Esperar,

        // ── Comunicación ──
        EnviarSerial,
        LeerSerial,
    }
}