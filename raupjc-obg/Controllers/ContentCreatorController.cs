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

        public IActionResult Index()
        {
            return View(new GameViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(GameViewModel gameVm)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            gameVm.Id = Guid.NewGuid().ToString();
            gameVm.UserId = currentUser.Id;
            gameVm.Name = gameVm.Name.Replace("@", "").Replace(";", "").Replace("->", " ").Replace(":", "_").Replace(",", " ").Replace("  ", " ");

            gameVm.GameModels = await _gameRepository.GetAll();

            if (gameVm.GameModels.Select(g => g.Name).ToList().Contains(gameVm.Name))
                return View(gameVm);

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

            return View(gameVm);
        }

        public async Task<IActionResult> Game(string id)
        {
            var game = await _gameRepository.GetGameById(Guid.Parse(id));
            var gameVm = game.CreateGameViewModel();

            gameVm.GameModels = game.Dependencies ?? new List<GameModel>();
            if (!gameVm.GameModels.Contains(game))
                gameVm.GameModels.Insert(0, game);

            gameVm.EventModels = await _eventRepository.GetAllByGames(gameVm.GameModels);
            gameVm.ItemModels = await _itemRepository.GetAllByGames(gameVm.GameModels);

            return View(gameVm);
        }
    }
}