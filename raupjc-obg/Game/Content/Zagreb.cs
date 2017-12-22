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
            MiniEvents.Add(new Kiosk());
            MiniEvents.Add(new Tramvaj());
        }
    }
}