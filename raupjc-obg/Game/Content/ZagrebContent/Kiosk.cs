using System.Collections.Generic;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Game.Content.ZagrebContent
{
    public class Kiosk : Event
    {
        public Kiosk()
        {
            Name = "Tisak";
            Description = "Kiosk sa raznim stvarima koje se mogu kupiti.";

            Behaviour = new List<string>
            {
                "@Begin",

                "@Choice -> Gle, kiosk!",
                "@C1 -> Idem kupit nes.; @Goto -> Kiosk",
                "@C2 -> Radije ne.; @Goto -> Izlaz",

                "@Kiosk",
                "@Monologue -> <b>Prodavac:</b> Pozdrav! Ovo su stvari koje prodajem.",
                "@Shop",
                "@Monologue -> Fala, bok.",

                "@Izlaz",
                "@End"
            };
        }
    }
}