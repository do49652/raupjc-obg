﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fleck;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
                GameManager game = null;
                try
                {
                    game = games[sockets[socket]["gamename"]];
                }
                catch (Exception e) { }

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
                                Players = new Dictionary<string, Player>(),
                                GameStarted = false,
                                Turn = 0,
                                Log = new List<string>()
                            };
                        }
                        else if (games.ContainsKey(gamename) && !games[gamename].Password.Equals(password))
                        {
                            socket.Send("wrong-password");
                            break;
                        }
                        else if (games.ContainsKey(gamename) && games[gamename].Password.Equals(password) && games[gamename].GameStarted)
                        {
                            socket.Send("game-already-started");
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
                            Space = 0,
                        };

                        if (games[gamename].Players[username].Admin)
                            socket.Send("admin");

                        goto case "list";

                    case "start":
                        game.StartGame();
                        sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList()
                            .ForEach(s => s.Send("start"));
                        game.ChangeScene("roll");
                        break;

                    case "ready":
                        socket.Send(JsonConvert.SerializeObject(game));
                        break;

                    case "roll":
                        game.ThrowDice();
                        game.ChangeScene("rolled");
                        goto case "sendReady";

                    case "move":
                        if (message.Contains(":"))
                            goto case "choice";

                        if (game.Scene.Equals("rolled"))
                            game.Move();
                        goto case "event";

                    case "choice":
                        if (game.Players[game.Players.Keys.ToList()[game.WhosTurn()]].CurrentEvent == null)
                            goto case "end";

                        var m = game.PlayEvent(message.Substring(5).Trim());
                        if (m != null && m.Equals("@End"))
                            goto case "end";

                        game.Message = m;
                        goto case "sendReady";

                    case "event":
                        if (!game.CheckEvent())
                            goto case "end";

                        m = game.PlayEvent();
                        if (m != null && m.Equals("@End"))
                            goto case "end";

                        game.Message = m;
                        goto case "sendReady";

                    case "end":
                        game.EndEvent();
                        game.Message = "";
                        game.ChangeScene("roll");
                        game.Next();
                        goto case "sendReady";

                    case "sendReady":
                        sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList()
                            .ForEach(s => s.Send("ready"));
                        break;
                }
            });
        }

        public IActionResult Index()
        {
            return View(new JoinGameViewModel());
        }

        [HttpPost]
        public IActionResult Index(JoinGameViewModel vm)
        {
            return View(vm);
        }
    }
}