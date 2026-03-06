using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
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

        private BlockControl? _dragging;
        private Point         _dragOffset;

        private readonly List<BlockControl> _blocks = new();

        public BlockCanvas(Canvas canvas, TextBox codeOutput, TextBlock blockCounter)
        {
            _canvas       = canvas;
            _codeOutput   = codeOutput;
            _blockCounter = blockCounter;
        }

        // ── Drag & Drop ────────────────────────────────────────────────

        public void HandleDrop(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("BlockType")) return;

            var type = (BlockType)e.Data.GetData("BlockType");
            var pos  = e.GetPosition(_canvas);

            AddBlock(type, pos);
        }

        public void AddBlock(BlockType type, Point position)
        {
            var block = new BlockControl(type);
            block.MouseLeftButtonDown += Block_MouseDown;
            block.MouseLeftButtonUp   += Block_MouseUp;
            block.MouseMove           += Block_MouseMove;
            block.DeleteRequested     += () => RemoveBlock(block);

            Canvas.SetLeft(block, position.X);
            Canvas.SetTop(block, position.Y);

            _canvas.Children.Add(block);
            _blocks.Add(block);

            RefreshCode();
            UpdateCounter();
        }

        private void RemoveBlock(BlockControl block)
        {
            _canvas.Children.Remove(block);
            _blocks.Remove(block);
            RefreshCode();
            UpdateCounter();
        }

        private void Block_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _dragging   = (BlockControl)sender;
            _dragOffset = e.GetPosition(_dragging);
            _dragging.CaptureMouse();
        }

        private void Block_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragging?.ReleaseMouseCapture();
            _dragging = null;
        }

        private void Block_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging is null || e.LeftButton != MouseButtonState.Pressed) return;

            var pos = e.GetPosition(_canvas);
            Canvas.SetLeft(_dragging, pos.X - _dragOffset.X);
            Canvas.SetTop(_dragging, pos.Y - _dragOffset.Y);
        }

        // ── Programa ───────────────────────────────────────────────────

        public BlockProgram GetProgram()
        {
            var items = _blocks
                .OrderBy(b => Canvas.GetTop(b))
                .Select(b => b.ToBlockItem())
                .ToList();

            return new BlockProgram { Blocks = items };
        }

        public void LoadProgram(BlockProgram program)
        {
            Clear();
            double y = 20;
            foreach (var item in program.Blocks)
            {
                AddBlock(item.Type, new Point(20, y));
                y += 70;
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
            => _codeOutput.Text = ArduinoCodeGenerator.Generate(GetProgram());

        private void UpdateCounter()
            => _blockCounter.Text = $"Bloques: {_blocks.Count}";
    }
}