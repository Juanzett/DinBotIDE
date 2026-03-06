namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Todos los tipos de bloques disponibles en el IDE.
    /// </summary>
    public enum BlockType
    {
        // ── Movimiento ────────────────────────────────
        MoverAdelante,
        MoverAtras,
        GirarIzquierda,
        GirarDerecha,
        Detener,

        // ── Sensores ──────────────────────────────────
        SiCNY70Izquierdo,
        SiCNY70Derecho,
        SiChoque,
        SiLDR,
        SiMicrofono,
        SiIR,

        // ── Control ───────────────────────────────────
        RepetirNVeces,
        RepetirSiempre,
        Si,
        Esperar,

        // ── Comunicación ──────────────────────────────
        EnviarSerial,
        LeerSerial
    }

    /// <summary>
    /// Categorías de bloques para organizar la paleta.
    /// </summary>
    public enum BlockCategory
    {
        Movimiento,
        Sensores,
        Control,
        Comunicacion
    }

    /// <summary>
    /// Metadatos de un tipo de bloque.
    /// </summary>
    public class BlockDefinition
    {
        public BlockType     Type        { get; init; }
        public BlockCategory Category    { get; init; }
        public string        Label       { get; init; } = "";
        public string        ColorHex    { get; init; } = "#607D8B";
        public string        Icon        { get; init; } = "⬛";
        public string[]      Parameters  { get; init; } = [];

        /// <summary>Catálogo completo de bloques del IDE.</summary>
        public static readonly BlockDefinition[] All =
        [
            // Movimiento
            new() { Type = BlockType.MoverAdelante,   Category = BlockCategory.Movimiento,   Label = "Mover adelante",   ColorHex = "#FF6B2C", Icon = "▶", Parameters = ["velocidad","tiempo (ms)"] },
            new() { Type = BlockType.MoverAtras,      Category = BlockCategory.Movimiento,   Label = "Mover atrás",      ColorHex = "#FF6B2C", Icon = "◀", Parameters = ["velocidad","tiempo (ms)"] },
            new() { Type = BlockType.GirarIzquierda,  Category = BlockCategory.Movimiento,   Label = "Girar izquierda",  ColorHex = "#FF9800", Icon = "↺", Parameters = ["velocidad","ángulo"] },
            new() { Type = BlockType.GirarDerecha,    Category = BlockCategory.Movimiento,   Label = "Girar derecha",    ColorHex = "#FF9800", Icon = "↻", Parameters = ["velocidad","ángulo"] },
            new() { Type = BlockType.Detener,         Category = BlockCategory.Movimiento,   Label = "Detener",          ColorHex = "#F44336", Icon = "⏹", Parameters = [] },

            // Sensores
            new() { Type = BlockType.SiCNY70Izquierdo, Category = BlockCategory.Sensores,   Label = "Si CNY70 izq. línea", ColorHex = "#9C27B0", Icon = "👁", Parameters = [] },
            new() { Type = BlockType.SiCNY70Derecho,   Category = BlockCategory.Sensores,   Label = "Si CNY70 der. línea", ColorHex = "#9C27B0", Icon = "👁", Parameters = [] },
            new() { Type = BlockType.SiChoque,         Category = BlockCategory.Sensores,   Label = "Si choque activado",  ColorHex = "#E91E63", Icon = "💥", Parameters = [] },
            new() { Type = BlockType.SiLDR,            Category = BlockCategory.Sensores,   Label = "Si LDR < umbral",     ColorHex = "#FF5722", Icon = "💡", Parameters = ["umbral"] },
            new() { Type = BlockType.SiMicrofono,      Category = BlockCategory.Sensores,   Label = "Si micrófono > umbral",ColorHex = "#795548", Icon = "🎤", Parameters = ["umbral"] },
            new() { Type = BlockType.SiIR,             Category = BlockCategory.Sensores,   Label = "Si IR recibe código", ColorHex = "#607D8B", Icon = "📡", Parameters = ["código IR"] },

            // Control
            new() { Type = BlockType.RepetirNVeces,   Category = BlockCategory.Control,     Label = "Repetir N veces",   ColorHex = "#2196F3", Icon = "🔁", Parameters = ["veces"] },
            new() { Type = BlockType.RepetirSiempre,  Category = BlockCategory.Control,     Label = "Repetir siempre",   ColorHex = "#1565C0", Icon = "∞",  Parameters = [] },
            new() { Type = BlockType.Si,              Category = BlockCategory.Control,     Label = "Si / Sino",         ColorHex = "#4CAF50", Icon = "❓", Parameters = [] },
            new() { Type = BlockType.Esperar,         Category = BlockCategory.Control,     Label = "Esperar",           ColorHex = "#009688", Icon = "⏱", Parameters = ["tiempo (ms)"] },

            // Comunicación
            new() { Type = BlockType.EnviarSerial,    Category = BlockCategory.Comunicacion, Label = "Enviar por serial", ColorHex = "#3F51B5", Icon = "📤", Parameters = ["mensaje"] },
            new() { Type = BlockType.LeerSerial,      Category = BlockCategory.Comunicacion, Label = "Leer serial",       ColorHex = "#3F51B5", Icon = "📥", Parameters = [] },
        ];
    }
}