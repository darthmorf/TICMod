using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TICMod.Commands;
using TICMod.Items;
using TICMod.UI;
using TICMod.Commands.Conditionals;
using TICMod.Commands.Influencers;
using InfluencerCommand = TICMod.Commands.Influencers.InfluencerCommand;
using TriggerCommand = TICMod.Commands.TriggerCommand;
using ConditionalCommand = TICMod.Commands.Conditionals.ConditionalCommand;

namespace TICMod
{
    public class TICSystem : ModSystem
    {
        // One instance applies to one specific tile
        [Serializable]
        public class Data
        {
            public int x;
            public int y;
            public bool enabled = true;
            public bool chatOutput;
            public string command;
            [NonSerialized()]  public Action trigger;
            public BlockType type;

            public Data(int x, int y, BlockType type, string command = "", bool enabled = true, bool chatOutput = true)
            {
                this.x = x;
                this.y = y;
                this.enabled = enabled;
                this.chatOutput = chatOutput;
                this.command = command;
                this.type = type;
            }
        }

        public Dictionary<(int x, int y), Data> data = new Dictionary<(int x, int y), Data>();
        internal bool tileOutput = true;

        internal UserInterface commandInterface;
        internal UserInterface coordInterface;
        internal UserInterface textDisplayInterface;

        internal UIStateReverse modUiState;
        internal List<CommandUI> commandUis;
        internal List<Command> commands;

        internal PlayerDataStore playerDataStore;
        internal NPCDataStore npcDataStore;

        internal UICoordDisplay coordDisplay;
        internal UITextDisplayer textDisplayer;

        public override void Load()
        {
            LoadCommands();

            commandUis = new List<CommandUI>();
            playerDataStore = new PlayerDataStore();
            npcDataStore = new NPCDataStore();

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

                textDisplayInterface = new UserInterface();
                textDisplayer = new UITextDisplayer();
                textDisplayer.Activate();
                textDisplayInterface?.SetState(textDisplayer);
            }

            base.Load();
        }

