using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TThrough.mvvm.Models
{
    [PrimaryKey(nameof(IdChat))]
    public class Chats
    {
        [Column(TypeName = "varchar(40)")]
        public string IdChat { get; set; }

        [Column(TypeName="varchar(40)")]
        public string NombreChat { get; set; }

        [Column(TypeName = "varbinary(max)")]

        public byte[] FotoChat { get; set; }

        public DateTime FechaInicioChat { get; set; }


    }
}
