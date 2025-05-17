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
    [PrimaryKey(nameof(IdMensaje))]
    public class Mensaje
    {
        [Column(TypeName = "varchar(40)")]
        public string IdMensaje { get; set; }

        public required DateTime FechaEnvio { get; set; }

        public required DateTime HoraEnvio { get; set; }

        [Column(TypeName = "varchar(40)")]
        public required string IdChat { get; set; }

        [ForeignKey(nameof(IdChat))]
        public Chats Chat { get; set; } = null!;

        public ICollection<MensajeUsuario> MensajesUsuarios { get; set; } = new List<MensajeUsuario>();
    }
}
