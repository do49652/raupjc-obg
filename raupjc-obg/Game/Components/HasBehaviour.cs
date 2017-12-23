using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
