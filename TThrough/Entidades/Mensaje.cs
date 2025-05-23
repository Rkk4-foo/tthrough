using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TThrough.Entidades
{
    public class MensajeJson
    {


        public string Tipo { get; set; }          //Funciona como cabecera para que el servidor sepa como manejar el mensaje
        public string Emisor { get; set; }

        public string Receptor { get; set; } 
        public string ChatId { get; set; }
        public object Datos { get; set; }

    }
}
