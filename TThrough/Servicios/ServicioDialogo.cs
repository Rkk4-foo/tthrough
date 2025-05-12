using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TThrough.Servicios
{
    public class ServicioDialogo : Interfaces.InterfazDialogo
    {
        public void MostrarDialogo(string titulo,string cuerpo)
        {
            TThrough.mvvm.View.DialogoPersonalizado.Mostrar(titulo,cuerpo);
        }
    }
}
