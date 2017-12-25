using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using raupjc_obg.Data;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly GameDbContext _dbContext;

        public ItemRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ItemModel>> GetAll()
        {
            return await _dbContext.Items.ToListAsync();
        }

        public async Task<List<ItemModel>> GetAllByUserAsync(Guid userId)
        {
            return await _dbContext.Items.Where(i => i.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<ItemModel> GetItemById(Guid id)
        {
            return await _dbContext.Items.FirstOrDefaultAsync(i => i.Id.Equals(id));
        }

        public async Task<ItemModel> GetItemByName(string name)
        {
            return await _dbContext.Items.FirstOrDefaultAsync(i => i.Name.Equals(name));
        }

        public async Task<List<ItemModel>> GetAllByGame(GameModel game)
        {
            return await _dbContext.Items.Where(i => i.Game.Name.Equals(game.Name)).ToListAsync();
        }

        public async Task<List<ItemModel>> GetAllByGames(List<GameModel> games)
        {
            return await _dbContext.Items.Where(i => games.Select(g => g.Name).ToList().Contains(i.Game.Name)).ToListAsync();
        }

        public async Task<bool> Add(ItemModel item)
        {
            var oItem = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id.Equals(item.Id)) ?? await _dbContext.Items.FirstOrDefaultAsync(i => i.Name.Equals(item.Name));
            if (oItem != null && oItem.UserId.Equals(item.UserId) && oItem.Game.Name.Equals(item.Game.Name))
            {
                oItem.Name = item.Name;
                oItem.Description = item.Description;
                oItem.Category = item.Category;
                oItem.Behaviour = item.Behaviour;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            if (oItem != null)
                return false;

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id.Equals(item.Game.Id)) ?? await _dbContext.Games.FirstOrDefaultAsync(g => g.Name.Equals(item.Game.Name));

            if (game == null)
                return false;

            item.Game = game;

            _dbContext.Items.Add(item);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}