using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TICMod
{
    public static partial class CommandHandler
    {
        public static CommandResponse Parse(string command, BlockType blockType, bool execute = true, int i=-1, int j=-1)
        {
            var commandsplit = command.Split(new[] {' '}, 2).ToList();
            string commandtype = commandsplit[0];
            string[] args = new string[0];

            if (commandsplit.Count == 2 && !String.IsNullOrEmpty(commandsplit[1]))
            {
                args = SplitArgs(commandsplit[1]);
            }

            CommandResponse resp = new CommandResponse(false, "Unknown Command Block");
            switch (blockType)
            {
                case BlockType.Trigger:
                    break;
                    resp = ParseTrigger(commandtype, args, execute, i, j);
                    break;
                case BlockType.Influencer:
                    resp = ParseInfluencer(commandtype, args, execute);
                    break;
                case BlockType.Conditional:
                    break;
                    resp = ParseConditional(commandtype, args, execute);
                    break;
            }

            return resp;
        }

        public static string[] SplitArgs(string input)
        {
            string escapeMarker = "\u0011"; // Arbitrary non-input character
            input = input.Replace("\\,", escapeMarker);
            var split = input.Split(',');

            for (int i = 0; i < split.Length; i++)
            {
                split[i] = split[i].Replace(escapeMarker, ",");
                split[i] = split[i].TrimStart(' ');
            }

            return split;
        }


        public static (List<Player>, CommandResponse, int) ParsePlayerTarget(string selector, string param, CommandResponse resp)
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

        public static (List<NPC>, CommandResponse, int) ParseNPCTarget(string selector, string param, CommandResponse resp)
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
                var ret = ParseInt(param, resp, 1, NPCID.Count);
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

        public static (uint[], CommandResponse) ParseTime(string args, CommandResponse resp)
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

        public static (int, CommandResponse) ParseInt(string args, CommandResponse resp, int minVal=Int32.MinValue, int maxVal=Int32.MaxValue)
        {
            bool success = int.TryParse(args, NumberStyles.Integer, CultureInfo.CurrentCulture, out int val);
            
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
                resp.response = $"{args} is not a valid integer{range}";
                return (-1, resp);
            }

            resp.valid = true;
            return (val, resp);
        }

        public static (double, CommandResponse) ParseDouble(string args, CommandResponse resp)
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

        public static string GetPlayerNames(List<Player> players)
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

    

    public class CommandResponse
    {
        public bool success;
        public string response;
        public bool valid;

        public CommandResponse(bool _success, string _response)
        {
            success = _success;
            response = _response;
        }
    }
}
