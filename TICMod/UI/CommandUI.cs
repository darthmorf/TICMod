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
    class CommandUI : UIDragablePanel
    {
        private UIText titleText;
        private UIBetterTextBox commandInput;
        private TICSystem world;
        private UIPanel button;
        private UICheckbox outputCheckbox;

        private BlockType uiType;

        public int i;
        public int j;

        private bool textBoxFocused = false;
        public override void OnInitialize()
        {
            int screenWidth = Main.screenWidth;
            this.Width.Set(0, 0.5f);
            this.Height.Set(105, 0f);
            this.Left.Set(screenWidth / 4, 0f); // can't use precent as that causes the draggable windows to jump when first dragged
            this.Top.Set(10f, 0f);

            titleText = new UIText("");
            titleText.Top.Pixels = -5;
            this.Append(titleText);

            commandInput = new UIBetterTextBox("Command");
            commandInput.BackgroundColor = Color.White;
            commandInput.Top.Set(0, 0.25f);
            commandInput.Width.Precent = 1f;
            commandInput.Height.Pixels = 30;
            commandInput.OnTextChanged += () => { CommandInputChanged(); };
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

            UIQuitButton quitButton = new UIQuitButton("Close Menu");
            quitButton.Top.Set(-6, 0f);
            quitButton.Left.Set(0, 0.983f);
            quitButton.OnClick += (evt, element) => { ModContent.GetInstance<TICSystem>().ToggleCommandUI(i, j, uiType,true);};
            this.Append(quitButton);

            switch (uiType)
            {
                case BlockType.Trigger:
                    BackgroundColor = new Color(99, 57, 100, 178);
                    return;

                case BlockType.Influencer:
                    BackgroundColor = new Color(0, 84, 78, 178);
                    return;

                case BlockType.Conditional:
                    BackgroundColor = new Color(75, 74, 22, 178);
                    return;
            }
        }

        public void InitValues(int i, int j, BlockType type)
        {
            world = ModContent.GetInstance<TICSystem>();
            this.i = i;
            this.j = j;
            uiType = type;

            OnInitialize();

            titleText.SetText($"{uiType} Block @ ({i},{j})");
            commandInput.SetText(world.data[(i, j)].command);
            outputCheckbox.Selected = world.data[(i, j)].chatOutput;

            commandInput.OnTabPressed += () => ModContent.GetInstance<TICSystem>().CycleCommandUIFocus(i, j);

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

        public void FocusText()
        {
            commandInput.Focus();
        }

        private void SaveBtnPress()
        {
            world.data[(i, j)].command = commandInput.currentString;
            world.data[(i, j)].chatOutput = outputCheckbox.Selected;

            world.updateTile(i, j, uiType);
        }

        private void CommandInputChanged()
        {
            CommandResponse resp = CommandHandler.Parse(commandInput.currentString, uiType, false);
            if (resp.valid)
            {
                commandInput.textColor = Color.Black;
                commandInput.hoverText = "";
            }
            else
            {
                if (commandInput.currentString == "")
                {
                    commandInput.textColor = Color.Black;
                }
                else
                {
                    commandInput.textColor = Color.Red;
                    commandInput.hoverText = resp.response;
                }
            }
        }
    }
}
