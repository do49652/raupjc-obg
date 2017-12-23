using raupjc_obg.Game.Components;
using System.Collections.Generic;

namespace raupjc_obg.Game.Content.ZagrebContent
{
    public class PonistenaZetKarta : Item
    {
        public PonistenaZetKarta()
        {
            Name = "Ponistena Zet karta";
            Description = "Karta za voznju ZET prijevozom.";
            Category = "NoEvent";
        }

        public override void SetBehaviour()
        {
            Behaviour = new List<string>
            {
                "@Begin",

                "@NoEvent -> Throw",
                "@Goto -> Kraj",

                "@Throw",
                "@Remove -> Zet karta",
                "@Log+ -> zagaduje okolis. ccc",

                "@Kraj",
                "@End"
            };
        }
    }
}