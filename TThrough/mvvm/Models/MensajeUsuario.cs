using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TThrough.mvvm.Models
{

    [PrimaryKey(nameof(IdMensaje),nameof(IdUsuario))]
    public class MensajeUsuario
    {

        [ForeignKey("Mensaje"), Column(Order = 1)]

        public string IdMensaje { get; set; }

        [ForeignKey("Usuario"), Column(Order = 2)]
        public string IdUsuario { get; set; }

        public required Usuario Usuario { get; set; }

        public required Mensaje Mensaje { get; set; }
    }
}
