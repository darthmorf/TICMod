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
        private static CommandResponse TriggerTime (List<String> commandArgs, CommandResponse resp, bool execute, int i, int j)
        {
            if (commandArgs.Count != 2)
            {
                resp.response =
                    $"Command must contain a time.";
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
                    SendChatMsg($"Reached time {currenttime}, triggering.", i, j, states.isChatEnabled(i,j));
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
