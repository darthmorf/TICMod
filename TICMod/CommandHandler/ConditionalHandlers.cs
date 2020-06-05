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
        private static CommandResponse ParseConditional(string command, string[] args, bool execute)
        {
            CommandResponse resp = new CommandResponse(false, $"Unknown Command '{command}'.");
            command = command.ToLower();

            switch (command)
            {
                case "day":
                    resp = ConditionalDay(args, resp, execute);
                    break;
            }

            return resp;
        }


        private static CommandResponse ConditionalDay(string[] args, CommandResponse resp, bool execute)
        {
            bool isDay = Main.dayTime;
            resp.valid = true;

            if (execute)
            {
                resp.response = $"Is Day? {isDay}";
            }

            resp.success = isDay;

            return resp;
        }
    }
}