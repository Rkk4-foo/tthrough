using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TThrough.mvvm
{
    public class RelayCommand : ICommand
    {
        private Action<object> Ejecutar;
        private Func<object, bool> PuedeEjecutar;


        public event EventHandler? CanExecuteChanged 
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand( Action<object> Ejecutar, Func<object, bool> PuedeEjecutar) 
        {
            this.Ejecutar = Ejecutar;

            this.PuedeEjecutar= PuedeEjecutar;
        }

        public bool CanExecute(object? parameter)
        {
            return PuedeEjecutar == null || PuedeEjecutar(parameter);
        }

        public void Execute(object? parameter)
        {
            Ejecutar(parameter);
        }
    }
}
