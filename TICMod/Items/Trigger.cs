using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace TICMod.Items
{
    public class Trigger : ModItem
    {
        public const string Name = "Trigger";
        public Trigger() { }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trigger");
            Tooltip.SetDefault("Sends a signal through top tile when internal condition is triggered." +
                               "\nCan be disabled/enabled by giving signal to bottom tile." +
                               "\nCHEAT ITEM - NOT BALANCED FOR REGULAR USE.");
        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.createTile = TileType<Tiles.Trigger>();
            Item.width = 12;
            Item.height = 12;
            Item.value = 0;
            Item.mech = true;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}