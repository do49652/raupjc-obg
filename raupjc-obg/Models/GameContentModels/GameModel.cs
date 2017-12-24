using System;
using System.Collections.Generic;
using System.Linq;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Models.GameContentModels
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool Private { get; set; }

        public bool Standalone { get; set; }
        public List<GameModel> Dependencies { get; set; }
        public List<EventModel> Events { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<EventModel> MiniEvents { get; set; }
        public Dictionary<int, EventModel> SetEvents { get; set; }
        public Dictionary<string, ItemModel> Items { get; set; }

        public float StartingMoney { get; set; }

        public Game.Components.Game CreateGameEntity(List<Game.Components.Game> loadedGameModels = null, List<Event> loadedEventModels = null, List<Item> loadedItemModels = null)
        {
            var game = new Game.Components.Game
            {
                Name = Name,
                Description = Description,
                LoadedGameModels = loadedGameModels ?? new List<Game.Components.Game>(),
                LoadedEventModels = loadedEventModels ?? new List<Event>(),
                LoadedItemModels = loadedItemModels ?? new List<Item>(),
                MiniEvents = new List<Event>(),
                SetEvents = new Dictionary<int, Event>(),
                Items = new Dictionary<string, Item>(),
                StartingMoney = StartingMoney

            };

            Dependencies.ForEach(dependency =>
            {
                if (game.LoadedGameModels.FirstOrDefault(d => d.Name.Equals(dependency.Name)) != null)
                    return;

                var game2 = dependency.CreateGameEntity(game.LoadedGameModels, game.LoadedEventModels, game.LoadedItemModels);
                game.LoadedGameModels.Add(game2);
                game.LoadedGameModels.AddRange(game2.LoadedGameModels.Except(game.LoadedGameModels));

                game.LoadedItemModels.AddRange(game2.LoadedItemModels.Except(game.LoadedItemModels));
            });

            Items.Keys.ToList().ForEach(item =>
            {
                if (game.LoadedItemModels.FirstOrDefault(i => i.Name.Equals(item)) == null)
                    game.LoadedItemModels.Add(Items[item].CreateGameItemEntity());
            });

            Events.ForEach(evnt =>
            {
                if (game.LoadedEventModels.FirstOrDefault(e => e.Name.Equals(evnt.Name)) != null)
                    return;

                var evnt2 = evnt.CreateGameEventEntity();

                game.LoadedEventModels.AddRange(((List<Event>)evnt2[1]).Except(game.LoadedEventModels));
                game.LoadedItemModels.AddRange(((List<Item>)evnt2[2]).Except(game.LoadedItemModels));

                game.LoadedEventModels.Add((Event)evnt2[0]);
            });

            MiniEvents.ForEach(miniEvent =>
            {
                var me = game.LoadedEventModels.FirstOrDefault(e => e.Name.Equals(miniEvent.Name));
                if (me != null)
                    game.MiniEvents.Add(me);
            });

            SetEvents.Keys.ToList().ForEach(setEvent =>
            {
                var se = game.LoadedEventModels.FirstOrDefault(e => e.Name.Equals(SetEvents[setEvent].Name));
                if (se != null)
                    game.SetEvents[setEvent] = se;
            });

            Items.Keys.ToList().ForEach(item =>
            {
                var i = game.LoadedItemModels.FirstOrDefault(ii => ii.Name.Equals(item));
                if (i != null)
                    game.Items[item] = i;
            });

            return game;
        }
    }
}