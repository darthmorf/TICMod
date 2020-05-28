using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

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
			item.width = 20;
			item.height = 20;
			item.maxStack = 1;
			item.value = 0;
            item.rare = ItemRarityID.Cyan;
            item.useStyle = 5;
            item.useAnimation = 5;

            UpdateState();
        }

        internal void UpdateState()
        {
            if (Main.dedServ == false)
            {
                if (enableTexture == null)
                    enableTexture = mod.GetTexture("Items/OutputToggleOn");
                if (disableTexture == null)
                    disableTexture = mod.GetTexture("Items/OutputToggleOff");

                bool output = ModContent.GetInstance<TICWorld>().tileOutput;
                if (output)
                {
                    Main.itemTexture[item.type] = disableTexture;
                }
                else
                {
                    Main.itemTexture[item.type] = enableTexture;
                }
            }
        }

        public override bool UseItem(Player player)
        {
            bool output = ModContent.GetInstance<TICWorld>().tileOutput;
            ModContent.GetInstance<TICWorld>().tileOutput = !output;

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
            bool output = ModContent.GetInstance<TICWorld>().tileOutput;
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
                    tooltip.text = tt;
                    return;
                }
            }

            tooltips.Add(new TooltipLine(mod, "Tooltip#0", tt));
        }
    }
}
