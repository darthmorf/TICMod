using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TICMod.Commands.Influencers;

namespace TICMod
{
    public static partial class CommandHandler
    {
        public static TICMod mod;
        private static CommandResponse ParseInfluencer(string command, string[] args, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'");

            mod = ModContent.GetInstance<TICMod>();

            List<Influencer> influencerCommands = mod.commands.OfType<Influencer>().ToList();

            foreach (Influencer influencer in influencerCommands)
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

        private static CommandResponse InfluencerClearDroppedItems (string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 0)
            {
                resp.response = $"Takes 0 parameters.";
                return resp;
            }

            if (execute)
            {
                foreach (var item in Main.item)
                {
                    item.TurnToAir();
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully Cleared all Dropped Items.";
            return resp;
        }

        // Note - if health > 500, will be reduced to 500 upon world rejoin
        private static CommandResponse InfluencerSetMaxHealth (string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 2)
            {
                resp.response = $"Takes 2 parameters; health value, player";
                return resp;
            }

            var ret = ParseInt(args[0], resp, 20);
            int health = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            string param = "";
            if (args.Length == 3)
            {
                param = args[2];
            }

            var ret2 = ParsePlayerTarget(args[1], param, resp);
            List<Player> players = ret2.Item1;
            resp = ret2.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    player.statLifeMax = health;
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully set max health of {GetPlayerNames(players)} to {health}.";
            return resp;
        }

        private static CommandResponse InfluencerSetHealth(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 2)
            {
                resp.response = $"Takes 2 parameters; health value, player";
                return resp;
            }

            var ret = ParseInt(args[0], resp, 0);
            int health = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            string param = "";
            if (args.Length == 3)
            {
                param = args[2];
            }

            var ret2 = ParsePlayerTarget(args[1], param, resp);
            List<Player> players = ret2.Item1;
            resp = ret2.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    player.statLife = health;
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully set health of {GetPlayerNames(players)} to {health}.";
            return resp;
        }

        private static CommandResponse InfluencerSetMaxMana(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 2)
            {
                resp.response = $"Takes 2 parameters; mana value, player";
                return resp;
            }

            var ret = ParseInt(args[0], resp, 0);
            int health = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            string param = "";
            if (args.Length == 3)
            {
                param = args[2];
            }

            var ret2 = ParsePlayerTarget(args[1], param, resp);
            List<Player> players = ret2.Item1;
            resp = ret2.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    player.statManaMax = health;
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully set max mana of {GetPlayerNames(players)} to {health}.";
            return resp;
        }

        private static CommandResponse InfluencerSetMana(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 2)
            {
                resp.response = $"Takes 2 parameters; mana value, player";
                return resp;
            }

            var ret = ParseInt(args[0], resp, 0);
            int health = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            string param = "";
            if (args.Length == 3)
            {
                param = args[2];
            }

            var ret2 = ParsePlayerTarget(args[1], param, resp);
            List<Player> players = ret2.Item1;
            resp = ret2.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    player.statMana = health;
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully set mana of {GetPlayerNames(players)} to {health}.";
            return resp;
        }
    }
}
