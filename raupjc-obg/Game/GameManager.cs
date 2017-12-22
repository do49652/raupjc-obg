using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using raupjc_obg.Game.Components;
using raupjc_obg.Game.Content.Vanilla;

namespace raupjc_obg.Game
{
    public class GameManager
    {
        [JsonIgnore]
        public Components.Game Game { get; set; }

        public string GameName { get; set; }
        [JsonIgnore]
        public string Password { get; set; }

        [JsonIgnore]
        public bool GameStarted { get; set; }
        public Dictionary<string, Player> Players { get; set; }

        public string Scene { get; set; }
        public string Message { get; set; }

        public int Turn { get; set; }
        public int LastRoll { get; set; }
        public List<string> Log { get; set; }

        public void StartGame()
        {
            if (Players == null)
                Players = new Dictionary<string, Player>();
            Log = new List<string>();

            Game = new Components.Game();
            Game.MiniEvents.Add(new Tramvaj());

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

            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[t] + " moved to space " +
                    Players[Players.Keys.ToList()[t]].Space + ".");
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

        public bool CheckEvent()
        {
            if (Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent == null &&
                new Random().Next(0, 100) < 30)
            {
                Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent =
                    Game.MiniEvents[new Random().Next(0, Game.Events.Count)];

                Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[WhosTurn()] + " triggered " +
                        Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Name + " event.");
            }

            if (Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent == null)
                return false;

            if (Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Behaviour.Count - 1 ==
                Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine)
                return false;

            return true;
        }

        public string PlayEvent()
        {
            ChangeScene("event");

            var behaviour = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Behaviour;
            var i = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine;
            string action = null;

            if (behaviour[i].Contains(";") && behaviour[i].Split(';')[0].Contains("%"))
            {
                var possibleActions = new List<string>();
                while (behaviour[i].Split(';')[0].Contains("%"))
                {
                    possibleActions.Add(behaviour[i]);
                    i++;
                }

                possibleActions.Sort();
                action = possibleActions.Last();

                var rnd = new Random().Next(0, 101);
                possibleActions.ForEach(pa =>
                {
                    if (int.Parse(pa.Split(';')[0].Substring(1, 2)) < rnd)
                        action = pa;
                });

                action = action.Substring(5).Trim();
            }

            if (action == null)
            {
                action = behaviour[i];
                i++;
            }

            if (action.Equals("@End"))
            {
                Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent = null;
                Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = 0;
                return "@End";
            }

            while (action != null)
            {
                var a = action;

                if (action.Contains(';'))
                {
                    a = action.Split(';')[0];
                    action = action.Substring(a.Length + 1).Trim();
                }
                if (action.Length < 2)
                    break;

                if (a.StartsWith("@Variable"))
                    ChangeVariable(a);
                else if (a.StartsWith("@Move"))
                    Move(WhosTurn(), int.Parse(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()));
                else if (a.StartsWith("@Goto"))
                {
                    var func = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                    for (var j = 0; j < behaviour.Count; j++)
                    {
                        if (!behaviour[j].Equals("@" + func)) continue;
                        if (i == j)
                        {
                            Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                            return PlayEvent();
                        }
                        i = j;
                        break;
                    }
                }
                else if (a.StartsWith("@Monologue"))
                {
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                    return a.Split(';')[0].Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                }
                else
                {
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                    return PlayEvent();
                }
            }

            Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
            return null;
        }

        public void ChangeVariable(string action)
        {
            var v = "0";
            try
            {
                v = Players[Players.Keys.ToList()[WhosTurn()]]
                    .Variables[action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[0]];
            }
            catch (Exception e) { }

            if (action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[1].Equals("="))
                Players[Players.Keys.ToList()[WhosTurn()]].Variables[action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[0]] =
                    action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[2].Replace(";", "");
            else if (action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[1].Equals("+"))
                Players[Players.Keys.ToList()[WhosTurn()]].Variables[action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[0]] = (int.Parse(v) +
                    int.Parse(action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[2].Replace(";", ""))).ToString();
            else if (action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[1].Equals("-"))
                Players[Players.Keys.ToList()[WhosTurn()]].Variables[action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[0]] = (int.Parse(v) -
                    int.Parse(action.Split(new[] { "@Variable -> " }, StringSplitOptions.None)[1].Split(' ')[2].Replace(";", ""))).ToString();
        }

        public void Next()
        {
            Turn++;

            Log.Add("[" + DateTime.Now + "] Next turn: " + Players.Keys.ToList()[WhosTurn()]);
        }
    }
}