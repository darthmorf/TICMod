using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TICMod
{
    // Handles all data that's tied to a specific Tile
    internal class TICWorld : ModWorld
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

            public Data(Point16 _postion, BlockType _type, string _command = "", bool _enabled = true, bool _chatOutput = true)
            {
                postion = _postion;
                enabled = _enabled;
                chatOutput = _chatOutput;
                command = _command;
                type = _type;
            }
        }

        internal Dictionary<(int x, int y), Data> data;
        internal bool tileOutput = true;

        public override void Initialize()
        {
            data = new Dictionary<(int x, int y), Data>();
        }

        // Load extra tile data saved with world
        public override void Load(TagCompound tag)
        {
            IList<Point16> points = tag.GetList<Point16>("TICPoints");
            IList<bool> enabled = tag.GetList<bool>("TICEnabled");
            IList<bool> chatOutput = tag.GetList<bool>("TICChat");
            IList<string> commands = tag.GetList<string>("TICCommand");
            IList<string> types = tag.GetList<string>("TICType");
            tileOutput = tag.GetBool("TICTileOutput");

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

                data.Add((points[i].X, points[i].Y), new Data(points[i], type, commands[i], enabled[i], chatOutput[i]));
            }

            // Initialise trigger methods for existing trigger tiles
            foreach (var tile in data)
            {
                if (tile.Value.type == BlockType.Trigger)
                {
                    CommandHandler.Parse(tile.Value.command, tile.Value.type, i: tile.Value.postion.X, j: tile.Value.postion.Y);
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

            foreach (var tile in data)
            {
                points.Add(tile.Value.postion);
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

            return new TagCompound {
                {"TICPoints", points },
                {"TICEnabled", enabled },
                {"TICChat", chatOutput },
                {"TICCommand", commands },
                {"TICType", types },
                {"TICTileOutput", tileOutput }
            };
        }

        // Initializes extra tile data for a specific tile
        public void addTile(int i, int j, bool enabled, bool chatEnabled, BlockType type)
        {
            Data tile = new Data(new Point16(i, j), type, _enabled: enabled, _chatOutput: chatEnabled);
            data.Add((i, j), tile);
        }

        public override void PostUpdate()
        {
            // Execute any trigger methods
            foreach (var tile in data)
            {
                tile.Value.trigger?.Invoke();
            }
        }

        public void SendChatMsg(string text, int x = -1, int y = -1)
        {
            bool showOutput = true;
            if (data.ContainsKey((x, y)))
            {
                showOutput = data[(x, y)].chatOutput;
            }

            if (showOutput && tileOutput)
            {
                Main.NewText($"[Trigger@{x},{y}] {text}", Color.Gray);
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
