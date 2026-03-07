using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DinBotIDE.ArduinoCLI;
using DinBotIDE.BlockEditor;
using DinBotIDE.Models;
using DinBotIDE.SerialMonitor;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;

namespace DinBotIDE
{
    /// <summary>
    /// Ventana principal del IDE DinBot.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BlockCanvas       _blockCanvas;
        private readonly ArduinoRunner     _arduinoRunner = new();
        private readonly BoardDetector     _boardDetector = new();
        private readonly SerialMonitor.SerialMonitor _serialMonitor = new();

        private string? _archivoActual;

        public MainWindow()
        {
            InitializeComponent();

            _blockCanvas = new BlockCanvas(CanvasBloques, TxtCodigoGenerado, LblBloques);

            CargarPaleta();
            RefrescarPuertos();

            _serialMonitor.DatoRecibido += (_, dato) =>
                Dispatcher.Invoke(() => TxtSalidaSerial.AppendText(dato + "\n"));

            _serialMonitor.ErrorOcurrido += (_, error) =>
                Dispatcher.Invoke(() => ActualizarEstado($"Error serial: {error}"));
        }

        // ══════════════════════════════════════════════════════════════
        //  PALETA DE BLOQUES
        // ══════════════════════════════════════════════════════════════

        private void CargarPaleta()
        {
            PanelPaleta.Children.Clear();
            foreach (var (titulo, bloques) in BlockFactory.GetGrupos())
            {
                // Título del grupo
                PanelPaleta.Children.Add(new TextBlock
                {
                    Text = titulo,
                    Foreground = System.Windows.Media.Brushes.White,
                    FontWeight = FontWeights.Bold,
                    FontSize = 13,
                    Margin = new Thickness(0, 10, 0, 4)
                });

                foreach (var tipo in bloques)
                {
                    var btn = BlockFactory.CrearBotonPaleta(tipo);
                    btn.PreviewMouseLeftButtonDown += PaletaBtn_PreviewMouseDown;
                    PanelPaleta.Children.Add(btn);
                }
            }
        }

