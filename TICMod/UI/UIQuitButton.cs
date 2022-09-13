using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TICMod.UI
{
    internal class UIQuitButton : UIPanel
    {
        public static Texture2D texture;

        public event EventHandler OnSelectedChanged;

        internal string hoverText;

        public UIQuitButton(string hoverText)
        {
            if (texture == null)
            {
                texture = ModContent.Request<Texture2D>("TICMod/UI/exit").Value;
            }
            this.hoverText = hoverText;
            this.BackgroundColor = Color.Transparent;
            this.BorderColor = Color.Transparent;
            this.Width.Set(texture.Width, 0);
            this.Height.Set(texture.Height, 0);
            Recalculate();
        }

        public void SetHoverText(string hoverText)
        {
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle innerDimensions = base.GetInnerDimensions();
            Vector2 pos = new Vector2(innerDimensions.X, innerDimensions.Y) - new Vector2((int)(Width.Pixels * 0.75f), (int)(Height.Pixels * 0.75f));


            spriteBatch.Draw(texture, pos, texture.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            if (IsMouseHovering)
            {
                Main.hoverItemName = hoverText;
            }
        }
    }
}