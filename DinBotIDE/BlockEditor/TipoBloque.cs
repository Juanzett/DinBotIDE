namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Todos los tipos de bloques disponibles en el editor DinBot IDE.
    /// </summary>
    public enum TipoBloque
    {
        // ── Movimiento ────────────────────────────────────────────
        MoverAdelante,
        MoverAtras,
        GirarIzquierda,
        GirarDerecha,
        Detener,

        // ── Sensores / Condiciones ────────────────────────────────
        SiCNY70Izquierdo,
        SiCNY70Derecho,
        SiChoque,
        SiLDR,
        SiMicrofono,
        SiIR,

        // ── Control de flujo ──────────────────────────────────────
        Repetir,
        RepetirSiempre,
        Si,
        Esperar,

        // ── Comunicación ──────────────────────────────────────────
        EnviarSerial,
        LeerSerial,
    }
}