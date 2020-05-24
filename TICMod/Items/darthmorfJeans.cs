using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Items
{
    [AutoloadEquip(EquipType.Legs)]
    public class darthmorfJeans : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("darthmorf's Jeans");
            Tooltip.SetDefault("Great for impersonating mod devs!\nArt by Jestex. Miss you.");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Cyan;
            item.vanity = true;
        }
    }
}