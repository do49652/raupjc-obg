using System.Collections.Generic;
using Newtonsoft.Json;

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

        [JsonIgnore]
        public int Repeat { get; set; }
        [JsonIgnore]
        public Event NextEvent { get; set; }
        [JsonIgnore]
        public bool HappensOnce { get; set; }
        public Dictionary<string, object[]> Items { get; set; }
    }
}