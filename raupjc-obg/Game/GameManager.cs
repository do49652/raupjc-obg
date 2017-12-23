using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
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
            Players.Values.ToList().ForEach(p => p.Money = Game.StartingMoney);

            Log = new List<string>();
            _random = new Random(DateTime.Now.Millisecond);

            GameStarted = true;
            Log.Add("[" + DateTime.Now + "] Game started.");
            Log.Add("[" + DateTime.Now + "] Next turn: " + Players.Keys.ToList()[0]);
        }

        // Which player is currently playing
        public int WhosTurn()
        {
            var i = Turn % Players.Count;
            return i;
        }

        // Move the currently playing player by last dice roll spaces
        public void Move()
        {
            Move(WhosTurn(), LastRoll);
        }

        // Move specific player by specified number of spaces
        public void Move(int t, int spaces)
        {
            Players[Players.Keys.ToList()[t]].Space += spaces;

            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[t] + " moved to space " +
                    Players[Players.Keys.ToList()[t]].Space + ".");
        }

        // Change scene (roll, rolled, choice, event, shop)
        public void ChangeScene(string scene)
        {
            Scene = scene;
        }

        // Get random number (2 dice)
        public void ThrowDice()
        {
            LastRoll = _random.Next(1, 7) + _random.Next(1, 7);

            Log.Add("[" + DateTime.Now + "] Rolled " + LastRoll);
        }

        // Check for running event or trigger a new event - WIP
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

        public void EndEvent()
        {
            if (Players[Players.Keys.ToList()[WhosTurn()]].RepeatEvent <= 1)
            {
                if (Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent != null)
                {
                    if (Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.NextEvent != null)
                        Players[Players.Keys.ToList()[WhosTurn()]].RepeatEvent = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.NextEvent.Repeat;
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.NextEvent;
                }
            }
            else
                Players[Players.Keys.ToList()[WhosTurn()]].RepeatEvent--;
            Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = 0;
        }

        // Continue with event behaviour or if parameter given, execute that instead
        public string PlayEvent(string action = null)
        {
            if (action == null)
                ChangeScene("event");

            var behaviour = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Behaviour;
            var i = Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine;

            // If parameter is given, this will be skipped
            // Check if next action is decided by random chance. If it is, choose one action and continue with it.
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

            // If action is not given and is not decided by random chance, just execute current action on behaviour list
            if (action == null)
            {
                action = behaviour[i];
                i++;
            }

            // If the action is to end the event, end it
            if (action.Equals("@End"))
                return "@End";

            // Loop unitl whole chain of actions is not executed
            while (action != null)
            {
                var a = action;

                // If the action is a chain of actions
                if (action.Contains(';'))
                {
                    a = action.Split(';')[0];
                    action = action.Substring(a.Length + 1).Trim();
                }
                else
                    action = action.Replace("@", "");

                if (action.Length < 2)
                    break;

                if (a.StartsWith("@Buy"))      // Do it with Buy() method
                {
                    var itemName = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                    Buy(itemName);
                    return "You bought <i>" + itemName + "</i>.";
                }
                else if (a.StartsWith("@Move"))     // Action should contain number of spaces. Move() method handles actual moving.
                    Move(WhosTurn(), int.Parse(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()));
                else if (a.StartsWith("@Use"))
                {

                }
                else if (a.StartsWith("@Log+"))     // Print text to log like "[Date] Player " + text
                    Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[WhosTurn()] + " " + a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim());
                else if (a.StartsWith("@Log"))      // Print text to  like "[Date] " + text
                    Log.Add("[" + DateTime.Now + "] " + a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim());
                else if (a.StartsWith("@Goto"))     // Goto serves to jump on different lines of behaviour
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
                else if (a.StartsWith("@Choice"))   // Give player a choice and respect it by following that line of behaviour
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
                else if (a.Equals("@Shop"))         // Open the shop menu
                {
                    ChangeScene("shop");
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                    return Players[Players.Keys.ToList()[WhosTurn()]].CurrentEvent.Name;
                }
                else if (a.StartsWith("@Monologue"))// Display plain text
                {
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                    return a.Split(';')[0].Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                }
                else                                // Everything else can be skipped (@Begin etc.)
                {
                    Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
                    return PlayEvent();
                }
            }

            Players[Players.Keys.ToList()[WhosTurn()]].CurrentEventLine = i;
            return null;
        }

        // If the player can buy the item, buy it
        public void Buy(string itemName)
        {
            if (!Game.Items.ContainsKey(itemName) ||
                !(Players[Players.Keys.ToList()[WhosTurn()]].Money >= (float)Game.Items[itemName][1])) return;
            Players[Players.Keys.ToList()[WhosTurn()]].Money -= (float)Game.Items[itemName][1];
            Players[Players.Keys.ToList()[WhosTurn()]].Items.Add((Item)Game.Items[itemName][0]);

            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[WhosTurn()] + " bought " + itemName + ".");
        }
        
        // Next turn
        public void Next()
        {
            Turn++;

            Log.Add("[" + DateTime.Now + "] Next turn: " + Players.Keys.ToList()[WhosTurn()]);
        }
    }
}