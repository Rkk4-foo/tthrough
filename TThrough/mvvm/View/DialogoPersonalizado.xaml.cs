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

namespace TThrough.mvvm.View
{
    /// <summary>
    /// Lógica de interacción para DialogoPersonalizado.xaml
    /// </summary>
    public partial class DialogoPersonalizado : Window
    {
        public DialogoPersonalizado()
        {
            InitializeComponent();
        }


        public DialogoPersonalizado(string titulo, string mensaje) 
        {
            InitializeComponent();
            this.titulo.Content = titulo;
            CuerpoMensaje.Text = mensaje;
        }

        public static void Mostrar(string titulo, string cuerpo) 
        {
            var dialogo = new DialogoPersonalizado(titulo,cuerpo);
            dialogo.ShowDialog();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();

        }
    }
}
