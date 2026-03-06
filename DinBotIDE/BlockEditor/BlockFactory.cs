using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Crea los botones de la paleta y los controles visuales de cada bloque.
    /// </summary>
    public static class BlockFactory
    {
        // Colores por categoría (inspirado en Scratch)
        private static readonly Dictionary<string, string> ColoresGrupo = new()
        {
            { "🚗 Movimiento", "#FF6600" },
            { "👁️ Sensores",   "#2196F3" },
            { "🔁 Control",    "#9C27B0" },
            { "📡 Comunicación","#009688" },
        };

        private static readonly Dictionary<TipoBloque, string> EtiquetasBloques = new()
        {
            { TipoBloque.MoverAdelante,      "▶ Mover adelante" },
            { TipoBloque.MoverAtras,         "◀ Mover atrás" },
            { TipoBloque.GirarIzquierda,     "↺ Girar izquierda" },
            { TipoBloque.GirarDerecha,       "↻ Girar derecha" },
            { TipoBloque.Detener,            "⏹ Detener" },
            { TipoBloque.SiCNY70Izquierdo,   "Si CNY70 izquierdo" },
            { TipoBloque.SiCNY70Derecho,     "Si CNY70 derecho" },
            { TipoBloque.SiChoque,           "Si choque detectado" },
            { TipoBloque.SiLDR,              "Si LDR < umbral" },
            { TipoBloque.SiMicrofono,        "Si micrófono activo" },
            { TipoBloque.SiIR,               "Si IR recibe código" },
            { TipoBloque.Repetir,            "🔁 Repetir N veces" },
            { TipoBloque.RepetirSiempre,     "♾ Repetir siempre" },
            { TipoBloque.Si,                 "❓ Si / Sino" },
            { TipoBloque.Esperar,            "⏱ Esperar ms" },
            { TipoBloque.EnviarSerial,       "📤 Enviar serial" },
            { TipoBloque.LeerSerial,         "📥 Leer serial" },
        };

        /// <summary>
        /// Retorna los grupos de bloques para renderizar la paleta.
        /// </summary>
        public static List<(string Titulo, List<TipoBloque> Bloques)> GetGrupos() => new()
        {
            ("🚗 Movimiento", new() {
                TipoBloque.MoverAdelante, TipoBloque.MoverAtras,
                TipoBloque.GirarIzquierda, TipoBloque.GirarDerecha,
                TipoBloque.Detener }),

            ("👁️ Sensores", new() {
                TipoBloque.SiCNY70Izquierdo, TipoBloque.SiCNY70Derecho,
                TipoBloque.SiChoque, TipoBloque.SiLDR,
                TipoBloque.SiMicrofono, TipoBloque.SiIR }),

            ("🔁 Control", new() {
                TipoBloque.Repetir, TipoBloque.RepetirSiempre,
                TipoBloque.Si, TipoBloque.Esperar }),

            ("📡 Comunicación", new() {
                TipoBloque.EnviarSerial, TipoBloque.LeerSerial }),
        };

        /// <summary>
        /// Crea el botón de paleta para arrastrar al canvas.
        /// </summary>
        public static Button CrearBotonPaleta(TipoBloque tipo)
        {
            var color = ObtenerColorTipo(tipo);
            return new Button
            {
                Content = EtiquetasBloques.TryGetValue(tipo, out var label) ? label : tipo.ToString(),
                Tag = tipo,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                Foreground = Brushes.White,
                FontSize = 12,
                Padding = new Thickness(8, 5, 8, 5),
                Margin = new Thickness(0, 2, 0, 2),
                BorderThickness = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Cursor = System.Windows.Input.Cursors.Hand,
                ToolTip = $"Arrastrar al canvas: {label}",
            };
        }

        public static string ObtenerEtiqueta(TipoBloque tipo)
            => EtiquetasBloques.TryGetValue(tipo, out var lbl) ? lbl : tipo.ToString();

        public static string ObtenerColorTipo(TipoBloque tipo) => tipo switch
        {
            TipoBloque.MoverAdelante or TipoBloque.MoverAtras or
            TipoBloque.GirarIzquierda or TipoBloque.GirarDerecha or
            TipoBloque.Detener => "#FF6600",

            TipoBloque.SiCNY70Izquierdo or TipoBloque.SiCNY70Derecho or
            TipoBloque.SiChoque or TipoBloque.SiLDR or
            TipoBloque.SiMicrofono or TipoBloque.SiIR => "#2196F3",

            TipoBloque.Repetir or TipoBloque.RepetirSiempre or
            TipoBloque.Si or TipoBloque.Esperar => "#9C27B0",

            TipoBloque.EnviarSerial or TipoBloque.LeerSerial => "#009688",

            _ => "#555555"
        };
    }
}