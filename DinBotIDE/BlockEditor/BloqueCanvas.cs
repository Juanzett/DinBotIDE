using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DinBotIDE.BlockEditor
{
    /// <summary>
    /// ViewModel del canvas de bloques.
    /// Mantiene la lista observable de bloques colocados en pantalla.
    /// </summary>
    public class BloqueCanvas : INotifyPropertyChanged
    {
        public ObservableCollection<BloqueInstancia> Bloques { get; } = new();

        public void Agregar(BloqueInstancia bloque) => Bloques.Add(bloque);
        public void Eliminar(BloqueInstancia bloque) => Bloques.Remove(bloque);
        public void Limpiar()                        => Bloques.Clear();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}