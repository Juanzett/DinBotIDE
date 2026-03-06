namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Tipos de bloques disponibles en la paleta del IDE.
    /// </summary>
    public enum TipoBloque
    {
        // Movimiento
        MoverAdelante,
        MoverAtras,
        GirarIzquierda,
        GirarDerecha,
        Detener,

        // Sensores
        SiCNY70Izq,
        SiCNY70Der,
        SiChoque,
        SiLDR,
        SiMicrofono,
        SiIR,

        // Control
        RepetirN,
        RepetirSiempre,
        SiSino,
        Esperar,

        // Comunicación
        EnviarSerial,
        LeerSerial
    }

    /// <summary>
    /// Categoría de un bloque (determina su color).
    /// </summary>
    public enum CategoriaBloque
    {
        Movimiento,
        Sensor,
        Control,
        Comunicacion
    }

    /// <summary>
    /// Metadata de un tipo de bloque.
    /// </summary>
    public record BloqueDefinicion(
        TipoBloque Tipo,
        CategoriaBloque Categoria,
        string Etiqueta,
        string ColorHex,
        string[] Parametros
    );
}