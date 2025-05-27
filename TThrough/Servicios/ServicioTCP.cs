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
            _client = new TcpClient("192.168.1.13", 65030);
            _stream = _client.GetStream();


            if (!_client.Connected) 
            {
                return false;
            }

            return true;
        }

        public async Task EnviarMensaje(string mensaje)
        {
            if (_stream != null && _client.Connected)
            {
                if (string.IsNullOrWhiteSpace(mensaje)) return;
                if (_stream != null && _client.Connected)
                {
                    byte[] mensajeBytes = Encoding.UTF8.GetBytes(mensaje);
                    int longitud = mensajeBytes.Length;

                    
                    byte[] longitudBytes = BitConverter.GetBytes(longitud);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(longitudBytes);

                    try
                    {
                        await _stream.WriteAsync(longitudBytes, 0, longitudBytes.Length);
                        await _stream.WriteAsync(mensajeBytes, 0, mensajeBytes.Length);
                        await _stream.FlushAsync();  

                        Console.WriteLine($"[Cliente] Enviado: {mensaje}");
                        MensajeEnviado?.Invoke(this, mensaje);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Cliente] Error al enviar: {ex.Message}");
                    }
                }
            }
        }


        /// <summary>
        /// Se introduce un token que propaga el cierre del hilo cuando llegue el mensaje
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task RecibirMensajes(CancellationToken token)
        {
            byte[] longitudBytes = new byte[4];

            while (!token.IsCancellationRequested)
            {
                try
                {
                    
                    int leidos = await _stream.ReadAsync(longitudBytes, 0, 4, token);
                    if (leidos == 0)
                    {
                        Console.WriteLine("[Cliente] Stream cerrado por el servidor.");
                        break;
                    }

                    if (leidos < 4)
                    {
                        Console.WriteLine($"[Cliente] Error: se leyeron solo {leidos} bytes de longitud.");
                        continue;
                    }

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(longitudBytes);

                    int longitudMensaje = BitConverter.ToInt32(longitudBytes, 0);

                    if (longitudMensaje <= 0 || longitudMensaje > 1_000_000)
                    {
                        Console.WriteLine($"[Cliente] Longitud inválida: {longitudMensaje}");
                        continue;
                    }

                    
                    byte[] mensajeBytes = new byte[longitudMensaje];
                    int totalLeido = 0;

                    while (totalLeido < longitudMensaje)
                    {
                        int bytesRestantes = longitudMensaje - totalLeido;
                        int bytesLeidos = await _stream.ReadAsync(mensajeBytes, totalLeido, bytesRestantes, token);

                        if (bytesLeidos == 0)
                        {
                            Console.WriteLine("[Cliente] Stream cerrado inesperadamente durante lectura de mensaje.");
                            break;
                        }

                        totalLeido += bytesLeidos;
                    }

                    if (totalLeido < longitudMensaje)
                    {
                        Console.WriteLine($"[Cliente] Solo se leyeron {totalLeido}/{longitudMensaje} bytes. Se descarta el mensaje.");
                        continue;
                    }

                    string mensaje = Encoding.UTF8.GetString(mensajeBytes);
                    Console.WriteLine($"[Cliente] Recibido: {mensaje}");

                    try
                    {
                        MensajeRecibido?.Invoke(this, mensaje);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[Cliente] Error al procesar el mensaje: {e.Message}");
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("[Cliente] Cancelación solicitada.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Cliente] Excepción inesperada: {ex.Message}");
                    await Task.Delay(500); // Esperar antes de reintentar
                }
            }

            Console.WriteLine("[Cliente] Finalizó la recepción de mensajes.");
        }
    }
}
