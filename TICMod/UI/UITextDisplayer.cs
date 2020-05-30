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
    class UITextDisplayer : UIState
    {
        internal Queue<DisplayText> toRemove = new Queue<DisplayText>();

        public void AddText(string text, Color color, TimeSpan displayTime)
        {
            this.Append(new DisplayText(text, displayTime, 20, 20));
        }
    }

    class DisplayText : UIText
    {
        private TimeSpan lifespan;
        private DateTime initTime;

        internal DisplayText(string text, TimeSpan lifespan, int xPos, int yPos,  float textScale=1, bool large=false) : base(text, textScale, large)
        {
            this.lifespan = lifespan;
            initTime = DateTime.Now;
            Top.Pixels = yPos;
            Left.Pixels = xPos;
        }

        public override void Update(GameTime gameTime)
        {
            TimeSpan elapsedTime = DateTime.Now - initTime;
            if (elapsedTime > lifespan)
            {
                Parent.Remove();
                Parent.RecalculateChildren();
            }
        }
    }
}
