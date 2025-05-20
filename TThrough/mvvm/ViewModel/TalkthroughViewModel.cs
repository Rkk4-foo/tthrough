using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Net.Sockets;
using TThrough.Servicios;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using TThrough.data;
using TThrough.Entidades;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;


namespace TThrough.mvvm.ViewModel
{
    public class TalkthroughViewModel : ViewModelBase
    {
        #region Properties

        public readonly TalkthroughContext context = TalkthroughContextFactory.SendContextFactory();

        public readonly string UsuarioConectadoActual;

        public Action? PopUpAmigosAction { get; set; }

        public Action? PopUpSolicitudesAction { get; set; }

        public Action? PopUpGruposAction { get; set; }
        public Action? VentanaConfigAbierta { get; set; }

        public ICommand BtnConfig => new RelayCommand(
            _ => { VentanaConfigAbierta?.Invoke(); },
            _=> true);

        public ICommand BtnGrupos => new RelayCommand(
            _ => { PopUpGruposAction?.Invoke(); },
            _ => true);

        public ICommand BtnSolicitudesPendientes => new RelayCommand(
            _ => { PopUpSolicitudesAction?.Invoke(); },
            _ => true);

        public ICommand BtnAñadirAmigos => new RelayCommand(_ =>
        {
            PopUpAmigosAction?.Invoke();
        },
        _ =>  true );
        
        public ICommand EnviarCommand => new RelayCommand(
             _ => EnviarMensaje(),
             _ => !string.IsNullOrWhiteSpace(Mensaje));

        private ServicioTCP _conexionTCP;

        

        private Models.Chats _selectedItem { get; set; }

        public Models.Chats SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private bool _solicitudesPendientes {  get; set; }

        public bool SolicitudesPendientes 
        {
            get { return _solicitudesPendientes; }
            set
            {
                _solicitudesPendientes = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Models.Chats> Chats { get; set; }

        public ObservableCollection<Models.Usuario> Usuarios { get; set; }

        public ObservableCollection<Models.Mensaje> Mensajes { get; set; }

        public ObservableCollection<VistaMensaje> ChatLineas { get; set; }

        private string _Mensaje;
        public string Mensaje
        {
            get => _Mensaje;
            set { _Mensaje = value; OnPropertyChanged(); }
        }

        private string _NombrePublico { get; set; }

        private byte[] _Imagen { get; set; }

        public byte[] Imagen { get { return _Imagen; } set { _Imagen = value; OnPropertyChanged(); } }

        public string NombrePublico
        {
            get { return _NombrePublico; }

            set { _NombrePublico = value; OnPropertyChanged(); }
        }

        #endregion


        #region Constructores

        public TalkthroughViewModel(ServicioTCP client,string nombreUsuarioConectado)
        {
            Usuarios = new ObservableCollection<Models.Usuario>();
            Chats = new ObservableCollection<Models.Chats>();
            Mensajes = new ObservableCollection<Models.Mensaje>();
            ChatLineas = new ObservableCollection<VistaMensaje>();
            UsuarioConectadoActual = nombreUsuarioConectado;
            InicializarServicio(client!);
            ComprobarSolicitudesPendientes();
            CargarChats();
        }


        #endregion

        #region Methods

        public void InicializarServicio(ServicioTCP tcp)
        {
            _conexionTCP = tcp;
            _conexionTCP.MensajeRecibido += EnMensajeRecibido;


            _ = _conexionTCP.RecibirMensajes(CancellationToken.None);
        }

        private void EnviarMensaje()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var aux = Mensaje;
                Mensaje = "";

                
                var datosMensajes = new Models.Mensaje()
                {
                    FechaEnvio = DateTime.Now,
                    HoraEnvio = DateTime.Now,
                    IdMensaje = Guid.NewGuid().ToString().ToLower(),
                    IdChat = SelectedItem.IdChat, 

                };


                var UsuarioSender = context.Usuarios.Single(u => u.NombrePublico == UsuarioConectadoActual);

                
                ChatLineas.Add(new VistaMensaje
                {
                    NombrePublico = UsuarioSender.NombrePublico,
                    CuerpoMensaje = aux,
                    FtPerfil = ConvertBytesToImage(UsuarioSender.FotoPerfil)
                });


                var usuariosChat = context.ChatsUsuarios
                    .Where(cu => cu.IdChat == SelectedItem.IdChat)
                    .Select(cu => cu.IdUsuario)
                    .ToList();

                foreach (var idUsuario in usuariosChat)
                {
                    var mensajeUsuario = new Models.MensajeUsuario
                    {
                        IdMensaje = datosMensajes.IdMensaje,
                        IdUsuario = idUsuario
                    };
                    context.MensajesUsuarios.Add(mensajeUsuario);
                }


                context.Mensajes.Add(datosMensajes);
                context.SaveChanges();


                _conexionTCP.EnviarMensaje(aux);
            });
        }
        private void EnMensajeRecibido(object? sender, string cuerpoMensaje)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var datosMensajes = new Models.Mensaje()
                {
                    FechaEnvio = DateTime.Now,
                    HoraEnvio = DateTime.Now,
                    IdMensaje = Guid.NewGuid().ToString().ToLower(),
                    IdChat = SelectedItem.IdChat,
                };

