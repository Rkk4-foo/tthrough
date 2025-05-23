using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TThrough.mvvm.Models
{
    [PrimaryKey(nameof(IdUsuario))]
    public class Usuario : INotifyPropertyChanged
    {
        private string _nombrePublico;
        private byte[] _fotoPerfil;

        [Column(TypeName = "varchar(40)")]
        public string IdUsuario { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[] FotoPerfil
        {
            get => _fotoPerfil;
            set
            {
                if (_fotoPerfil != value)
                {
                    _fotoPerfil = value;
                    OnPropertyChanged(nameof(FotoPerfil));
                }
            }
        }

        [Column(TypeName = "varchar(25)")]
        public string NombreUsuario { get; set; }

        public string Contrasena { get; set; }

        [Column(TypeName = "varchar(25)")]
        public string NombrePublico
        {
            get => _nombrePublico;
            set
            {
                if (_nombrePublico != value)
                {
                    _nombrePublico = value;
                    OnPropertyChanged(nameof(NombrePublico));
                }
            }
        }

        public DateTime FechaRegistro { get; set; }
        public DateTime UltimoLogin { get; set; }

        public ICollection<LlamadaUsuario> LlamadasUsuario { get; set; } = new List<LlamadaUsuario>();
        public ICollection<MensajeUsuario> MensajeUsuario { get; set; } = new List<MensajeUsuario>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
