﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace TICMod.UI
{
    // ty jopojelly
    internal class UICheckbox : UIText
    {
        public static Texture2D checkboxTexture;
        public static Texture2D checkmarkTexture;
        private bool disabled;
        internal string hoverText;

        private bool selected;

        public UICheckbox(string text, string hoverText, float textScale = 1, bool large = false) : base(text,
            textScale, large)
        {
            if (checkboxTexture == null)
            {
                checkboxTexture = ModContent.Request<Texture2D>("TICMod/UI/checkBox", AssetRequestMode.ImmediateLoad).Value;
            }

            if (checkmarkTexture == null)
            {
                checkmarkTexture = ModContent.Request<Texture2D>("TICMod/UI/checkMark", AssetRequestMode.ImmediateLoad).Value;
            }


            this.Left.Pixels += 20;
            //TextColor = Color.Blue;
            text = "   " + text;
            this.hoverText = hoverText;
            SetText(text);
            OnClick += UICheckbox_onLeftClick;
            Recalculate();
        }

        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value != selected)
                {
                    selected = value;
                    OnSelectedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler OnSelectedChanged;

        private void UICheckbox_onLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (disabled) return;

            this.Selected = !Selected;
        }

        public void SetDisabled(bool disabled = true)
        {
            this.disabled = disabled;
            if (disabled)
            {
                Selected = false;
            }

            TextColor = disabled ? Color.Gray : Color.White;
        }

        public void SetHoverText(string hoverText)
        {
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle innerDimensions = base.GetInnerDimensions();
            Vector2 pos = new Vector2(innerDimensions.X, innerDimensions.Y - 5);

            //Rectangle hitbox = GetInnerDimensions().ToRectangle();
            //Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);

            spriteBatch.Draw(checkboxTexture, pos, null, disabled ? Color.Gray : Color.White, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);

            if (Selected)
            {
                spriteBatch.Draw(checkmarkTexture, pos, null, disabled ? Color.Gray : Color.White, 0f, Vector2.Zero, 1f,
                    SpriteEffects.None, 0f);

            }

            if (IsMouseHovering)
            {
                Main.hoverItemName = hoverText;
            }
        }
    }
}