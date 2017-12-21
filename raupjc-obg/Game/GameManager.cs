using System.Collections.Generic;

namespace raupjc_obg.Game
{
    public class GameManager
    {
        public string GameName { get; set; }
        public string Password { get; set; }
        public int FinishSpace { get; set; }
        public int StartGold { get; set; }
        public Dictionary<string, Player> Players { get; set; }
        public bool GameStarted { get; set; }
        public int Turn { get; set; }
    }
}