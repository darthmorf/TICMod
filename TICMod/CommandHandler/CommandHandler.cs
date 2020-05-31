using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TICMod
{
    public static partial class CommandHandler
    {
        public static CommandResponse Parse(string command, BlockType blockType, bool execute = true, int i=-1, int j=-1)
        {
            var commandArgs = command.Split(new[] {' '}, 2).ToList();

            CommandResponse resp = new CommandResponse(false, "Unknown Command Block");
            switch (blockType)
            {
                case BlockType.Trigger:
                    resp = ParseTrigger(commandArgs, execute, i, j);
                    break;
                case BlockType.Influencer:
                    resp = ParseInfluencer(commandArgs, execute);
                    break;
                case BlockType.Conditional:
                    resp = ParseConditional(commandArgs, execute);
                    break;
            }

            return resp;
        }

        public static (List<Player>, CommandResponse) ParsePlayerTarget(List<string> args, CommandResponse resp)
        {
            List<Player> players = new List<Player>();

            if (args.Count == 0)
            {
                resp.response = "Command requires player target";
                return (players, resp);
            }
            else if (args[0] == "@s")
            {
                if (args.Count != 2 || String.IsNullOrWhiteSpace(args[1]))
                {
                    resp.response = $"{args[0]} requires datastore name parameter";
                    return (players, resp);
                }

                Player player = ModContent.GetInstance<TICMod>().playerDataStore.GetItem(args[1]);
                if (player != null)
                {
                    players.Add(player);
                }
            }
            else if (args[0] == "@a")
            {
                if (args.Count != 1)
                {
                    resp.response = $"{args[0]} requires no parameters.";
                    return (players, resp);
                }

                players = Main.player.ToList();
            }
            else if (args[0] == "@n")
            {
                string name = String.Join(" ", args.Skip(1));

                if (args.Count < 2 || String.IsNullOrWhiteSpace(name))
                {
                    resp.response = $"{args[0]} requires player name parameter";
                    return (players, resp);
                }

                foreach (var player in Main.player)
                {
                    if (player.name == name)
                    {
                        players.Add(player);
                    }
                }
            }
            else if (args[0] == "@r")
            {
                if (args.Count != 1)
                {
                    resp.response = $"{args[0]} requires no parameters.";
                    return (players, resp);
                }

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
                resp.response = $"{args[0]} is not a valid player target.";
                return (players, resp);
            }

            resp.valid = true;
            return (players, resp);
        }

        public static (Color, CommandResponse) ParseColor(string args, CommandResponse resp)
        {
            Color color = new Color();
            
            var rgbStr = args.Split(new[] { ',' }, 3);
            List<int> rgb = new List<int>(3);
            foreach (var str in rgbStr)
            {
                bool success = int.TryParse(str, NumberStyles.Integer, CultureInfo.CurrentCulture, out int rgbVal);
                if (!success)
                {
                    break;
                }
                rgb.Add(rgbVal);
            }

            if (rgb.Count != 3)
            {
                resp.response =
                    $"{args[0]} is not a valid RGB string. Should be in the format r,g,b each in the range 0-255.";
                return (color, resp);
            }

            color = new Color(rgb[0], rgb[1], rgb[2]);

            resp.valid = true;
            return (color, resp);
        }

        public static (int[], CommandResponse) ParseCoordinate(string args, CommandResponse resp)
        {
            var posStr = args.Split(new[] { ',' }, 2);
            List<int> pos = new List<int>(2);
            foreach (var str in posStr)
            {
                bool success = int.TryParse(str, NumberStyles.Integer, CultureInfo.CurrentCulture, out int posVal);
                if (!success)
                {
                    break;
                }
                pos.Add(posVal);
            }
            if (pos.Count != 2)
            {
                resp.response =
                    $"{args[0]} is not a valid position string.";
                return (pos.ToArray(), resp);
            }

            pos[0] *= 16;
            pos[1] *= 16;

            resp.valid = true;
            return (pos.ToArray(), resp);
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
