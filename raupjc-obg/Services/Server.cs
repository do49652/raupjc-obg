using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Fleck;

namespace raupjc_obg.Services
{
    public class Server : IServer
    {
        public bool StartServer(Dictionary<IWebSocketConnection, Dictionary<string, string>> sockets, string address, Action onOpen, Action onClose, Action<Dictionary<IWebSocketConnection, Dictionary<string, string>>, IWebSocketConnection, string> onMessage)
        {
            var server = new WebSocketServer(address);
            try
            {
                server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        sockets.Add(socket, new Dictionary<string, string>());
                        onOpen.Invoke();
                    };
                    socket.OnClose = () =>
                    {
                        onClose.Invoke();
                        sockets.Remove(socket);
                    };
                    socket.OnMessage = m => onMessage(sockets, socket, m);
                });
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}