using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using raupjc_obg.Game.Components;
using raupjc_obg.Game.Content;

namespace raupjc_obg.Game
{
    public class GameManager
    {
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

        private Random _random;

        public void StartGame()
        {
            Game = new Zagreb();
            
            if (Players == null)
                Players = new Dictionary<string, Player>();
            Players.Values.ToList().ForEach(p=>p.Money=Game.StartingMoney);

            Log = new List<string>();
            _random = new Random(DateTime.Now.Millisecond);

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
            LastRoll = _random.Next(1, 7) + _random.Next(1, 7);

            Log.Add("[" + DateTime.Now + "] Rolled " + LastRoll);
        }

        public bool CheckEvent()
        {
            if (Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent == null &&
                _random.Next(0, 100) < 30)
            {
                Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent =
                    Game.MiniEvents[_random.Next(0, Game.MiniEvents.Count)];

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

        public string PlayEvent(string action = null)
        {
            ChangeScene("event");

            var behaviour = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Behaviour;
            var i = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine;

            if (action == null && behaviour[i].Contains(";") && behaviour[i].Split(';')[0].Contains("%"))
            {
                var possibleActions = new List<string>();
                while (behaviour[i].Split(';')[0].Contains("%"))
                {
                    possibleActions.Add(behaviour[i]);
                    i++;
                }

                possibleActions.Sort();
                action = possibleActions.Last();

                var rnd = _random.Next(0, 101);
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
                else
                    action = action.Replace("@", "");

                if (action.Length < 2)
                    break;

                if (a.StartsWith("@Variable"))
                    ChangeVariable(a);
                else if (a.StartsWith("@Buy"))
                {
                    Buy(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim());
                    return null;
                }
                else if (a.StartsWith("@Move"))
                    Move(WhosTurn(), int.Parse(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()));
                else if (a.StartsWith("@Goto"))
                {
                    var func = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                    for (var j = 0; j < behaviour.Count; j++)
                    {
                        if (!behaviour[j].Equals("@" + func)) continue;
                        if (i == j && !action.Contains("@"))
                        {
                            Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                            return PlayEvent();
                        }
                        i = j;
                        break;
                    }
                }
                else if (a.StartsWith("@Choice"))
                {
                    ChangeScene("choice");
                    var str = behaviour[i - 1];
                    var j = 1;
                    while (behaviour[i].StartsWith("@C" + j))
                    {
                        str += "\n" + behaviour[i];
                        i++;
                        j++;
                    }
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                    return str;
                }
                else if (a.Equals("@Shop"))
                {
                    ChangeScene("shop");
                    return Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Name;
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

        public void Buy(string itemName)
        {
            if (Game.Items.ContainsKey(itemName) &&
                Players[Players.Keys.ToList()[WhosTurn()]].Money >= (float)Game.Items[itemName][1])
            {
                Players[Players.Keys.ToList()[WhosTurn()]].Money -= (float)Game.Items[itemName][1];
                Players[Players.Keys.ToList()[WhosTurn()]].Items.Add((Item)Game.Items[itemName][0]);
            }
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