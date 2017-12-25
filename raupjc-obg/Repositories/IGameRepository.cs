using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Repositories
{
    public interface IGameRepository
    {
        Task<List<GameModel>> GetAll();
        Task<List<GameModel>> GetAllPublicGames();
        Task<List<GameModel>> GetAllPrivateGames();

        Task<List<GameModel>> GetAllByUser(Guid userId);
        Task<List<GameModel>> GetPublicGames(Guid userId);
        Task<List<GameModel>> GetPrivateGames(Guid userId);

        Task<GameModel> GetGameById(Guid id);
        Task<GameModel> GetGameByName(string name);

        Task<bool> Add(GameModel game);
    }
}