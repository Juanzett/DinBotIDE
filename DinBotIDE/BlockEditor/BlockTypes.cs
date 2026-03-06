namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Tipos de bloques disponibles en el IDE DinBot.
    /// Organizados por categoría: Movimiento, Sensores, Control, Comunicación.
    /// </summary>
    public enum TipoBloque
    {
        // ── Movimiento ──────────────────────────────
        MoverAdelante,
        MoverAtras,
        GirarIzquierda,
        GirarDerecha,
        Detener,

        // ── Sensores ────────────────────────────────
        SiCNY70Izquierdo,
        SiCNY70Derecho,
        SiChoque,
        SiLDRBajo,
        SiMicrofonoAlto,
        SiIRRecibe,

        // ── Control ─────────────────────────────────
        Repetir,
        RepetirSiempre,
        Si,
        Esperar,

        // ── Comunicación ────────────────────────────
        EnviarSerial,
        LeerSerial
    }
}