using System;
using System.Collections.Generic;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Models.ContentViewModels
{
    public class GameViewModel
    {
        public List<GameModel> GameModels { get; set; }
        public List<EventModel> EventModels { get; set; }
        public List<ItemModel> ItemModels { get; set; }

        public string Id { get; set; }
        public string UserId { get; set; }
        public bool Private { get; set; }
        public bool Standalone { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public float StartingMoney { get; set; }

        public string Dependencies { get; set; }
        public string Events { get; set; }
        public string MiniEvents { get; set; }
        public string SetEvents { get; set; }
        public string Items { get; set; }
    }
}