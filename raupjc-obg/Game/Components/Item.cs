using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public abstract class Item : HasBehaviour
    {
        public string Category { get; set; }

        public Item()
        {
            SetBehaviour();
        }

    }
}