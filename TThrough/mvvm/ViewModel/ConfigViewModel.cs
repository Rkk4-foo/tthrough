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
using Microsoft.EntityFrameworkCore;
using TThrough.Servicios;
using System.Text.Json;

namespace TThrough.mvvm.ViewModel
{
    public class ConfigViewModel : ViewModelBase
    {
        public ServicioTCP tcp;

        public ICommand EjecutarBuscarArchivo => new RelayCommand(
            _ => { SeleccionarNuevaFoto(); },
            _=>true);

        public ICommand GuardarCambiosCommand => new AsyncRelayCommand(GuardarCambios);
            

        public Action? CerrarPopUp;

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
        public ConfigViewModel(Models.Usuario usuario, ServicioTCP tcp) 
        {
            UsuarioConectado = usuario;
            FotoPerfil = ConvertBytesToImage(UsuarioConectado.FotoPerfil);
            NombreUsuario = UsuarioConectado.NombreUsuario;
            NombrePublico= UsuarioConectado.NombrePublico;
            this.tcp = tcp;
        }
        #endregion

        #region Methods


        /// <summary>
        /// Recoge la Imagen que se ha recogido en el FilePicker y la codifica como un Array de bytes
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static byte[] ConvertImageToBytes(ImageSource imageSource)
        {
            var bitmapSource = imageSource as BitmapSource;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Convierte el array de bytes recuperado previamente de la base de datos y lo convierte en la imagen a mostrar.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Guarda los cambios que haya realizado el usuario en su perfil en caso de que haya
        /// </summary>
        /// <returns></returns>
        public async Task GuardarCambios()
        {
            bool cambios = false;

            if (UsuarioConectado.NombrePublico != NombrePublico)
            {
                UsuarioConectado.NombrePublico = NombrePublico;
                cambios = true;
            }


            //Recoge la última foto de perfil del usuario para compararla posteriormente con la foto que tiene ahora el usuario
            var ultimaFotoPerfil = _context.Usuarios
                .AsNoTracking()
                .Single(x => x.IdUsuario == UsuarioConectado.IdUsuario)
                .FotoPerfil;

            var FotoActual = UsuarioConectado.FotoPerfil ?? Array.Empty<byte>();
            var UltimaFoto = ultimaFotoPerfil ?? Array.Empty<byte>();

            if (!FotoActual.SequenceEqual(UltimaFoto))
            {
                cambios = true;
            }

            if (cambios)
            {
                _context.Usuarios.Update(UsuarioConectado);
                await _context.SaveChangesAsync();

                var mensajeJson = new MensajeJson
                {
                    Tipo = "actualizacion_perfil",
                    Emisor = UsuarioConectado.IdUsuario,
                    Receptor = null
                };

                string mensaje = JsonSerializer.Serialize(mensajeJson);

                await tcp.EnviarMensaje(mensaje);

                CerrarPopUp?.Invoke();
            }
        }

        #endregion

    }
}
