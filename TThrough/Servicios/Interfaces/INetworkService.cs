using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TThrough.Entidades;

namespace TThrough.Servicios.Interfaces
{
    internal interface INetworkService
    {
        bool ClienteConectado();
        Task EnviarMensaje(string jsonSerializado);

        Task RecibirMensajes(CancellationToken token);
    }
}
