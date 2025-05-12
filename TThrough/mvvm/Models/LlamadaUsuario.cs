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

    [PrimaryKey(nameof(IdUsuario),nameof(IdLlamada))]
    public class LlamadaUsuario
    {
        [Column(Order = 1)]
        public string IdUsuario { get; set; }

        [Column(Order = 2)]
        public string IdLlamada { get; set; }

        public  DateTime FechaLlamada { get; set; } 

        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey(nameof(IdLlamada))]
        public Llamada Llamada { get; set; } = null!;
    }
}
