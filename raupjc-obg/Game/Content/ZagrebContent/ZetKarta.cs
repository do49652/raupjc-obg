using System.Collections.Generic;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Game.Content.ZagrebContent
{
    public class ZetKarta : Item
    {
        public ZetKarta()
        {
            Name = "Zet karta";
            Description = "Karta za voznju ZET prijevozom.";
            Category = "NoEvent, OnEvent";
        }

        public override void SetBehaviour()
        {
            Behaviour = new List<string>
            {
                "@Begin",

                "@OnEvent -> Tramvaj; @Goto -> Tramvaj",
                "@NoEvent -> Baci",
                "@Goto -> Kraj",

                "@Tramvaj",
                "@Choice -> Ponisti kartu?",
                "@C1 -> Da; @Goto -> PonistiKartu",
                "@C2 -> Ne; @Goto -> Kraj",

                "@PonistiKartu",
                "@Remove -> Zet karta; @Give -> Ponistena Zet karta",
                "@Log+ -> je ponistio kartu.",
                "@Monologue -> Karta ponistena. Sad se mogu voziti bez frke!",
                "@Goto -> Kraj",

                "@Baci",
                "@Remove -> Zet karta",
                "@Log+ -> zagaduje okolis. ccc",
                "@Monologue -> Ne treba meni ovo!",

                "@Kraj",
                "@End"
            };
        }
    }
}