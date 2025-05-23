using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace TThrough.mvvm.Models
{
    [PrimaryKey(nameof(IdChat))]
    public class Chats : INotifyPropertyChanged
    {
        private string _nombreChat;
        private byte[] _fotoChat;

        [Column(TypeName = "varchar(40)")]
        public string IdChat { get; set; }

        [Column(TypeName = "varchar(40)")]
        public string NombreChat
        {
            get => _nombreChat;
            set
            {
                if (_nombreChat != value)
                {
                    _nombreChat = value;
                    OnPropertyChanged(nameof(NombreChat));
                }
            }
        }

        [Column(TypeName = "varbinary(max)")]
        public byte[] FotoChat
        {
            get => _fotoChat;
            set
            {
                if (_fotoChat != value)
                {
                    _fotoChat = value;
                    OnPropertyChanged(nameof(FotoChat));
                }
            }
        }

        [NotMapped]
        public Usuario UsuarioAmigo { get; set; }

        public DateTime FechaInicioChat { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
