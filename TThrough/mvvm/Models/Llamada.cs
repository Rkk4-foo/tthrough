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
    [PrimaryKey(nameof(IdLlamada))]
    public class Llamada
    {
        [Column(TypeName = "varchar(40)")]
        public string IdLlamada { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        public ICollection<LlamadaUsuario> LlamadasUsuario { get; set; } = new List<LlamadaUsuario>();
    }
}
