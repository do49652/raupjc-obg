using System.Collections.Generic;

namespace raupjc_obg.Models.GameViewModels
{
    public class JoinGameViewModel
    {
        public List<string> gameNames { get; set; }

        public string GameName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}