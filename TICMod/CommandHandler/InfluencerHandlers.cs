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

                case "giveitem":
                    resp = InfluencerGiveItem(commandArgs, resp, execute);
                    break;

                case "forcegiveitem":
                    resp  = InfluencerForceGiveItem(commandArgs, resp, execute);
                    break;

                case "drawtext":
                    resp = InfluencerDrawText(commandArgs, resp, execute);
                    break;
            }

            return resp;
        }


        private static CommandResponse InfluencerSay(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            if (commandArgs.Count != 2)
            {
                resp.response = $"Command must contain both an RGB code and a string to print.";
                return resp;
            }

            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
            if (args.Count != 2)
            {
                resp.response = $"Command must contain both an RGB code and a string to print.";
                return resp;
            }

            var ret = ParseColor(args[0], resp);
            Color textColor = ret.Item1;
            resp = ret.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            if (args.Count < 2)
            {
                args.Add("");
            }
            resp.success = true;
            resp.valid = true;
            resp.response = $"Displaying '{args[1]}' as colour {textColor.ToString()}";

            if (execute)
            {
                args[1].Split(new String[] { "\\n" }, StringSplitOptions.None).ToList().ForEach(line => Utils.ChatOutput(line, textColor));
            }

            return resp;
        }

        private static CommandResponse InfluencerSpawnNPC(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            if (commandArgs.Count != 2)
            {
                resp.response =
                    $"Command must contain both a coordinate and an NPC name.";
                return resp;
            }
            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
            if (args.Count != 2)
            {
                resp.response =
                    $"Command must contain both a coordinate and an NPC name.";
                return resp;
            }

            var ret = ParseCoordinate(args[0], resp);
            int[] pos = ret.Item1;
            resp = ret.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            string npcName = args[1].ToLower();

            NPC npc = new NPC();

            bool foundNpc = false;
            for (int i = -65; i < Main.npcTexture.Length; i++)
            {
                npc.SetDefaults(i);
                {
                    if (npc.GivenOrTypeName.ToLower() == npcName)
                    {
                        foundNpc = true;
                        break;
                    }
                }
            }

            if (foundNpc && npc.netID != 0)
            {
                // correct NPC IDs
                switch (npc.netID)
                {
                    case 392: // Martian Saucer
                        npc.netID = 395;
                        break;

                    case 396: // Moon Lord
                        npc.netID = 398;
                        break;
                }

                if (execute)
                {
                    int index = NPC.NewNPC(pos[0], pos[1], npc.type);
                    Main.npc[index].SetDefaults(npc.netID);
                }

                resp.success = true;
                resp.valid = true;
                resp.response = $"Successfully spawned {npc.GivenOrTypeName}, ID:{npc.netID} @ {pos[0] / 16},{pos[1] / 16}.";
            }
            else
            {
                resp.response = $"Could not find NPC with name '{args[1]}'.";
            }

            return resp;
        }

        private static CommandResponse InfluencerSpawnNPCID(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            if (commandArgs.Count != 2)
            {
                resp.response =
                    $"Command must contain both a coordinate and an NPC ID.";
                return resp;
            }
            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
            if (args.Count != 2)
            {
                resp.response =
                    $"Command must contain both a coordinate and an NPC ID.";
                return resp;
            }
            var ret = ParseCoordinate(args[0], resp);
            int[] pos = ret.Item1;
            resp = ret.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            int npcID;
            bool isId = int.TryParse(args[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out npcID);

            if (npcID >= Main.npcTexture.Length || npcID < -65 || !isId || npcID == 0)
            {
                resp.response =
                    $"{args[0]} is not a valid position NPC ID. Must be > -66 and < {Main.npcTexture.Length} and not 0.";
                return resp;
            }

            NPC npc = new NPC();
            npc.SetDefaults(npcID);
            if (execute)
            {
                int index = NPC.NewNPC(pos[0], pos[1], npc.type);
                Main.npc[index].SetDefaults(npc.netID);
            }

            resp.success = true;
            resp.valid = true;
            resp.response = $"Successfully spawned {npc.GivenOrTypeName}, ID:{npc.netID} @ {pos[0] / 16},{pos[1] / 16}.";


            return resp;
        }

        private static CommandResponse InfluencerGiveItem(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            List<Player> players = new List<Player>();
            int itemId;
            bool validId;
            int itemCount;
            bool validCount;

            if (commandArgs.Count > 1)
            {
                var args = commandArgs[1].Split(new[] { ' ' }).ToList();
                if (args.Count < 3 || args[2] == "")
                {
                    resp.response = "Command requires item ID, item count and player target.";
                    return resp;
                }

                validId = int.TryParse(args[0], NumberStyles.Integer, CultureInfo.CurrentCulture, out itemId);
                if (!validId || itemId < 1 || itemId > Main.item.Length) // TODO: Allow for negative item IDs
                {
                    resp.response = $"{args[0]} is not a valid item ID.";
                    return resp;
                }

                validCount = int.TryParse(args[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out itemCount);
                if (!validCount || itemCount < 1)
                {
                    resp.response = $"{args[1]} is not a valid item count.";
                    return resp;
                }

                args.RemoveRange(0, 2);
                var ret = ParsePlayerTarget(args, resp);
                players = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }
            else
            {
                resp.response = $"Command requires item ID, item count and player target.";
                return resp;
            }

            string playernames = "";
            foreach (var player in players)
            {
                if (execute)
                {
                    player.QuickSpawnItem(itemId, itemCount);
                }

                if (player.name != "")
                {
                    playernames += $"{player.name}, ";
                }
            }

            if (playernames != "")
            {
                playernames = playernames.Substring(0, playernames.LastIndexOf(", "));
            }

            resp.success = true;
            resp.response = $"Gave item id {itemId} x{itemCount} to {playernames}.";

            return resp;
        }

        private static CommandResponse InfluencerForceGiveItem(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            List<Player> players = new List<Player>();
            int itemId;
            bool validId;
            int itemCount;
            bool validCount;

            if (commandArgs.Count > 1)
            {
                var args = commandArgs[1].Split(new[] { ' ' }).ToList();
                if (args.Count < 3 || args[2] == "")
                {
                    resp.response = "Command requires item ID, item count and player target.";
                    return resp;
                }

                validId = int.TryParse(args[0], NumberStyles.Integer, CultureInfo.CurrentCulture, out itemId);
                if (!validId || itemId < 1 || itemId > Main.item.Length) // TODO: Allow for negative item IDs
                {
                    resp.response = $"{args[0]} is not a valid item ID.";
                    return resp;
                }

                validCount = int.TryParse(args[1], NumberStyles.Integer, CultureInfo.CurrentCulture, out itemCount);
                if (!validCount || itemCount < 1)
                {
                    resp.response = $"{args[1]} is not a valid item count.";
                    return resp;
                }

                args.RemoveRange(0, 2);
                var ret = ParsePlayerTarget(args, resp);
                players = ret.Item1;
                resp = ret.Item2;

                if (!resp.valid)
                {
                    return resp;
                }
            }
            else
            {
                resp.response = $"Command requires item ID, item count and player target.";
                return resp;
            }

            string playernames = "";
            foreach (var player in players)
            {
                if (execute)
                {
                    for(int i = 0; i < itemCount; i++)
                    {
                        player.PutItemInInventory(itemId);
                    }
                }

                if (player.name != "")
                {
                    playernames += $"{player.name}, ";
                }
            }

            if (playernames != "")
            {
                playernames = playernames.Substring(0, playernames.LastIndexOf(", "));
            }

            resp.success = true;
            resp.response = $"Gave item id {itemId} x{itemCount} to {playernames}.";

            return resp;
        }

        private static CommandResponse InfluencerDrawText(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            if (commandArgs.Count != 2)
            {
                resp.response = $"Command must contain RGB code, Coodinate, Duration and a string to print.";
                return resp;
            }

            var args = commandArgs[1].Split(new[] { ' ' }, 4).ToList();
            if (args.Count != 4)
            {
                resp.response = $"Command must contain RGB code, Coodinate, Duration and a string to print.";
                return resp;
            }

            var ret1 = ParseColor(args[0], resp);
            Color textColor = ret1.Item1;
            resp = ret1.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            resp.valid = false;
            var ret2 = ParseCoordinate(args[1], resp);
            int[] pos = ret2.Item1;
            resp = ret2.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            resp.valid = false;
            var ret3 = ParseInt(args[2], resp);
            int timeout = ret3.Item1;
            resp = ret3.Item2;

            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                ModContent.GetInstance<TICMod>().textDisplayer.AddText(args[3], textColor, timeout, pos[0], pos[1], true);
                string timeoutText = (timeout < 1) ? "until world restart." : $"for {timeout} seconds.";
                resp.response = $"Displaying '{args[3]}' as {textColor} at ({pos[0]}, {pos[1]}) {timeoutText}";
                resp.success = true;
            }

            return resp;
        }
    }
}
