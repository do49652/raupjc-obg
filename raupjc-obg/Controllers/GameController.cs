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
using Microsoft.AspNetCore.Identity;
using raupjc_obg.Models;
using System.Threading.Tasks;
using raupjc_obg.Models.OtherModels;
using raupjc_obg.Extensions;

namespace raupjc_obg.Controllers
{
    public class GameController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IServer _server;

        public GameController(UserManager<ApplicationUser> userManager, IServer server)
        {
            _userManager = userManager;
            _server = server;

            server.StartServer("ws://[::]:8181", () => { },
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
                                 Log = new List<string>(),
                                 FinishingSpace = 150
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
                         game.Players.Values.ToList().ForEach(p =>
                         {
                             if (p.Space >= game.FinishingSpace)
                             {
                                 game.Log.Add("[" + DateTime.Now + "] Game finished!");
                                 game.Log.Add("[" + DateTime.Now + "] " + p.Username + " won!");
                                 game.ChangeScene("end");
                             }
                         });

                         sockets.Keys.Where(s => sockets[s]["gamename"].Equals(sockets[socket]["gamename"])).ToList()
                             .ForEach(s => s.Send("ready"));
                         break;
                 }
             });
        }

        public async Task<IActionResult> Index()
        {
            var vm = new JoinGameViewModel();
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null && currentUser.InGameName != null && !currentUser.InGameName.Contains("@") && !currentUser.InGameName.Equals(""))
                vm.Username = currentUser.InGameName;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(JoinGameViewModel vm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (vm.Username == null || vm.GameName == null || vm.Password == null || "".Equals(vm.Username) || "".Equals(vm.GameName) || "".Equals(vm.Password))
                return View(vm);

            List<Review> reviews = null;
            List<GameModel> games = null;
            using (var db = new GameDbContext(((Server)_server).ConnectionString))
            {
                games = db.Games.Where(g => !g.Private).ToList();
                reviews = db.Reviews.Include(r => r.Game).Where(r => !r.Game.Private).ToList();

                if (currentUser != null)
                {
                    games.AddRange(db.Games.Where(g => (g.Private && g.UserId.ToString().Equals(currentUser.Id))).ToList());
                    reviews.AddRange(db.Reviews.Include(r => r.Game).Where(r => (r.Game.Private && r.Game.UserId.ToString().Equals(currentUser.Id))).ToList());
                }
            }
            while (reviews == null || games == null) ;

            var revGames = reviews.Select(r => r.Game.Name).Distinct().ToList();
            var gamGames = games.Select(g => g.Name).Distinct().ToList();

            if (revGames.Count != gamGames.Count)
            {
                var diff = gamGames.Except(revGames).ToList();
                if (diff.Count > 0)
                {
                    foreach (var gn in diff)
                    {
                        reviews.Add(new Review
                        {
                            Game = new GameModel
                            {
                                Name = gn,
                                Description = games.First(g => g.Name.Equals(gn)).Description
                            },
                            Comment = null
                        });
                    }
                }
            }

            vm.Username = RegExHelperExtensions.ReplaceNotMatching(vm.Username, "[a-z0-9]*", "", RegexOptions.IgnoreCase);
            vm.GameName = RegExHelperExtensions.ReplaceNotMatching(vm.GameName, "[a-z0-9]*", "", RegexOptions.IgnoreCase);
            vm.Password = RegExHelperExtensions.ReplaceNotMatching(vm.Password, "[a-z0-9]*", "", RegexOptions.IgnoreCase);
            vm.reviews = reviews;

            if (currentUser == null)
                return View(vm);

            currentUser.InGameName = vm.Username;
            await _userManager.UpdateAsync(currentUser);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Rate(RateGameViewModel vm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
                return RedirectToAction("Index", "Home");
            using (var db = new GameDbContext(((Server)_server).ConnectionString))
            {
                var review = db.Reviews.FirstOrDefault(r => r.UserId.ToString().Equals(currentUser.Id) && r.Game.Name.Equals(vm.GameName));
                if (review == null)
                {
                    review = new Review
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.Parse(currentUser.Id),
                        Game = db.Games.First(g => g.Name.Equals(vm.GameName)),
                        Rating = 0,
                        Comment = ""
                    };
                    db.Reviews.Add(review);
                }

                review.Rating = vm.Rating;
                review.Comment = vm.Comment;

                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

    }
}