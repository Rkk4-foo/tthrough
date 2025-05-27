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

        public static readonly string rutaBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TThrough", "MensajesLocales");

        private bool _mostrarComponentes;

        public bool MostrarComponentes
        {
            get { return _mostrarComponentes; }
            set { _mostrarComponentes = value; OnPropertyChanged(); }
        }

        public readonly TalkthroughContext context = TalkthroughContextFactory.SendContextFactory();

        public readonly string UsuarioConectadoActual;

        public Action<Models.Chats> ChatCreado;

        public Action<Models.Chats> ChatEliminado;

        public Action? PopUpAmigosAction { get; set; }

        public Action? PopUpSolicitudesAction { get; set; }

        public Action? PopUpGruposAction { get; set; }
        public Action? VentanaConfigAbierta { get; set; }

        public ICommand btnEliminar => new RelayCommand(
            _ => { EliminarAmigo(); },
            _ => AmigoSeleccionado);

        public ICommand BtnConfig => new RelayCommand(
            _ => { VentanaConfigAbierta?.Invoke(); },
            _ => true);

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
        _ => true);

        public ICommand EnviarCommand => new RelayCommand(
             _ => EnviarMensaje(),
             _ => !string.IsNullOrWhiteSpace(Mensaje));

        public ServicioTCP conexionTCP;



        private Models.Chats _selectedItem { get; set; }

        public Models.Chats SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;

                    AmigoSeleccionado = _selectedItem != null && context.ChatsUsuarios.Count(x => x.IdChat == _selectedItem.IdChat) == 2;
                    ChatLineas.Clear();

                    //if (_selectedItem != null)
                    //CargarMensajesDesdeFichero(_selectedItem.IdChat);
                    if (_selectedItem == null)
                    {
                        MostrarComponentes = false;
                    }
                    else
                    {
                        MostrarComponentes = true;
                    }

                    OnPropertyChanged();
                }
            }
        }

        private bool _amigoSeleccionado { get; set; }

        public bool AmigoSeleccionado
        {
            get { return _amigoSeleccionado; }
            set 
            { 
                _amigoSeleccionado = value;
                
                

                OnPropertyChanged(); 
            }
        }

        private bool _solicitudesPendientes { get; set; }

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

        public TalkthroughViewModel(ServicioTCP client, string nombreUsuarioConectado)
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
            conexionTCP = tcp;
            conexionTCP.MensajeRecibido += EnMensajeRecibido;


            _ = conexionTCP.RecibirMensajes(CancellationToken.None);
        }






        //void GuardarMensajesEnFichero(string chatId, ObservableCollection<VistaMensaje> mensajes)
        //{
        //    Directory.CreateDirectory(rutaBase);
        //    string rutaFichero = Path.Combine(rutaBase, $"{chatId}.json");

        //    var mensajesSerializables = mensajes.Select(m => new VistaMensajeSerializable
        //    {
        //        NombrePublico = m.NombreUsuario,
        //        CuerpoMensaje = m.CuerpoMensaje,
        //        FtPerfil = ConvertBytesToImage(m.FtPerfil)
        //    });

        //    string json = JsonSerializer.Serialize(mensajesSerializables, new JsonSerializerOptions { WriteIndented = true });
        //    File.WriteAllText(rutaFichero, json);
        //}

        //ObservableCollection<VistaMensaje> CargarMensajesDesdeFichero(string chatId)
        //{
        //    string rutaFichero = Path.Combine(rutaBase, $"{chatId}.json");
        //    if (!File.Exists(rutaFichero)) return new ObservableCollection<VistaMensaje>();

        //    var json = File.ReadAllText(rutaFichero);
        //    var mensajesSerializables = JsonSerializer.Deserialize<List<VistaMensajeSerializable>>(json);

        //    var mensajes = new ObservableCollection<VistaMensaje>(
        //        mensajesSerializables.Select(m => new VistaMensaje
        //        {
        //            NombreUsuario = m.NombrePublico,
        //            CuerpoMensaje = m.CuerpoMensaje,
        //            FtPerfil = ConvertBase64ToImage(m.FtPerfil)
        //        })
        //    );

        //    return mensajes;
        //}

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


                var UsuarioSender = context.Usuarios.Single(u => u.NombreUsuario == UsuarioConectadoActual);


                var mensajeJson = new MensajeJson
                {
                    Tipo = "mensaje_chat",
                    Emisor = UsuarioSender.IdUsuario,
                    Receptor = null,
                    ChatId = SelectedItem.IdChat,
                    Datos = aux
                };


                ChatLineas.Add(new VistaMensaje
                {
                    NombreUsuario = UsuarioSender.NombreUsuario,
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

                //GuardarMensajesEnFichero(SelectedItem.IdChat, ChatLineas);
                context.Mensajes.Add(datosMensajes);
                context.SaveChangesAsync();

                string mensajeSerializado = JsonSerializer.Serialize(mensajeJson);
                _ = conexionTCP.EnviarMensaje(mensajeSerializado);
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

            var usuarioEmisor = context.Usuarios.SingleOrDefault(x => x.IdUsuario == mensajeJson.Emisor);
            if (usuarioEmisor == null)
            {
                Console.WriteLine($"[Advertencia] Usuario con ID '{mensajeJson.Emisor}' no encontrado en contexto.");
                return; // Salir para evitar null reference
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                switch (mensajeJson.Tipo)
                {
                    case "mensaje_chat":
                        string texto = mensajeJson.Datos?.ToString() ?? "";

                        if (mensajeJson.ChatId == _selectedItem.IdChat)
                            ChatLineas.Add(new VistaMensaje
                            {
                                NombreUsuario = usuarioEmisor.NombrePublico,
                                CuerpoMensaje = texto,
                                FtPerfil = ObtenerFotoPerfil(usuarioEmisor.IdUsuario)
                            });

                        Console.WriteLine("Mensaje de chat recibido");

                        var datosMensajes = new Models.Mensaje()
                        {
                            FechaEnvio = DateTime.Now,
                            HoraEnvio = DateTime.Now,
                            IdMensaje = Guid.NewGuid().ToString().ToLower(),
                            IdChat = SelectedItem.IdChat,
                        };


                        context.Mensajes.Add(datosMensajes);
                        context.SaveChanges();

                        break;

                    case "actualizacion_perfil":
                        ActualizarVistaChats(mensajeJson.Emisor);
                        break;
                    case "solicitud_amistad":
                        ComprobarSolicitudesPendientes();
                        break;
                    case "solicitud_aceptada":
                        CargarChats();
                        break;
                    case "amigo_eliminado":
                        CargarChats();
                        break;
                    case "grupo_creado":
                        CargarChats();
                        break;
                    default:
                        Console.WriteLine($"[Advertencia] Tipo de mensaje desconocido: {mensajeJson.Tipo}");
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

        public void ActualizarVistaChats(string idEmisor)
        {
            Chats.Clear();

            using (var context = TalkthroughContextFactory.SendContextFactory())
            {
                var usuario = context.Usuarios.Single(u => u.NombreUsuario == UsuarioConectadoActual);

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

                foreach (var chatId in chatsConAmigos)
                {
                    var chat = chats.SingleOrDefault(c => c.IdChat == chatId);
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
                ActualizarMensajesDelChatActivo(idEmisor);
            }
        }
        public void ActualizarMensajesDelChatActivo(string IdEmisor)
        {
            var usuario = context.Usuarios.Single(x => x.IdUsuario == IdEmisor);

            if (usuario == null)
                return;

            foreach (var linea in ChatLineas.Where(l => l.NombreUsuario == usuario.NombreUsuario))
            {
                linea.NombreUsuario = usuario.NombrePublico;
                linea.FtPerfil = ConvertBytesToImage(usuario.FotoPerfil);
            }
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
            Chats.Clear();

            using (var context = TalkthroughContextFactory.SendContextFactory())
            {
                var usuario = context.Usuarios.Single(x => x.NombreUsuario == UsuarioConectadoActual);

                
                var chatsUsuarioIds = context.ChatsUsuarios
                    .Where(cu => cu.IdUsuario == usuario.IdUsuario)
                    .Select(cu => cu.IdChat)
                    .ToList();

                var chats = context.Chats
                    .Where(c => chatsUsuarioIds.Contains(c.IdChat))
                    .ToList();

                foreach (var chat in chats)
                {
                    var participantes = context.ChatsUsuarios
                        .Where(cu => cu.IdChat == chat.IdChat)
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


        /// <summary>
        /// Elimina el registro de la base de datos para acabar con la solicitud de amistad. El usuario desaparece de los chats del usuario conectado.
        /// </summary>
        private void EliminarAmigo()
        {
            var usuarioConectado = context.Usuarios.Single(x => x.NombreUsuario == UsuarioConectadoActual);

            bool EsGrupo = context.ChatsUsuarios.Count(x => x.IdChat == _selectedItem.IdChat) > 2;

            if (EsGrupo)
            {
                AmigoSeleccionado = false;
                return;
            }
            else
            {

                var otroUsuario = context.ChatsUsuarios.Where(x => x.IdChat == _selectedItem.IdChat && x.IdUsuario != usuarioConectado.IdUsuario)
                                .Select(x => x.IdUsuario).FirstOrDefault();

                if (otroUsuario != null)
                {
                    var solicitud = context.Amigos.FirstOrDefault(a =>
                                    (a.IdUsuarioEnvio == usuarioConectado.IdUsuario && a.IdUsuarioRemitente == otroUsuario) ||
                                    (a.IdUsuarioEnvio == otroUsuario && a.IdUsuarioRemitente == usuarioConectado.IdUsuario));

                    if (solicitud != null)
                    {
                        context.Amigos.Remove(solicitud);


                    }
                    var chatUsuarios = context.ChatsUsuarios.Where(cu => cu.IdChat == _selectedItem.IdChat).ToList();
                    context.ChatsUsuarios.RemoveRange(chatUsuarios);

                    var chat = context.Chats.FirstOrDefault(c => c.IdChat == _selectedItem.IdChat);
                    if (chat != null)
                    {

                        var participantes = context.ChatsUsuarios
                        .Where(cu => cu.IdChat == chat.IdChat)
                        .Select(cu => cu.IdUsuario)
                        .ToList();

                        
                        
                        context.Chats.Remove(chat);
                        //var mensajeJson = new MensajeJson
                        //{
                        //    Tipo = "amigo_eliminado",
                        //    Emisor = usuarioConectado.IdUsuario,
                        //    Receptor = otroUsuario,
                        //    ChatId = chat.IdChat
                        //};

                        //string msj = JsonSerializer.Serialize(mensajeJson);

                        //await conexionTCP.EnviarMensaje(msj);
                    }

                }
                ChatEliminado.Invoke(_selectedItem);
                AmigoSeleccionado = false;
            }


        }

        #endregion
    }
}
