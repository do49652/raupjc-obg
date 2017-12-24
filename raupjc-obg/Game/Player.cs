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

        public Event CurrentEvent { get; set; }
        public int RepeatEvent { get; set; }
        public List<Event> VisitedEvents { get; set; }

        public List<Item> Items;
        public float Money { get; set; }

        public Player()
        {
            Items = new List<Item>();
            VisitedEvents = new List<Event>();
        }
    }
}