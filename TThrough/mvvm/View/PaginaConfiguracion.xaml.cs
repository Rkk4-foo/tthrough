﻿using Microsoft.Win32;
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

namespace TThrough.mvvm.ViewModel
{
    /// <summary>
    /// Lógica de interacción para PaginaConfiguracion.xaml
    /// </summary>
    public partial class PaginaConfiguracion : Window
    {

        #region Propiedades

        public ConfigViewModel PageModel { get; set; }

        #endregion

        #region Constructores
        public PaginaConfiguracion()
        {
            InitializeComponent();
        }

        public PaginaConfiguracion(ConfigViewModel vm) 
        {
            InitializeComponent();

            DataContext = PageModel = vm;

            PageModel.SeleccionarArchivo = AbrirDialogoSeleccionImagen;
        }

        #endregion

        #region Methods 

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void AbrirDialogoSeleccionImagen(string _) 
        {
            var fileDialog = new OpenFileDialog()
            {
                Filter = "*.jpg|*.png"
            };

            if (fileDialog.ShowDialog() == true) 
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(fileDialog.FileName);
                bitmap.EndInit();

                PageModel.FotoPerfil = bitmap;
                PageModel.UsuarioConectado.FotoPerfil = ConfigViewModel.ConvertImageToBytes(bitmap);
            }
        }
        #endregion

        
    }
}
