using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TICMod.UI
{
    class UICoordDisplay : UIState
    {
        private UIText text;
        public override void OnInitialize()
        {
            base.OnInitialize();
            text = new UIText("(-1, -1)");

            this.Append(text);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Main.player[Main.myPlayer].GetModPlayer<TICPlayer>().CoordDisplay)
            {
                return;
            }

            text.SetText($"({(int)Main.MouseWorld.X/16}, {(int)Main.MouseWorld.Y/16})");
            text.Left.Pixels = Main.mouseX;
            text.Top.Pixels = Main.mouseY - 20;
            Recalculate();

            base.Draw(spriteBatch);
        }
    }
}
