﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.UI;
using TICMod.UI;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Tiles
{
	public class Influencer : ModTile
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
            DustType = 202;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.addTile(Type);

            world = ModContent.GetInstance<TICSystem>();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool Drop(int i, int j)
		{
			Item.NewItem(null, i * 16, j * 16, 16, 16, ItemType<Items.Influencer>()); // TODO: Look into creating a custom entity source for Influencer Blocks
            return base.Drop(i, j);
		}

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            string command = world.data[(i, j)].command;

            CommandResponse resp = CommandHandler.Parse(command, BlockType.Influencer);

            if (resp.success)
            {
                command = resp.response;
            }
            else
            {
                command = $"Error, invalid syntax: {resp.response}";
            }

            world.SendChatMsg(command, i, j);
        }

        public override bool RightClick(int i, int j)
        {
            GetInstance<TICSystem>().ToggleCommandUI(i, j, BlockType.Influencer);
            
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ItemType<Items.Influencer>();
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            world.addTile(i, j, true, true, BlockType.Influencer);
            base.PlaceInWorld(i, j, item);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            world.data.Remove((i, j));
            GetInstance<TICSystem>().ToggleCommandUI(i, j, BlockType.Influencer, true);
           base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

    }
}
