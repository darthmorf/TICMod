using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TICMod.Commands;
using TICMod.Commands.Conditionals;
using TICMod.Commands.Influencers;

namespace TICMod
{
    public static partial class CommandHandler
    {
        public static TICMod mod;
        public static TICSystem modSystem;

        public static CommandResponse Parse(string command, BlockType blockType, bool execute = true, int i = -1, int j = -1)
        {
            mod = ModContent.GetInstance<TICMod>();
            modSystem = ModContent.GetInstance<TICSystem>();
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
                    resp = ParseTrigger(commandtype, args, execute, i, j);
                    break;
                case BlockType.Influencer:
                    resp = ParseInfluencer(commandtype, args, execute);
                    break;
                case BlockType.Conditional:
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

        private static CommandResponse ParseTrigger(string command, string[] args, bool execute, int i, int j)
        {
            TICSystem world = ModContent.GetInstance<TICSystem>();
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'");

            List<TriggerCommand> triggerCommands = modSystem.commands.OfType<TriggerCommand>().ToList();

            foreach (TriggerCommand trigger in triggerCommands)
            {
                if (trigger.IsAlias(command))
                {
                    resp.valid = trigger.ParseArguments(args, out resp.response);

                    if (resp.valid)
                    {
                        resp.response = "";

                        if (execute)
                        {
                            trigger.Execute(i, j, world);
                        }
                    }

                    return resp;
                }
            }

            return resp;
        }

        private static CommandResponse ParseInfluencer(string command, string[] args, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'");

            List<InfluencerCommand> influencerCommands = modSystem.commands.OfType<InfluencerCommand>().ToList();

            foreach (InfluencerCommand influencer in influencerCommands)
            {
                if (influencer.IsAlias(command))
                {
                    resp.valid = influencer.ParseArguments(args, out resp.response);

                    if (resp.valid)
                    {
                        resp.response = "";
                        resp.success = true;

                        if (execute)
                        {
                            resp.response = influencer.Execute();
                        }
                    }

                    return resp;
                }
            }

            return resp;
        }

        private static CommandResponse ParseConditional(string command, string[] args, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'");

            List<ConditionalCommand> conditionalCommands = modSystem.commands.OfType<ConditionalCommand>().ToList();

            foreach (ConditionalCommand conditional in conditionalCommands)
            {
                if (conditional.IsAlias(command))
                {
                    resp.valid = conditional.ParseArguments(args, out resp.response);

                    if (resp.valid)
                    {
                        resp.response = "";

                        if (execute)
                        {
                            resp.response = conditional.Execute(out resp.success);
                        }
                    }

                    return resp;
                }
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
