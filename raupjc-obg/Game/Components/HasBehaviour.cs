using System.Collections.Generic;

namespace raupjc_obg.Game.Components
{
    public abstract class HasBehaviour
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Behaviour { get; set; }
        public abstract void SetBehaviour();
    }
}