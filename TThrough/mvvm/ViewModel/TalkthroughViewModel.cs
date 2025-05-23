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
using System.Text.Json;
using TThrough.mvvm.Models;


namespace TThrough.mvvm.ViewModel
{
    public class TalkthroughViewModel : ViewModelBase
    {
        #region Properties

        public static readonly string rutaBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"TThrough","MensajesLocales");

        public string rutaGuardadoChat = Path.Combine(rutaBase, $"{chatId}.json");

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

                ChatLineas.Clear();

                CargarMensajesDeFichero(_selectedItem.IdChat);
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

        void GuardarMensajesEnFichero(string chatId, ObservableCollection<VistaMensaje> mensajes)
        {
            Directory.CreateDirectory(rutaBase);
            string rutaFichero = Path.Combine(rutaBase, $"{chatId}.json");
            string json = JsonSerializer.Serialize(mensajes);
            File.WriteAllText(rutaFichero, json);
        }

        ObservableCollection<VistaMensaje> CargarMensajesDeFichero(string chatId)
        {
            string rutaFichero = Path.Combine(rutaBase, $"{chatId}.json");
            if (File.Exists(rutaFichero))
            {
                string json = File.ReadAllText(rutaFichero);
                return JsonSerializer.Deserialize<ObservableCollection<VistaMensaje>>(json) ?? new();
            }
            return new ObservableCollection<VistaMensaje>();
        }

        /// <summary>
        /// Crea un mensaje usando como referencia el modelo de mensaje JSON creado en la carpeta entidades. A partir de ahí, se hacen las comprobaciones pertinentes de IdChat para poder enviarlo a los usuarios
        /// Posteriormente, los datos se guardan en la bbdd
        /// 
        /// </summary>
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


                var mensajeJson = new MensajeJson
                {
                    Tipo = "mensaje_usuario",
                    Emisor = UsuarioSender.IdUsuario,
                    Receptor = null,
                    ChatId = SelectedItem.IdChat,
                    Datos = aux
                };


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

                GuardarMensajesEnFichero(SelectedItem.IdChat, ChatLineas);
                context.Mensajes.Add(datosMensajes);
                context.SaveChanges();

                string mensajeSerializado = JsonSerializer.Serialize(mensajeJson);
                _conexionTCP.EnviarMensaje(mensajeSerializado);
            });
        }

        /// <summary>
        /// Recoge el mensaje JSON como texto plano, posteriormente se deserializa y se recogen los datos de la clase que se necesiten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mensaje"></param>
        private void EnMensajeRecibido(object? sender, string mensaje)
        {
            var mensajeJson = JsonSerializer.Deserialize<MensajeJson>(mensaje);
            if (mensajeJson == null) return;

            var usuarioEmisor = context.Usuarios.Single(x=>x.IdUsuario == mensajeJson.Emisor);

            Application.Current.Dispatcher.Invoke(() =>
            {
                

                
                switch (mensajeJson.Tipo)
                {
                    case "mensaje_chat":
                        string texto = mensajeJson.Datos?.ToString() ?? "";
                        ChatLineas.Add(new VistaMensaje
                        {
                            NombrePublico = NombrePublico,
                            CuerpoMensaje = texto,
                            FtPerfil = ObtenerFotoPerfil(usuarioEmisor.IdUsuario)
                        });
                        var datosMensajes = new Models.Mensaje()
                        {
                            FechaEnvio = DateTime.Now,
                            HoraEnvio = DateTime.Now,
                            IdMensaje = Guid.NewGuid().ToString().ToLower(),
                            IdChat = SelectedItem.IdChat,
                        };
                        break;

                    case "actualizacion_perfil":
                        if (mensajeJson.Datos is JsonElement datos)
                        {
                            
                            string nuevoNombrePublico = datos.GetProperty("NombrePublico").GetString();

                            string fotoPerfilBase64 = datos.GetProperty("FotoPerfil").GetString();

                            byte[] fotoBytes = Convert.FromBase64String(fotoPerfilBase64);

                            // Obtener el usuario a actualizar (esto depende de cómo mantienes la lista de usuarios en tu UI)
                            var usuario = context.Usuarios.FirstOrDefault(u => u.IdUsuario == mensajeJson.Emisor);
                            if (usuario != null)
                            {
                                usuario.NombrePublico = nuevoNombrePublico;
                                usuario.FotoPerfil = fotoBytes;

                                context.Usuarios.Update(usuario);
                                context.SaveChanges();

                               
                                ActualizarVistaUsuario(usuario);
                            }
                        }
                        break;
                        

                        

                }
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

        private ImageSource ObtenerFotoPerfil(string idUser) 
        {
            var fotoPerfilUsuario = context.Usuarios.SingleOrDefault(x => x.IdUsuario == idUser).FotoPerfil;

            return ConvertBytesToImage(fotoPerfilUsuario);
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

                    
                    Chats.Add(chat);
                }
            }
        }


        private void ActualizarVistaUsuario(Usuario usuario)
        {
            
            var item = Usuarios.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
            if (item != null)
            {
                item.NombrePublico = usuario.NombrePublico;
                item.FotoPerfil = usuario.FotoPerfil;
                
            }
        }
        #endregion
    }
}
