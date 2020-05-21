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
            }

            return resp;
        }


        private static CommandResponse InfluencerSay(List<String> commandArgs, CommandResponse resp, bool execute)
        {
            if (commandArgs.Count != 2)
            {
                resp.response =
                    $"Command must contain both an RGB code and a string to print.";
                return resp;
            }
            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
            if (args.Count != 2)
            {
                resp.response =
                    $"Command must contain both an RGB code and a string to print.";
                return resp;
            }
            var rgbStr = args[0].Split(new[] { ',' }, 3);
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
                return resp;
            }

            Color textColor = new Color(rgb[0], rgb[1], rgb[2]);

            if (args.Count < 2)
            {
                args.Add("");
            }
            resp.success = true;
            resp.valid = true;
            resp.response = $"Displaying '{args[1]}' as colour {textColor.ToString()}";

            if (execute)
            {
                args[1].Split(new String[] { "\\n" }, StringSplitOptions.None).ToList().ForEach(line => Main.NewText(line, textColor));
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
            var posStr = args[0].Split(new[] { ',' }, 2);
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
                return resp;
            }

            pos[0] *= 16;
            pos[1] *= 16;

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
            var posStr = args[0].Split(new[] { ',' }, 2);
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
                return resp;
            }

            pos[0] *= 16;
            pos[1] *= 16;

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
            string itemIdString = "";
            bool validId;

            if (commandArgs.Count > 1)
            {
                var args = commandArgs[1].Split(new[] { ' ' }).ToList();
                if (args.Count == 0 || args[0] == "")
                {
                    resp.response = "Command takes at least two parameters.";
                    return resp;
                }

                if (args[0] == "@s")
                {
                    if (args.Count != 3)
                    {
                        resp.response = $"{commandArgs[0]} {args[0]} requires 2 following parameters.";
                        return resp;
                    }

                    players = ModContent.GetInstance<TICMod>().playerDataStore.GetItem(args[1]);
                    itemIdString = args[2];
                }
                else if (args[0] == "@a") //TODO: Seperate player selection logic to seperate method
                {
                    if (args.Count != 2)
                    {
                        resp.response = $"{commandArgs[0]} {args[0]} requires 1 following parameter.";
                        return resp;
                    }

                    players = Main.player.ToList();
                    itemIdString = args[1];
                }
                else
                {
                    resp.response = $"{args[0]} is not a valid player target.";
                    return resp;
                }
            }
            else
            {
                resp.response = $"Requires player target and item ID parameter.";
                return resp;
            }

            if (itemIdString == "")
            {
                resp.response = $"Requires item ID parameter";
                return resp;
            }

            validId = int.TryParse(itemIdString, NumberStyles.Integer, CultureInfo.CurrentCulture, out itemId);
            if (!validId || itemId < 1 || itemId > Main.item.Length) // TODO: Allow for negative item IDs
            {
                resp.response = $"{itemIdString} is not a valid item ID.";
                return resp;
            }

            string playernames = "";
            foreach (var player in players)
            {
                player.PutItemInInventory(itemId);

                if (player.name != "")
                {
                    playernames += $"{player.name}, ";
                }
            }

            if (playernames != "")
            {
                playernames = playernames.Substring(0, playernames.LastIndexOf(", "));
            }

            resp.valid = true;
            resp.success = true;
            resp.response = $"Gave item id {itemId} to {playernames}.";

            return resp;
        }
    }
}
