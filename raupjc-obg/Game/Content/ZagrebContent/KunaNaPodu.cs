using raupjc_obg.Game.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raupjc_obg.Game.Content.ZagrebContent
{
    public class KunaNaPodu : Event
    {
        public KunaNaPodu()
        {
            Name = "Kuna na podu";
            Description = "Nekome je ispala kuna.";
        }

        public override void SetBehaviour()
        {
            Behaviour = new List<string>
            {
                "@Begin",

                "@Choice -> Gle, kuna!",
                "@C1 -> Uzmi.; @Goto -> Uzmi",
                "@C2 -> Ostavi.; @Goto -> Izlaz",

                "@Uzmi",
                "@Money -> +1; @Monologue -> Hehehe",

                "@Izlaz",
                "@End"
            };
        }
    }
}
