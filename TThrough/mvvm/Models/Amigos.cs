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
    [PrimaryKey(nameof(IdUsuarioEnvio),nameof(IdUsuarioRemitente))]
    public class Amigos
    {
        [ForeignKey("UsuarioPeticion"), Column(Order = 1, TypeName = "varchar(40)")]

        public string IdUsuarioEnvio { get; set; }

        [ForeignKey("UsuarioRemitente"), Column(Order = 2, TypeName = "varchar(40)")]

        public string IdUsuarioRemitente { get; set; }
        
        public bool SolicitudAceptada { get; set; }

        
        public required Usuario UsuarioPeticion {  get; set; }

        
        public required Usuario UsuarioRemitente {  set; get; }
    }
}