        public override void OnWorldLoad()
        {
            data.Clear();
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

            if (textDisplayInterface?.CurrentState != null)
            {
                textDisplayInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            int nameIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: MP Player Names"));
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
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

            if (nameIndex != -1)
            {
                layers.Insert(nameIndex, new LegacyGameInterfaceLayer(
                "TICMod: TICTextDisplay",
                delegate
                {
                    if (_lastUpdateUiGameTime != null && textDisplayInterface?.CurrentState != null)
                    {
                        textDisplayInterface.Draw(Terraria.Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                },
                InterfaceScaleType.Game));
            }
        }

        // Show/Hide individual instance of UI, tied to specific tile
        internal void ToggleCommandUI(int i, int j, BlockType uiType, bool onlyClose = false)
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
            textDisplayer.RemoveAllChildren();
            base.PreSaveAndQuit();
        }

        public override void Unload()
        {
            commandInterface = null;
            coordInterface = null;
            textDisplayInterface = null;
            UIQuitButton.texture = null;
            UICheckbox.checkboxTexture = null;
            UICheckbox.checkmarkTexture = null;
        }

        // Load extra tile data saved with world
        public override void LoadWorldData(TagCompound tag)
        {
            IList<int> xpoints = tag.GetList<int>("TICXPoints");
            IList<int> ypoints = tag.GetList<int>("TICYPoints");
            IList<bool> enabled = tag.GetList<bool>("TICEnabled");
            IList<bool> chatOutput = tag.GetList<bool>("TICChat");
            IList<string> commands = tag.GetList<string>("TICCommand");
            IList<string> types = tag.GetList<string>("TICType");
            tileOutput = tag.GetBool("TICTileOutput");

            data.Clear();
            // Convert type string to enum
            for (int i = 0; i < types.Count; i++)
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

                data.Add((xpoints[i], ypoints[i]),
                    new Data(xpoints[i], ypoints[i], type, commands[i], enabled[i], chatOutput[i]));
            }

            // Initialise trigger methods for existing trigger tiles
            foreach (var tile in data)
            {
                if (tile.Value.type == BlockType.Trigger)
                {
                    CommandHandler.Parse(tile.Value.command, tile.Value.type, i: tile.Value.x, j: tile.Value.y);
                }
            }
        }

        private void LoadCommands()
        {
            commands = new List<Command>();

            var enumerable = Assembly.GetExecutingAssembly().GetTypes().Where(ac => IsTypeOf(typeof(TriggerCommand), ac));
            foreach (var commandClass in enumerable)
            {
                commands.Add((TriggerCommand)Activator.CreateInstance(commandClass));
            }

            enumerable = Assembly.GetExecutingAssembly().GetTypes().Where(ac => IsTypeOf(typeof(InfluencerCommand), ac));
            foreach (var commandClass in enumerable)
            {
                commands.Add((InfluencerCommand)Activator.CreateInstance(commandClass));
            }

            enumerable = Assembly.GetExecutingAssembly().GetTypes().Where(ac => IsTypeOf(typeof(ConditionalCommand), ac));
            foreach (var commandClass in enumerable)
            {
                commands.Add((ConditionalCommand)Activator.CreateInstance(commandClass));
            }
        }

        // Recursively check if based off of type
        private bool IsTypeOf(Type type, Type thisType)
        {
            if (thisType.BaseType == null)
            {
                return false;
            }
            else if (thisType.BaseType == type)
            {
                return true;
            }

            return IsTypeOf(type, thisType.BaseType);
        }

        // Save extra tile data saved with world
        public override void SaveWorldData(TagCompound tag)
        {
            IList<int> xpoints = new List<int>();
            IList<int> ypoints = new List<int>();
            IList<bool> enabled = new List<bool>();
            IList<bool> chatOutput = new List<bool>();
            IList<string> commands = new List<string>();
            IList<string> types = new List<string>();

            foreach (var tile in data)
            {
                xpoints.Add(tile.Value.x);
                ypoints.Add(tile.Value.y);
                enabled.Add(tile.Value.enabled);
                chatOutput.Add(tile.Value.chatOutput);
                commands.Add(tile.Value.command);

                // Convert enum to type string
                switch (tile.Value.type)
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

            tag.Add(new KeyValuePair<string, object>("TICXPoints", xpoints));
            tag.Add(new KeyValuePair<string, object>("TICYPoints", ypoints));
            tag.Add(new KeyValuePair<string, object>("TICEnabled", enabled));
            tag.Add(new KeyValuePair<string, object>("TICChat", chatOutput));
            tag.Add(new KeyValuePair<string, object>("TICCommand", commands));
            tag.Add(new KeyValuePair<string, object>("TICType", types));
            tag.Add(new KeyValuePair<string, object>("TICTileOutput", tileOutput));
        }

        // Initializes extra tile data for a specific tile
        public void addTile(int i, int j, bool enabled, bool chatEnabled, BlockType type)
        {
            Data tile = new Data(i, j, type, enabled: enabled, chatOutput: chatEnabled);
            data.Add((i, j), tile);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModContent.GetInstance<TICMod>().SendTICTileUpdatePacket(data[(i, j)]);
            }
        }

        public void updateTile(int i, int j, BlockType uiType)
        {
            if (uiType == BlockType.Trigger)
            {
                CommandResponse resp = CommandHandler.Parse(data[(i, j)].command, uiType, i: i, j: j);
                if (resp.valid == false)
                {
                    data[(i, j)].trigger = null;
                }
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModContent.GetInstance<TICMod>().SendTICTileUpdatePacket(data[(i, j)]);
            }
        }

        public override void PostUpdateWorld()
        {
            // Execute any trigger methods
            foreach (var tile in data)
            {
                tile.Value.trigger?.Invoke();
            }
        }

        public void SendChatMsg(string text, int x = -1, int y = -1, bool debug = false)
        {
            if (debug)
            {
                Utils.ChatOutput(text, Color.Magenta);
                return;
            }

            bool showOutput = false;
            if (data.ContainsKey((x, y)))
            {
                showOutput = data[(x, y)].chatOutput;
            }

            if (showOutput && tileOutput)
            {
                Utils.ChatOutput($"[{data[(x, y)].type}@{x},{y}] {text}", Color.Gray);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            if (Main.dedServ)
            {
                byte[] dataBytes;
                BinaryFormatter bf = new BinaryFormatter();
                
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, data);
                    dataBytes = ms.ToArray();
                }

                writer.Write(BitConverter.GetBytes((Int32)dataBytes.Length));
                writer.Write(dataBytes);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            try
            {
                int byteCount = reader.ReadInt32();
                byte[] byteData = reader.ReadBytes(byteCount);

                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Write(byteData, 0, byteData.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    data = (Dictionary < (int x, int y), Data > )binForm.Deserialize(memStream);
                }

                foreach (var tile in data)
                {
                    // Rebuild trigger methods as they cannot be transmitted
                    if (tile.Value.type == BlockType.Trigger)
                    {
                        CommandResponse resp = CommandHandler.Parse(tile.Value.command, tile.Value.type, i: tile.Value.x, j: tile.Value.y);
                        if (resp.valid == false)
                        {
                            tile.Value.trigger = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }

    // Handles sending wire hits 
    // ty Rartrin
    public class ExtraWireTrips : ModSystem
    {
        public Queue<Point16> updates = new Queue<Point16>();
        public override void OnWorldLoad()
        {
            updates.Clear();
        }
        public override void PostUpdateWorld()
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
