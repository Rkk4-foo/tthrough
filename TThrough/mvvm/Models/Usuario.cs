using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TThrough.mvvm.Models
{
    [PrimaryKey(nameof(IdUsuario))]
    public class Usuario
    {
        [Column(TypeName="varchar(40)")]
        public string IdUsuario { get; set; }

        [Column(TypeName ="varbinary(max)")]
        public required byte[] FotoPerfil { get; set; }

        [Column(TypeName = "varchar(25)")]
        public required string NombreUsuario { get; set; }
        

        public required string Contrasena { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string NombrePublico { get; set; } = null!;
        public DateTime FechaRegistro { get; set; }
        public DateTime UltimoLogin { get; set; }

        public ICollection<LlamadaUsuario> LlamadasUsuario { get; set; } = new List<LlamadaUsuario>();

        public ICollection<MensajeUsuario> MensajeUsuario { get;set; } = new List<MensajeUsuario>();

    }
}
