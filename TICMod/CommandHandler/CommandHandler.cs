using System;
using System.Collections.Generic;
using System.Linq;
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
                    resp.response = $"{args[0]} requires datastore name";
                    return (players, resp);
                }

                players.Add(ModContent.GetInstance<TICMod>().playerDataStore.GetItem(args[1]));
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
            else
            {
                resp.response = $"{args[0]} is not a valid player target.";
                return (players, resp);
            }

            resp.valid = true;
            return (players, resp);
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
