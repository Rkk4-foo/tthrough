using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TThrough.mvvm.Models
{
    [PrimaryKey(nameof(IdChat),nameof(IdUsuario))]
    public class ChatsUsuarios
    {
        [ForeignKey("Chat"), Column(Order = 1, TypeName = "varchar(40)")]
        public string IdChat { get; set; }

        [ForeignKey("Usuario"), Column(Order = 1, TypeName = "varchar(40)")]
        public string IdUsuario { get; set; }

        public Models.Usuario Usuario { get; set; }

        public Models.Chats Chat {  get; set; }
    }
}
