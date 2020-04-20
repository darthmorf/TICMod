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
    class InfluencerUI : UIState
    {
        private UIPanel panel;
        private UIText titleText;
        private UIBetterTextBox commandInput;
        private TICStates states;
        private UIPanel button;
        private UICheckbox outputCheckbox;

        private bool textBoxFocused = false;
        public override void OnInitialize()
        {
            panel = new UIDragablePanel();
            panel.Width.Set(0, 0.5f);
            panel.Height.Set(0, 0.09f);
            panel.Left.Set(0, 0.25f);
            panel.Top.Set(10f, 0f);
            Append(panel);

            titleText = new UIText("");
            panel.Append(titleText);

            commandInput = new UIBetterTextBox("Command");
            commandInput.BackgroundColor = Color.White;
            commandInput.Top.Set(0, 0.25f);
            commandInput.Width.Precent = 1f;
            commandInput.Height.Pixels = 30;
            panel.Append(commandInput);

            outputCheckbox = new UICheckbox("Show debug output?", "");
            outputCheckbox.Top.Precent = 0.78f;
            outputCheckbox.Left.Pixels = 70f;
            panel.Append(outputCheckbox);

            button = new UIPanel();
            button.Width.Pixels = 60f;
            button.Height.Pixels = 30f;
            button.Top.Precent = 0.70f;
            button.OnClick += (evt, element) => { SaveBtnPress(); };
            UIText buttonText = new UIText("Save");
            buttonText.Top.Pixels = -4f;
            buttonText.Left.Pixels = 0f;
            button.Append(buttonText);
            panel.Append(button);

            states = ModContent.GetInstance<TICStates>();
        }

        private int lasti, lastj = -1;
        public void InitValues(int i, int j)
        {
            states.setUiOpen(lasti, lastj, false);
            lasti = i;
            lastj = j;

            titleText.SetText($"Influencer Block @ ({i},{j})");
            commandInput.SetText(states.getCommand(i, j));
            outputCheckbox.Selected = states.isChatEnabled(i, j);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (PlayerInput.Triggers.JustPressed.Inventory)
            {
                ModContent.GetInstance<TICMod>().HideInfluencerUI();
            }
        }

        private void SaveBtnPress()
        {
            states.setCommand(lasti, lastj, commandInput.currentString);
            states.setChatEnabled(lasti, lastj, outputCheckbox.Selected);
        }
    }
}
