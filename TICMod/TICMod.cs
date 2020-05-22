using System;
using System.Collections.Generic;
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
        internal UserInterface userInterface;
        internal UIStateReverse modUiState;
        internal List<CommandUI> commandUis;
        internal PlayerDataStore playerDataStore;

        public override void Load()
        {
            commandUis = new List<CommandUI>();
            playerDataStore = new PlayerDataStore();

            if (!Terraria.Main.dedServ)
            {
                // Load UI
                userInterface = new UserInterface();
                modUiState = new UIStateReverse();
                modUiState.Activate();
                userInterface?.SetState(modUiState);
            }
            base.Load();
        }

        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (userInterface?.CurrentState != null)
            {
                userInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                "TICMod: InfluencerUI",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && userInterface?.CurrentState != null)
                    {
                        userInterface.Draw(Terraria.Main.spriteBatch, _lastUpdateUiGameTime);
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
    }


    // Handles all data that's tied to a specific Tile
    internal class TICStates : ModWorld
    {
        // One instance applies to one specific tile
        public class Data
        {
            public Point16 postion;
            public bool enabled;
            public bool chatOutput;
            public string command;
            public Action trigger;
            public BlockType type;

            public Data(Point16 _postion, BlockType _type, string _command="", bool _enabled=true, bool _chatOutput=true)
            {
                postion = _postion;
                enabled = _enabled;
                chatOutput = _chatOutput;
                command = _command;
                type = _type;
            }
        }

        public static List<Data> data;

        public override void Initialize()
        {
            data = new List<Data>();
        }

        // Load extra tile data saved with world
        public override void Load(TagCompound tag)
        {
            IList<Point16> points = tag.GetList<Point16>("TICPoints");
            IList<bool> enabled = tag.GetList<bool>("TICEnabled");
            IList<bool> chatOutput = tag.GetList<bool>("TICChat");
            IList<string> commands = tag.GetList<string>("TICCommand");
            IList<string> types = tag.GetList<string>("TICType");

            // Convert type string to enum
            for (int i = 0; i < points.Count; i++)
            {
                BlockType type = BlockType.Influencer;
                switch (types[i])
                {
                    case "trigger":
                        type = BlockType.Trigger;
                        break;

                    case "influencer":
                        type = BlockType.Influencer;
                        break;

                    case "conditional":
                        type = BlockType.Conditional;
                        break;
                }

                data.Add(new Data(points[i], type, commands[i], enabled[i], chatOutput[i]));
            }

            // Initialise trigger methods for existing trigger tiles
            foreach (var tile in data)
            {
                if (tile.type == BlockType.Trigger)
                {
                    CommandHandler.Parse(tile.command, tile.type, i: tile.postion.X, j: tile.postion.Y);
                }
            }
        }

        // Save extra tile data saved with world
        public override TagCompound Save()
        {
            IList<Point16> points = new List<Point16>();
            IList<bool> enabled = new List<bool>();
            IList<bool> chatOutput = new List<bool>();
            IList<string> commands = new List<string>();
            IList<string> types = new List<string>();

            foreach (Data tile in data)
            {
                points.Add(tile.postion);
                enabled.Add(tile.enabled);
                chatOutput.Add(tile.chatOutput);
                commands.Add(tile.command);

                // Convert enum to type string
                switch (tile.type)
                {
                    case BlockType.Trigger: 
                        types.Add("trigger");
                        break;

                    case BlockType.Influencer:
                        types.Add("influencer");
                        break;

                    case BlockType.Conditional:
                        types.Add("conditional");
                        break;
                }
            }

            return new TagCompound {
                {"TICPoints", points },
                {"TICEnabled", enabled },
                {"TICChat", chatOutput },
                {"TICCommand", commands },
                {"TICType", types }
            };
        }

        // Initializes extra tile data for a specific tile
        public void addTile(int i, int j, bool enabled, bool chatEnabled, BlockType type)
        {
            Data tile = new Data(new Point16(i, j), type, _enabled: enabled, _chatOutput: chatEnabled);
            data.Add(tile);
        }

        // Deinitializes extra tile data for a specific tile
        public void removeTile(int i, int j)
        {
            Point16 point = new Point16(i, j);
            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data.RemoveAt(k);
                    return;
                }
            }
        }

        public override void PostUpdate()
        {
            // Execute any trigger methods
            foreach (var tile in data)
            {
                tile.trigger?.Invoke();
            }
        }

        // Getter and Setters for various data properties
        public bool isEnabled(int i, int j)
        {
            Point16 point = new Point16(i, j);

            foreach (Data tile in data)
            {
                if (tile.postion == point)
                {
                    return tile.enabled;
                }
            }

            return false;
        }

        public bool isChatEnabled(int i, int j)
        {
            Point16 point = new Point16(i, j);

            foreach (Data tile in data)
            {
                if (tile.postion == point)
                {
                    return tile.chatOutput;
                }
            }

            return false;
        }

        public string getCommand(int i, int j)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    return data[k].command;
                }
            }

            return "";
        }

        public Action getTrigger(int i, int j)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    return data[k].trigger;
                }
            }

            return null;
        }

        public void setEnabled(int i, int j, bool value)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data[k].enabled = value;
                    return;
                }
            }
        }

        public void setCommand(int i, int j, string value)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data[k].command = value;
                    return;
                }
            }
        }

        public void setChatEnabled(int i, int j, bool value)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data[k].chatOutput = value;
                    return;
                }
            }
        }

        public void setTrigger(int i, int j, Action value)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data[k].trigger = value;
                    return;
                }
            }
        }
    }

    // Handles sending wire hits 
    // ty Rartrin
    public class ExtraWireTrips : ModWorld
    {
        public Queue<Point16> updates = new Queue<Point16>();
        public override void Initialize()
        {
            updates.Clear();
        }
        public override void PostUpdate()
        {
            while (updates.Count > 0)
            {
                var tile = updates.Dequeue();
                Terraria.Wiring.TripWire(tile.X, tile.Y, 1, 1);
            }
        }
        public void AddWireUpdate(int x, int y) => updates.Enqueue(new Point16(x, y));
    }
}