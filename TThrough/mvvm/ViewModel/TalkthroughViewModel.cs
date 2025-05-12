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

namespace TThrough.mvvm.ViewModel
{
    public class TalkthroughViewModel : ViewModelBase
    {
        #region Properties

        private ICommand _enviarCommand { get; }

        private ServicioTCP _conexionTCP;

        public ObservableCollection<Models.Usuario> Usuarios { get; set; }

        public ObservableCollection<Models.Mensaje> Mensajes { get; set; }

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

        public TalkthroughViewModel(TcpClient tcpClient)
        {
            Usuarios = new ObservableCollection<Models.Usuario>();
            Mensajes = new ObservableCollection<Models.Mensaje>();
        }


        #endregion

        #region Methods

        public void InicializarServicio(ServicioTCP tcp) 
        {
            _conexionTCP = tcp;
            _conexionTCP.MensajeRecibido += EnMensajeRecibido;
        }

        private void EnMensajeRecibido(object? sender, string cuerpoMensaje) 
        {
            Dispatcher.Invoke(() =>
            {

                Mensajes.Add(new Models.Mensaje
                {
                    HoraEnvio = DateTime.Now,
                    FechaEnvio = DateTime.Now,
                    IdMensaje = Guid.NewGuid().ToString().ToLower()
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
