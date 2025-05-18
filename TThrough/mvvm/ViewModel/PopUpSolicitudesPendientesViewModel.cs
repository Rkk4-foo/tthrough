using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TThrough.data;
using TThrough.Entidades;

namespace TThrough.mvvm.ViewModel
{
    public class PopUpSolicitudesPendientesViewModel : ViewModelBase
    {
        #region Propiedades
        private readonly TalkthroughContext _context;

        private Models.Usuario _usuarioConectado;

        public Action<Models.Chats> ChatCreado {  get; set; }

        public ICommand añadirUser => new RelayCommand(
            _=> AceptarSolicitud(),
            _=> true);

        public ICommand rechazarUser => new RelayCommand(
            _ => RechazarSolicitud(),
            _ => true);

        public List<VistaSolicitudes> Solicitudes { get; set; }

        private VistaSolicitudes _solicitudSeleccionada { get; set; }

        public VistaSolicitudes SolicitudSeleccionada
        {
            get { return _solicitudSeleccionada; }
            set { _solicitudSeleccionada = value; OnPropertyChanged(); }
        }

        public Action? CerrarPopupAction { get; set; }

        #endregion


        #region Constructores
        public PopUpSolicitudesPendientesViewModel(Models.Usuario u)
        {
            Solicitudes = new List<VistaSolicitudes>();
            _context = TalkthroughContextFactory.SendContextFactory();
            _usuarioConectado = u;
            CargarSolicitudes();
        }
        #endregion

        #region Methods

        private void CargarSolicitudes()
        {
            Solicitudes.Clear();

            var listaSolicitudes = _context.Amigos.Where(x => x.IdUsuarioRemitente == _usuarioConectado.IdUsuario).Select(x => x.UsuarioPeticion).ToList();

            foreach (var usuario in listaSolicitudes)
            {
                Solicitudes.Add(new VistaSolicitudes()
                {
                    NombreUsuario = usuario.NombreUsuario,
                    FtPerfil = usuario.FotoPerfil,
                });
            }
        }


        private void RechazarSolicitud()
        {
            var usuario = _context.Usuarios.Single(x => x.NombreUsuario == SolicitudSeleccionada.NombreUsuario);

            if (usuario != null)
            {
                var solicitud = _context.Amigos.Single(x => x.IdUsuarioEnvio == usuario.IdUsuario);
                _context.Amigos.Remove(solicitud);
                _context.SaveChanges();
            }

            Solicitudes.Clear();
            CerrarPopupAction?.Invoke();
        }
        #endregion

        private void AceptarSolicitud() 
        {
            var usuario = _context.Usuarios.Single(x => x.NombreUsuario == SolicitudSeleccionada.NombreUsuario);
            if (usuario != null) 
            {
                var solicitud = _context.Amigos.Single(x=>x.IdUsuarioEnvio ==usuario.IdUsuario);

                solicitud.SolicitudAceptada = true;

                _context.Amigos.Update(solicitud);
            }

            var nuevoChat = new Models.Chats
            {
                IdChat = Guid.NewGuid().ToString(),
                NombreChat = usuario.NombrePublico,
            };
            _context.Chats.Add(nuevoChat);

            // Añadir a tabla intermedia de usuarios y chats
            _context.ChatsUsuarios.Add(new Models.ChatsUsuarios
            {
                IdChat = nuevoChat.IdChat,
                IdUsuario = usuario.IdUsuario 
            });

            _context.ChatsUsuarios.Add(new Models.ChatsUsuarios
            {
                IdChat = nuevoChat.IdChat,
                IdUsuario = _usuarioConectado.IdUsuario // tú
            });

            _context.SaveChanges();

            
            ChatCreado?.Invoke(nuevoChat);
            Solicitudes.Clear();
            CerrarPopupAction?.Invoke();
        }

        
    
    }
    
}
