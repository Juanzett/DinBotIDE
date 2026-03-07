using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace DinBotIDE.SerialMonitor
{
    /// <summary>
    /// Monitor serial para leer/escribir datos desde/hacia el DinBot.
    /// </summary>
    public class SerialMonitor : IDisposable
    {
        private SerialPort? _puerto;
        private CancellationTokenSource? _cts;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public event EventHandler<string>? DatoRecibido;
        public event EventHandler<string>? ErrorOcurrido;

        public bool Conectado => _puerto?.IsOpen == true;
        public string? PuertoActual => _puerto?.PortName;

        public void Conectar(string nombrePuerto, int baudRate = 115200)
        {
            Desconectar();
            _puerto = new SerialPort(nombrePuerto, baudRate)
            {
                ReadTimeout  = 500,
                WriteTimeout = 500,
                NewLine      = "\n",
                DtrEnable    = true,
                RtsEnable    = true
            };
            _puerto.Open();

            DatoRecibido?.Invoke(this,
                $"── Conectado a {nombrePuerto} @ {baudRate} baud ──");

            _cts = new CancellationTokenSource();
            _ = LeerLoop(_cts.Token);
        }

        public void Desconectar()
        {
            _cts?.Cancel();
            try
            {
                if (_puerto?.IsOpen == true)
                    _puerto.Close();
            }
            catch (Exception) { /* ignorar errores al cerrar */ }
            _puerto?.Dispose();
            _puerto = null;
        }

        public async void Enviar(string mensaje)
        {
            if (!Conectado) return;

            await _lock.WaitAsync();
            try
            {
                await Task.Run(() => _puerto!.WriteLine(mensaje));
                DatoRecibido?.Invoke(this, $">> {mensaje}");
            }
            catch (Exception ex)
            {
                ErrorOcurrido?.Invoke(this, $"Error al enviar: {ex.Message}");
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task LeerLoop(CancellationToken ct)
        {
            var buffer = new byte[1024];
            var sb = new StringBuilder();

            while (!ct.IsCancellationRequested && Conectado)
            {
                await _lock.WaitAsync(ct);
                try
                {
                    if (!Conectado) break;

                    var stream = _puerto!.BaseStream;
                    int bytesLeidos = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    if (bytesLeidos == 0) continue;

                    sb.Append(Encoding.ASCII.GetString(buffer, 0, bytesLeidos));

                    string contenido = sb.ToString();
                    int idx;
                    while ((idx = contenido.IndexOf('\n')) >= 0)
                    {
                        var linea = contenido[..idx].TrimEnd('\r');
                        if (!string.IsNullOrEmpty(linea))
                            DatoRecibido?.Invoke(this, linea);
                        contenido = contenido[(idx + 1)..];
                    }
                    sb.Clear();
                    sb.Append(contenido);
                }
                catch (OperationCanceledException) { break; }
                catch (IOException)
                {
                    // El envío interrumpió la lectura — reintentar
                    await Task.Delay(50, CancellationToken.None);
                    continue;
                }
                catch (Exception ex)
                {
                    ErrorOcurrido?.Invoke(this, ex.Message);
                    break;
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        public void Dispose()
        {
            Desconectar();
            _lock.Dispose();
        }
    }
}