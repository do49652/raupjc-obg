using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using raupjc_obg.Game.Components;

namespace raupjc_obg.Game
{
    public class GameManager
    {
        private Random _random;
        public Components.Game Game { get; set; }
        [JsonIgnore]
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

        public void StartGame(Components.Game game)
        {
            Game = game;

            if (Players == null)
                Players = new Dictionary<string, Player>();
            Players.Values.ToList().ForEach(p => p.Money = Game.StartingMoney);

            Log = new List<string>();
            _random = new Random(DateTime.Now.Millisecond);

            GameStarted = true;
            Log.Add("[" + DateTime.Now + "] Game started!");
            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[0] + " is playing first.");
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
            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[WhosTurn()] + " rolled " + LastRoll);
        }

        // Check for running event or trigger a new event
        public bool CheckEvent()
        {
            var player = Players[Players.Keys.ToList()[WhosTurn()]];
            var currentSpace = player.Space;

            if (Game.SetEvents.ContainsKey(currentSpace) &&
                !player.VisitedEvents.Contains(Game.SetEvents[currentSpace]))
            {
                player.CurrentEvent = Game.SetEvents[currentSpace];
                player.RepeatEvent = 0;
                if (Game.SetEvents[currentSpace].HappensOnce)
                    player.VisitedEvents.Add(Game.SetEvents[currentSpace]);
                Game.SetEvents.Remove(currentSpace);
            }

            if (player.CurrentEvent == null && _random.Next(0, 100) < 80)
            {
                var evnts = Game.MiniEvents.Except(player.VisitedEvents).ToList();
                if (evnts.Count == 0)
                    return false;

                var evnt = evnts[_random.Next(0, evnts.Count)];
                player.CurrentEvent = evnt;

                if (evnt.HappensOnce)
                    player.VisitedEvents.Add(evnt);
            }

            if (player.CurrentEvent == null)
                return false;

            return true;
        }

        public void EndEvent()
        {
            var player = Players[Players.Keys.ToList()[WhosTurn()]];

            if (player.RepeatEvent <= 1)
            {
                if (player.CurrentEvent != null)
                {
                    if (player.CurrentEvent.NextEvent != null)
                        player.RepeatEvent = player.CurrentEvent.NextEvent.Repeat;
                    player.CurrentEvent = player.CurrentEvent.NextEvent;
                }
            }
            else
            {
                player.RepeatEvent--;
            }
        }

        public string RunBehaviour(HasBehaviour hb = null, int i = 0, string action = null, int t = -1)
        {
            if (hb == null && action == null)
                return null;

            if (t == -1)
                t = WhosTurn();

            if (action == null && hb is Event)
                ChangeScene("event");

            var player = Players[Players.Keys.ToList()[t]];
            List<string> behaviour = null;
            if (hb != null)
                behaviour = hb.Behaviour;

            // If parameter is given, this will be skipped
            // Check if next action is decided by random chance. If it is, choose one action and continue with it.
            if (action == null && behaviour.Count > i && behaviour[i].Contains(";") && behaviour[i].Split(';')[0].Contains("%"))
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
            if (action == null && behaviour.Count > i)
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
                {
                    action = action.Replace("@", "");
                }

                if (action.Length < 2)
                    break;

                if (a.StartsWith("@Buy")) // Do it with Buy() method
                {
                    var itemName = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                    Buy(itemName);
                    return "You bought <i>" + itemName + "</i>. <!<" + i + ">!>";
                }
                if (a.StartsWith("@OnEvent")) // Execute if specific event is running
                    if (player.CurrentEvent != null && Players.Keys.ToList().IndexOf(player.Username) == WhosTurn() &&
                        Scene.Equals("event") &&
                        player.CurrentEvent.Name.Equals(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()))
                        continue;
                    else
                        break;
                if (a.StartsWith("@NoEvent")
                ) // Same as @Goto, used for item behaviour. Used for easier understanding of the behaviour
                {
                    var func = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                    for (var j = 0; j < behaviour.Count; j++)
                    {
                        if (!behaviour[j].Equals("@" + func)) continue;
                        if (i == j && !action.Contains("@"))
                            return RunBehaviour(hb, i, null, t);
                        i = j;
                        break;
                    }
                }
                else if (a.StartsWith("@Move")
                ) // Action should contain number of spaces. Move() method handles actual moving.
                {
                    Move(t, int.Parse(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()));
                }
                else if (a.StartsWith("@Remove")) // Remove an item from inventory
                {
                    player.Items.Remove(player.Items.First(item =>
                        item.Name.Equals(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim())));
                }
                else if (a.StartsWith("@Give")) // Add an item to inventory
                {
                    player.Items.Add(Game.Items[a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()]);
                }
                else if (a.StartsWith("@Money")) // Give or take money
                {
                    var operation = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim()[0];
                    var ammount = int.Parse(a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim().Substring(1));

                    if (operation.Equals('+') || operation.Equals('-'))
                        player.Money += (operation.Equals('-') ? -1 : 1) * ammount;
                    else
                        player.Money = ammount;
                }
                else if (a.StartsWith("@Log+")) // Print text to log like "[Date] Player " + text
                {
                    Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[t] + " " +
                            a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim());
                }
                else if (a.StartsWith("@Log")) // Print text to  like "[Date] " + text
                {
                    Log.Add("[" + DateTime.Now + "] " + a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim());
                }
                else if (a.StartsWith("@Goto")) // Goto serves to jump on different lines of behaviour
                {
                    var func = a.Split(new[] { "->" }, StringSplitOptions.None)[1].Trim();
                    for (var j = 0; j < behaviour.Count; j++)
                    {
                        if (!behaviour[j].Equals("@" + func)) continue;
                        if (i == j && !action.Contains("@"))
                            return RunBehaviour(hb, i, null, t);
                        i = j;
                        break;
                    }
                }
                else if (a.StartsWith("@Choice")
                ) // Give player a choice and respect it by following that line of behaviour
                {
                    if (hb is Event)
                        ChangeScene("choice");
                    var str = behaviour[i - 1];
                    var j = 1;
                    while (behaviour[i].StartsWith("@C" + j))
                    {
                        str += "\n" + behaviour[i];
                        i++;
                        j++;
                    }
                    return str + "<!<" + i + ">!>";
                }
                else if (a.Equals("@Shop") && hb is Event) // Open the shop menu
                {
                    ChangeScene("shop");
                    return player.CurrentEvent.Name + "<!<" + i + ">!>";
                }
                else if (a.StartsWith("@Monologue")) // Display plain text
                {
                    return a.Split(';')[0].Split(new[] { "->" }, StringSplitOptions.None)[1].Trim() + "<!<" + i + ">!>";
                }
                else // Everything else can be skipped (@Begin etc.)
                {
                    return RunBehaviour(hb, i, null, t);
                }
            }

            //return "<!<" + i + ">!>";
            return RunBehaviour(hb, i, null, t);
        }

        // If the player can buy the item, buy it
        public void Buy(string itemName)
        {
            var player = Players[Players.Keys.ToList()[WhosTurn()]];

            if (!player.CurrentEvent.Items.ContainsKey(itemName) ||
                !(player.Money >= (float)player.CurrentEvent.Items[itemName][1]))
                return;
            player.Money -= (float)player.CurrentEvent.Items[itemName][1];
            player.Items.Add((Item)player.CurrentEvent.Items[itemName][0]);
            Log.Add("[" + DateTime.Now + "] " + Players.Keys.ToList()[WhosTurn()] + " just bought something.");
        }

        // Next turn
        public void Next()
        {
            Turn++;
        }
    }
}