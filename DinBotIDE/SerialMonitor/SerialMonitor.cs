using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace DinBotIDE.SerialMonitor
{
    /// <summary>
    /// Monitor serial para leer/escribir datos desde/hacia el DinBot.
    /// </summary>
    public class SerialMonitor : IDisposable
    {
        private SerialPort? _puerto;
        private CancellationTokenSource? _cts;

        public event EventHandler<string>? DatoRecibido;
        public event EventHandler<string>? ErrorOcurrido;

        public bool Conectado => _puerto?.IsOpen == true;

        public void Conectar(string nombrePuerto, int baudRate = 115200)
        {
            Desconectar();
            _puerto = new SerialPort(nombrePuerto, baudRate)
            {
                ReadTimeout  = 500,
                WriteTimeout = 500,
                NewLine      = "\n"
            };
            _puerto.Open();
            _cts = new CancellationTokenSource();
            _ = LeerLoop(_cts.Token);
        }

        public void Desconectar()
        {
            _cts?.Cancel();
            if (_puerto?.IsOpen == true)
                _puerto.Close();
            _puerto?.Dispose();
            _puerto = null;
        }

        public void Enviar(string mensaje)
        {
            if (!Conectado) return;
            _puerto!.WriteLine(mensaje);
        }

        private async Task LeerLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested && Conectado)
            {
                try
                {
                    var linea = await Task.Run(() => _puerto!.ReadLine(), ct);
                    DatoRecibido?.Invoke(this, linea);
                }
                catch (TimeoutException) { /* normal */ }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    ErrorOcurrido?.Invoke(this, ex.Message);
                    break;
                }
            }
        }

        public void Dispose() => Desconectar();
    }
}