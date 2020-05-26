using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TICMod.UI;
using Point16 = Terraria.DataStructures.Point16;

namespace TICMod
{
    public enum BlockType { Trigger, Influencer, Conditional }

    public class TICMod : Mod
    {
        internal UserInterface commandInterface;
        internal UIStateReverse modUiState;
        internal List<CommandUI> commandUis;

        internal UserInterface coordInterface;
        internal UICoordDisplay coordDisplay;

        internal PlayerDataStore playerDataStore;

        public override void Load()
        {
            commandUis = new List<CommandUI>();
            playerDataStore = new PlayerDataStore();

            if (!Terraria.Main.dedServ)
            {
                // Load UI
                commandInterface = new UserInterface();
                modUiState = new UIStateReverse();
                modUiState.Activate();
                commandInterface?.SetState(modUiState);

                coordInterface = new UserInterface();
                coordDisplay = new UICoordDisplay();
                coordDisplay.Activate();
                coordInterface?.SetState(coordDisplay);
            }
            base.Load();
        }

        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (commandInterface?.CurrentState != null)
            {
                commandInterface.Update(gameTime);
            }

            if (coordInterface?.CurrentState != null)
            {
                coordInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "TICMod: CoordDisplayUI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && coordInterface?.CurrentState != null)
                    {
                        coordInterface.Draw(Terraria.Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));

                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "TICMod: TICUI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && commandInterface?.CurrentState != null)
                    {
                        commandInterface.Draw(Terraria.Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.UI));
            }
        }

        // Show/Hide individual instance of UI, tied to specific tile
        internal void ToggleCommandUI(int i, int j, BlockType uiType, bool onlyClose=false)
        {
            foreach (var commandUi in commandUis)
            {
                if (commandUi.i == i && commandUi.j == j) // UI is open
                {
                    modUiState.RemoveChild(commandUi);
                    commandUis.Remove(commandUi);
                    return;
                }
            }

            if (!onlyClose)
            {
                CommandUI cUI = new CommandUI();
                modUiState.Append(cUI);
                commandUis.Add(cUI);
                cUI.InitValues(i, j, uiType);
            }
        }

        public void CycleCommandUIFocus(int i, int j)
        {
            int index = 0;
            foreach (var commandUi in commandUis)
            {
                index++;
                if (commandUi.i == i && commandUi.j == j)
                {
                    break;
                }
            }

            index = index % commandUis.Count;
            commandUis[index].FocusText();
        }

        public override void PreSaveAndQuit()
        {
            modUiState.RemoveAllChildren();
            base.PreSaveAndQuit();
        }

        public override void Unload()
        {
            commandInterface = null;
            coordInterface = null;
        }
    }

    class TICPlayer : ModPlayer
    {
        public bool CoordDisplay = false;

        public override void ResetEffects()
        {
            CoordDisplay = false;
            base.ResetEffects();
        }
    }
}