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
            Tooltip.SetDefault("When equipped, shows a tile coordinate display above the mouse.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.accessory = true;
            item.value = 0;
            item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<TICPlayer>().CoordDisplay = !hideVisual;
        }
    }
}
