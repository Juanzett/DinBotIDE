using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Lógica de drag & drop sobre el canvas principal de bloques.
    /// </summary>
    public static class BlockCanvas
    {
        private static UIElement? _elementoArrastrado;
        private static Point _offsetArrastre;

        /// <summary>
        /// Habilita el arrastre sobre un elemento del canvas.
        /// </summary>
        public static void HabilitarArrastre(UIElement elemento)
        {
            elemento.MouseLeftButtonDown += Elemento_MouseLeftButtonDown;
            elemento.MouseMove           += Elemento_MouseMove;
            elemento.MouseLeftButtonUp   += Elemento_MouseLeftButtonUp;
        }

        private static void Elemento_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement el)
            {
                _elementoArrastrado = el;
                var canvas = VisualTreeHelper.GetParent(el) as Canvas;
                if (canvas is not null)
                {
                    _offsetArrastre = e.GetPosition(canvas);
                    _offsetArrastre.X -= Canvas.GetLeft(el);
                    _offsetArrastre.Y -= Canvas.GetTop(el);
                    el.CaptureMouse();
                }
            }
        }

        private static void Elemento_MouseMove(object sender, MouseEventArgs e)
        {
            if (_elementoArrastrado is null || e.LeftButton != MouseButtonState.Pressed) return;
            var canvas = VisualTreeHelper.GetParent(_elementoArrastrado) as Canvas;
            if (canvas is null) return;

            var pos = e.GetPosition(canvas);
            double x = Math.Max(0, Math.Min(pos.X - _offsetArrastre.X, canvas.ActualWidth  - 20));
            double y = Math.Max(0, Math.Min(pos.Y - _offsetArrastre.Y, canvas.ActualHeight - 20));

            Canvas.SetLeft(_elementoArrastrado, x);
            Canvas.SetTop(_elementoArrastrado,  y);
        }

        private static void Elemento_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _elementoArrastrado?.ReleaseMouseCapture();
            _elementoArrastrado = null;
        }
    }
}