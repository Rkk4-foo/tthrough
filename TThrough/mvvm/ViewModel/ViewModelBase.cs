using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string NombrePropiedad = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(NombrePropiedad));
    }

    protected bool SetProperty<T>(ref T campo, T valor, [CallerMemberName] string Propiedad = null)
    {
        if (Equals(campo, valor)) return false;
        campo = valor;
        OnPropertyChanged(Propiedad);
        return true;
    }
}
