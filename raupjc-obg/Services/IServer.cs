using System;
using System.Collections.Generic;
using Fleck;
using raupjc_obg.Game;

namespace raupjc_obg.Services
{
    public interface IServer
    {
        bool StartServer(string address, Action onOpen, Action onClose, Action<Dictionary<IWebSocketConnection, Dictionary<string, string>>, Dictionary<string, GameManager>, IWebSocketConnection, string> onMessage);
    }
}