using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TICMod.UI;

namespace TICMod
{
    public static class CommandHandler
    {
        public static CommandResponse Parse(string command, BlockType blockType)
        {
            /*List<string> args = Regex
                .Matches(command, @"(?<match>\w+)|\""(?<match>[\w\s]*)""")
                .Cast<Match>()
                .Select(m => m.Groups["match"].Value)
                .ToList();

            args[0] = args[0].ToLower();*/

            var commandArgs = command.Split(new[] {' '}, 2).ToList();

            CommandResponse resp = new CommandResponse(false, "Unkown Command Block");
            switch (blockType)
            {
                case BlockType.Trigger:
                    resp = ParseTrigger(commandArgs);
                    break;
                case BlockType.Influencer:
                    resp = ParseInfluencer(commandArgs);
                    break;
                case BlockType.Conditional:
                    resp = ParseConditional(commandArgs);
                    break;
            }

            return resp;
        }

        private static CommandResponse ParseTrigger(List<String> args)
        {
            return null;
        }

        private static CommandResponse ParseInfluencer(List<String> commandArgs)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{commandArgs[0]}'.");
            commandArgs[0] = commandArgs[0].ToLower();

            switch (commandArgs[0])
            {
                case "say":
                    resp = CommandSay(commandArgs, resp);
                    break;

                case "spawnnpc":
                    resp = CommandSpawnNPC(commandArgs, resp);
                    break;

                case "spawnnpcid":
                    resp = CommandSpawnNPCID(commandArgs, resp);
                    break;
            }

            return resp;
        }

        private static CommandResponse ParseConditional(List<String> args)
        {
            return null;
        }


        private static CommandResponse CommandSay(List<String> commandArgs, CommandResponse resp)
        {
            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
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
            resp.response = $"Displaying '{args[1]}' as colour {textColor.ToString()}";

            args[1].Split(new String[] { "\\n" }, StringSplitOptions.None).ToList().ForEach(line => Main.NewText(line, textColor));

            return resp;
        }

        private static CommandResponse CommandSpawnNPC(List<String> commandArgs, CommandResponse resp)
        {
            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
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

            if (foundNpc)
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


                int index = NPC.NewNPC(pos[0], pos[1], npc.type);
                Main.npc[index].SetDefaults(npc.netID);
                resp.success = true;
                resp.response = $"Successfully spawned {npc.GivenOrTypeName}, ID:{npc.netID} @ {pos[0] / 16},{pos[1] / 16}.";
            }
            else
            {
                resp.response = $"Could not find NPC with name '{args[1]}'.";
            }

            return resp;
        }

        private static CommandResponse CommandSpawnNPCID(List<String> commandArgs, CommandResponse resp)
        {
            var args = commandArgs[1].Split(new[] { ' ' }, 2).ToList();
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

            if (npcID >= Main.npcTexture.Length || npcID < -65 || !isId)
            {
                resp.response =
                    $"{args[0]} is not a valid position NPC ID. Must be > -66 and < {Main.npcTexture.Length}.";
                return resp;
            }

            NPC npc = new NPC();
            npc.SetDefaults(npcID);

            int index = NPC.NewNPC(pos[0], pos[1], npc.type);
            Main.npc[index].SetDefaults(npc.netID);
            resp.success = true;
            resp.response = $"Successfully spawned {npc.GivenOrTypeName}, ID:{npc.netID} @ {pos[0] / 16},{pos[1] / 16}.";
            

            return resp;
        }
    }

    public class CommandResponse
    {
        public bool success;
        public string response;

        public CommandResponse(bool _success, string _response)
        {
            success = _success;
            response = _response;
        }
    }
}
