using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TThrough.mvvm.ViewModel;
using TThrough.Servicios;

namespace TThrough.mvvm.View
{
    /// <summary>
    /// Lógica de interacción para TalkThrough.xaml
    /// </summary>
    public partial class TalkThrough : Window
    {
        public TalkThrough()
        {
            InitializeComponent();
        }

        public TalkThrough(TalkthroughViewModel viewModel)
        {
            InitializeComponent();



            DataContext = viewModel;


            viewModel.ChatCreado = chatCreado =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.Chats.Add(chatCreado);
                });
            };

            viewModel.ChatEliminado = chatAEliminar =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    viewModel.Chats.Remove(chatAEliminar);
                });
            };

            viewModel.PopUpAmigosAction = () =>
            {
                var usuarioActual = viewModel.context.Usuarios.Single(u => u.NombreUsuario == viewModel.UsuarioConectadoActual);
                var popUpViewModel = new PopUpAñadirAmigosViewModel(viewModel.context, usuarioActual,viewModel.conexionTCP);

                var popUpAñadirAmigos = new PopUpAñadirAmigos(popUpViewModel);

                popUpViewModel.CerrarPopup = () => popUpAñadirAmigos.Close();

                popUpAñadirAmigos.Show();
            };

            viewModel.PopUpSolicitudesAction = () =>
            {
                var usuarioActual = viewModel.context.Usuarios.Single(u => u.NombreUsuario == viewModel.UsuarioConectadoActual);
                var popUpSolicitudesPendientesVM = new PopUpSolicitudesPendientesViewModel(usuarioActual,viewModel.conexionTCP);
                var popUpSolicitudes = new PopUpSolicitudesPendientes(popUpSolicitudesPendientesVM);

                popUpSolicitudesPendientesVM.SolicitudAceptada = () =>
                {
                    viewModel.ComprobarSolicitudesPendientes();
                };

                popUpSolicitudesPendientesVM.ChatCreado = nuevoChat =>
                {

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        viewModel.Chats.Add(nuevoChat);
                    });
                };

                popUpSolicitudesPendientesVM.CerrarPopupAction = () => popUpSolicitudes.Close();



                popUpSolicitudes.Show();
            };

            viewModel.PopUpGruposAction = () =>
            {
                var usuarioActual = viewModel.context.Usuarios.Single(u => u.NombreUsuario == viewModel.UsuarioConectadoActual);
                var popUpGruposVM = new PopUpGruposViewModel(usuarioActual,viewModel.conexionTCP);
                var popUpGrupos = new PopUpCrearGrupos(popUpGruposVM);

                popUpGruposVM.CerrarPopupAction += () => popUpGrupos.Close();

                popUpGruposVM.ChatCreado = nuevoChat =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        viewModel.Chats.Add(nuevoChat);
                    });
                };

                popUpGrupos.Show();
            };

            viewModel.VentanaConfigAbierta = () =>
            {
                var usuarioActual = viewModel.context.Usuarios.Single(u => u.NombreUsuario == viewModel.UsuarioConectadoActual);
                var configuracionVM = new ConfigViewModel(usuarioActual,viewModel.conexionTCP);
                var config = new PaginaConfiguracion(configuracionVM);

                configuracionVM.CerrarPopUp = () => config.Close();

                config.Show();

            };

        }



        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;

        }

        private void Minimizar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}
