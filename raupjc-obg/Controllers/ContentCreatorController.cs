using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using raupjc_obg.Models;
using raupjc_obg.Models.ContentViewModels;
using raupjc_obg.Models.GameContentModels;
using raupjc_obg.Repositories;

namespace raupjc_obg.Controllers
{
    [Authorize]
    public class ContentCreatorController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IGameRepository _gameRepository;
        private IEventRepository _eventRepository;
        private IItemRepository _itemRepository;

        public ContentCreatorController(UserManager<ApplicationUser> userManager, IGameRepository gameRepository,
            IEventRepository eventRepository, IItemRepository itemRepository)
        {
            _userManager = userManager;
            _gameRepository = gameRepository;
            _eventRepository = eventRepository;
            _itemRepository = itemRepository;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var games = (await _gameRepository.GetAllByUser(Guid.Parse(currentUser.Id))).Select(g => g.CreateGameViewModel()).OrderBy(g => g.Name).ToList();
            return View(games);
        }

        [HttpPost]
        public async Task<IActionResult> Game(GameViewModel gameVm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            gameVm.Id = Guid.NewGuid().ToString();
            gameVm.UserId = currentUser.Id;
            gameVm.Name = gameVm.Name.Replace("@", "").Replace(";", "").Replace("->", " ").Replace(":", "_").Replace(",", " ").Replace("  ", " ");

            gameVm.GameModels = await _gameRepository.GetAll();

            if (gameVm.GameModels.Select(g => g.Name).ToList().Contains(gameVm.Name))
                return RedirectToAction("Game", new { id = gameVm.Id });

            if (await _gameRepository.Add(new GameModel
            {
                Id = Guid.Parse(gameVm.Id),
                UserId = Guid.Parse(gameVm.UserId),
                Name = gameVm.Name,
                Description = gameVm.Description,
                Dependencies = new List<GameModel>(),
                Events = new List<EventModel>(),
                Items = new List<ItemModel>(),
                MiniEvents = "",
                Private = true,
                SetEvents = "",
                StartingMoney = 0,
                Standalone = false
            }))
                return RedirectToAction("Game", new { id = gameVm.Id });

            return RedirectToAction("Game", new { id = gameVm.Id });
        }

        public async Task<IActionResult> Game(string id)
        {
            var game = await _gameRepository.GetGameById(Guid.Parse(id));
            var gameVm = game.CreateGameViewModel();

            gameVm.GameModels = game.Dependencies ?? new List<GameModel>();
            if (!gameVm.GameModels.Contains(game))
                gameVm.GameModels.Insert(0, game);

            gameVm.EventModels = (await _eventRepository.GetAllByGames(gameVm.GameModels)).OrderBy(g => g.Name).ToList();
            gameVm.ItemModels = (await _itemRepository.GetAllByGames(gameVm.GameModels)).OrderBy(i => i.Name).ToList();

            return View(gameVm);
        }

        [HttpPost]
        public async Task<IActionResult> Event(EventViewModel eventVm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            eventVm.Id = Guid.NewGuid().ToString();
            eventVm.UserId = currentUser.Id;
            eventVm.GameName = eventVm.GameName.Replace("@", "").Replace(";", "").Replace("->", " ").Replace(":", "_").Replace(",", " ").Replace("  ", " ");
            eventVm.Name = eventVm.Name.Replace("@", "").Replace(";", "").Replace("->", " ").Replace(":", "_").Replace(",", " ").Replace("  ", " ");

            var game = await _gameRepository.GetGameByName(eventVm.GameName);
            if (game == null)
                return RedirectToAction("Index");

            eventVm.EventModels = await _eventRepository.GetAll();

            if (eventVm.EventModels.Select(e => e.Name).ToList().Contains(eventVm.Name))
                return RedirectToAction("Game", new { id = game.Id });

            if (await _eventRepository.Add(new EventModel
            {
                Id = Guid.Parse(eventVm.Id),
                UserId = Guid.Parse(eventVm.UserId),
                Game = game,
                Name = eventVm.Name,
                Description = eventVm.Description,
                Behaviour = "@Begin\n@End",
                Repeat = 0,
                NextEvent = null,
                HappensOnce = false,
                Items = ""
            }))
                return RedirectToAction("Event", new { name = eventVm.Name });

            return RedirectToAction("Game", new { id = game.Id });
        }

        public async Task<IActionResult> Event(string name)
        {
            var _event = await _eventRepository.GetEventByName(name);
            var game = await _gameRepository.GetGameById(_event.Game.Id);
            var eventVm = _event.CreateEventViewModel();

            eventVm.GameModels = game.Dependencies ?? new List<GameModel>();
            if (!eventVm.GameModels.Contains(game))
                eventVm.GameModels.Insert(0, game);

            eventVm.EventModels = await _eventRepository.GetAllByGames(eventVm.GameModels);
            eventVm.ItemModels = await _itemRepository.GetAllByGames(eventVm.GameModels);

            if (eventVm.Items == null)
                eventVm.Items = "";

            return View(eventVm);
        }

