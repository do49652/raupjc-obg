using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public class Game
    {
        public Game()
        {
            Events = new List<Event>();
            MiniEvents = new List<Event>();
            SetEvents = new Dictionary<int, Event>();
            Items = new Dictionary<string, Item>();
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public List<Event> Events { get; set; }
        public List<Event> MiniEvents { get; set; }
        public Dictionary<int, Event> SetEvents { get; set; }
        public Dictionary<string, Item> Items { get; set; }

        public float StartingMoney { get; set; }
    }
}