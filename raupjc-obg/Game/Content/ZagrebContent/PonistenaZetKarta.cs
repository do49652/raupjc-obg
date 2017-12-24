using System.Collections.Generic;
using raupjc_obg.Game.Components;

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

                "@NoEvent -> Baci",
                "@Goto -> Kraj",

                "@Baci",
                "@Remove -> Ponistena Zet karta",
                "@Log+ -> zagaduje okolis. ccc",
                "@Monologue -> Ne treba meni ovo!",

                "@Kraj",
                "@End"
            };
        }
    }
}