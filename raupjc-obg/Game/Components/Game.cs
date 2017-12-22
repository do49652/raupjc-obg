using System;
using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public class Game
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Event> Events { get; set; }
        public List<Event> MiniEvents { get; set; }
        public Dictionary<string, object[]> Items { get; set; }

        public List<string> Variables { get; set; }

        public float StartingMoney { get; set; }

        public Game()
        {
            Events = new List<Event>();
            MiniEvents = new List<Event>();
            Variables = new List<string>();
            Items = new Dictionary<string, object[]>();
        }
    }
}