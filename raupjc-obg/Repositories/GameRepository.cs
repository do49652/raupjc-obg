using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using raupjc_obg.Data;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameDbContext _dbContext;

        public GameRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GameModel>> GetAll()
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).ToListAsync();
        }

        public async Task<List<GameModel>> GetAllPublicGames()
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).Where(g => !g.Private).ToListAsync();
        }

        public async Task<List<GameModel>> GetAllPrivateGames()
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).Where(g => g.Private).ToListAsync();
        }

        public async Task<List<GameModel>> GetAllByUser(Guid userId)
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).Where(g => g.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<GameModel>> GetPublicGames(Guid userId)
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).Where(g => g.UserId.Equals(userId) && !g.Private).ToListAsync();
        }

        public async Task<List<GameModel>> GetPrivateGames(Guid userId)
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).Where(g => g.UserId.Equals(userId) && g.Private).ToListAsync();
        }

        public async Task<GameModel> GetGameById(Guid id)
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).FirstOrDefaultAsync(g => g.Id.Equals(id));
        }

        public async Task<GameModel> GetGameByName(string name)
        {
            return await _dbContext.Games.Include(g => g.Events).Include(g => g.Items).FirstOrDefaultAsync(g => g.Name.Equals(name));
        }

        public async Task<bool> Add(GameModel game)
        {
            var oGame = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id.Equals(game.Id)) ?? await _dbContext.Games.FirstOrDefaultAsync(g => g.Name.Equals(game.Name));
            if (oGame != null && oGame.UserId.Equals(game.UserId))
            {
                oGame.Name = game.Name;
                oGame.Description = game.Description;
                oGame.Private = game.Private;
                oGame.Standalone = game.Standalone;
                oGame.StartingMoney = game.StartingMoney;
                oGame.MiniEvents = game.MiniEvents;
                oGame.SetEvents = game.SetEvents;
            }
            else if (oGame != null)
                return false;
            else
                oGame = game;

            var missingDependency = false;
            var updatedDependencies = new List<GameModel>();
            game.Dependencies.ForEach(async dependency =>
            {
                if (missingDependency)
                    return;

                var d = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id.Equals(dependency.Id)) ?? await _dbContext.Games.FirstOrDefaultAsync(g => g.Name.Equals(dependency.Name));
                if (d == null)
                {
                    missingDependency = true;
                    return;
                }

                if (!updatedDependencies.Contains(d))
                    updatedDependencies.Add(d);
            });

            if (missingDependency)
                return false;

            oGame.Dependencies = updatedDependencies;

            var missingEvent = false;
            var updatedEvents = new List<EventModel>();
            game.Events.ForEach(async _event =>
            {
                if (missingEvent)
                    return;

                var e = await _dbContext.Events.FirstOrDefaultAsync(ev => ev.Id.Equals(_event.Id)) ?? await _dbContext.Events.FirstOrDefaultAsync(ev => ev.Name.Equals(_event.Name));
                if (e == null)
                {
                    missingEvent = true;
                    return;
                }

                if (!updatedEvents.Contains(e))
                    updatedEvents.Add(e);
            });

            if (missingEvent)
                return false;

            oGame.Events = updatedEvents;

            var missingItem = false;
            var updatedItems = new List<ItemModel>();
            game.Items.ForEach(async item =>
            {
                if (missingItem)
                    return;

                var i = await _dbContext.Items.FirstOrDefaultAsync(it => it.Id.Equals(item.Id)) ??
                        await _dbContext.Items.FirstOrDefaultAsync(it => it.Name.Equals(item.Name));
                if (i == null)
                {
                    missingItem = true;
                    return;
                }

                if (!updatedItems.Contains(i))
                    updatedItems.Add(i);
            });

            if (missingItem)
                return false;

            oGame.Items = updatedItems;

            _dbContext.Games.Add(oGame);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}