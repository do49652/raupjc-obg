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
        
        public List<Event> MiniEvents { get; set; }
        public Dictionary<int, Event> SetEvents { get; set; }
        public Dictionary<string, Item> Items { get; set; }

        public float StartingMoney { get; set; }
    }
}