using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using TThrough.data;
using System.Windows.Input;
using TThrough.Servicios;
using TThrough.Entidades;
using System.Text.Json;

namespace TThrough.mvvm.ViewModel
{
    public class PopUpGruposViewModel : ViewModelBase
    {
        #region Propiedades

        private readonly ServicioTCP tcp;

        private readonly TalkthroughContext _context = TalkthroughContextFactory.SendContextFactory();

        public Action? MostrarPantalla;

        public Action? CerrarPopupAction { get; set; }

        public Action<Models.Chats> ChatCreado { get; set; }

        public ObservableCollection<Models.Usuario> UsuariosAmigos { get; set; }

        private List<Models.Usuario> _usuariosSeleccionados { get; set; }

        public List<Models.Usuario> UsuariosSeleccionados
        {
            get { return _usuariosSeleccionados; }
            set { _usuariosSeleccionados = value; OnPropertyChanged(); }
        }

        private Models.Usuario UsuarioActivo { get; set; }

        private string _nombreGrupo { get; set; }

        public string NombreGrupo
        {
            get { return _nombreGrupo; }
            set { _nombreGrupo = value; OnPropertyChanged(); }
        }

        public ICommand btnCrear => new RelayCommand(
            _ => CrearGrupoAsync(),
            _ => true);
        #endregion

        #region Constructor

        public PopUpGruposViewModel(Models.Usuario usuario, ServicioTCP tcp)
        {
            UsuariosAmigos = new ObservableCollection<Models.Usuario>();

            UsuarioActivo = usuario;
            CargarUsuariosAmigos();
            this.tcp = tcp;
        }

        #endregion


        #region Metodos


        /// <summary>
        /// Guarda en la colección observable aquellos usuarios que sean amigos del usuario que crea el grupo
        /// </summary>
        private async void CargarUsuariosAmigos()
        {
            var usuariosAmigos = _context.Amigos
                                 .Where(a => a.IdUsuarioRemitente == UsuarioActivo.IdUsuario || a.IdUsuarioEnvio == UsuarioActivo.IdUsuario)
                                 .Select(a => a.IdUsuarioRemitente == UsuarioActivo.IdUsuario ? a.UsuarioPeticion : a.UsuarioRemitente)
                                 .Distinct()
                                 .ToList();

            foreach (var usuario in usuariosAmigos)
            {
                this.UsuariosAmigos.Add(usuario);
            }
        }


        /// <summary>
        /// Método usado para crear el nuevo grupo.
        /// Para ello, recoge a los usuarios seleccionados y los introduce en una nueva entidad para almacenarla en la BBDD
        /// Posteriormente, recoge todos los elementos de la lista de Usuarios seleccionados para añadirlos a la BBDD en la tabla intermedia
        /// </summary>
        /// <returns></returns>
        private async Task CrearGrupoAsync()
        {

            var imageSource = new BitmapImage(new Uri("pack://application:,,,/TThrough;component/icons/defaultGroup.png"));


            if (UsuariosSeleccionados != null)
            {
                if (!NombreGrupo.IsNullOrEmpty())
                {

                    var nuevoChat = new Models.Chats()
                    {
                        IdChat = Guid.NewGuid().ToString(),
                        NombreChat = NombreGrupo,
                        FotoChat = ConvertImageToBytes(imageSource),
                        FechaInicioChat = DateTime.Now,
                    };
                    _context.Chats.Add(nuevoChat);
                    _context.ChatsUsuarios.Add(new Models.ChatsUsuarios()
                    {
                        IdChat = nuevoChat.IdChat,
                        IdUsuario = UsuarioActivo.IdUsuario,
                    });
                    foreach (var usuario in UsuariosSeleccionados)
                    {

                        _context.ChatsUsuarios.Add(new Models.ChatsUsuarios()
                        {
                            IdChat = nuevoChat.IdChat,
                            IdUsuario = usuario.IdUsuario,
                        });   
                    }


                    //Notifica al resto de miembros que el grupo ha sido creado.
                    var mensajeJson = new MensajeJson 
                    {
                        Tipo = "grupo_creado",
                        Emisor = UsuarioActivo.IdUsuario,
                        ChatId = nuevoChat.IdChat,
                    };

                    string json = JsonSerializer.Serialize(mensajeJson);
                    await tcp.EnviarMensaje(json);

                    ChatCreado?.Invoke(nuevoChat);
                    CerrarPopupAction?.Invoke();
                    _context.SaveChanges();
                }
            }
        }

        public static byte[] ConvertImageToBytes(ImageSource imageSource)
        {
            var bitmapSource = imageSource as BitmapSource;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }

        #endregion

    }
}
