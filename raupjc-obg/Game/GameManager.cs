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
        public List<string> Log { get; set; }

        public string Scene { get; set; }

        public int Turn { get; set; }
        public int LastRoll { get; set; }

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

        public void Move()
        {
            Move(WhosTurn(), LastRoll);
        }

        public void Move(int t, int spaces)
        {
            Players[Players.Keys.ToList()[t]].Space += spaces;
            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[t] + " moved to space " + Players[Players.Keys.ToList()[t]].Space + ".");
        }

        public void ChangeScene(string scene)
        {
            Scene = scene;
        }

        public void ThrowDice()
        {
            var r = new Random(DateTime.Now.Millisecond);
            LastRoll = r.Next(1, 7) + r.Next(1, 7);
            Log.Add("[" + DateTime.Now + "] Rolled " + LastRoll);
        }

        public void Next()
        {
            Turn++;
            Log.Add("[" + DateTime.Now + "] Next turn: " + Players.Keys.ToList()[WhosTurn()]);
        }
    }
}