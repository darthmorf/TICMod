using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TICMod.Commands
{
    public abstract class Command
    {
        protected abstract HashSet<String> aliases
        {
            get;
        }

        public bool IsAlias(string alias)
        {
            return aliases.Contains(alias.ToLower());
        }

        public abstract bool ParseArguments(string[] args, out string err);

        public abstract string Execute();





        private static (List<Player>, CommandResponse, int) ParsePlayerTarget(string selector, string param, CommandResponse resp)
        {
            List<Player> players = new List<Player>();
            int argCount = 0;

            if (selector == "@s")
            {
                if (String.IsNullOrWhiteSpace(param))
                {
                    resp.response = $"{selector} requires 1 parameter; Datastore name";
                    return (players, resp, argCount);
                }

                Player player = ModContent.GetInstance<TICMod>().playerDataStore.GetItem(param);
                if (player != null)
                {
                    players.Add(player);
                }

                argCount = 1;
            }
            else if (selector == "@a")
            {
                foreach (var player in Main.player)
                {
                    if (player.name != "")
                    {
                        players.Add(player);
                    }
                }
            }
            else if (selector == "@n")
            {
                if (String.IsNullOrWhiteSpace(param))
                {
                    resp.response = $"{selector} requires 1 parameter; Player name";
                    return (players, resp, argCount);
                }

                foreach (var player in Main.player)
                {
                    if (player.name == param)
                    {
                        players.Add(player);
                    }
                }

                argCount = 1;
            }
            else if (selector == "@r")
            {
                List<Player> validPlayers = new List<Player>();
                foreach (var player in Main.player)
                {
                    if (player.name != "")
                    {
                        validPlayers.Add(player);
                    }
                }

                Random rand = new Random();
                int index = rand.Next(validPlayers.Count);

                players.Add(validPlayers[index]);
            }
            else
            {
                resp.response = $"{selector} is not a valid player target";
                return (players, resp, argCount);
            }

            resp.valid = true;
            return (players, resp, argCount);
        }

        protected static (List<NPC>, CommandResponse, int) ParseNPCTarget(string selector, string param, CommandResponse resp)
        {
            List<NPC> npcs = new List<NPC>();
            int argCount = 0;

            if (selector == "@s")
            {
                if (String.IsNullOrWhiteSpace(param))
                {
                    resp.response = $"{selector} requires 1 parameter; Datastore name";
                    return (npcs, resp, argCount);
                }

                NPC npc = ModContent.GetInstance<TICMod>().npcDataStore.GetItem(param);
                if (npc != null)
                {
                    npcs.Add(npc);
                }

                argCount = 1;
            }
            else if (selector == "@a")
            {
                npcs = Main.npc.ToList();
            }
            else if (selector == "@i")
            {
                var ret = CommandHandler.ParseInt(param, resp, 1, NPCID.Count);
                int id = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    resp.response = $"{selector} requires 1 parameter; NPC ID";
                    return (npcs, resp, argCount);
                }

                resp.valid = false;

                foreach (var npc in Main.npc)
                {
                    if (npc.netID == id)
                    {
                        npcs.Add(npc);
                    }
                }

                argCount = 1;
            }
            else if (selector == "@e")
            {
                foreach (var npc in Main.npc)
                {
                    if (!npc.friendly)
                    {
                        npcs.Add(npc);
                    }
                }
            }
            else if (selector == "@t")
            {
                foreach (var npc in Main.npc)
                {
                    if (npc.townNPC)
                    {
                        npcs.Add(npc);
                    }
                }
            }
            else if (selector == "@f")
            {
                foreach (var npc in Main.npc)
                {
                    if (npc.friendly)
                    {
                        npcs.Add(npc);
                    }
                }
            }
            else
            {
                resp.response = $"{selector} is not a valid NPC target";
                return (npcs, resp, argCount);
            }


            resp.valid = true;
            return (npcs, resp, argCount);
        }

        protected static (uint[], CommandResponse) ParseTime(string args, CommandResponse resp)
        {
            var posStr = args.Split(new[] { ':' }, 2);
            List<uint> time = new List<uint>(2);
            foreach (var str in posStr)
            {
                bool success = uint.TryParse(str, NumberStyles.Integer, CultureInfo.CurrentCulture, out uint posVal);
                if (!success)
                {
                    break;
                }
                time.Add(posVal);
            }
            if (time.Count != 2 || time[0] > 24 || time[1] > 59)
            {
                resp.response = $"{args} is not a valid time in format hh:mm.";
                return (time.ToArray(), resp);
            }

            resp.valid = true;
            return (time.ToArray(), resp);
        }

        protected static bool ParseInt(string args, out int ret, out string err, int minVal = Int32.MinValue, int maxVal = Int32.MaxValue)
        {
            bool success = int.TryParse(args, NumberStyles.Integer, CultureInfo.CurrentCulture, out int val);
            err = "";
            ret = -1;

            string range = "";
            if (minVal != Int32.MinValue)
            {
                range += $" greater than {minVal - 1}";
            }

            if (maxVal != Int32.MaxValue)
            {
                range += $" less than {maxVal + 1}";
            }

            if (!success || val > maxVal || val < minVal)
            {
                err = $"{args} is not a valid integer{range}";
                return false;
            }

            ret = val;
            return true;
        }

        protected static (double, CommandResponse) ParseDouble(string args, CommandResponse resp)
        {
            bool success = double.TryParse(args, NumberStyles.Float, CultureInfo.CurrentCulture, out double posVal);
            if (!success)
            {
                resp.response = $"{args} is not a valid decimal";
                return (-1, resp);
            }

            resp.valid = true;
            return (posVal, resp);
        }

        protected static string GetPlayerNames(List<Player> players)
        {
            string playernames = "";

            foreach (var player in players)
            {
                if (player.name != "")
                {
                    playernames += $"{player.name}, ";
                }
            }

            if (playernames != "")
            {
                playernames = playernames.Substring(0, playernames.LastIndexOf(", "));
            }

            return playernames;
        }
    }
}
