﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Items
{
    [AutoloadEquip(EquipType.Body)]
    public class darthmorfCoat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("darthmorf's Lab Coat");
            Tooltip.SetDefault("Great for impersonating mod devs!\nArt by Jestex. Miss you.");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.rare = ItemRarityID.Cyan;
            Item.vanity = true;
        }
    }
}