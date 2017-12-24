using System.Linq;
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

            var tramvaj = new Tramvaj();
            tramvaj.Repeat = 2;
            var kiosk = new Kiosk();
            kiosk.NextEvent = tramvaj;

            MiniEvents.Add(kiosk);
            MiniEvents.Add(new Tramvaj());

            kiosk.Items.Keys.ToList().ForEach(k => Items[k] = (Item) kiosk.Items[k][0]);
            tramvaj.Items.Keys.ToList().ForEach(k => Items[k] = (Item) tramvaj.Items[k][0]);

            var pKarta = new PonistenaZetKarta();
            Items[pKarta.Name] = pKarta;

            SetEvents[35] = new KunaNaPodu();
        }
    }
}