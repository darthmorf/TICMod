using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IL.Terraria;
using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Point16 = Terraria.DataStructures.Point16;

namespace TICMod
{
	public class TICMod : Mod
    {
        public TICMod()
        {
        }
    }

    internal class TriggerStates : ModWorld
    {
        public class Data
        {
            public Point16 postion;
            public bool enabled;
            public bool chatOutput;

            public Data(Point16 _postion, bool _enabled, bool _chatOutput)
            {
                postion = _postion;
                enabled = _enabled;
                chatOutput = _chatOutput;
            }
        }

        public static List<Data> data;

        public override void Initialize()
        {
            data = new List<Data>();
        }

        public override void Load(TagCompound tag)
        {
            IList<Point16> triggerPoints = tag.GetList<Point16>("triggerPoints");
            IList<bool> triggerEnabled = tag.GetList<bool>("triggerEnabled");
            IList<bool> triggerChatOutput = tag.GetList<bool>("triggerChat");

            for (int i = 0; i < triggerPoints.Count; i++)
            {
                data.Add(new Data(triggerPoints[i], triggerEnabled[i], triggerChatOutput[i]));
            }
        }

        public override TagCompound Save()
        {
            IList<Point16> triggerPoints = new List<Point16>();
            IList<bool> triggerEnabled = new List<bool>();
            IList<bool> triggerChatOutput = new List<bool>();

            foreach (Data tile in data)
            {
                triggerPoints.Add(tile.postion);
                triggerEnabled.Add(tile.enabled);
                triggerChatOutput.Add(tile.chatOutput);
            }

            return new TagCompound {
                {"triggerPoints", triggerPoints },
                {"triggerEnabled", triggerEnabled },
                {"triggerChat", triggerChatOutput }
            };
        }

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

        public void setEnabled(int i, int j, bool value)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data[k].enabled = value;
                }
            }
        }

        public void addTile(int i, int j, bool enabled, bool chatEnabled)
        {
            Data tile = new Data(new Point16(i,j), enabled, chatEnabled);
            data.Add(tile);
        }
    }

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
                var cur = updates.Dequeue();
                Terraria.Wiring.TripWire(cur.X, cur.Y, 1, 1);
            }
        }
        public void AddWireUpdate(int x, int y) => updates.Enqueue(new Point16(x, y));
    }
}