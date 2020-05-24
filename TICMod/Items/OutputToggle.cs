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
			Tooltip.SetDefault("Toggles whether TIC Blocks display their debug output on this world.");
        }

        public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 1;
			item.value = 0;
            item.rare = 13;
            item.useStyle = 5;
            item.useAnimation = 20;



            UpdateState();
        }

        internal void UpdateState()
        {
            if (enableTexture == null)
                enableTexture = mod.GetTexture("Items/OutputToggleOn");
            if (disableTexture == null)
                disableTexture = mod.GetTexture("Items/OutputToggleOff");

            bool output = ModContent.GetInstance<TICStates>().tileOutput;
            if (output)
            {
                Main.itemTexture[item.type] = disableTexture;
            }
            else
            {
                Main.itemTexture[item.type] = enableTexture;
            }
        }

        public override bool UseItem(Player player)
        {
            bool output = ModContent.GetInstance<TICStates>().tileOutput;
            ModContent.GetInstance<TICStates>().tileOutput = !output;

            if (!output)
            {
				Main.NewText($"Enabled TIC Tile Debug outputs", new Color(0, 127, 255));
            }
            else
            {
                Main.NewText($"Disabled TIC Tile Debug outputs", new Color(0, 127, 255));
            }

            UpdateState();

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            bool output = ModContent.GetInstance<TICStates>().tileOutput;
            if (output)
            {
                tooltips[1].text = "Disables TIC blocks displaying their debug output on this world.";
            }
            else
            {
                tooltips[1].text = "Enables TIC blocks displaying their debug output on this world.";
            }
        }
    }
}
