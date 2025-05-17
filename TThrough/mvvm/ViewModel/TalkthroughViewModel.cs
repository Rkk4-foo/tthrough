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


namespace TThrough.mvvm.ViewModel
{
    public class TalkthroughViewModel : ViewModelBase
    {
        #region Properties

        private readonly TalkthroughContext _context = TalkthroughContextFactory.SendContextFactory();

        public ICommand _enviarCommand => new RelayCommand(
             _ => EnviarMensaje(),
             _ => !string.IsNullOrWhiteSpace(Mensaje));
        private ServicioTCP _conexionTCP;

        private TextBox _TextBoxChat { get; set; }

        public TextBox TextBoxChat { get { return _TextBoxChat; } set { _TextBoxChat = value; OnPropertyChanged(); } }

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

        public ObservableCollection<Models.ChatsUsuarios> Chats { get; set; }

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

        public TalkthroughViewModel(ServicioTCP client)
        {
            Usuarios = new ObservableCollection<Models.Usuario>();
            Mensajes = new ObservableCollection<Models.Mensaje>();
            ChatLineas = new ObservableCollection<VistaMensaje>();
            InicializarServicio(client!);

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

                // 1. Crear mensaje
                var datosMensajes = new Models.Mensaje()
                {
                    FechaEnvio = DateTime.Now,
                    HoraEnvio = DateTime.Now,
                    IdMensaje = Guid.NewGuid().ToString().ToLower(),
                    IdChat = SelectedItem.IdChat, // Asegúrate de que exista esta relación
                    
                };

                
                var UsuarioSender = _context.Usuarios.Single(u => u.NombrePublico == this.NombrePublico);

                // 3. Añadir a la vista del chat
                ChatLineas.Add(new VistaMensaje
                {
                    NombrePublico = UsuarioSender.NombrePublico,
                    CuerpoMensaje = aux,
                    FtPerfil = ConvertBytesToImage(UsuarioSender.FotoPerfil)
                });

                
                var usuariosDelChat = _context.ChatsUsuarios
                    .Where(cu => cu.IdChat == SelectedItem.IdChat)
                    .Select(cu => cu.IdUsuario)
                    .ToList();

                foreach (var idUsuario in usuariosDelChat)
                {
                    var mensajeUsuario = new Models.MensajeUsuario
                    {
                        IdMensaje = datosMensajes.IdMensaje,
                        IdUsuario = idUsuario


                    };
                    _context.MensajesUsuarios.Add(mensajeUsuario);
                }

                
                _context.Mensajes.Add(datosMensajes);
                _context.SaveChanges();

                // 6. Enviar mensaje por red (opcionalmente a cada usuario)
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

        #endregion
    }
}
