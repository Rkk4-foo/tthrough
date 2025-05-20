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
using TThrough.mvvm.Models;
using TThrough.mvvm.ViewModel;

namespace TThrough.mvvm.View
{
    /// <summary>
    /// Lógica de interacción para PopUpCrearGrupos.xaml
    /// </summary>
    public partial class PopUpCrearGrupos : Window
    {
        public PopUpGruposViewModel PageModel { get; set; }

        public PopUpCrearGrupos()
        {
            InitializeComponent();
        }

        public PopUpCrearGrupos(PopUpGruposViewModel vm)
        {
            InitializeComponent();

            DataContext = PageModel = vm;

            
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListaUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PopUpGruposViewModel gruposVM) 
            {
                gruposVM.UsuariosSeleccionados = ListaUsuarios.SelectedItems.Cast<Models.Usuario>().ToList();
            }
        }
    }
}
