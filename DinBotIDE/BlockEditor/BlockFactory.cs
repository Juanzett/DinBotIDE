using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Crea y gestiona los bloques visuales en el canvas.
    /// </summary>
    public static class BlockFactory
    {
        private static readonly Dictionary<TipoBloque, BloqueDefinicion> _definiciones = new()
        {
            // Movimiento
            [TipoBloque.MoverAdelante]   = new(TipoBloque.MoverAdelante,   CategoriaBloque.Movimiento,    "Mover adelante",   "#4CAF50", ["velocidad", "tiempo_ms"]),
            [TipoBloque.MoverAtras]      = new(TipoBloque.MoverAtras,      CategoriaBloque.Movimiento,    "Mover atrás",      "#4CAF50", ["velocidad", "tiempo_ms"]),
            [TipoBloque.GirarIzquierda]  = new(TipoBloque.GirarIzquierda,  CategoriaBloque.Movimiento,    "Girar izquierda",  "#66BB6A", ["velocidad", "tiempo_ms"]),
            [TipoBloque.GirarDerecha]    = new(TipoBloque.GirarDerecha,    CategoriaBloque.Movimiento,    "Girar derecha",    "#66BB6A", ["velocidad", "tiempo_ms"]),
            [TipoBloque.Detener]         = new(TipoBloque.Detener,         CategoriaBloque.Movimiento,    "Detener",          "#388E3C", []),

            // Sensores
            [TipoBloque.SiCNY70Izq]     = new(TipoBloque.SiCNY70Izq,     CategoriaBloque.Sensor,        "Si CNY70 izq.",    "#2196F3", []),
            [TipoBloque.SiCNY70Der]     = new(TipoBloque.SiCNY70Der,     CategoriaBloque.Sensor,        "Si CNY70 der.",    "#2196F3", []),
            [TipoBloque.SiChoque]       = new(TipoBloque.SiChoque,       CategoriaBloque.Sensor,        "Si choque",        "#1976D2", []),
            [TipoBloque.SiLDR]          = new(TipoBloque.SiLDR,          CategoriaBloque.Sensor,        "Si LDR",           "#1565C0", ["umbral"]),
            [TipoBloque.SiMicrofono]    = new(TipoBloque.SiMicrofono,    CategoriaBloque.Sensor,        "Si micrófono",     "#0D47A1", ["umbral"]),
            [TipoBloque.SiIR]           = new(TipoBloque.SiIR,           CategoriaBloque.Sensor,        "Si IR recibe",     "#42A5F5", ["codigo"]),

            // Control
            [TipoBloque.RepetirN]       = new(TipoBloque.RepetirN,       CategoriaBloque.Control,       "Repetir N veces",  "#FF9800", ["veces"]),
            [TipoBloque.RepetirSiempre] = new(TipoBloque.RepetirSiempre, CategoriaBloque.Control,       "Repetir siempre",  "#F57C00", []),
            [TipoBloque.SiSino]         = new(TipoBloque.SiSino,         CategoriaBloque.Control,       "Si / Sino",        "#EF6C00", ["condicion"]),
            [TipoBloque.Esperar]        = new(TipoBloque.Esperar,        CategoriaBloque.Control,       "Esperar ms",       "#E65100", ["tiempo_ms"]),

            // Comunicación
            [TipoBloque.EnviarSerial]   = new(TipoBloque.EnviarSerial,   CategoriaBloque.Comunicacion,  "Enviar por serial","#9C27B0", ["mensaje"]),
            [TipoBloque.LeerSerial]     = new(TipoBloque.LeerSerial,     CategoriaBloque.Comunicacion,  "Leer serial",      "#7B1FA2", []),
        };

        /// <summary>
        /// Crea un bloque visual (Border+StackPanel) para el canvas.
        /// </summary>
        public static FrameworkElement CrearBloque(TipoBloque tipo)
        {
            var def = _definiciones[tipo];
            var color = (Color)ColorConverter.ConvertFromString(def.ColorHex);

            var panel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(6) };

            // Etiqueta del bloque
            panel.Children.Add(new TextBlock
            {
                Text       = def.Etiqueta,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize   = 13
            });

            // Campos de parámetros
            foreach (var param in def.Parametros)
            {
                var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 3, 0, 0) };
                row.Children.Add(new TextBlock
                {
                    Text       = param + ": ",
                    Foreground = Brushes.LightGray,
                    FontSize   = 11,
                    VerticalAlignment = VerticalAlignment.Center
                });
                row.Children.Add(new TextBox
                {
                    Width       = 60,
                    Text        = "0",
                    FontSize    = 11,
                    Tag         = param,
                    Background  = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0)),
                    Foreground  = Brushes.White,
                    BorderBrush = Brushes.Gray
                });
                panel.Children.Add(row);
            }

            // Botón eliminar
            var btnEliminar = new Button
            {
                Content    = "✕",
                Width      = 18, Height  = 18,
                FontSize   = 10,
                Background = Brushes.Transparent,
                Foreground = Brushes.LightGray,
                BorderThickness = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin     = new Thickness(0, 4, 0, 0),
                Cursor     = System.Windows.Input.Cursors.Hand,
                Tag        = tipo
            };

            var border = new Border
            {
                Background      = new SolidColorBrush(color),
                CornerRadius    = new CornerRadius(6),
                Padding         = new Thickness(8),
                MinWidth        = 160,
                Child           = panel,
                Tag             = tipo
            };

            btnEliminar.Click += (s, e) =>
            {
                if (VisualTreeHelper.GetParent(border) is Canvas canvas)
                {
                    canvas.Children.Remove(border);
                    // Regenerar código desde la ventana principal
                    if (Application.Current.MainWindow is MainWindow mw)
                        mw.RegenerarCodigo();
                }
            };
            panel.Children.Add(btnEliminar);

            // Habilitar arrastre
            BlockCanvas.HabilitarArrastre(border);

            return border;
        }

        /// <summary>
        /// Extrae la lista ordenada de bloques del canvas (de arriba a abajo).
        /// </summary>
        public static List<BloqueInstancia> ExtraerBloques(Canvas canvas)
        {
            return canvas.Children
                .OfType<FrameworkElement>()
                .Where(el => el.Tag is TipoBloque)
                .OrderBy(el => Canvas.GetTop(el))
                .Select(el =>
                {
                    var tipo = (TipoBloque)el.Tag;
                    var parametros = new Dictionary<string, string>();

                    // Recorrer TextBox de parámetros
                    foreach (var tb in FindTextBoxes(el))
                        if (tb.Tag is string key)
                            parametros[key] = tb.Text;

                    return new BloqueInstancia(tipo, parametros);
                })
                .ToList();
        }

        private static IEnumerable<TextBox> FindTextBoxes(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox tb) yield return tb;
                foreach (var sub in FindTextBoxes(child)) yield return sub;
            }
        }
    }

    /// <summary>
    /// Instancia concreta de un bloque con sus parámetros actuales.
    /// </summary>
    public record BloqueInstancia(TipoBloque Tipo, Dictionary<string, string> Parametros);
}