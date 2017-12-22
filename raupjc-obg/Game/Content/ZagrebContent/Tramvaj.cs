using System.Collections.Generic;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Game.Content.ZagrebContent
{
    public class Tramvaj : Event
    {
        public Tramvaj()
        {
            Name = "Tramvaj";
            Description = "ZET prijevoz";

            Behaviour = new List<string>
            {
                "@Begin",

                "@Ulazak",
                "@Monologue -> Ulazim u tramvaj.",
                "@75%; @Goto -> PunTram; @Monologue -> Tramvaj je pun ljudi. Mozda izadem ranije...",
                "@25%; @Goto -> PrazanTram; @Monologue -> Tramvaj je prazan. Super! Vozim se do kraja.",

                "@PrazanTram",
                "@Move -> 10; @Monologue -> Izlazim iz tramvaja. <i>(Moved 10 spaces)</i>",
                "@Goto -> Izlaz",

                "@PunTram",
                "@75%; @Move -> 2; @Monologue -> Ovdje je nemoguce biti jos i sekundu. Izlazim. <i>(Moved 2 spaces)</i>",
                "@25%; @Goto -> PrazanTram",

                "@Izlaz",
                "@End"
            };
        }
    }
}