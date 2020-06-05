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
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace TICMod.UI
{
    class UITextDisplayer : UIStateReverse
    { 

        public void AddText(string text, Color color, int displayTime, int xPos, int yPos, bool tileAttach)
        {
            this.Append(new DisplayText(text, displayTime, xPos, yPos, color, tileAttach));
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            DisplayText[] texts = Elements.OfType<DisplayText>().ToArray();

            foreach (var text in texts)
            {
                if (text.delete)
                {
                    Elements.Remove(text);
                }
            }

            base.DrawChildren(spriteBatch);
        }
    }

    class DisplayText : UIText
    {
        private int lifespan;
        private DateTime initTime;
        private bool tileAttach;
        private int xPos, yPos;

        public bool delete = false;

        internal DisplayText(string text, int lifespan, int xPos, int yPos, Color color, bool tileAttach, float textScale=1, bool large=false) : base(text, textScale, large)
        {
            this.lifespan = lifespan;
            this.tileAttach = tileAttach;
            this.xPos = xPos;
            this.yPos = yPos;
            if (tileAttach)
            {
                Top.Pixels = yPos;
                Left.Pixels = xPos;
            }
            else
            {
                Top.Precent = (float)yPos / 100;
                Left.Precent = (float)xPos / 100;
            }
            initTime = DateTime.Now;
            TextColor = color;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (tileAttach)
            {
                Vector2 pos = new Vector2(xPos, yPos);
                Vector2 screenPos = new Vector2(Main.screenPosition.X, Main.screenPosition.Y);
                pos = pos - screenPos;

                Top.Pixels = pos.Y;
                Left.Pixels = pos.X;

                Recalculate();
            }


            TimeSpan elapsedTime = DateTime.Now - initTime;
            TimeSpan lifespan_ = TimeSpan.FromSeconds(lifespan);
            if (elapsedTime > lifespan_ && lifespan > 0)
            {
                delete = true;
            }

            base.Draw(spriteBatch);
        }
    }
}
