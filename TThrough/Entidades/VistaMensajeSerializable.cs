using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TThrough.Entidades
{

    /// <summary>
    /// Clase para serializar los mensajes y guardarlos en ficheros JSON
    /// </summary>
    public class VistaMensajeSerializable
    {
        public string NombrePublico { get; set; }

        public string CuerpoMensaje { get; set; }

        public string FtPerfil { get; set; }
    }
}
