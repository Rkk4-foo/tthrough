using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using TThrough.data;
using TThrough.Entidades;
using TThrough.mvvm.Models;
using TThrough.Servicios;

namespace TThrough.mvvm.ViewModel
{
    public class PopUpAñadirAmigosViewModel : ViewModelBase
    {

        #region Properties

        private ServicioTCP TCP;

        private readonly Usuario _usuarioConectadoLocal;

        public Action? CerrarPopup;
        public ICommand ComandoAñadir => new RelayCommand(
            _ => AñadirAmigo(),
            _ => true);

        public ICommand ComandoBuscar => new RelayCommand(
            _ => BusquedaUsuarios(),
            _ => true);

        private TalkthroughContext _context { get; set; }



        private string _textoBusqueda { get; set; }

        public string Busqueda
        {
            get { return _textoBusqueda; }
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
            }
        }
        private Models.Usuario _usuarioSeleccionado { get; set; }

        public Models.Usuario UsuarioSeleccionado
        {
            get { return _usuarioSeleccionado; }
            set { _usuarioSeleccionado = value; OnPropertyChanged(); }
        }

        private bool _isChecked { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }

            set { _isChecked = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Models.Usuario> _usuarios { get; set; }

        public ObservableCollection<Models.Usuario> Usuarios
        {
            get { return _usuarios; }
            set
            {
                _usuarios = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Constructores

        public PopUpAñadirAmigosViewModel(TalkthroughContext c, Usuario u,ServicioTCP sTCP)
        {
            Usuarios = new ObservableCollection<Models.Usuario>();
            _context = c;
            _usuarioConectadoLocal = u;
            TCP = sTCP;
        }

        #endregion

        #region Metodos

        private void BusquedaUsuarios()
        {
            Usuarios.Clear();
            if (!Busqueda.IsNullOrEmpty())
            {
                List<Models.Usuario> usuarios = _context.Usuarios.Where(x => x.NombreUsuario.Contains(Busqueda))
                                                .ToList();

                Usuarios.Clear();
                foreach (var usuario in usuarios)
                {
                    if (usuario.IdUsuario != _usuarioConectadoLocal.IdUsuario)
                        Usuarios.Add(usuario);
                }
            }
            else Usuarios.Clear();
        }

        private void AñadirAmigo()
        {
            bool amistadExistente = _context.Amigos.Any(a =>
                                    (a.IdUsuarioEnvio == _usuarioConectadoLocal.IdUsuario && a.IdUsuarioRemitente == UsuarioSeleccionado.IdUsuario) ||
                                    (a.IdUsuarioEnvio == UsuarioSeleccionado.IdUsuario && a.IdUsuarioRemitente == UsuarioSeleccionado.IdUsuario));

            

            var UsuarioRemitente = _context.Usuarios.Single(a => a.IdUsuario == UsuarioSeleccionado.IdUsuario);

            if (!amistadExistente)
            {
                var peticion = new Amigos
                {
                    IdUsuarioEnvio = _usuarioConectadoLocal.IdUsuario,
                    IdUsuarioRemitente = UsuarioSeleccionado.IdUsuario,
                    UsuarioPeticion = _usuarioConectadoLocal,
                    UsuarioRemitente = UsuarioRemitente,
                    SolicitudAceptada = false
                };

                _context.Amigos.Add(peticion);

                EnviarSolicitud();

                _context.SaveChangesAsync();
                CerrarPopup?.Invoke();

            }
        }


        private async void EnviarSolicitud() 
        {
            var mensaje = new MensajeJson
            {
                Tipo = "solicitud_amistad",
                Emisor = _usuarioConectadoLocal.IdUsuario,
                Receptor = UsuarioSeleccionado.IdUsuario
            };

            var json = JsonSerializer.Serialize(mensaje);

            await TCP.EnviarMensaje(json);
        }

        
        #endregion

    }
}