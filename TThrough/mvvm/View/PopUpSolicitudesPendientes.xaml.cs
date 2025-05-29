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

namespace TThrough.mvvm.View
{
    /// <summary>
    /// Lógica de interacción para PopUpSolicitudesPendientes.xaml
    /// </summary>
    public partial class PopUpSolicitudesPendientes : Window
    {
        public PopUpSolicitudesPendientes()
        {
            InitializeComponent();
        }

        public PopUpSolicitudesPendientes(PopUpSolicitudesPendientesViewModel viewModel) 
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Maximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;

        }

        private void Minimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
