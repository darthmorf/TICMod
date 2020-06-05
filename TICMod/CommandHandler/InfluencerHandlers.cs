using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TICMod
{
    public static partial class CommandHandler
    {
        private static CommandResponse ParseInfluencer(string command, string[] args, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'");
            command = command.ToLower();

            switch (command)
            {
                case "say":
                    resp = InfluencerSay(args, resp, execute);
                    break;

                case "spawnnpc":
                    resp = InfluencerSpawnNPC(args, resp, execute);
                    break;

                case "spawnnpcid":
                    resp = InfluencerSpawnNPCID(args, resp, execute);
                    break;

                case "giveitem":
                    resp = InfluencerGiveItem(args, resp, execute);
                    break;

                case "forcegiveitem":
                    resp  = InfluencerForceGiveItem(args, resp, execute);
                    break;

                case "drawworldtext":
                    resp = InfluencerDrawWorldText(args, resp, execute);
                    break;

                case "drawuitext":
                    resp = InfluencerDrawUIText(args, resp, execute);
                    break;

                case "respawn":
                    resp = InfluencerRespawnPlayer(args, resp, execute);
                    break;

                case "kill":
                    resp = InfluencerKillPlayer(args, resp, execute);
                    break;
            }

            return resp;
        }


        private static CommandResponse InfluencerSay(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 4)
            {
                resp.response = $"Takes 4 parameters; R value, G value, B value & string to print";
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

            resp.success = true;
            resp.valid = true;
            resp.response = $"Displaying '{args[1]}' as color {textColor.ToString()}.";

            if (execute)
            {
                args[3].Split(new String[] { "\\n" }, StringSplitOptions.None).ToList().ForEach(line => Utils.ChatOutput(line, textColor));
            }

            return resp;
        }

        private static CommandResponse InfluencerSpawnNPC(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 3)
            {
                resp.response = $"Takes 3 parameters; X Co-ordinate, Y Co-ordinate & NPC Name";
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

            string npcName = args[2].ToLower();

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

        private static CommandResponse InfluencerSpawnNPCID(string[] args, CommandResponse resp, bool execute)
        {
            if (args.Length != 3)
            {
                resp.response = $"Takes 3 parameters; X Co-ordinate, Y Co-ordinate & NPC ID";
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

            var ret2 = ParseInt(args[2], resp, -65, NPCID.Count);
            int npcID = ret2.Item1;
            resp = ret2.Item2;

            if (!resp.valid || npcID == 0)
            {
                resp.response += " and not 0";
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

        private static CommandResponse InfluencerGiveItem(string[] args, CommandResponse resp, bool execute)
        {
            List<Player> players = new List<Player>();
            int itemId;
            int itemCount;

            if (args.Length < 3 || args.Length > 4)
            {
                resp.response = "Takes 3-4 parameters; Item ID, Item Count & Player target";
                return resp;
            }
            
            var ret = ParseInt(args[0], resp, 1, ItemID.Count);
            itemId = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid) // TODO: Allow for negative item IDs (wait for 1.4)
            {
                return resp;
            }
            resp.valid = false;

            ret = ParseInt(args[1], resp, 1);
            itemCount = ret.Item1;
            resp = ret.Item2;
            if (!resp.valid)
            {
                return resp;
            }
            resp.valid = false;

            string playerParam = "";
            if (args.Length == 4)
            {
                playerParam = args[3];
            }

            var ret2 = ParsePlayerTarget(args[2], playerParam, resp);
            players = ret2.Item1;
            resp = ret2.Item2;
            if (!resp.valid)
            {
                return resp;
            }

            if (execute)
            {
                foreach (var player in players)
                {
                    player.QuickSpawnItem(itemId, itemCount);
                }
            }

            string playernames = GetPlayerNames(players);

            resp.success = true;
            resp.response = $"Gave item id {itemId} x{itemCount} to {playernames}.";

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
                resp.response = $"Takes 7 parameters; R value, G value, B value, X Co-ordinate, Y Co-ordinate, Time & Message";
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
                pos[i-3] = ret.Item1 * 16;
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
                    ModContent.GetInstance<TICMod>().textDisplayer.AddText(args[6], textColor, timeout, pos[0], pos[1], true);
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
                pos[i-3] = ret.Item1;
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
                    ModContent.GetInstance<TICMod>().textDisplayer.AddText(args[6], textColor, timeout, pos[0], pos[1], false);
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
    }
}
