using System;
using System.Collections.Generic;
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

        public Event CreateGameEventEntity()
        {
            var _event = new NewEvent
            {
                Name = Name,
                Description = Description,
                Behaviour = Behaviour,
                Repeat = Repeat,
                HappensOnce = HappensOnce,
            };
            // TODO: NextEvent, Items
            return _event;
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