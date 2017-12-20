using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;
using Microsoft.AspNetCore.Mvc;
using raupjc_obg.Models.GameViewModels;
using raupjc_obg.Services;

namespace raupjc_obg.Controllers
{
    public class GameController : Controller
    {
        private string _game;

        public GameController(IServer server)
        {
            server.StartServer(new Dictionary<IWebSocketConnection, Dictionary<string, string>>(), "ws://0.0.0.0:8181", () => { }, () => { }, (sockets, socket, message) =>
            {
                switch (message.Contains(":") ? message.Split(':')[0].Trim() : message)
                {
                    case "list":
                        sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList().ForEach(s => s.Send(string.Join("<br>", sockets.Values.Where(u => u["gamename"].Equals(sockets[socket]["gamename"])).Select(u => u["username"]).ToList())));
                        break;
                    case "new":
                        sockets[socket]["username"] = message.Split(':')[1].Trim();
                        sockets[socket]["gamename"] = message.Split(':')[2].Trim();
                        goto case "list";
                }
            });
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Join()
        {
            return View(new JoinGameViewModel());
        }

        [HttpPost]
        public IActionResult Join(JoinGameViewModel vm)
        {
            _game = vm.GameName;
            return View(vm);
        }
    }
}