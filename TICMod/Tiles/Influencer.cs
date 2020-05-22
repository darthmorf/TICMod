using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
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
        private TICStates states;
        public override void SetDefaults()
		{
            Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileSolidTop[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            dustType = 202;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleSwitch);
            TileObjectData.addTile(Type);

            states = ModContent.GetInstance<TICStates>();
        }

        public override bool HasSmartInteract() => true;

        public override bool Drop(int i, int j)
		{
			Item.NewItem(i * 16, j * 16, 16, 16, ItemType<Items.Influencer>());
			return base.Drop(i, j);
		}

        public override void HitWire(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            string command = states.data[(i, j)].command;

            CommandResponse resp = CommandHandler.Parse(command, BlockType.Influencer);

            if (resp.success)
            {
                command = resp.response;
            }
            else
            {
                command = $"Error, invalid syntax: {resp.response}";
            }

            SendChatMsg(command, i, j, states.data[(i, j)].chatOutput);
        }

        public void SendChatMsg(string text, int x = -1, int y = -1, bool showOutput=true)
        {
            if (showOutput)
            {
                
                Main.NewText($"[Influencer@{x},{y}] {text}", Color.Gray);
            }
        }

        public override bool NewRightClick(int i, int j)
        {
            GetInstance<TICMod>().ToggleCommandUI(i, j, BlockType.Influencer);
            
            return true;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = ItemType<Items.Influencer>();
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            states.addTile(i, j, true, true, BlockType.Influencer);
            base.PlaceInWorld(i, j, item);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            states.data.Remove((i, j));
            GetInstance<TICMod>().ToggleCommandUI(i, j, BlockType.Influencer, true);
           base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

    }
}
