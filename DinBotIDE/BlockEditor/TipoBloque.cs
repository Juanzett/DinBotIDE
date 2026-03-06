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
        SiCNY70Izq,
        SiCNY70Der,
        SiChoque,
        SiLDR,
        SiMicrofono,
        SiIR,

        // ── Control de flujo ──────────────────────────────────────
        RepetirN,
        RepetirSiempre,
        SiSino,
        Esperar,

        // ── Comunicación ──────────────────────────────────────────
        EnviarSerial,
        LeerSerial,
    }
}