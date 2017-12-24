using System;
using System.Collections.Generic;

namespace raupjc_obg.Models.GameContentModels
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool Private { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<EventModel> Events { get; set; }
        public List<EventModel> MiniEvents { get; set; }
        public Dictionary<int, EventModel> SetEvents { get; set; }
        public Dictionary<string, ItemModel> Items { get; set; }

        public float StartingMoney { get; set; }

        public Game.Components.Game CreateGameEntity()
        {
            var game = new Game.Components.Game
            {
                Name = Name,
                Description = Description
            };
            //TODO: Events, MiniEvents, SetEvents, Items
            return game;
        }
    }
}