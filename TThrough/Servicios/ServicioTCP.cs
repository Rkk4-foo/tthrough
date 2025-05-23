using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TThrough.mvvm.Models;

namespace TThrough.Servicios
{
    /// <summary>
    /// Método que integra la interfaz de para realizar todas las tareas en las que se requiere conectarse al servidor
    /// </summary>
    public class ServicioTCP : Interfaces.INetworkService
    {
        private TcpClient _client {  get; set; }
        private NetworkStream _stream;
        public event EventHandler<string> MensajeRecibido;

        public event EventHandler<string> MensajeEnviado;

        public bool ClienteConectado()
        {
            _client = new TcpClient("192.168.1.13", 65032);
            _stream = _client.GetStream();


            if (!_client.Connected) 
            {
                return false;
            }

            return true;
        }

        public Task EnviarMensaje(string mensaje)
        {
            return Task.Run(() =>
            {
                if (_stream != null && _client.Connected)
                {
                    string json = JsonSerializer.Serialize(mensaje);
                    byte[] data = Encoding.UTF8.GetBytes(json);
                    _stream.Write(data, 0, data.Length);
                    MensajeEnviado?.Invoke(this, json);
                }
            });
        }

        /// <summary>
        /// Se introduce un token que propaga el cierre del hilo cuando llegue el mensaje
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RecibirMensajes(CancellationToken token)
        {
            byte[] buffer = new byte[1024];

            //Si la cancelación no se ha pedido
            if (!token.IsCancellationRequested) 
            {
                try
                {   
                    
                    int bytesLeidos = await _stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesLeidos > 0) 
                    {
                        string message = Encoding.UTF8.GetString(buffer,0,bytesLeidos);
                        MensajeRecibido?.Invoke(this, message);
                    }

                }
                catch (Exception ex) when (!(ex is OperationCanceledException)) 
                {
                    
                }
            }
        }
    }
}
