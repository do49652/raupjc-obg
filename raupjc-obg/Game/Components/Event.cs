using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public abstract class Event : HasBehaviour
    {
        public Event()
        {
            HappensOnce = false;
            Items = new Dictionary<string, object[]>();
            SetBehaviour();
        }

        public int Repeat { get; set; }
        public Event NextEvent { get; set; }
        public bool HappensOnce { get; set; }
        public Dictionary<string, object[]> Items { get; set; }
    }
}