using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using TICMod.UI;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Tiles
{
	public class Conditional : ModTile
	{
        private TICStates states;
        public override void SetDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileSolidTop[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            dustType = 233;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            states = ModContent.GetInstance<TICStates>();
        }

        public override bool HasSmartInteract() => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 48, ItemType<Items.Conditional>());
        }

        public override void HitWire(int i, int j)
        {
            bool isBottom = false;
            Tile tile = Main.tile[i, j];

            // Check if the base of the block was triggered by wire
            if (tile.frameY / 18 == 2)
            {
                isBottom = true;
            }

            if (isBottom)
            {
                string command = states.data[(i, j)].command;
                CommandResponse resp = CommandHandler.Parse(command, BlockType.Conditional, true);

                bool condition = resp.success;


                states.SendChatMsg(resp.response, i, j);

                ExtraWireTrips trips = ModContent.GetInstance<ExtraWireTrips>();

                if (condition)
                {
                    trips.AddWireUpdate(i, j-2);
                }
                else
                {
                    trips.AddWireUpdate(i, j - 1);
                }
            }
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            states.addTile(i, j, true, true, BlockType.Conditional);
            base.PlaceInWorld(i, j, item);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            states.data.Remove((i, j));
            GetInstance<TICMod>().ToggleCommandUI(i, j, BlockType.Conditional, true);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ItemType<Items.Conditional>();
        }

        public override bool NewRightClick(int i, int j)
        {
            GetInstance<TICMod>().ToggleCommandUI(i, j, BlockType.Conditional);

            return true;
        }
    }
}
