using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace TICMod
{
    public static class Utils
    {
        public static void ChatOutput(string message, Color color)
        {
            if (Main.netMode == NetmodeID.Server || Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), color);
            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(message, color);
            }
        }

        public static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        public static bool IsPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key);
        }

        // TY direwolf420
        public static string GetTimeAsString(double time, bool accurate = true)
        {
            if (!Main.dayTime)
            {
                time += 54000.0;
            }
            time = time / 86400.0 * 24.0;
            double val = 7.5;
            time = time - val - 12.0;
            if (time < 0.0)
            {
                time += 24.0;
            }
            int hours = (int)time;
            double doubleminutes = time - hours;
            doubleminutes = (int)(doubleminutes * 60.0);
            string minutes = string.Concat(doubleminutes);
            if (doubleminutes < 10.0)
            {
                minutes = "0" + minutes;
            }
            if (!accurate) minutes = (!(doubleminutes < 30.0)) ? "30" : "00";
            return $"{hours}:{minutes}";
        }

        // this function took me too long to work out
        public static double TimeToMainTime(uint hours, uint mins, out bool dayTime)
        {
            dayTime = (hours == 4  && mins >= 30 || hours >= 5) &&
                      (hours == 19 && mins < 30  || hours <= 18);
            uint hrOffset = 8;

            if (dayTime)
            {
                hrOffset = 5;
            }
            else
            {
                if (hours < 12)
                {
                    hours += 12;
                }
                else if (hours > 12)
                {
                    hours -= 12;
                }
            }

            uint halfHrOffset = 0;

            if (mins >= 30)
            {
                halfHrOffset = 1;
            }

            uint deltaH = hours - hrOffset + halfHrOffset;
            uint deltaM = (60 + mins - 30) % 60;

            return deltaH * 3600 + deltaM * 60;
        }
    }
}
