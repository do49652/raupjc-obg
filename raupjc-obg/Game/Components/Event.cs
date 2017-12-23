using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public abstract class Event : HasBehaviour
    {
        public int Repeat { get; set; }
        public Event NextEvent { get; set; }

        public Event()
        {
            SetBehaviour();
        }
    }
}