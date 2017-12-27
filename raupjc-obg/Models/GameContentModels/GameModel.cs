using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using raupjc_obg.Game.Components;
using raupjc_obg.Models.ContentViewModels;

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

        public string MiniEvents { get; set; }
        public string SetEvents { get; set; }
        public List<ItemModel> Items { get; set; }

        public float StartingMoney { get; set; }

        public GameModel() { }

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

            Dependencies?.ForEach(dependency =>
            {
                if (game.LoadedGameModels.FirstOrDefault(d => d.Name.Equals(dependency.Name)) != null)
                    return;

                var game2 = dependency.CreateGameEntity(game.LoadedGameModels, game.LoadedEventModels, game.LoadedItemModels);
                game.LoadedGameModels.Add(game2);
                game.LoadedGameModels.AddRange(game2.LoadedGameModels.Except(game.LoadedGameModels));

                game.LoadedEventModels.AddRange(game2.LoadedEventModels.Except(game.LoadedEventModels));
                game.LoadedItemModels.AddRange(game2.LoadedItemModels.Except(game.LoadedItemModels));
            });

            Items?.ToList().ForEach(item =>
            {
                if (game.LoadedItemModels.FirstOrDefault(i => i.Name.Equals(item.Name)) == null)
                    game.LoadedItemModels.Add(item.CreateGameItemEntity());
            });

            Events?.ForEach(evnt =>
            {
                if (game.LoadedEventModels.FirstOrDefault(e => e.Name.Equals(evnt.Name)) != null)
                    return;

                var evnt2 = evnt.CreateGameEventEntity();

                game.LoadedEventModels.AddRange(((List<Event>)evnt2[1]).Except(game.LoadedEventModels));
                game.LoadedItemModels.AddRange(((List<Item>)evnt2[2]).Except(game.LoadedItemModels));

                game.LoadedEventModels.Add((Event)evnt2[0]);
            });

            MiniEvents?.Split('\n').ToList().ForEach(miniEvent =>
            {
                var me = game.LoadedEventModels.FirstOrDefault(e => e.Name.Equals(miniEvent));
                if (me != null)
                    game.MiniEvents.Add(me);
            });

            SetEvents?.Split('\n').ToList().ForEach(setEvent =>
            {
                var se = game.LoadedEventModels.FirstOrDefault(e => e.Name.Equals(setEvent.Split(':')[1]));
                if (se != null)
                    game.SetEvents[int.Parse(setEvent.Split(':')[0])] = se;
            });

            Items?.ToList().ForEach(item =>
            {
                var i = game.LoadedItemModels.FirstOrDefault(ii => ii.Name.Equals(item.Name));
                if (i != null)
                    game.Items[item.Name] = i;
            });

            return game;
        }

        public GameViewModel CreateGameViewModel()
        {
            var game = new GameViewModel
            {
                Id = Id.ToString(),
                UserId = UserId.ToString(),
                Private = Private,
                Standalone = Standalone,
                Name = Name,
                Description = Description,
                StartingMoney = StartingMoney,
                Dependencies = "",
                Events = "",
                Items = "",
                MiniEvents = MiniEvents,
                SetEvents = SetEvents
            };

            if (Dependencies != null && Dependencies.Count > 0)
                game.Dependencies = string.Join("\n", Dependencies.Select(d => d.Name).ToList());
            if (Events != null && Events.Count > 0)
                game.Events = string.Join("\n", Events.Select(e => e.Name).ToList());
            if (Items != null && Items.Count > 0)
                game.Items = string.Join("\n", Items.Select(i => i.Name).ToList());

            return game;
        }
    }
}