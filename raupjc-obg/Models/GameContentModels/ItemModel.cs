﻿using System;
using System.Collections.Generic;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Models.GameContentModels
{
    public class ItemModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public GameModel Game { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Behaviour { get; set; }
        public string Category { get; set; }

        public Item CreateGameItemEntity()
        {
            return new NewItem
            {
                Name = Name,
                Description = Description,
                Behaviour = Behaviour,
                Category = Category
            };
        }

        private class NewItem : Item
        {
            public override void SetBehaviour()
            {
                if (Behaviour == null)
                    Behaviour = new List<string>();
            }
        }
    }
}