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

        private static CommandResponse InfluencerCopyTiles(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 6)
            {
                resp.response = $"Takes 6 parameters; Start X Co-ordinate, Start Y Co-ordinate, End X Co-ordinate, End Y Co-ordinate, Destination X & Destination Y";
                return resp;
            }

            int[] pos1 = new int[2];
            for (int i = 0; i < 2; i++)
            {
                var ret = ParseInt(args[i], resp, 0);
                pos1[i] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            int[] pos2 = new int[2];
            for (int i = 0; i < 2; i++)
            {
                var ret = ParseInt(args[i + 2], resp, 0);
                pos2[i] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            int[] pos3 = new int[2];
            for (int i = 0; i < 2; i++)
            {
                var ret = ParseInt(args[i + 4], resp, 0);
                pos3[i] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            int[] startpos = { Math.Min(pos1[0], pos2[0]), Math.Min(pos1[1], pos2[1]) };
            int[] endpos = { Math.Max(pos1[0], pos2[0]), Math.Max(pos1[1], pos2[1]) };

            if (execute)
            {
                for (int x = 0; x < endpos[0] - startpos[0] + 1; x++)
                {
                    for (int y = 0; y < endpos[1] - startpos[1] + 1; y++)
                    {
                        Main.tile[pos3[0] + x, pos3[1] + y].CopyFrom(Main.tile[startpos[0] + x, startpos[1] + y]);
                    }
                }
            }


            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully copied tiles @ {startpos[0]},{startpos[1]} - {endpos[0]},{endpos[1]} to {pos3[0]}, {pos3[1]}.";
            return resp;
        }

        private static CommandResponse InfluencerTeleportPlayer(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                resp.response = $"Takes 3-4 parameters; X Co-ordinate, Y Co-ordinate, Player Target";
                return resp;
            }

            int[] pos = new int[2];
            for (int i = 0; i < 2; i++)
            {
                var ret = ParseInt(args[i], resp, 0);
                pos[i] = ret.Item1 * 16;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            string param = "";
            if (args.Length == 4)
            {
                param = args[3];
            }

            var ret2 = ParsePlayerTarget(args[2], param, resp);
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
                    player.position = new Vector2(pos[0], pos[1]);
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully teleported {GetPlayerNames(players)} to ({pos[0]/16}, {pos[1]/16}).";
            return resp;
        }

        private static CommandResponse InfluencerSpawnItem(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 4)
            {
                resp.response = $"Takes 4 parameters; X Co-ordinate, Y Co-ordinate, Item ID & Item Count";
                return resp;
            }

            int[] pos = new int[2];
            for (int i = 0; i < 2; i++)
            {
                var ret = ParseInt(args[i], resp, 0);
                pos[i] = ret.Item1 * 16;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            var ret2 = ParseInt(args[2], resp, 1, ItemID.Count);
            int itemId = ret2.Item1;
            resp = ret2.Item2;

            if (!resp.valid)
            {
                return resp;
            }
            resp.valid = false;

            ret2 = ParseInt(args[3], resp, 1);
            int count = ret2.Item1;
            resp = ret2.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                Item.NewItem(new Vector2(pos[0], pos[1]), itemId, Stack: count);
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully spawned item with ID {itemId} x{count} at ({pos[0] / 16}, {pos[1] / 16}).";
            return resp;
        }

        private static CommandResponse InfluencerRemovePlayerItem(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length < 3 || args.Length > 4)
            {
                resp.response = $"Takes 3-4 parameters; Item ID, Count, Player Target";
                return resp;
            }

            var ret = ParseInt(args[0], resp, 1, ItemID.Count);
            int itemId = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }
            resp.valid = false;

            ret = ParseInt(args[1], resp, 1);
            int count = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }
            resp.valid = false;

            string param = "";
            if (args.Length == 4)
            {
                param = args[3];
            }

            var ret2 = ParsePlayerTarget(args[2], param, resp);
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
                    int removeCount = 0;
                    foreach (var item in player.inventory)
                    {
                        if (item.netID == itemId)
                        {
                            while (item.stack >= 0 && removeCount <= count)
                            {
                                item.stack--;
                                removeCount++;
                            }

                            if (item.stack <= 0)
                            {
                                item.TurnToAir();
                            }

                            if (removeCount >= count)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully removed {itemId} x{count} at from {GetPlayerNames(players)}.";
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
