using Microsoft.Xna.Framework;
using System;
using IL.Terraria.UI.Chat;
using Microsoft.Xna.Framework.Graphics;
using On.Terraria.Chat;
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
using ChatLine = Terraria.UI.Chat.ChatLine;
using ChatMessage = Terraria.Chat.ChatMessage;

namespace TICMod.Tiles
{
	public class Trigger : ModTile
    {
        private TICSystem world;
		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileSolidTop[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            DustType = 145;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
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
            Item.NewItem(i * 16, j * 16, 16, 32, ItemType<Items.Trigger>());
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            world.data.Remove((i, j));
            GetInstance<TICSystem>().ToggleCommandUI(i, j, BlockType.Conditional, true);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            world.addTile(i, j, true, true, BlockType.Trigger);
            base.PlaceInWorld(i, j, item);
        }

        public override void HitWire(int i, int j)
        { 
            bool isBottom = false;
            Tile tile = Main.tile[i, j];

            // Check if the base of the block was triggered by wire
            if (tile.TileFrameY / 18 == 1)
            {
                isBottom = true;
            }

            if (isBottom)
            {
                
                world.data.TryGetValue((i, j), out var data);
                world.data[(i, j)].enabled = !world.data[(i, j)].enabled;
                short frameAdjustment = (short)(tile.TileFrameX > 0 ? -18 : 18);
                Main.tile[i, j].TileFrameX += frameAdjustment;
                Main.tile[i, j-1].TileFrameX += frameAdjustment;
                NetMessage.SendTileSquare(-1, i, j - 1, 2, TileChangeType.None);
                string state = world.data[(i, j)].enabled ? "Disabled" : "Enabled";
                world.SendChatMsg($"{state}", i, j);
            }
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<Items.Trigger>();
        }

        public override bool RightClick(int i, int j)
        {
            GetInstance<TICSystem>().ToggleCommandUI(i, j, BlockType.Trigger);

            return true;
        }
    }
}
