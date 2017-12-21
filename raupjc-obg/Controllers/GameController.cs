using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;
using Microsoft.AspNetCore.Mvc;
using raupjc_obg.Game;
using raupjc_obg.Models.GameViewModels;
using raupjc_obg.Services;

namespace raupjc_obg.Controllers
{
    public class GameController : Controller
    {
        public GameController(IServer server)
        {
            server.StartServer("ws://0.0.0.0:8181", () => { }, () => { }, (sockets, games, socket, message) =>
            {
                switch (message.Contains(":") ? message.Split(':')[0].Trim() : message)
                {
                    case "list":
                        sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList().ForEach(s =>
                        {
                            s.Send(string.Join("<br>",
                                    sockets.Values.Where(u => u["gamename"].Equals(sockets[socket]["gamename"]))
                                        .Select(u => u["username"]).ToList()));
                        });
                        break;
                    case "new":
                        var username = message.Split(':')[1].Trim();
                        var gamename = message.Split(':')[2].Trim();
                        var password = message.Split(':')[3].Trim();

                        sockets[socket]["username"] = username;
                        sockets[socket]["gamename"] = gamename;

                        if (!games.ContainsKey(sockets[socket]["gamename"]))
                        {
                            games[gamename] = new GameManager
                            {
                                GameName = gamename,
                                Password = password,
                                FinishSpace = 100,
                                StartGold = 10,
                                Players = new Dictionary<string, Player>(),
                                GameStarted = false,
                                Turn = 0
                            };
                        }
                        else if (games.ContainsKey(gamename) && !games[gamename].Password.Equals(password))
                        {
                            socket.Send("wrong-password");
                            break;
                        }
                        else if (games.ContainsKey(gamename) && games[gamename].Password.Equals(password) && games[gamename].Players.ContainsKey(username))
                        {
                            socket.Send("username-taken");
                            break;
                        }

                        games[gamename].Players[username] = new Player
                        {
                            Username = username,
                            Admin = games[gamename].Players.Count == 0,
                            Space = 0
                        };

                        if (games[gamename].Players[username].Admin)
                            socket.Send("admin");

                        goto case "list";
                    case "start":
                        sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList()
                            .ForEach(s => s.Send("clean"));
                        break;
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
            return View(vm);
        }
    }
}