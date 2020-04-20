using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace TICMod.UI
{
    class InfluencerUI : UIDragablePanel
    {
        private UIText titleText;
        private UIBetterTextBox commandInput;
        private TICStates states;
        private UIPanel button;
        private UICheckbox outputCheckbox;

        public int i;
        public int j;

        private bool textBoxFocused = false;
        public override void OnInitialize()
        {
            this.Width.Set(0, 0.5f);
            this.Height.Set(0, 0.09f);
            this.Left.Set(0, 0.25f);
            this.Top.Set(10f, 0f);

            titleText = new UIText("");
            this.Append(titleText);

            commandInput = new UIBetterTextBox("Command");
            commandInput.BackgroundColor = Color.White;
            commandInput.Top.Set(0, 0.25f);
            commandInput.Width.Precent = 1f;
            commandInput.Height.Pixels = 30;
            this.Append(commandInput);

            outputCheckbox = new UICheckbox("Show debug output?", "");
            outputCheckbox.Top.Precent = 0.78f;
            outputCheckbox.Left.Pixels = 70f;
            this.Append(outputCheckbox);

            button = new UIPanel();
            button.Width.Pixels = 60f;
            button.Height.Pixels = 30f;
            button.Top.Precent = 0.70f;
            button.BackgroundColor = new Color(73, 94, 171);
            button.OnClick += (evt, element) => { SaveBtnPress(); };
            UIText buttonText = new UIText("Save");
            buttonText.Top.Pixels = -4f;
            buttonText.Left.Pixels = 0f;
            button.Append(buttonText);
            this.Append(button);

            states = ModContent.GetInstance<TICStates>();
        }

        public void InitValues(int _i, int _j)
        {
            OnInitialize();

            i = _i;
            j = _j;

            titleText.SetText($"Influencer Block @ ({i},{j})");
            commandInput.SetText(states.getCommand(i, j));
            outputCheckbox.Selected = states.isChatEnabled(i, j);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (button.IsMouseHovering)
            {
               button.BackgroundColor = new Color(100, 118, 184);
            }
            else
            {
                button.BackgroundColor = new Color(73, 94, 171);
            }
        }

        private void SaveBtnPress()
        {
            states.setCommand(i, j, commandInput.currentString);
            states.setChatEnabled(i, j, outputCheckbox.Selected);
        }
    }
}
