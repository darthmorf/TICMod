using System;
using System.Collections.Generic;
using IL.Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using TextureAssets = Terraria.GameContent.TextureAssets;

namespace TICMod.Items
{
	public class OutputToggle : ModItem
    {
        internal Texture2D enableTexture;
        internal Texture2D disableTexture;

        internal string name = "Tile Output Toggler";

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault(name);
        }

        public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 1;
			Item.value = 0;
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = 5;
            Item.useAnimation = 5;

            UpdateState();
        }

        internal void UpdateState()
        {
            if (Main.dedServ == false)
            {
                if (enableTexture == null)
                    enableTexture = ModContent.Request<Texture2D>("Items/OutputToggleOn").Value;
                if (disableTexture == null)
                    disableTexture = ModContent.Request<Texture2D>("Items/OutputToggleOff").Value;

                bool output = ModContent.GetInstance<TICSystem>().tileOutput;
                if (output)
                {
                    TextureAssets.Item[Item.type].Value = disableTexture;
                }
                else
                {
                    TextureAssets.Item[Item.type].Value = enableTexture;
                }
            }
        }

        public override Nullable<bool> UseItem(Player player)
        {
            bool output = ModContent.GetInstance<TICSystem>().tileOutput;
            ModContent.GetInstance<TICSystem>().tileOutput = !output;

            if (!output)
            {
                Utils.ChatOutput($"Enabled TIC Tile Debug outputs", new Color(0, 127, 255));
            }
            else
            {
                Utils.ChatOutput($"Disabled TIC Tile Debug outputs", new Color(0, 127, 255));
            }

            UpdateState();

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool output = ModContent.GetInstance<TICSystem>().tileOutput;
            string tt = "";
            if (output)
            {
                tt = "Disables TIC blocks displaying their debug output on this world.";
            }
            else
            {
                tt = "Enables TIC blocks displaying their debug output on this world.";
            }

            foreach (var tooltip in tooltips)
            {
                if (tooltip.Name == "Tooltip#0")
                {
                    tooltip.Text = tt;
                    return;
                }
            }

            tooltips.Add(new TooltipLine(Mod, "Tooltip#0", tt));
        }
    }
}
