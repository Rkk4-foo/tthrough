using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TThrough.data;
using TThrough.mvvm.Models;
using TThrough.Servicios;

namespace TThrough.mvvm.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Propiedades
        private readonly TalkthroughContext _context = TalkthroughContextFactory.SendContextFactory();

        public Servicios.ServicioTCP _servicioTCP;

        private ObservableCollection<Usuario> _usuarios { get; set; }

        private string _NombreUsuario;
        public string NombreUsuario
        {
            get => _NombreUsuario;
            set { _NombreUsuario = value; OnPropertyChanged(); }
        }

        public string _Contrasena;

        public string Contrasena
        {
            get => _Contrasena;
            set { _Contrasena = value; OnPropertyChanged(); }
        }

        public ICommand ComandoRegistro { get; }

        public ICommand ComandoLogin { get; }

        public event EventHandler<Usuario> LoginAsync;


        public ServicioDialogo _dialog;

        #endregion

        #region Constructores

        public MainWindowViewModel() 
        {
            _dialog = new ServicioDialogo();
            _servicioTCP = new Servicios.ServicioTCP();
            _usuarios = new ObservableCollection<Usuario>();
            ComandoRegistro = new AsyncRelayCommand(RegistrarUsuario);
            ComandoLogin = new AsyncRelayCommand(LoginCorrecto);
        }

        #endregion


        #region Metodos

        private async Task LoginCorrecto() 
        {
            using (var context = TalkthroughContextFactory.SendContextFactory())
            {
                bool resultado = await VerificarCredenciales(context, NombreUsuario, Contrasena);

                Usuario user = await ObtenerUsuario(context,NombreUsuario);

                if (resultado && _servicioTCP.ClienteConectado())
                {
                    LoginAsync?.Invoke(this, user);
                    
                }
                else 
                {
                    _dialog.MostrarDialogo("ERROR","No se pudo contectar al servidor");
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

        private async Task<Usuario> ObtenerUsuario(TalkthroughContext context, string Username) 
        {
            return await context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == Username);
        }

        private async Task<bool> VerificarCredenciales(TalkthroughContext context,string Username,string pass) 
        { 
            Usuario usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == Username);

            if (usuario == null) return false;

            PasswordVerificationResult resultado = new PasswordHasher<Usuario>().VerifyHashedPassword(usuario,usuario.Contrasena, pass);

            return resultado == PasswordVerificationResult.Success;
        } 

        private async Task RegistrarUsuario() 
        {
            if (await ExisteUsuario(NombreUsuario)) 
            {
                MessageBox.Show("El usuario ya existe");
                return;
            }

            var imageSource = new BitmapImage(new Uri("pack://application:,,,/TThrough;component/icons/icondefault.png"));

            Usuario NuevoUsuario = new Usuario
            {
                IdUsuario = Guid.NewGuid().ToString().ToLower(),
                FotoPerfil = ConvertImageToBytes(imageSource),
                NombreUsuario = this.NombreUsuario,
                NombrePublico = NombreUsuario,
                Contrasena = null!,
                FechaRegistro = DateTime.Now
            };

            //Crea un hash para la contraseña automáticamente de manera que se guarda de forma segura.

            NuevoUsuario.Contrasena = new PasswordHasher<Usuario>().HashPassword(NuevoUsuario,this.Contrasena);

            _context.Usuarios.Add(NuevoUsuario);
            await _context.SaveChangesAsync();

            _usuarios.Add(NuevoUsuario);
            MessageBox.Show("Usuario añadido correctamente");
        }

        private async Task<bool> ExisteUsuario(string NombreUsuario) 
        {
            return await _context.Usuarios.AnyAsync(u => u.NombreUsuario == NombreUsuario);
        }

        #endregion
    }
}
