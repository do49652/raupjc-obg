using raupjc_obg.Models.GameContentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace raupjc_obg.Models.OtherModels
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public GameModel Game { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
