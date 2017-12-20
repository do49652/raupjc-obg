using System;
using System.Collections.Generic;
using Fleck;

namespace raupjc_obg.Services
{
    public interface IServer
    {
        bool StartServer(Dictionary<IWebSocketConnection, Dictionary<string, string>> sockets, string address, Action onOpen, Action onClose, Action<Dictionary<IWebSocketConnection, Dictionary<string, string>>, IWebSocketConnection, string> onMessage);
    }
}