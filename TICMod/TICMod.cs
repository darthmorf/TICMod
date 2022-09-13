using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoMod.Utils;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TICMod.Commands;
using TICMod.Commands.Conditionals;
using TICMod.Commands.Influencers;
using TICMod.UI;
using Point16 = Terraria.DataStructures.Point16;

namespace TICMod
{
    public enum BlockType { Trigger, Influencer, Conditional }

    public class TICMod : Mod
    {
        public override void Load()
        {
            base.Load();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            if (Main.dedServ)
            {
                byte msgType = reader.ReadByte();
                if (msgType == 0) // Command tile updated
                {
                    Dictionary<(int x, int y), TICSystem.Data> data = ModContent.GetInstance<TICSystem>().data;
                    TICSystem.Data tile;
                    int byteCount = reader.ReadInt32();
                    byte[] byteData = reader.ReadBytes(byteCount);

                    using (var memStream = new MemoryStream())
                    {
                        var binForm = new BinaryFormatter();
                        memStream.Write(byteData, 0, byteData.Length);
                        memStream.Seek(0, SeekOrigin.Begin);
                        tile = (TICSystem.Data)binForm.Deserialize(memStream);
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
                    int r = reader.Read7BitEncodedInt(); // TODO: Verify that this is the right int reading method
                    int g = reader.Read7BitEncodedInt();
                    int b = reader.Read7BitEncodedInt();
                    int timeout = reader.Read7BitEncodedInt();
                    int xPos = reader.Read7BitEncodedInt();
                    int yPos = reader.Read7BitEncodedInt();
                    bool tileAttach = reader.ReadBoolean();

                    ModContent.GetInstance<TICSystem>().textDisplayer.AddText(message, new Color(r,g,b), timeout, xPos, yPos, tileAttach);
                }
            }
        }

        public void SendTICTileUpdatePacket(TICSystem.Data data) // Client -> Server
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
            packet.Write7BitEncodedInt(color.R);
            packet.Write7BitEncodedInt(color.G);
            packet.Write7BitEncodedInt(color.B);
            packet.Write7BitEncodedInt(timeout);
            packet.Write7BitEncodedInt(xPos);
            packet.Write7BitEncodedInt(yPos);
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

        public override void PreUpdate()
        {
            base.PreUpdate();
        }
    }
}