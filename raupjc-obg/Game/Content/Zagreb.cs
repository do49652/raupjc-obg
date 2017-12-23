using System;
using System.Collections.Generic;
using raupjc_obg.Game.Components;
using raupjc_obg.Game.Content.ZagrebContent;

namespace raupjc_obg.Game.Content
{
    public class Zagreb : Components.Game
    {
        public Zagreb()
        {
            Name = "Zagreb";
            Description = "Snadi se u glavnom gradu Hrvatske.";
            StartingMoney = 100;

            var karta = new ZetKarta();
            Items[karta.Name] = new object[] { karta, 10f };

            var pKarta = new PonistenaZetKarta();
            Items[pKarta.Name] = new object[] { pKarta, 10f };

            var kiosk = new Kiosk();
            var tramvaj = new Tramvaj();
            tramvaj.Repeat = 2;
            kiosk.NextEvent = tramvaj;
            
            MiniEvents.Add(kiosk);
            MiniEvents.Add(new Tramvaj());
        }
    }
}