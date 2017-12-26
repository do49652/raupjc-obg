using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using raupjc_obg.Models;
using raupjc_obg.Models.ContentViewModels;
using raupjc_obg.Repositories;

namespace raupjc_obg.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private IGameRepository _gameRepository;
        private IEventRepository _eventRepository;
        private IItemRepository _itemRepository;

        public ContentController(UserManager<ApplicationUser> userManager, IGameRepository gameRepository,
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
    }
}