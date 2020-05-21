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
        private static CommandResponse ParseTrigger(List<String> commandArgs, bool execute, int i, int j)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{commandArgs[0]}'.");
            commandArgs[0] = commandArgs[0].ToLower();

            switch (commandArgs[0])
            {
                case "time":
                    resp = TriggerTime(commandArgs, resp, execute, i, j);
                    break;

                case "playerdeath":
                    resp = TriggerPlayerDeath(commandArgs, resp, execute, i, j);
                    break;
            }

            return resp;
        }



        private static CommandResponse TriggerPlayerDeath(List<String> commandArgs, CommandResponse resp, bool execute, int i, int j)
        {
            string storeName = null;
            if (commandArgs.Count > 1)
            {
                var args = commandArgs[1].Split(new[] { ' ' }).ToList();

                if (args.Count > 1)
                {
                    resp.response = "Command must have either 0 or 1 parameter.";
                    return resp;
                }

                storeName = args[0];
            }

            bool triggered = false;
            TICMod mod = ModContent.GetInstance<TICMod>();
            TICStates states = ModContent.GetInstance<TICStates>();
            List<Player> deadPlayers = new List<Player>();
            states.setTrigger(i, j, (() =>
            {
                foreach (var player in Main.player)
                {
                    if (player.dead && !deadPlayers.Contains(player))
                    {
                        deadPlayers.Add(player);
                        mod.playerDataStore.AddItem(storeName, player);
                        ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(i, j - 1);
                        SendChatMsg($"{player.name} died, triggering.", i, j, states.isChatEnabled(i, j));
                        triggered = true;
                    }
                    else if (!player.dead && deadPlayers.Contains(player))
                    {
                        deadPlayers.Remove(player);
                        mod.playerDataStore.RemoveItem(storeName);
                    }
                }
            }));

            resp.valid = true;
            return resp;
        }

        private static CommandResponse TriggerTime(List<String> commandArgs, CommandResponse resp, bool execute, int i, int j)
        {
            if (commandArgs.Count != 2)
            {
                resp.response =
                    $"Command must contain a player name.";
                return resp;
            }
            var posStr = commandArgs[1].Split(new[] { ':' }, 2);
            List<uint> time = new List<uint>(2);
            foreach (var str in posStr)
            {
                bool success = uint.TryParse(str, NumberStyles.Integer, CultureInfo.CurrentCulture, out uint posVal);
                if (!success)
                {
                    break;
                }
                time.Add(posVal);
            }
            if (time.Count != 2 || time[0] > 24 || time[1] > 59)
            {
                resp.response =
                    $"{commandArgs[0]} is not a valid time in format hh:mm.";
                return resp;
            }

            string givenTime = commandArgs[1];
            bool triggered = false;
            TICStates states = ModContent.GetInstance<TICStates>();
            states.setTrigger(i, j, (() =>
            {
                string currenttime = Utilities.GetTimeAsString(Main.time);


                if (states.isEnabled(i, j) && currenttime == givenTime && !triggered)
                {
                    ModContent.GetInstance<ExtraWireTrips>().AddWireUpdate(i, j - 1);
                    SendChatMsg($"Reached time {currenttime}, triggering.", i, j, states.isChatEnabled(i, j));
                    triggered = true;
                }
                else if (currenttime != givenTime && triggered)
                {
                    triggered = false;
                }
            }));

            resp.valid = true;
            return resp;
        }

        public static void SendChatMsg(string text, int x = -1, int y = -1, bool showOutput = true)
        {
            if (showOutput)
            {
                Main.NewText($"[Trigger@{x},{y}] {text}", Color.Gray);
            }
        }
    }
}
