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

        private static CommandResponse InfluencerForceGiveItem(string[] args, CommandResponse resp, bool execute)
        {
            List<Player> players = new List<Player>();
            int itemId;
            bool validId;
            int itemCount;
            bool validCount;

            if (args.Length < 3 || args.Length > 4)
            {
                resp.response = "Takes 3-4 parameters; Item ID, Item Count & Player target";
                return resp;
            }


            var ret1 = ParseInt(args[0], resp);
            itemId = ret1.Item1;
            resp = ret1.Item2;
            if (!resp.valid || itemId < 1 || itemId > ItemID.Count) // TODO: Allow for negative item IDs (wait for 1.4)
            {
                resp.response = $"{args[0]} is not a valid item ID.";
                return resp;
            }

            resp.valid = false;

            var ret2 = ParseInt(args[1], resp);
            itemCount = ret2.Item1;
            resp = ret2.Item2;
            if (!resp.valid || itemCount < 1)
            {
                resp.response = $"{args[1]} is not a valid item count.";
                return resp;
            }

            resp.valid = false;

            string playerParam = "";
            if (args.Length == 4)
            {
                playerParam = args[3];
            }

            var ret = ParsePlayerTarget(args[2], playerParam, resp);
            players = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    for (int i = 0; i < itemCount; i++)
                    {
                        player.PutItemInInventory(itemId);
                    }
                }
            }

            string playernames = GetPlayerNames(players);

            resp.success = true;
            resp.response = $"Gave item id {itemId} x{itemCount} to {playernames}.";

            return resp;
        }

        private static CommandResponse InfluencerDrawWorldText(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 7)
            {
                resp.response =
                    $"Takes 7 parameters; R value, G value, B value, X Co-ordinate, Y Co-ordinate, Time & Message";
                return resp;
            }

            int[] colors = new int[3];
            for (int i = 0; i < 3; i++)
            {
                var ret = ParseInt(args[i], resp, 0, 255);
                colors[i] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            Color textColor = new Color(colors[0], colors[1], colors[2]);

            int[] pos = new int[2];
            for (int i = 3; i < 5; i++)
            {
                var ret = ParseInt(args[i], resp, 0);
                pos[i - 3] = ret.Item1 * 16;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            resp.valid = false;
            var ret1 = ParseInt(args[5], resp);
            int timeout = ret1.Item1;
            resp = ret1.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                TICMod mod = ModContent.GetInstance<TICMod>();
                if (Main.dedServ)
                {
                    mod.SendTextDisplayPacket(args[6], textColor, timeout, pos[0], pos[1], true);
                }
                else
                {
                    ModContent.GetInstance<TICMod>().textDisplayer
                        .AddText(args[6], textColor, timeout, pos[0], pos[1], true);
                }

                string timeoutText = (timeout < 1) ? "until world restart." : $"for {timeout} seconds.";
                resp.response = $"Displaying '{args[6]}' as {textColor} at ({pos[0]}, {pos[1]}) {timeoutText}";
                resp.success = true;
            }

            return resp;
        }

        private static CommandResponse InfluencerDrawUIText(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 7)
            {
                resp.response = $"Takes 7 parameters; R Value, G Value, B Value, Width %, Height %, Time & Message";
                return resp;
            }

            int[] colors = new int[3];
            for (int i = 0; i < 3; i++)
            {
                var ret = ParseInt(args[i], resp, 0, 255);
                colors[i] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            Color textColor = new Color(colors[0], colors[1], colors[2]);

            int[] pos = new int[2];
            for (int i = 3; i < 5; i++)
            {
                var ret = ParseInt(args[i], resp, 0, 100);
                pos[i - 3] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            var ret1 = ParseInt(args[5], resp);
            int timeout = ret1.Item1;
            resp = ret1.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                TICMod mod = ModContent.GetInstance<TICMod>();
                if (Main.dedServ)
                {
                    mod.SendTextDisplayPacket(args[6], textColor, timeout, pos[0], pos[1], false);
                }
                else
                {
                    ModContent.GetInstance<TICMod>().textDisplayer
                        .AddText(args[6], textColor, timeout, pos[0], pos[1], false);
                }

                string timeoutText = (timeout < 1) ? "until world restart." : $"for {timeout} seconds.";
                resp.response = $"Displaying '{args[3]}' as {textColor} at ({pos[0]}%, {pos[1]}%) {timeoutText}";
                resp.success = true;
            }

            return resp;
        }

        private static CommandResponse InfluencerRespawnPlayer(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                resp.response = "Takes 1-2 parameters; Player Target";
                return resp;
            }

            string playerParam = "";
            if (args.Length == 2)
            {
                playerParam = args[1];
            }

            var ret = ParsePlayerTarget(args[0], playerParam, resp);
            List<Player> players = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    player.respawnTimer = 0;
                }
            }

            string playernames = GetPlayerNames(players);
            resp.response = $"Respawned players {playernames}.";
            resp.success = true;

            return resp;
        }

        private static CommandResponse InfluencerKillPlayer(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                resp.response = "Takes 2-3 parameters; Player Target & Message";
                return resp;
            }

            string playerParam = "";
            if (args.Length == 3)
            {
                playerParam = args[1];
            }

            var ret = ParsePlayerTarget(args[0], playerParam, resp);
            List<Player> players = ret.Item1;
            resp = ret.Item2;
            int argCount = ret.Item3;

            if (!resp.valid)
            {
                return resp;
            }

            string reason = args[1];
            if (args.Length == 3)
            {
                reason = args[2];
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    string thisreason = reason.Replace("#name", player.name);
                    player.KillMe(PlayerDeathReason.ByCustomReason($"{thisreason}"), Double.MaxValue, 0, false);
                }
            }

            string playernames = GetPlayerNames(players);
            resp.response = $"Killed players {playernames}.";
            resp.success = true;

            return resp;
        }

        private static CommandResponse InfluencerKillNPC(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                resp.response = "Takes 1 parameter; NPC Target";
                return resp;
            }

            string playerParam = "";
            if (args.Length == 2)
            {
                playerParam = args[1];
            }

            var ret = ParseNPCTarget(args[0], playerParam, resp);
            List<NPC> npcs = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            int count = 0;
            if (execute)
            {
                foreach (var npc in npcs)
                {
                    if (npc.life > 0)
                    {
                        npc.StrikeNPCNoInteraction(npc.life + (int)(npc.defense * 0.5f) * 2, 0, 0); // Kill NPC
                        count++;
                    }
                }
            }
            
            resp.valid = true;
            resp.success = true;
            resp.response = $"Killed {count} NPCs.'";
            return resp;
        }

        private static CommandResponse InfluencerPlaceTile(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length < 5 || args.Length > 6)
            {
                resp.response = $"Takes 5-6 parameters; Start X Co-ordinate, Start Y Co-ordinate, End X Co-ordinate, End Y Co-ordinate, Tile ID & Tile Style(optional)";
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
                var ret = ParseInt(args[i+2], resp, 0);
                pos2[i] = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            var ret2 = ParseInt(args[4], resp, 0, TileID.Count);
            int tileID = ret2.Item1;
            resp = ret2.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            int style = 0;
            if (args.Length == 4)
            {
                ret2 = ParseInt(args[5], resp, 0, TileID.Count);
                style = ret2.Item1;
                resp = ret2.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }

            int[] startpos = { Math.Min(pos1[0], pos2[0]), Math.Min(pos1[1], pos2[1]) };
            int[] endpos = { Math.Max(pos1[0], pos2[0]), Math.Max(pos1[1], pos2[1]) };

            if (execute)
            {
                for (int x = startpos[0]; x < endpos[0]+1; x++)
                {
                    for (int y = startpos[1]; y < endpos[1]+1; y++)
                    {
                        WorldGen.PlaceTile(x, y, tileID, style: style, forced: true);
                    }
                }
            }


            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully spawned tile ID:{tileID} @ {startpos[0]},{startpos[1]} - {endpos[0]},{endpos[1]}.";
            return resp;
        }

        private static CommandResponse InfluencerKillTile(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 4)
            {
                resp.response = $"Takes 4 parameters; Start X Co-ordinate, Start Y Co-ordinate, End X Co-ordinate & End Y Co-ordinate";
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

            int[] startpos = { Math.Min(pos1[0], pos2[0]), Math.Min(pos1[1], pos2[1]) };
            int[] endpos = { Math.Max(pos1[0], pos2[0]), Math.Max(pos1[1], pos2[1]) };

            if (execute)
            {
                for (int x = startpos[0]; x < endpos[0] + 1; x++)
                {
                    for (int y = startpos[1]; y < endpos[1] + 1; y++)
                    {
                        WorldGen.KillTile(x, y, noItem:true);
                    }
                }
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully cleared tiles @ {startpos[0]},{startpos[1]} - {endpos[0]},{endpos[1]}.";
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
