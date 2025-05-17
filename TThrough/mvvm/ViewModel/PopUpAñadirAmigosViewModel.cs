using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TThrough.data;

namespace TThrough.mvvm.ViewModel
{
    public class PopUpAñadirAmigosViewModel : ViewModelBase
    {

        #region Properties
        public ICommand comandoAñadir { get; set; }

        public ICommand ComandoBuscar => new RelayCommand(
            _ =>  BusquedaUsuarios(), 
            _ => true);

        private TalkthroughContext _context { get; set; }

        private string _textoBusqueda {  get; set; }

        public string Busqueda 
        {
            get { return _textoBusqueda; }
            set 
            {
                _textoBusqueda = value;
                OnPropertyChanged();
            } 
        }
        private Models.Usuario _usuarioSeleccionado {  get; set; }

        public Models.Usuario UsuarioSeleccionado 
        {
            get { return  _usuarioSeleccionado;}
            set { _usuarioSeleccionado = value; OnPropertyChanged(); }
        }

        private bool _isChecked {  get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }

            set { _isChecked = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Models.Usuario> _usuarios { get; set; }

        public ObservableCollection<Models.Usuario> Usuarios 
        {
            get { return _usuarios; }
            set 
            {
                _usuarios = value;
                OnPropertyChanged();
            } 
        }


        #endregion

        #region Constructores

        public PopUpAñadirAmigosViewModel(TalkthroughContext c) 
        {
            Usuarios = new ObservableCollection<Models.Usuario>();
            _context = c;
        }

        #endregion

        #region Metodos

        private void BusquedaUsuarios() 
        {
            Usuarios.Clear();
            if (!Busqueda.IsNullOrEmpty())
            {
                List<Models.Usuario> usuarios = _context.Usuarios.Where(x => x.NombreUsuario.Contains(Busqueda))
                                                .ToList();

                Usuarios.Clear();
                foreach (var usuario in usuarios)
                {
                    Usuarios.Add(usuario);
                }
            }
            else Usuarios.Clear();
        }

        #endregion

    }
}