using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Repositories
{
    public interface IItemRepository
    {
        Task<List<ItemModel>> GetAll();
        Task<List<ItemModel>> GetAllByUserAsync(Guid userId);

        Task<ItemModel> GetItemById(Guid id);
        Task<ItemModel> GetItemByName(string name);
        Task<List<ItemModel>> GetAllByGame(GameModel game);
        Task<List<ItemModel>> GetAllByGames(List<GameModel> games);

        Task<bool> Add(ItemModel item);
    }
}