using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using TThrough.data;
using System.Windows.Input;
using TThrough.Entidades;

namespace TThrough.mvvm.ViewModel
{
    public class ConfigViewModel : ViewModelBase
    {
        public Action<Models.Chats>? NotificarColeccion;

        public ICommand EjecutarBuscarArchivo => new RelayCommand(
            _ => { SeleccionarNuevaFoto(); },
            _=>true);

        public Action<string>? SeleccionarArchivo;

        private TalkthroughContext _context = TalkthroughContextFactory.SendContextFactory();
        private ImageSource _fotoPerfil { get; set; }

        public ImageSource FotoPerfil
        {
            get { return _fotoPerfil; }
            set { _fotoPerfil = value; OnPropertyChanged(); }
        }

        private string _nombreUsuario;
        public string NombreUsuario 
        {
            get { return _nombreUsuario; } set { _nombreUsuario = value; OnPropertyChanged(); }
        }

        private string _nombrePublico;

        public string NombrePublico 
        {
            get { return _nombrePublico; } 
            set { _nombrePublico = value; OnPropertyChanged(); }
        }

        public Models.Usuario UsuarioConectado { get; set; }

        #region Constructores
        public ConfigViewModel(Models.Usuario usuario) 
        {
            UsuarioConectado = usuario;
            FotoPerfil = ConvertBytesToImage(UsuarioConectado.FotoPerfil);
            NombreUsuario = UsuarioConectado.NombreUsuario;
            NombrePublico= UsuarioConectado.NombrePublico;
        }
        #endregion

        #region Methods

        public static byte[] ConvertImageToBytes(ImageSource imageSource)
        {
            var bitmapSource = imageSource as BitmapSource;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
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

        public void SeleccionarNuevaFoto() 
        {
            SeleccionarArchivo?.Invoke("Seleccionar imagen");
        }

        public void GuardarCambios() 
        {
            bool cambios = false;

            if(UsuarioConectado.NombrePublico != NombrePublico) 
            {
                UsuarioConectado.NombrePublico = NombrePublico;

                cambios = true;
            }

            var ultimaFotoPerfil = _context.Usuarios.Single(x=>x.IdUsuario == UsuarioConectado.IdUsuario).FotoPerfil;

            if (UsuarioConectado.FotoPerfil != ultimaFotoPerfil) 
            {
                cambios= true;
            }

            if (cambios) 
            {
                _context.Usuarios.Update(UsuarioConectado);
                _context.SaveChangesAsync();

                var nuevosDatosPerfil = new
                {
                    NombrePublico = UsuarioConectado.NombrePublico,
                    FotoPerfil = Convert.ToBase64String(UsuarioConectado.FotoPerfil)
                };

                var mensajeJson = new MensajeJson
                {
                    Tipo = "actualizacion_perfil",
                    Emisor = UsuarioConectado.IdUsuario,
                    Receptor = null,
                    ChatId = null, 
                    Datos = nuevosDatosPerfil
                };
            }
            


        }
        #endregion

    }
}