                Mensajes.Add(datosMensajes);
                ChatLineas.Add(new VistaMensaje
                {
                    NombrePublico = this.NombrePublico,
                    CuerpoMensaje = cuerpoMensaje,
                    FtPerfil = ConvertBytesToImage(Imagen)
                });
            });
        }

        public static ImageSource ConvertBytesToImage(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public void ComprobarSolicitudesPendientes() 
        {
            var usuarioConectado = context.Usuarios.SingleOrDefault(x => x.NombreUsuario == UsuarioConectadoActual);

            if (usuarioConectado == null)
                return;

            SolicitudesPendientes = context.Amigos.Any(x => x.IdUsuarioRemitente == usuarioConectado.IdUsuario && !x.SolicitudAceptada);
        }

        public void CargarChats() 
        {
            var usuario = context.Usuarios.Single(x=>x.NombreUsuario == UsuarioConectadoActual);

            
            var amigosIds = context.Amigos
                .Where(a =>
                    (a.IdUsuarioRemitente == usuario.IdUsuario || a.IdUsuarioEnvio == usuario.IdUsuario)
                    && a.SolicitudAceptada)
                .Select(a => a.IdUsuarioRemitente == usuario.IdUsuario ? a.IdUsuarioEnvio : a.IdUsuarioRemitente)
                .ToList();

            
            var chatsUsuario = context.ChatsUsuarios
                .Where(cu => cu.IdUsuario == usuario.IdUsuario)
                .Select(cu => cu.IdChat)
                .ToList();

            
            var chatsConAmigos = context.ChatsUsuarios
                .Where(cu => amigosIds.Contains(cu.IdUsuario) && chatsUsuario.Contains(cu.IdChat))
                .Select(cu => cu.IdChat)
                .Distinct()
                .ToList();

            
            var chats = context.Chats
                .Where(c => chatsConAmigos.Contains(c.IdChat))
                .ToList();

            Chats.Clear(); 

            foreach (var chatId in chatsConAmigos)
            {
                var chat = context.Chats.SingleOrDefault(c => c.IdChat == chatId);
                if (chat != null)
                {
                    var participantes = context.ChatsUsuarios
                        .Where(cu => cu.IdChat == chatId)
                        .Select(cu => cu.IdUsuario)
                        .ToList();

                    bool esGrupo = participantes.Count > 2;

                    if (!esGrupo)
                    {
                        var otroUsuarioId = participantes.FirstOrDefault(id => id != usuario.IdUsuario);
                        var amigo = context.Usuarios.FirstOrDefault(u => u.IdUsuario == otroUsuarioId);
                        if (amigo != null)
                        {
                            chat.NombreChat = amigo.NombrePublico;
                            chat.FotoChat = amigo.FotoPerfil;
                        }
                    }

                    // Si es grupo, asumimos que ya tiene nombre y foto asignados
                    Chats.Add(chat);
                }
            }
        }

        #endregion
    }
}
