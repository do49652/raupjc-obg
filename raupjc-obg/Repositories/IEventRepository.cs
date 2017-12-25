using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Repositories
{
    public interface IEventRepository
    {
        Task<List<EventModel>> GetAll();
        Task<List<EventModel>> GetAllByUser(Guid userId);

        Task<EventModel> GetEventById(Guid id);
        Task<EventModel> GetEventByName(string name);
        Task<List<EventModel>> GetAllByGame(GameModel game);
        Task<List<EventModel>> GetAllByGames(List<GameModel> games);

        Task<bool> Add(EventModel _event);
    }
}