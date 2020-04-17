﻿using Microsoft.Xna.Framework;
using System;
using IL.Terraria.UI.Chat;
using On.Terraria.Chat;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;
using ChatLine = Terraria.UI.Chat.ChatLine;
using ChatMessage = Terraria.Chat.ChatMessage;

namespace TICMod.Tiles
{
	public class Trigger : ModTile
    {
        private bool enabled = true; // This var is shared between all instances - need individual one for tile data.
        private bool chatOutput = true;
		public override void SetDefaults()
        {
            Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileSolidTop[Type] = true;
            dustType = 145;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);
		}

		public override bool Dangersense(int i, int j, Player player) => true;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(i * 16, j * 16, 16, 32, ItemType<Items.Trigger>());
        }

		public override void HitWire(int i, int j)
        {
            bool isBottom = false;
            Tile tile = Main.tile[i, j];

            // Check if the base of the block was triggered by wire
            if (tile.frameY / 18 == 1)
            {
                isBottom = true;
            }

            if (isBottom)
            {
                // toggle enabled
                enabled = !enabled;
                short frameAdjustment = (short)(tile.frameX > 0 ? -18 : 18);
                Main.tile[i, j].frameX += frameAdjustment;
                Main.tile[i, j-1].frameX += frameAdjustment;
                NetMessage.SendTileSquare(-1, i, j - 1, 2, TileChangeType.None);
                string state = enabled ? "Enabled" : "Disabled";
                SendChatMsg($"{state}", i, j);
            }
        }

        public void SendChatMsg(string text, int x = -1, int y = -1)
        {
            if (chatOutput)
            {
                Main.NewText($"[Trigger@{x},{y}] {text}", Color.Gray);
            }
        }
    }
}
