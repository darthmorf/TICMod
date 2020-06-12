using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using MonoMod.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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

        internal UserInterface textDisplayInterface;
        internal UITextDisplayer textDisplayer;

        internal PlayerDataStore playerDataStore;
        internal NPCDataStore npcDataStore;

        public override void Load()
        {
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
            textDisplayer.RemoveAllChildren();
            base.PreSaveAndQuit();
        }

        public override void Unload()
        {
            commandInterface = null;
            coordInterface = null;
            textDisplayInterface = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.dedServ)
            {
                byte msgType = reader.ReadByte();
                if (msgType == 0) // Command tile updated
                {
                    Dictionary<(int x, int y), TICWorld.Data> data = ModContent.GetInstance<TICWorld>().data;
                    TICWorld.Data tile;
                    int byteCount = reader.ReadInt32();
                    byte[] byteData = reader.ReadBytes(byteCount);

                    using (var memStream = new MemoryStream())
                    {
                        var binForm = new BinaryFormatter();
                        memStream.Write(byteData, 0, byteData.Length);
                        memStream.Seek(0, SeekOrigin.Begin);
                        tile = (TICWorld.Data)binForm.Deserialize(memStream);
                    }

                    if (data.ContainsKey((tile.x, tile.y)))
                    {
                        data[(tile.x, tile.y)] = tile;
                    }
                    else
                    {
                        data.Add((tile.x, tile.y), tile);
                    }

                    // Rebuild trigger methods as they cannot be transmitted
                    if (tile.type == BlockType.Trigger)
                    {
                        CommandResponse resp = CommandHandler.Parse(tile.command, tile.type, i: tile.x, j: tile.y);
                        if (resp.valid == false)
                        {
                            tile.trigger = null;
                        }
                    }

                    NetMessage.SendData(MessageID.WorldData);
                    // TODO: Handle triggers
                }
            }
            else
            {
                byte msgType = reader.ReadByte();
                if (msgType == 1)
                {
                    string message = reader.ReadNullTerminatedString();
                    int r = reader.ReadVarInt();
                    int g = reader.ReadVarInt();
                    int b = reader.ReadVarInt();
                    int timeout = reader.ReadVarInt();
                    int xPos = reader.ReadVarInt();
                    int yPos = reader.ReadVarInt();
                    bool tileAttach = reader.ReadBoolean();

                    ModContent.GetInstance<TICMod>().textDisplayer.AddText(message, new Color(r,g,b), timeout, xPos, yPos, tileAttach);
                }
            }
        }

        public void SendTICTileUpdatePacket(TICWorld.Data data) // Client -> Server
        {
            ModPacket packet = GetPacket();
            packet.Write((byte)0);
            byte[] dataBytes;
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, data);
                dataBytes = ms.ToArray();
            }

            packet.Write(BitConverter.GetBytes((Int32)dataBytes.Length));
            packet.Write(dataBytes);
            packet.Send();
        }

        public void SendTextDisplayPacket(string message, Color color, int timeout, int xPos, int yPos, bool tileAttach) // Server -> Client
        {
            ModPacket packet = GetPacket();
            packet.Write((byte)1);
            packet.WriteNullTerminatedString(message);
            packet.WriteVarInt(color.R);
            packet.WriteVarInt(color.G);
            packet.WriteVarInt(color.B);
            packet.WriteVarInt(timeout);
            packet.WriteVarInt(xPos);
            packet.WriteVarInt(yPos);
            packet.Write(tileAttach);
            packet.Send();
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