using System;
using System.Collections.Generic;
using System.Linq;

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
        public List<string> Log { get; set; }

        public void StartGame()
        {
            GameStarted = true;
            Log.Add("[" + DateTime.Now + "] Game started.");
            Log.Add("[" + DateTime.Now + "] Next turn: " + Players.Keys.ToList()[0]);
        }

        public int WhosTurn()
        {
            var i = Turn % Players.Count;
            return i;
        }

        public int DiceThrow()
        {
            var r = new Random(DateTime.Now.Millisecond);
            var d1 = r.Next(1, 7);
            var d2 = r.Next(1, 7);
            Log.Add("[" + DateTime.Now + "] Rolled " + (d1 + d2));
            return d1 + d2;
        }

        public void Rolled(int i, int r)
        {
            Players[Players.Keys.ToList()[i]].Space += r;
            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[i] + " moved to space " + Players[Players.Keys.ToList()[i]].Space + ".");
        }

        public void Next()
        {
            Turn++;
            Log.Add("[" + DateTime.Now + "] Next turn: " + Players.Keys.ToList()[WhosTurn()]);
        }
    }
}