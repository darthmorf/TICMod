using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TICMod.Items
{
    class CoordDisplay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mouse Coordinate Display");
            Tooltip.SetDefault("When equipped, shows a tile coordinate display above the mouse. If smart cursor is enabled, will use that tile instead!");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
            Item.value = 0;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TICPlayer>().CoordDisplay = !hideVisual;
        }
    }
}
