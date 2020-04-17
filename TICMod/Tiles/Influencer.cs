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
	public class Influencer : ModTile
    {
        private bool chatOutput = true;
		public override void SetDefaults()
		{
            Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolidTop[Type] = true;
            dustType = 202;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.addTile(Type);
		}

        public override bool Dangersense(int i, int j, Player player) => true;

		public override bool Drop(int i, int j)
		{
			Item.NewItem(i * 16, j * 16, 16, 16, ItemType<Items.Influencer>());
			return base.Drop(i, j);
		}

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            SendChatMsg($"Did Something!", i, j);

            // Do something
        }

        public void SendChatMsg(string text, int x = -1, int y = -1)
        {
            if (chatOutput)
            {
                
                Main.NewText($"[Influencer@{x},{y}] {text}", Color.Gray);
            }
        }
    }
}
