using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DinBotIDE.CodeGenerator;
using DinBotIDE.Models;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// Gestiona el canvas de programación: drag & drop, renderizado y extracción del programa.
    /// </summary>
    public class BlockCanvas
    {
        private readonly Canvas    _canvas;
        private readonly TextBox   _codeOutput;
        private readonly TextBlock _blockCounter;

        private readonly List<BloqueVisual> _blocks = new();

        public BlockCanvas(Canvas canvas, TextBox codeOutput, TextBlock blockCounter)
        {
            _canvas       = canvas;
            _codeOutput   = codeOutput;
            _blockCounter = blockCounter;
        }

        // ── Drag & Drop ────────────────────────────────────────────────

        public void HandleDrop(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("TipoBloque")) return;

            var type = (TipoBloque)e.Data.GetData("TipoBloque");
            var pos  = e.GetPosition(_canvas);

            AddBlock(type, pos);
        }

        public void AddBlock(TipoBloque type, Point position)
        {
            var block = new BloqueVisual(type);
            block.EliminacionSolicitada += b => RemoveBlock(b);

            Canvas.SetLeft(block, position.X);
            Canvas.SetTop(block, position.Y);

            _canvas.Children.Add(block);
            _blocks.Add(block);

            RefreshCode();
            UpdateCounter();
        }

        public void RemoveBlock(BloqueVisual block)
        {
            _canvas.Children.Remove(block);
            _blocks.Remove(block);
            RefreshCode();
            UpdateCounter();
        }

        // ── Programa ───────────────────────────────────────────────────

        public List<DatoBloque> GetProgramData()
        {
            return _blocks
                .OrderBy(b => Canvas.GetTop(b))
                .Select(b => b.ObtenerDatos())
                .ToList();
        }

        public BlockProgram GetProgram()
        {
            var items = _blocks
                .OrderBy(b => Canvas.GetTop(b))
                .Select(b =>
                {
                    var datos = b.ObtenerDatos();
                    return new BloqueGuardado
                    {
                        Tipo = b.Tipo,
                        X = datos.PosX,
                        Y = datos.PosY,
                        Parametros = datos.Parametros
                    };
                })
                .ToList();

            return new BlockProgram { Bloques = items };
        }

        public void LoadProgram(BlockProgram program)
        {
            Clear();
            foreach (var item in program.Bloques)
            {
                AddBlock(item.Tipo, new Point(item.X, item.Y));
            }
        }

        public void Clear()
        {
            _canvas.Children.Clear();
            _blocks.Clear();
            _codeOutput.Text = string.Empty;
            UpdateCounter();
        }

        // ── Helpers ────────────────────────────────────────────────────

        private void RefreshCode()
            => _codeOutput.Text = ArduinoCodeGenerator.Generar(GetProgramData());

        private void UpdateCounter()
            => _blockCounter.Text = $"Bloques: {_blocks.Count}";
    }
}