        private void PaletaBtn_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button btn && btn.Tag is TipoBloque tipo)
            {
                var data = new DataObject("TipoBloque", tipo);
                DragDrop.DoDragDrop(btn, data, DragDropEffects.Copy);
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  CANVAS — DRAG & DROP
        // ══════════════════════════════════════════════════════════════

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            _blockCanvas.HandleDrop(e);
        }

        private void Canvas_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent("TipoBloque")
                ? DragDropEffects.Copy
                : DragDropEffects.None;
            e.Handled = true;
        }

        // ══════════════════════════════════════════════════════════════
        //  ARCHIVO: NUEVO / ABRIR / GUARDAR
        // ══════════════════════════════════════════════════════════════

        private void NuevoPrograma_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Crear un programa nuevo? Se perderán los cambios no guardados.",
                "Nuevo programa", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _blockCanvas.Clear();
                _archivoActual = null;
                ActualizarEstado("Programa nuevo creado");
            }
        }

        private void AbrirPrograma_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Programa DinBot (*.dinbot)|*.dinbot|JSON (*.json)|*.json",
                DefaultExt = ".dinbot"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var json = File.ReadAllText(dlg.FileName);
                    var programa = JsonConvert.DeserializeObject<BlockProgram>(json);
                    if (programa != null)
                    {
                        _blockCanvas.LoadProgram(programa);
                        _archivoActual = dlg.FileName;
                        ActualizarEstado($"Abierto: {Path.GetFileName(dlg.FileName)}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al abrir: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void GuardarPrograma_Click(object sender, RoutedEventArgs e)
        {
            if (_archivoActual == null)
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "Programa DinBot (*.dinbot)|*.dinbot|JSON (*.json)|*.json",
                    DefaultExt = ".dinbot",
                    FileName = "mi_programa.dinbot"
                };
                if (dlg.ShowDialog() != true) return;
                _archivoActual = dlg.FileName;
            }

            try
            {
                var programa = _blockCanvas.GetProgram();
                var json = JsonConvert.SerializeObject(programa, Formatting.Indented);
                File.WriteAllText(_archivoActual, json);
                ActualizarEstado($"Guardado: {Path.GetFileName(_archivoActual)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ══════════════════════════════════════════════════════════════
        //  COMPILAR / SUBIR
        // ══════════════════════════════════════════════════════════════

        private async void Compilar_Click(object sender, RoutedEventArgs e)
        {
            var codigo = TxtCodigoGenerado.Text;
            if (string.IsNullOrWhiteSpace(codigo))
            {
                ActualizarEstado("No hay código para compilar. Agregá bloques al canvas.");
                return;
            }

            BtnCompilar.IsEnabled = false;
            ActualizarEstado("Compilando...");

            try
            {
                var resultado = await _arduinoRunner.CompilarAsync(codigo);
                ActualizarEstado("Compilación finalizada");
                BtnSubir.IsEnabled = true;
                TxtSalidaSerial.AppendText($"── Compilación ──\n{resultado}\n");
            }
            catch (Exception ex)
            {
                ActualizarEstado($"Error: {ex.Message}");
            }
            finally
            {
                BtnCompilar.IsEnabled = true;
            }
        }

        private async void SubirAlRobot_Click(object sender, RoutedEventArgs e)
        {
            var puerto = ComboPuertos.SelectedItem as string;
            if (string.IsNullOrEmpty(puerto))
            {
                ActualizarEstado("Seleccioná un puerto COM primero.");
                return;
            }

            BtnSubir.IsEnabled = false;
            ActualizarEstado($"Subiendo a {puerto}...");

            try
            {
                var resultado = await _arduinoRunner.SubirAsync(puerto);
                ActualizarEstado("¡Código subido al DinBot!");
                TxtSalidaSerial.AppendText($"── Carga ──\n{resultado}\n");
            }
            catch (Exception ex)
            {
                ActualizarEstado($"Error al subir: {ex.Message}");
            }
            finally
            {
                BtnSubir.IsEnabled = true;
            }
        }

        private void GenerarCodigo_Click(object sender, RoutedEventArgs e)
        {
            var datos = _blockCanvas.GetProgramData();
            TxtCodigoGenerado.Text = CodeGenerator.ArduinoCodeGenerator.Generar(datos);
            ActualizarEstado("Código regenerado");
        }

        // ══════════════════════════════════════════════════════════════
        //  PUERTOS COM
        // ══════════════════════════════════════════════════════════════

        private void RefrescarPuertos_Click(object sender, RoutedEventArgs e)
            => RefrescarPuertos();

        private void RefrescarPuertos()
        {
            ComboPuertos.Items.Clear();
            var puertos = _boardDetector.ObtenerPuertos();
            foreach (var p in puertos)
                ComboPuertos.Items.Add(p);

            if (ComboPuertos.Items.Count > 0)
                ComboPuertos.SelectedIndex = 0;

            LblPuerto.Text = puertos.Count > 0
                ? $"{puertos.Count} puerto(s) encontrado(s)"
                : "Sin conexión";
        }

        // ══════════════════════════════════════════════════════════════
        //  MONITOR SERIAL
        // ══════════════════════════════════════════════════════════════

        private void ToggleMonitorSerial_Click(object sender, RoutedEventArgs e)
        {
            // El monitor está siempre visible; este botón podría usarse
            // para expandir/contraer la columna en el futuro.
        }

        private void ConectarSerial_Click(object sender, RoutedEventArgs e)
        {
            if (_serialMonitor.Conectado)
            {
                _serialMonitor.Desconectar();
                BtnConectarSerial.Content = "🔌 Conectar";
                ActualizarEstado("Monitor serial desconectado");
                return;
            }

            var puerto = ComboPuertos.SelectedItem as string;
            if (string.IsNullOrEmpty(puerto))
            {
                ActualizarEstado("Seleccioná un puerto COM primero.");
                return;
            }

            try
            {
                _serialMonitor.Conectar(puerto);
                BtnConectarSerial.Content = "🔌 Desconectar";
                ActualizarEstado($"Conectado a {puerto}");
            }
            catch (Exception ex)
            {
                ActualizarEstado($"Error serial: {ex.Message}");
            }
        }

        private void EnviarSerial_Click(object sender, RoutedEventArgs e)
        {
            if (!_serialMonitor.Conectado)
            {
                ActualizarEstado("No hay conexión serial activa.");
                return;
            }

            var msg = TxtEntradaSerial.Text;
            if (string.IsNullOrWhiteSpace(msg)) return;

            _serialMonitor.Enviar(msg);
            TxtSalidaSerial.AppendText($">> {msg}\n");
            TxtEntradaSerial.Clear();
        }

        private void LimpiarSerial_Click(object sender, RoutedEventArgs e)
        {
            TxtSalidaSerial.Clear();
        }

        // ══════════════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════════════

        private void ActualizarEstado(string mensaje)
        {
            LblEstado.Text = mensaje;
        }

        protected override void OnClosed(EventArgs e)
        {
            _serialMonitor.Dispose();
            base.OnClosed(e);
        }
    }
}