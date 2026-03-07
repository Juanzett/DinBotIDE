using DinBotIDE.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Control visual de un bloque en el canvas. Soporta arrastre y parámetros editables.
    /// </summary>
    public class BloqueVisual : Border
    {
        public TipoBloque Tipo { get; }
        private Point _offsetArrastre;
        private readonly StackPanel _panelParams;

        /// <summary>
        /// Se dispara cuando el usuario hace clic en el botón eliminar.
        /// El suscriptor (BlockCanvas) debe encargarse de quitar el bloque.
        /// </summary>
        public event Action<BloqueVisual>? EliminacionSolicitada;

        public BloqueVisual(TipoBloque tipo)
        {
            Tipo = tipo;
            var color = BlockFactory.ObtenerColorTipo(tipo);

            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            CornerRadius = new CornerRadius(6);
            Padding = new Thickness(10, 6, 10, 6);
            MinWidth = 160;
            Cursor = Cursors.SizeAll;
            BorderBrush = Brushes.White;
            BorderThickness = new Thickness(0);
            Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Black,
                Opacity = 0.4,
                BlurRadius = 8,
                ShadowDepth = 2
            };

            var stack = new StackPanel();

            // ── Cabecera del bloque ──
            var header = new DockPanel();
            var titulo = new TextBlock
            {
                Text = BlockFactory.ObtenerEtiqueta(tipo),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center
            };
            var btnEliminar = new Button
            {
                Content = "✕",
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 11,
                Cursor = Cursors.Hand,
                Padding = new Thickness(4, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            btnEliminar.Click += (_, _) => EliminacionSolicitada?.Invoke(this);
            DockPanel.SetDock(btnEliminar, Dock.Right);
            header.Children.Add(btnEliminar);
            header.Children.Add(titulo);
            stack.Children.Add(header);

            // ── Parámetros según tipo ──
            _panelParams = new StackPanel { Margin = new Thickness(0, 4, 0, 0) };
            AgregarParametros(tipo, _panelParams);
            stack.Children.Add(_panelParams);

            Child = stack;

            // Arrastre dentro del canvas
            MouseLeftButtonDown += (_, e) =>
            {
                _offsetArrastre = e.GetPosition(this);
                CaptureMouse();
                e.Handled = true;
            };
            MouseMove += (_, e) =>
            {
                if (!IsMouseCaptured) return;
                var pos = e.GetPosition((Canvas)Parent);
                Canvas.SetLeft(this, Math.Max(0, pos.X - _offsetArrastre.X));
                Canvas.SetTop(this,  Math.Max(0, pos.Y - _offsetArrastre.Y));
            };
            MouseLeftButtonUp += (_, _) => ReleaseMouseCapture();
        }

        private static void AgregarParametros(TipoBloque tipo, Panel panel)
        {
            switch (tipo)
            {
                case TipoBloque.MoverAdelante:
                case TipoBloque.MoverAtras:
                    panel.Children.Add(CrearFila("Velocidad (0-255):", "150", "Velocidad"));
                    panel.Children.Add(CrearFila("Tiempo (ms):",        "1000", "Tiempo"));
                    break;

                case TipoBloque.GirarIzquierda:
                case TipoBloque.GirarDerecha:
                    panel.Children.Add(CrearFila("Velocidad (0-255):", "120", "Velocidad"));
                    panel.Children.Add(CrearFila("Tiempo (ms):",        "500",  "Tiempo"));
                    break;

                case TipoBloque.Esperar:
                    panel.Children.Add(CrearFila("Milisegundos:", "1000", "Ms"));
                    break;

                case TipoBloque.Repetir:
                    panel.Children.Add(CrearFila("Repeticiones:", "3", "Veces"));
                    break;

                case TipoBloque.SiLDR:
                    panel.Children.Add(CrearFila("Umbral (0-1023):", "400", "Umbral"));
                    break;

                case TipoBloque.SiMicrofono:
                    panel.Children.Add(CrearFila("Umbral (0-1023):", "600", "Umbral"));
                    break;

                case TipoBloque.SiIR:
                    panel.Children.Add(CrearFila("Código IR:", "0xFF30CF", "Codigo"));
                    break;

                case TipoBloque.EnviarSerial:
                    panel.Children.Add(CrearFila("Mensaje:", "Hola DinBot", "Mensaje"));
                    break;
            }
        }

        private static StackPanel CrearFila(string etiqueta, string valorDefault, string tag)
        {
            var fila = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 0) };
            fila.Children.Add(new TextBlock
            {
                Text = etiqueta,
                Foreground = new SolidColorBrush(Colors.WhiteSmoke),
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 4, 0)
            });
            fila.Children.Add(new TextBox
            {
                Text = valorDefault,
                Tag = tag,
                Width = 75,
                FontSize = 11,
                Padding = new Thickness(3, 1, 3, 1),
                Background = new SolidColorBrush(Color.FromArgb(80, 0, 0, 0)),
                Foreground = Brushes.White,
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(0, 0, 0, 1)
            });
            return fila;
        }

        /// <summary>
        /// Extrae los datos del bloque para serialización o generación de código.
        /// </summary>
        public DatoBloque ObtenerDatos()
        {
            var datos = new DatoBloque
            {
                Tipo = Tipo.ToString(),
                PosX = Canvas.GetLeft(this),
                PosY = Canvas.GetTop(this)
            };

            foreach (var hijo in _panelParams.Children)
            {
                if (hijo is StackPanel fila)
                    foreach (var item in fila.Children)
                        if (item is TextBox tb && tb.Tag is string key)
                            datos.Parametros[key] = tb.Text;
            }
            return datos;
        }
    }
}