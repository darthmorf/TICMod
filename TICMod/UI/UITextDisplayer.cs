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

        public void AddText(string text, Color color, TimeSpan displayTime, int xPos, int yPos)
        {
            this.Append(new DisplayText(text, displayTime, xPos, yPos, color));
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
        private TimeSpan lifespan;
        private DateTime initTime;

        public bool delete = false;

        internal DisplayText(string text, TimeSpan lifespan, int xPos, int yPos, Color color, float textScale=1, bool large=false) : base(text, textScale, large)
        {
            this.lifespan = lifespan;
            initTime = DateTime.Now;
            Top.Pixels = yPos;
            Left.Pixels = xPos;
            TextColor = color;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = new Vector2(4560*16, 480*16);
            Vector2 screenPos = new Vector2(Main.screenPosition.X, Main.screenPosition.Y);
            pos = pos - screenPos;
            
            Top.Pixels = pos.Y;
            Left.Pixels = pos.X;

            TimeSpan elapsedTime = DateTime.Now - initTime; // not effected by speeding up time ingame
            if (elapsedTime > lifespan)
            {
                delete = true;
            }

            base.Draw(spriteBatch);
        }
    }
}
