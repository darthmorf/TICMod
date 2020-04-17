using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Tiles
{
	public class Conditional : ModTile
	{
        private bool chatOutput = true;
        public override void SetDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileSolidTop[Type] = true;
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
		}

		public override bool Dangersense(int i, int j, Player player) => true;

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
                // placeholder condition
                bool condition = Main.dayTime;
                
                SendChatMsg($"{condition}", i, j);

                ExtraWireTrips trips = ModContent.GetInstance<ExtraWireTrips>();

                if (condition)
                {
                    //Wiring.TripWire(i, (j-2), 1, 1);
                    trips.AddWireUpdate(i, j-2);
                }
                else
                {
                    //Wiring.TripWire(i, (j-1), 1, 1);
                    trips.AddWireUpdate(i, j - 1);
                }
            }
        }

        public void SendChatMsg(string text, int x = -1, int y = -1)
        {
            if (chatOutput)
            {
                Main.NewText($"[Conditional@{x},{y}] {text}", Color.Gray);
            }
        }
    }
}
