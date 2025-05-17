using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TThrough.mvvm.Models;
using TThrough.mvvm.ViewModel;


namespace TThrough.mvvm.View
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel pageModel;
        public MainWindow()
        {
            InitializeComponent();

            DataContext = pageModel = new MainWindowViewModel();

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

        private void EnviarAVM(object sender, RoutedEventArgs e)
        {

            pageModel.Contrasena = ((PasswordBox)sender).Password;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            pageModel.LoginAsync += Vm_LoginAsync;


        }

        /// <summary>
        /// Crea el viewmodel de la ventana del chat tras el login correcto del usuario, así como la muestra del
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Vm_LoginAsync(object? sender, Usuario e)
        {
            
            TalkthroughViewModel talkthroughViewModel = new TalkthroughViewModel(pageModel._servicioTCP);
            var panel = new TalkThrough(talkthroughViewModel);
            talkthroughViewModel.Usuarios.Add(e);
            

            panel.DataContext = talkthroughViewModel;

            panel.Show();

            
            this.Close();
        }
    }
}
