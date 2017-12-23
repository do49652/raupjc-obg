using System.Collections.Generic;
using Newtonsoft.Json;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Game
{
    public class Player
    {
        public string Username { get; set; }
        public bool Admin { get; set; }

        public int Space { get; set; }

        [JsonIgnore]
        public List<Event> EventsVisited { get; set; }
        
        public Event CurrentEvent { get; set; }
        [JsonIgnore]
        public int CurrentEventLine { get; set; }
        public int RepeatEvent { get; set; }
        
        public List<Item> Items;
        public float Money { get; set; }

        public Player()
        {
            EventsVisited = new List<Event>();
            Items = new List<Item>();
        }
    }
}