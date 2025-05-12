using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TThrough.Servicios.Interfaces
{
    internal interface INetworkService
    {
        bool ClienteConectado();
        Task EnviarMensaje(string message);

        

        Task RecibirMensajes(CancellationToken token);
    }
}
