using raupjc_obg.Models.OtherModels;
using System.Collections.Generic;

namespace raupjc_obg.Models.GameViewModels
{
    public class JoinGameViewModel
    {
        public List<Review> reviews { get; set; }

        public string GameName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public JoinGameViewModel()
        {
            GameName = "";
            Username = "";
            Password = "";
        }
    }
}