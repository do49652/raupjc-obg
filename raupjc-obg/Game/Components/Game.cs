using System.Collections.Generic;
using Newtonsoft.Json;

namespace raupjc_obg.Game.Components
{
    public class Game
    {
        [JsonIgnore]
        public List<Game> LoadedGameModels { get; set; }
        [JsonIgnore]
        public List<Event> LoadedEventModels { get; set; }
        [JsonIgnore]
        public List<Item> LoadedItemModels { get; set; }

        public Game()
        {
            MiniEvents = new List<Event>();
            SetEvents = new Dictionary<int, Event>();
            Items = new Dictionary<string, Item>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public List<Event> MiniEvents { get; set; }
        [JsonIgnore]
        public Dictionary<int, Event> SetEvents { get; set; }
        [JsonIgnore]
        public Dictionary<string, Item> Items { get; set; }

        [JsonIgnore]
        public float StartingMoney { get; set; }
    }
}