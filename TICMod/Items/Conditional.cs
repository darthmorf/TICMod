using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Items
{
    public class Conditional : ModItem
    {
        public const string Name = "Conditional";
        public Conditional() { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(Name);
            Tooltip.SetDefault("When given a signal through bottom tile, assesses internal condition");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.maxStack = 9999;
            item.consumable = true;
            item.createTile = TileType<Tiles.Conditional>();
            item.width = 12;
            item.height = 12;
            item.value = 0;
            item.mech = true;
            item.rare = ItemRarityID.Cyan;
        }
    }
}