using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using raupjc_obg.Data;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly GameDbContext _dbContext;

        public EventRepository(GameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EventModel>> GetAll()
        {
            return await _dbContext.Events.ToListAsync();
        }

        public async Task<List<EventModel>> GetAllByUser(Guid userId)
        {
            return await _dbContext.Events.Where(e => e.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<EventModel> GetEventById(Guid id)
        {
            return await _dbContext.Events.FirstOrDefaultAsync(e => e.Id.Equals(id));
        }

        public async Task<EventModel> GetEventByName(string name)
        {
            return await _dbContext.Events.FirstOrDefaultAsync(e => e.Name.Equals(name));
        }

        public async Task<List<EventModel>> GetAllByGame(GameModel game)
        {
            return await _dbContext.Events.Where(e => e.Game.Name.Equals(game.Name)).ToListAsync();
        }

        public async Task<List<EventModel>> GetAllByGames(List<GameModel> games)
        {
            var gamesNames = games.Select(g => g.Name).ToList();
            return await _dbContext.Events.Where(e => gamesNames.Contains(e.Game.Name)).ToListAsync();
        }

        public async Task<bool> Add(EventModel _event)
        {
            var oEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Id.Equals(_event.Id)) ?? await _dbContext.Events.FirstOrDefaultAsync(e => e.Name.Equals(_event.Name));
            if (oEvent != null && oEvent.UserId.Equals(_event.UserId) && oEvent.Game.Name.Equals(_event.Game.Name))
            {
                oEvent.Name = _event.Name;
                oEvent.Description = _event.Description;
                oEvent.Repeat = _event.Repeat;
                oEvent.HappensOnce = _event.HappensOnce;
                oEvent.Behaviour = _event.Behaviour;
                oEvent.Items = _event.Items;

                if (oEvent.NextEvent == null || (oEvent.NextEvent != null && _event.NextEvent != null && !oEvent.NextEvent.Id.Equals(_event.NextEvent.Id)))
                {
                    EventModel oNextEvent = null;
                    if (_event.NextEvent != null)
                        oNextEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Name.Equals(_event.NextEvent.Name));
                    oEvent.NextEvent = oNextEvent;
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }

            if (oEvent != null)
                return false;

            var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id.Equals(_event.Game.Id)) ?? await _dbContext.Games.FirstOrDefaultAsync(g => g.Name.Equals(_event.Game.Name));

            if (game == null)
                return false;

            _event.Game = game;

            if (_event.NextEvent != null)
            {
                var nextEvent = await _dbContext.Events.FirstOrDefaultAsync(e => e.Name.Equals(_event.NextEvent.Name));
                if (nextEvent == null)
                    return false;
                _event.NextEvent = nextEvent;
            }

            _dbContext.Events.Add(_event);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}