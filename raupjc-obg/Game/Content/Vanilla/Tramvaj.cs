using System.Collections.Generic;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Game.Content.Vanilla
{
    public class Tramvaj : Event
    {
        public Tramvaj()
        {
            Name = "Tramvaj";
            Description = "ZET prijevoz";
            Type = "Transport";

            Behaviour = new List<string>
            {
                "@Begin",

                "@Ulazak",
                "@Monologue -> Ulazim u tramvaj.",
                "@75%; @Goto -> PunTram; @Monologue -> Tramvaj je pun ljudi.",
                "@25%; @Goto -> PrazanTram; @Monologue -> Tramvaj je prazan.",

                "@PrazanTram",
                "@Move -> 10; @Monologue -> Silazim na stanici.",
                "@Goto -> Izlaz",

                "@PunTram",
                "@75%; @Move -> 2; @Monologue -> Ovdje je nemoguce biti jos i sekundu. Izlazim.",
                "@25%; @Goto -> PrazanTram",

                "@Izlaz",
                "@End"
            };
        }
    }
}