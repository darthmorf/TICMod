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
            Tooltip.SetDefault("Sends a signal through top tile when internal condition is triggered.\nCan be disabled/enabled by giving signal to bottom tile.");
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
            item.createTile = TileType<Tiles.Trigger>();
            item.width = 12;
            item.height = 12;
            item.value = 0;
            item.mech = true;
            item.rare = 13;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.DirtBlock);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}