using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TICMod.Commands
{
    public abstract class Command
    {
        protected static TICMod mod;
        protected abstract HashSet<String> aliases { get; }

        public bool IsAlias(string alias)
        {
            return aliases.Contains(alias.ToLower());
        }

        public abstract bool ParseArguments(string[] args, out string err);

        public abstract string Execute();

        public Command()
        {
            mod = ModContent.GetInstance<TICMod>();
        }


        protected static bool ParsePlayerTarget(string selector, string param, out List<Player> players, out int argCount, out string err)
        {
            players = new List<Player>();
            err = "";
            argCount = 0;

            if (selector == "@s")
            {
                if (String.IsNullOrWhiteSpace(param))
                {
                    err = $"{selector} requires 1 parameter; Datastore name";
                    return false;
                }

                Player player = mod.playerDataStore.GetItem(param);
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
                    err = $"{selector} requires 1 parameter; Player name";
                    return false;
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
                err = $"{selector} is not a valid player target";
                return false;
            }

            return true;
        }

        protected static bool ParseNPCTarget(string selector, string param, out List<NPC> npcs, out int argCount, out string err)
        {
            npcs = new List<NPC>();
            argCount = 0;
            err = "";

            if (selector == "@s")
            {
                if (String.IsNullOrWhiteSpace(param))
                {
                    err = $"{selector} requires 1 parameter; Datastore name";
                    return false;
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
                bool valid = ParseInt(param, out int id, out err, 1, NPCID.Count);
                if (!valid)
                {
                    err = $"{selector} requires 1 parameter; NPC ID";
                    return false;
                }

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
                err = $"{selector} is not a valid NPC target";
                return false;
            }

            return true;
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

        protected static bool ParseCoord(string pos1, string pos2, out Point position, out string err, bool multiplier=false)
        {
            int[] pos = new int[2];
            err = "";
            string[] args = new[] {pos1, pos2};
            position = Point.Zero;

            for (int i = 0; i < 2; i++)
            {
                bool valid = ParseInt(args[i], out pos[i], out err, 0);
                if (multiplier)
                {
                    pos[i] *= 16;
                }

                if (!valid)
                {
                    return false;
                }
            }

            position = new Point(pos[0], pos[1]);
            return true;
        }

        protected static bool ParseColor(string c1, string c2, string c3, out Color color, out string err)
        {
            int[] colors = new int[3];
            err = "";
            string[] args = new[] { c1, c2, c3 };
            color = Color.White;;

            for (int i = 0; i < 3; i++)
            {
                bool valid = ParseInt(args[i], out colors[i], out err, 0, 255);

                if (!valid)
                {
                    return false;
                }
            }

            color = new Color(colors[0], colors[1], colors[2]);
            return true;
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
