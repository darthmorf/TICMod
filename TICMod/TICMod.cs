using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IL.Terraria;
using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TICMod.UI;
using Point16 = Terraria.DataStructures.Point16;

namespace TICMod
{
	public class TICMod : Mod
    {
        internal UserInterface userInterface;
        internal InfluencerUI influencerUI;

        public TICMod()
        {
        }

        public override void Load()
        {
            if (!Terraria.Main.dedServ)
            {
                userInterface = new UserInterface();

                influencerUI = new InfluencerUI();
                influencerUI.Activate();
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

        internal void ShowInfluencerUI(int i, int j)
        {
            userInterface?.SetState(influencerUI);
            influencerUI.InitValues(i, j);
        }

        internal void HideInfluencerUI()
        {
            userInterface?.SetState(null);
        }
    }



    internal class TICStates : ModWorld
    {
        public class Data
        {
            public Point16 postion;
            public bool enabled;
            public bool chatOutput;
            public string command;
            public bool uiOpen;

            public Data(Point16 _postion, string _command="", bool _enabled=true, bool _chatOutput=true)
            {
                postion = _postion;
                enabled = _enabled;
                chatOutput = _chatOutput;
                command = _command;
                uiOpen = false;
            }
        }

        public static List<Data> data;

        public override void Initialize()
        {
            data = new List<Data>();
        }

        public override void Load(TagCompound tag)
        {
            IList<Point16> points = tag.GetList<Point16>("TICPoints");
            IList<bool> enabled = tag.GetList<bool>("TICEnabled");
            IList<bool> chatOutput = tag.GetList<bool>("TICChat");
            IList<string> commands = tag.GetList<string>("TICCommand");

            for (int i = 0; i < points.Count; i++)
            {
                data.Add(new Data(points[i], commands[i], enabled[i], chatOutput[i]));
            }
        }

        public override TagCompound Save()
        {
            IList<Point16> points = new List<Point16>();
            IList<bool> enabled = new List<bool>();
            IList<bool> chatOutput = new List<bool>();
            IList<string> commands = new List<string>();

            foreach (Data tile in data)
            {
                points.Add(tile.postion);
                enabled.Add(tile.enabled);
                chatOutput.Add(tile.chatOutput);
                commands.Add(tile.command);
            }

            return new TagCompound {
                {"TICPoints", points },
                {"TICEnabled", enabled },
                {"TICChat", chatOutput },
                {"TICCommand", commands }
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

        public bool isUiOpen(int i, int j)
        {
            Point16 point = new Point16(i, j);

            foreach (Data tile in data)
            {
                if (tile.postion == point)
                {
                    return tile.uiOpen;
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

        public void setUiOpen(int i, int j, bool value)
        {
            Point16 point = new Point16(i, j);

            for (int k = 0; k < data.Count; k++)
            {
                if (data[k].postion == point)
                {
                    data[k].uiOpen = value;
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

        public void addTile(int i, int j, bool enabled, bool chatEnabled)
        {
            Data tile = new Data(new Point16(i,j));
            data.Add(tile);
        }

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