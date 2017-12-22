using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Behaviour { get; set; }

        public Event()
        {
            Behaviour = new List<string>();
        }
    }
}