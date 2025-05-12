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
        public MainWindow()
        {
            InitializeComponent();
            
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
            if(Application.Current.MainWindow.WindowState != WindowState.Maximized)
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
            if (DataContext is MainWindowViewModel vm)
            {
                vm.Contrasena = ((PasswordBox)sender).Password;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm) 
            {
                vm.LoginAsync += Vm_LoginAsync;
                
            }
        }

        

        private void Vm_LoginAsync(object? sender, Usuario e)
        {
            TalkthroughViewModel talkthroughViewModel = new TalkthroughViewModel();
            talkthroughViewModel.Usuarios.Add(e);
            // Abre la siguiente ventana
            var panel = new TalkThrough();
            panel.DataContext = talkthroughViewModel;
            
            panel.Show();

            // Cierra esta ventana
            this.Close();
        }
    }
}
