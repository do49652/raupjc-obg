using System;
using System.Collections.Generic;
using raupjc_obg.Models.GameContentModels;

namespace raupjc_obg.Models.ContentViewModels
{
    public class ItemViewModel
    {
        public List<GameModel> GameModels { get; set; }
        public List<EventModel> EventModels { get; set; }
        public List<ItemModel> ItemModels { get; set; }

        public string Id { get; set; }
        public string UserId { get; set; }

        public string GameName { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public string Behaviour { get; set; }
    }
}