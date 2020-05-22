using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using UIElement = On.Terraria.UI.UIElement;

namespace TICMod.UI
{
    // Draws child elements in reverse order so that mouseclicks hit the TOP element not the bottom one
    public class UIStateReverse : UIState
    {
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            this.Elements.Reverse();
            base.DrawChildren(spriteBatch);
            this.Elements.Reverse();
        }
    }
}
