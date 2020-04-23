using System;
using System.Collections.Generic;
using System.Linq;

namespace TICMod
{
    public static partial class CommandHandler
    {
        public static CommandResponse Parse(string command, BlockType blockType, bool execute = true)
        {
            var commandArgs = command.Split(new[] {' '}, 2).ToList();

            CommandResponse resp = new CommandResponse(false, "Unknown Command Block");
            switch (blockType)
            {
                case BlockType.Trigger:
                    resp = ParseTrigger(commandArgs, execute);
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

        private static CommandResponse ParseTrigger(List<String> commandArgs, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{commandArgs[0]}'.");
            commandArgs[0] = commandArgs[0].ToLower();

            return resp;
        }

        private static CommandResponse ParseInfluencer(List<String> commandArgs, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{commandArgs[0]}'.");
            commandArgs[0] = commandArgs[0].ToLower();

            switch (commandArgs[0])
            {
                case "say":
                    resp = InfluencerSay(commandArgs, resp, execute);
                    break;

                case "spawnnpc":
                    resp = InfluencerSpawnNPC(commandArgs, resp, execute);
                    break;

                case "spawnnpcid":
                    resp = InfluencerSpawnNPCID(commandArgs, resp, execute);
                    break;
            }

            return resp;
        }

        private static CommandResponse ParseConditional(List<String> commandArgs, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{commandArgs[0]}'.");
            commandArgs[0] = commandArgs[0].ToLower();

            switch (commandArgs[0])
            {
                case "day":
                    resp = ConditionalDay(resp, execute);
                    break;
            }

            return resp;
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