        [HttpPost]
        public async Task<IActionResult> Item(ItemViewModel itemVm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            itemVm.Id = Guid.NewGuid().ToString();
            itemVm.UserId = currentUser.Id;
            itemVm.GameName = itemVm.GameName.Replace("@", "").Replace(";", "").Replace("->", " ").Replace(":", "_").Replace(",", " ").Replace("  ", " ");
            itemVm.Name = itemVm.Name.Replace("@", "").Replace(";", "").Replace("->", " ").Replace(":", "_").Replace(",", " ").Replace("  ", " ");

            var game = await _gameRepository.GetGameByName(itemVm.GameName);
            if (game == null)
                return RedirectToAction("Index");

            itemVm.ItemModels = await _itemRepository.GetAll();

            if (itemVm.ItemModels.Select(e => e.Name).ToList().Contains(itemVm.Name))
                return RedirectToAction("Game", new { id = game.Id });

            if (await _itemRepository.Add(new ItemModel
            {
                Id = Guid.Parse(itemVm.Id),
                UserId = Guid.Parse(itemVm.UserId),
                Game = game,
                Name = itemVm.Name,
                Description = itemVm.Description,
                Behaviour = "@Begin\n@End",
                Category = ""
            }))
                return RedirectToAction("Item", new { name = itemVm.Name });

            return RedirectToAction("Game", new { id = game.Id });
        }

        public async Task<IActionResult> Item(string name)
        {
            var item = await _itemRepository.GetItemByName(name);
            var game = await _gameRepository.GetGameById(item.Game.Id);
            var itemVm = item.CreateItemViewModel();

            itemVm.GameModels = game.Dependencies ?? new List<GameModel>();
            if (!itemVm.GameModels.Contains(game))
                itemVm.GameModels.Insert(0, game);

            itemVm.EventModels = await _eventRepository.GetAllByGames(itemVm.GameModels);
            itemVm.ItemModels = await _itemRepository.GetAllByGames(itemVm.GameModels);

            return View(itemVm);
        }

        [HttpPost]
        public async Task<bool> SaveItemBehaviour(ItemViewModel itemVm)
        {
            var item = await _itemRepository.GetItemById(Guid.Parse(itemVm.Id)) ?? await _itemRepository.GetItemByName(itemVm.Name);
            item.Behaviour = itemVm.Behaviour;
            return await _itemRepository.Add(item);
        }

        [HttpPost]
        public async Task<bool> SaveEventBehaviour(EventViewModel eventVm)
        {
            var _event = await _eventRepository.GetEventById(Guid.Parse(eventVm.Id)) ?? await _eventRepository.GetEventByName(eventVm.Name);
            _event.Behaviour = eventVm.Behaviour;
            return await _eventRepository.Add(_event);
        }

        [HttpPost]
        public async Task<bool> SaveEventItems(EventViewModel eventVm)
        {
            var _event = await _eventRepository.GetEventById(Guid.Parse(eventVm.Id)) ?? await _eventRepository.GetEventByName(eventVm.Name);
            _event.Items = eventVm.Items;
            return await _eventRepository.Add(_event);
        }

        [HttpPost]
        public async Task<bool> SaveGameStartingMoney(GameViewModel gameVm)
        {
            var game = await _gameRepository.GetGameById(Guid.Parse(gameVm.Id)) ?? await _gameRepository.GetGameByName(gameVm.Name);
            game.StartingMoney = gameVm.StartingMoney;
            return await _gameRepository.Add(game);
        }

        [HttpPost]
        public async Task<bool> SaveGameRandomSetEvents(GameViewModel gameVm)
        {
            var game = await _gameRepository.GetGameById(Guid.Parse(gameVm.Id)) ?? await _gameRepository.GetGameByName(gameVm.Name);
            game.MiniEvents = gameVm.MiniEvents ?? "";
            game.SetEvents = gameVm.SetEvents ?? "";
            return await _gameRepository.Add(game);
        }

        [HttpPost]
        public async Task<bool> PublishGame(GameViewModel gameVm)
        {
            var game = await _gameRepository.GetGameById(Guid.Parse(gameVm.Id)) ?? await _gameRepository.GetGameByName(gameVm.Name);
            game.Private = false;
            return await _gameRepository.Add(game);
        }


        [HttpPost]
        public async Task<bool> RemoveGame(GameViewModel gameVm)
        {
            var game = await _gameRepository.GetGameById(Guid.Parse(gameVm.Id)) ?? await _gameRepository.GetGameByName(gameVm.Name);
            return await _gameRepository.Remove(game);
        }

        [AllowAnonymous]
        [HttpPost]
        public String GetGames()
        {
            List<GameModel> games = null;
            var t = _gameRepository.GetAllPublicGames();
            while (!t.IsCompleted) ;
            games = t.Result;

            String str = "";
            games.ForEach(g =>
            {
                str += g.Name + ":";
            });
            return str;
        }

        public IActionResult Documentation()
        {
            return View();
        }


    }
}