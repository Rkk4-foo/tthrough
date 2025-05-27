using Microsoft.EntityFrameworkCore;
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

namespace TThrough.mvvm.ViewModel
{
    public class PopUpSolicitudesPendientesViewModel : ViewModelBase
    {
        #region Propiedades
        private Servicios.ServicioTCP tcp;

        private readonly TalkthroughContext _context;

        private Models.Usuario _usuarioConectado;


        public Action? SolicitudAceptada;
        public Action<Models.Chats> ChatCreado { get; set; }

        public ICommand añadirUser => new RelayCommand(
            _ => AceptarSolicitud(),
            _ => true);

        public ICommand rechazarUser => new RelayCommand(
            _ => RechazarSolicitud(),
            _ => true);

        public ObservableCollection<VistaSolicitudes> Solicitudes { get; set; }

        private VistaSolicitudes _solicitudSeleccionada { get; set; }

        public VistaSolicitudes SolicitudSeleccionada
        {
            get { return _solicitudSeleccionada; }
            set { _solicitudSeleccionada = value; OnPropertyChanged(); }
        }

        public Action? CerrarPopupAction { get; set; }

        #endregion


        #region Constructores
        public PopUpSolicitudesPendientesViewModel(Models.Usuario u, Servicios.ServicioTCP tcp)
        {
            Solicitudes = new();
            _context = TalkthroughContextFactory.SendContextFactory();
            _usuarioConectado = u;
            CargarSolicitudes();
            this.tcp = tcp;
        }
        #endregion

        #region Methods

        private void CargarSolicitudes()
        {
            Solicitudes.Clear();

            var listaSolicitudes = _context.Amigos.Where(x => x.IdUsuarioRemitente == _usuarioConectado.IdUsuario && !x.SolicitudAceptada).Select(x => x.UsuarioPeticion).ToList();

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
            if (SolicitudSeleccionada != null)
            {
                var usuario = _context.Usuarios.SingleOrDefault(x => x.NombreUsuario == SolicitudSeleccionada.NombreUsuario);

                if (usuario != null)
                {
                    var solicitud = _context.Amigos.SingleOrDefault(x => x.IdUsuarioEnvio == usuario.IdUsuario && x.IdUsuarioRemitente == _usuarioConectado.IdUsuario);
                    if (solicitud != null)
                    {
                        _context.Amigos.Remove(solicitud);
                        _context.SaveChanges();
                    }

                    Solicitudes.Remove(SolicitudSeleccionada);
                    if (!Solicitudes.Any())
                        CerrarPopupAction?.Invoke();
                }
            }
        }
        #endregion

        private void AceptarSolicitud()
        {
            var usuario = _context.Usuarios.SingleOrDefault(x => x.NombreUsuario == SolicitudSeleccionada.NombreUsuario);
            if (usuario == null) return;

            var solicitud = _context.Amigos.SingleOrDefault(x => x.IdUsuarioEnvio == usuario.IdUsuario && x.IdUsuarioRemitente == _usuarioConectado.IdUsuario);
            if (solicitud != null)
            {
                solicitud.SolicitudAceptada = true;
                _context.Amigos.Update(solicitud);
            }

            var nuevoChat = new Models.Chats
            {
                IdChat = Guid.NewGuid().ToString(),
                NombreChat = usuario.NombreUsuario,
                FotoChat = usuario.FotoPerfil,
                FechaInicioChat = DateTime.Now,
            };


            _context.Chats.Add(nuevoChat);

            _context.ChatsUsuarios.Add(new Models.ChatsUsuarios { IdChat = nuevoChat.IdChat, IdUsuario = usuario.IdUsuario });
            _context.ChatsUsuarios.Add(new Models.ChatsUsuarios { IdChat = nuevoChat.IdChat, IdUsuario = _usuarioConectado.IdUsuario });

            _context.SaveChanges();
            ChatCreado?.Invoke(nuevoChat);
            AceptarSolicitud(usuario,nuevoChat);
            Solicitudes.Remove(SolicitudSeleccionada);

            if (!Solicitudes.Any())
                CerrarPopupAction?.Invoke();
        }

        private async void AceptarSolicitud(Usuario usuarioSeleccionado, Chats chat)
        {

            var chatEnviar = new Chats
            {
                IdChat = chat.IdChat,
                NombreChat = usuarioSeleccionado.NombreUsuario,
                FechaInicioChat = chat.FechaInicioChat,
            };

            var mensaje = new MensajeJson
            {
                Tipo = "solicitud_aceptada",
                Emisor = _usuarioConectado.IdUsuario,
                Receptor = usuarioSeleccionado.IdUsuario,
                Datos = chatEnviar,
            };

            SolicitudAceptada?.Invoke();

            var json = JsonSerializer.Serialize(mensaje);

            await tcp.EnviarMensaje(json);
        }

    }

}
