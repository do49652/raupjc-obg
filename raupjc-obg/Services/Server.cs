using System;
using System.Collections.Generic;
using Fleck;
using raupjc_obg.Game;
using raupjc_obg.Repositories;

namespace raupjc_obg.Services
{
    public class Server : IServer
    {
        public string ConnectionString { get; set; }

        public Server(string cnnstr)
        {
            ConnectionString = cnnstr;
        }

        public bool StartServer(string address, Action onOpen, Action onClose,
            Action<string, Dictionary<IWebSocketConnection, Dictionary<string, string>>, Dictionary<string, GameManager>,
                IWebSocketConnection, string> onMessage)
        {
            var sockets = new Dictionary<IWebSocketConnection, Dictionary<string, string>>();
            var games = new Dictionary<string, GameManager>();
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
                    socket.OnMessage = message => onMessage(ConnectionString, sockets, games, socket, message);
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