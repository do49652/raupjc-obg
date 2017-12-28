using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using raupjc_obg.Game;
using raupjc_obg.Models.GameViewModels;
using raupjc_obg.Repositories;
using raupjc_obg.Services;
using raupjc_obg.Data;
using raupjc_obg.Models.GameContentModels;
using System.Data.Entity;

namespace raupjc_obg.Controllers
{
    public class GameController : Controller
    {

        private IServer _server;

        public GameController(IServer server)
        {
            _server = server;

            server.StartServer("ws://192.168.1.5:8181", () => { },
                (sockets, games, socket) =>
            {
                GameManager game = null;
                try { game = games[sockets[socket]["gamename"]]; }
                catch (Exception e) { return; }

                game.RunBehaviour(null, 0, "@Log+ -> left the game.", game.Players.Keys.ToList().IndexOf(sockets[socket]["username"]));
                game.Players.Remove(sockets[socket]["username"]);
                sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList().ForEach(s => s.Send("ready"));

                if (game.Players.Count == 0)
                    games.Remove(sockets[socket]["gamename"]);
            },
                (cnnstr, sockets, games, socket, message) =>
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
                         sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList()
                             .ForEach(s =>
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
                         sockets[socket]["lastI"] = "0";
                         sockets[socket]["lastItemI"] = "0";

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
                         else if (games.ContainsKey(gamename) && games[gamename].Password.Equals(password) &&
                                  games[gamename].GameStarted)
                         {
                             socket.Send("game-already-started");
                             break;
                         }
                         else if (games.ContainsKey(gamename) && games[gamename].Password.Equals(password) &&
                                  games[gamename].Players.ContainsKey(username))
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
                         var gameToStart = message.Split(':')[1];
                         GameModel gameModel = null;
                         using (var db = new GameDbContext(cnnstr))
                         {
                             gameModel = db.Games.Include(g => g.Events).Include(g => g.Items).FirstOrDefault(g => g.Name.Equals(gameToStart));
                         }

                         while (gameModel == null) ;
                         game.StartGame(gameModel.CreateGameEntity());
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

                         var m = game.RunBehaviour(
                             game.Players[game.Players.Keys.ToList()[game.WhosTurn()]].CurrentEvent, int.Parse(sockets[socket]["lastI"]),
                             message.Substring(5).Trim());
                         if (!m.Equals("@End"))
                         {
                             sockets[socket]["lastI"] = m.Split(new[] { "<!<" }, StringSplitOptions.None)[1]
                                 .Split(new[] { ">!>" }, StringSplitOptions.None)[0];
                             var rgx = new Regex("<!<[0-9]*>!>");
                             m = rgx.Replace(m, "");
                         }
                         else
                         {
                             sockets[socket]["lastI"] = "0";
                         }
                         if (m != null && m.Equals("@End"))
                             goto case "end";

                         game.Message = m;
                         goto case "sendReady";

                     case "event":
                         if (!game.CheckEvent())
                             goto case "end";

                         m = game.RunBehaviour(game.Players[game.Players.Keys.ToList()[game.WhosTurn()]].CurrentEvent,
                             int.Parse(sockets[socket]["lastI"]));
                         if (!m.Equals("@End"))
                         {
                             sockets[socket]["lastI"] = m.Split(new[] { "<!<" }, StringSplitOptions.None)[1]
                                 .Split(new[] { ">!>" }, StringSplitOptions.None)[0];
                             var rgx = new Regex("<!<[0-9]*>!>");
                             m = rgx.Replace(m, "");
                         }
                         else
                         {
                             sockets[socket]["lastI"] = "0";
                         }
                         if (m != null && m.Equals("@End"))
                             goto case "end";

                         game.Message = m;
                         goto case "sendReady";

                     case "item":
                         var mm = "";
                         if (message.Split(':').Length > 2)
                             mm = message.Substring(message.Split(':')[0].Length + message.Split(':')[1].Length + 2)
                                 .Trim();

                         m = game.RunBehaviour(
                             game.Game.Items.Values.FirstOrDefault(ii => ii.Name.Equals(message.Split(':')[1].Trim())),
                             int.Parse(sockets[socket]["lastItemI"]), mm.Length == 0 ? null : mm,
                             game.Players.Keys.ToList().IndexOf(sockets[socket]["username"]));
                         if (!m.Equals("@End"))
                         {
                             sockets[socket]["lastItemI"] = m.Split(new[] { "<!<" }, StringSplitOptions.None)[1]
                                 .Split(new[] { ">!>" }, StringSplitOptions.None)[0];
                             var rgx = new Regex("<!<[0-9]*>!>");
                             m = rgx.Replace(m, "");
                         }
                         else
                         {
                             sockets[socket]["lastItemI"] = "0";
                         }
                         socket.Send("item:" + message.Split(':')[1].Trim() + ":" + m);
                         goto case "sendReady";

                     case "chat":
                         sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList()
                             .ForEach(s => s.Send(message));
                         break;

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
            List<string> games = null;
            using (var db = new GameDbContext(((Server)_server).ConnectionString))
            {
                games = db.Games.Select(g => g.Name).ToList();
            }

            while (games == null) ;

            vm.gameNames = games;

            return View(vm);
        }
    }
}