using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TThrough.data;
using TThrough.mvvm.ViewModel;

namespace TThrough.mvvm.View
{
    /// <summary>
    /// Lógica de interacción para PopUpAñadirAmigos.xaml
    /// </summary>
    public partial class PopUpAñadirAmigos : Window
    {

        #region Propiedades
        public PopUpAñadirAmigosViewModel PageModel { get; set; }

        #endregion



        #region Constructores

        public PopUpAñadirAmigos(PopUpAñadirAmigosViewModel vm)
        { 
            InitializeComponent();

            DataContext = PageModel = vm;
        }
        #endregion

        #region Methods



        #endregion

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
