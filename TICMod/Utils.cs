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
    }
}
