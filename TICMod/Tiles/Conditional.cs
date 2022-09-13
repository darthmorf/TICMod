using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
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
        private TICSystem world;
        public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileSolidTop[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            DustType = 233;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            world = ModContent.GetInstance<TICSystem>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(null, i * 16, j * 16, 16, 48, ItemType<Items.Conditional>()); // TODO: Look into creating a custom entity source for Influencer Blocks
        }

        public override void HitWire(int i, int j)
        {
            bool isBottom = false;
            Tile tile = Main.tile[i, j];

            // Check if the base of the block was triggered by wire
            if (tile.TileFrameY / 18 == 2)
            {
                isBottom = true;
            }

            if (isBottom)
            {
                string command = world.data[(i, j)].command;
                CommandResponse resp = CommandHandler.Parse(command, BlockType.Conditional, true);

                bool condition = resp.success;


                world.SendChatMsg(resp.response, i, j);

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
            world.addTile(i, j, true, true, BlockType.Conditional);
            base.PlaceInWorld(i, j, item);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            world.data.Remove((i, j));
            GetInstance<TICSystem>().ToggleCommandUI(i, j, BlockType.Conditional, true);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<Items.Conditional>();
        }

        public override bool RightClick(int i, int j)
        {
            GetInstance<TICSystem>().ToggleCommandUI(i, j, BlockType.Conditional);

            return true;
        }
    }
}
