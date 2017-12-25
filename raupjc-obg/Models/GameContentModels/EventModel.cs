using System;
using System.Collections.Generic;
using System.Linq;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Models.GameContentModels
{
    public class EventModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public GameModel Game { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Behaviour { get; set; }
        public int Repeat { get; set; }
        public EventModel NextEvent { get; set; }
        public bool HappensOnce { get; set; }
        public Dictionary<string, object[]> Items { get; set; }

        public object[] CreateGameEventEntity(List<Event> loadedEventModels = null, List<Item> loadedItemModels = null)
        {
            loadedEventModels = loadedEventModels ?? new List<Event>();
            loadedItemModels = loadedItemModels ?? new List<Item>();

            var _event = new NewEvent
            {
                Name = Name,
                Description = Description,
                Behaviour = Behaviour,
                Repeat = Repeat,
                HappensOnce = HappensOnce,
            };

            var lEvent = loadedEventModels.FirstOrDefault(e => e.Name.Equals(NextEvent.Name));
            if (lEvent == null)
            {
                var lEventObj = NextEvent.CreateGameEventEntity(loadedEventModels, loadedItemModels);
                lEvent = (Event)lEventObj[0];
                loadedEventModels.Add(lEvent);

                loadedEventModels.AddRange(((List<Event>)lEventObj[1]).Except(loadedEventModels));
                loadedItemModels.AddRange(((List<Item>)lEventObj[2]).Except(loadedItemModels));
            }
            _event.NextEvent = lEvent;

            Items.Keys.ToList().ForEach(itemName =>
            {
                var item = (string)Items[itemName][0];
                var lItem = loadedItemModels.FirstOrDefault(i => i.Name.Equals(item));
                if (lItem == null)
                {
                    var iiii = Game.Items.FirstOrDefault(i => i.Name.Equals(item));
                    if (iiii == null)
                        return;
                    lItem = iiii.CreateGameItemEntity();
                    loadedItemModels.Add(lItem);
                }
                _event.Items[itemName] = new object[] { lItem, (float)Items[itemName][1] };
            });

            return new object[] { _event, loadedEventModels, loadedItemModels };
        }

        private class NewEvent : Event
        {
            public override void SetBehaviour()
            {
                if (Behaviour == null)
                    Behaviour = new List<string>();
            }
        }
    }
}